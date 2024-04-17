using ETicaretAPI.API.Configurations.ColumnWriters;
using ETicaretAPI.API.Extentions;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Persistence;
using ETicaretAPI.SignalR;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();//Client'tan gelen request neticesinde oluþturulan HttpContex nesnesine katmanlardaki class`lar üzerinden(busineess logic) eriþebilmemizi saðlayan bir servistir

// Add services to the container.
builder.Services.AddPersistenceServices();//Kendi servislerimizi ekledik
builder.Services.AddInfrastructureServices();//Kendi servislerimizi ekledik
builder.Services.AddApplicationServices();//MetdiatR
//builder.Services.AddStorage(StorageType.Azure);
//builder.Services.AddStorage<LocalStorage>(); // Burada verdiðimiz nesne ne ise uygulama nýn kullandýðý Storage o oluyor..
builder.Services.AddSignalRServices(); //Hubs lar için
builder.Services.AddStorage<AzureStorage>();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200/").AllowAnyHeader().AllowAnyMethod().AllowCredentials() //buradan gelen isteklere izin ver dedik
));
Serilog.Core.Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log/log.txt")
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs", needAutoCreateTable: true,
       columnOptions: new Dictionary<string, ColumnWriterBase>
       {
           {"message", new RenderedMessageColumnWriter() },
           {"message_template", new MessageTemplateColumnWriter() },
           {"level", new LevelColumnWriter() },
           {"exception", new ExceptionColumnWriter() },
           {"log_event", new LogEventSerializedColumnWriter() },
           {"user_name", new UsernameColumnWriter() }
       })
    .WriteTo.Seq(builder.Configuration["Seq:ServerURL"])
    .Enrich.FromLogContext()//contexte tanýmladýðýmýz prop u da taný gör demiþ olduk. user_name
    .CreateLogger();

builder.Host.UseSerilog(log);

//yapýlan requestleri log mekanýzmasý ile yakalamak için bu kodu ekledik. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});


builder.Services.AddControllers(options=> options.Filters.Add<ValidationFilter>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin",options =>
    {
        options.TokenValidationParameters = new() 
        {
            ValidateAudience = true, //kimlerin/hangi originlerin/sitelerin kullanacaðýný belirlediðimiz deðer. Token ý nerede kullanabilirsin
            ValidateIssuer = true, // Bu token ý kim oluþturdu. bizim API misal
            ValidateLifetime = true, //Oluþturulan token deðerinin süresini kontrol eder.
            ValidateIssuerSigningKey = true, //SÝmetrik güvenlik key i, uygulamaya özel key
            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires> DateTime.UtcNow : false,
            NameClaimType = ClaimTypes.Name //JWT üzerinde Name claim i ne karþýlýk gelen deðeri User.Identity.Name propertysinden elde edebiliriz.
            
        };
    });
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());
app.UseStaticFiles();// wwwroot dizinindeki dosyalara eriþip upload yapabilmek için izin veriyor.

app.UseSerilogRequestLogging();//loglama süreci bu middleware den sonra baþlar...
app.UseHttpLogging();//Yukarýda "AddHttpLogging" ile bereber istekleri log mekanýzmasý ile yakalamayý saðlarlar
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
//burada middleware tanýmý yapýyoruz çünkü kullanýcý authentike olmuþ mu ona da bakýcaz
app.Use(async (context , next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name",username);//user_name i context e gönderdik, bu sayede eðer böyle bir kiþi varsa "UsernameColumnWriter" sýnýfý da onun ismini elde edebilecek ve bize döndürecektir. 
    await next();
});
app.MapControllers();
app.MapHubs(); // Burada tek tek yazmamak için extention fonk oluþturduk ilgili sýnýf da deðerleri verdik.
app.Run();
