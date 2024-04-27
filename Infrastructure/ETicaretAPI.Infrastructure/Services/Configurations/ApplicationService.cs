using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs.Configuration;
using ETicaretAPI.Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Configurations
{
    internal class ApplicationService : IApplicationService
    {
        public List<Menu> GetAuthorizeDefinitionEndPoints(Type assemblyType)
        {
            Assembly assembly = Assembly.GetAssembly(assemblyType);//hangi type ı verdiysek orada çalışan assembly ler
            var controllers = assembly.GetTypes()//sistemdeki bütün tür,class hepsi gelir o an çalışan
                .Where(t => t.IsAssignableTo(typeof(ControllerBase)));//çalışan controller listesi
            List<Menu> menus = new();
            if (controllers != null)
                foreach (var controller in controllers)
                {
                    var actions = controller.GetMethods().Where(m=>m.IsDefined(typeof(AuthorizeDefinitionAttribute)));//controller daki ile işaretli methodlar
                    if (actions != null)
                        foreach (var action in actions)
                        {
                            Menu menu =null;
                            var attributes= action.GetCustomAttributes(true);
                            if (attributes != null)
                            {
                                var authorizeDefinitionAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;
                                if (!menus.Any(m => m.Name == authorizeDefinitionAttribute.Menu))
                                {
                                    menu = new() { Name = authorizeDefinitionAttribute.Menu };
                                    menus.Add(menu);
                                }
                                else
                                    menu = menus.FirstOrDefault(m=> m.Name == authorizeDefinitionAttribute.Menu);

                                Application.DTOs.Configuration.Action _action = new()
                                {
                                    ActionType = Enum.GetName(typeof(ActionTypes),authorizeDefinitionAttribute.ActionTypes),
                                    Definition = authorizeDefinitionAttribute.Definition
                                };

                                var httpAttribute = attributes.FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpMethodAttribute;//HttpMethodAttribute den kalıtım alanı getir böylelikle türünü yakaladık "Get, Post vs "
                                if (httpAttribute != null)
                                    _action.HttpType = httpAttribute.HttpMethods.First();
                                else
                                    _action.HttpType = HttpMethods.Get;
                                _action.Code = $"{_action.HttpType}.{_action.ActionType}.{_action.Definition.Replace(" ","")}";
                                menu.Actions.Add(_action);
                            }
                        }
                }
            return menus;
          
        }
    }
}
