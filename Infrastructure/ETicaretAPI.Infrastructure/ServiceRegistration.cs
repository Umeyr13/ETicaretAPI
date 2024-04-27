
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.Abstractions.Stroge;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Infrastructure.Enums;
using ETicaretAPI.Infrastructure.Services;
using ETicaretAPI.Infrastructure.Services.Configurations;
using ETicaretAPI.Infrastructure.Services.Storage;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IStorageService, StorageService>(); //IStorageService aynı zamanda bir IStorage olduğu için IStorage gelen nesne ne     ise IStorageService e de gelmiş oluyor
            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
            serviceCollection.AddScoped<IMailService, MailService>();
            serviceCollection.AddScoped<IApplicationService, ApplicationService>();
        }

        public static void AddStorage<T>(this IServiceCollection serviceCollection) where T : Storage, IStorage // IStorage ı implemente eden ve Storage sınıfından türeyen bütün  sınıflar buraya verilebilir dedik...
        {
            serviceCollection.AddScoped<IStorage,T>(); //IStorage e karşılık hangi nesnenin verileceğini Program.cs de belirtiyoruz.
        }
        public static void AddStorage<T>(this IServiceCollection serviceCollection,StorageType storageType) where T : class, IStorage
        {//örnek olsun
            switch (storageType)
            {
                case StorageType.Local:
                    serviceCollection.AddScoped<IStorage, LocalStorage>();
                    break;
                case StorageType.Azure:
                    serviceCollection.AddScoped<IStorage,AzureStorage>();
                    break;
                case StorageType.AWS:
                    break;
                default:
                    serviceCollection.AddScoped<IStorage, LocalStorage>();
                    break;
            }
        }
    }
}
