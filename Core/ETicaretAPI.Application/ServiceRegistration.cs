using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection collection) 
        {
            collection.AddMediatR(typeof(ServiceRegistration));//Bu  assebly deki sınıfları yaka demiş olduk. Yani apllication katmanındaki MediatR nesnelerini yakalayacak.
            collection.AddHttpClient();
        }
    }
}
