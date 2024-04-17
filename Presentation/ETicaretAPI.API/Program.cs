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

builder.Services.AddHttpContextAccessor();//Client'tan gelen request neticesinde olu�turulan HttpContex nesnesine katmanlardaki class`lar �zerinden(busineess logic) eri�ebilmemizi sa�layan bir servistir

// Add services to the container.
builder.Services.AddPersistenceServices();//Kendi servislerimizi ekledik
builder.Services.AddInfrastructureServices();//Kendi servislerimizi ekledik
builder.Services.AddApplicationServices();//MetdiatR
//builder.Services.AddStorage(StorageType.Azure);
//builder.Services.AddStorage<LocalStorage>(); // Burada verdi�imiz nesne ne ise uygulama n�n kulland��� Storage o oluyor..
builder.Services.AddSignalRServices(); //Hubs lar i�in
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
    .Enrich.FromLogContext()//contexte tan�mlad���m�z prop u da tan� g�r demi� olduk. user_name
    .CreateLogger();

builder.Host.UseSerilog(log);

//yap�lan requestleri log mekan�zmas� ile yakalamak i�in bu kodu ekledik. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0
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
            ValidateAudience = true, //kimlerin/hangi originlerin/sitelerin kullanaca��n� belirledi�imiz de�er. Token � nerede kullanabilirsin
            ValidateIssuer = true, // Bu token � kim olu�turdu. bizim API misal
            ValidateLifetime = true, //Olu�turulan token de�erinin s�resini kontrol eder.
            ValidateIssuerSigningKey = true, //S�metrik g�venlik key i, uygulamaya �zel key
            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires> DateTime.UtcNow : false,
            NameClaimType = ClaimTypes.Name //JWT �zerinde Name claim i ne kar��l�k gelen de�eri User.Identity.Name propertysinden elde edebiliriz.
            
        };
    });
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());
app.UseStaticFiles();// wwwroot dizinindeki dosyalara eri�ip upload yapabilmek i�in izin veriyor.

app.UseSerilogRequestLogging();//loglama s�reci bu middleware den sonra ba�lar...
app.UseHttpLogging();//Yukar�da "AddHttpLogging" ile bereber istekleri log mekan�zmas� ile yakalamay� sa�larlar
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
//burada middleware tan�m� yap�yoruz ��nk� kullan�c� authentike olmu� mu ona da bak�caz
app.Use(async (context , next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name",username);//user_name i context e g�nderdik, bu sayede e�er b�yle bir ki�i varsa "UsernameColumnWriter" s�n�f� da onun ismini elde edebilecek ve bize d�nd�recektir. 
    await next();
});
app.MapControllers();
app.MapHubs(); // Burada tek tek yazmamak i�in extention fonk olu�turduk ilgili s�n�f da de�erleri verdik.
app.Run();
