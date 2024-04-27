using ETicaretAPI.Application.DTOs.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Configurations
{
    public interface IApplicationService
    {
        List<Menu> GetAuthorizeDefinitionEndPoints(Type assemblyType); //uygulama da yetki gerektiren endpointlerin listesi
    }
}
