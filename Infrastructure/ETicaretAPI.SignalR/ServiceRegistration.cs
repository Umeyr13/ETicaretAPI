using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.SignalR.HubServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.SignalR
{
    public static class ServiceRegistration
    {
        public static void AddSignalRServices(this IServiceCollection colllection)
        {
            colllection.AddTransient<IProductHubService, ProductHubService>();
            colllection.AddTransient<IOrderHubService, OrderHubService>();
            colllection.AddSignalR();//Hub ile ilgili hubContext vs. kendisi ile alakalı şeyleri Ioc ye atıyor bizde ilgili yerde kullanıyoruz..
        }
    }
}
