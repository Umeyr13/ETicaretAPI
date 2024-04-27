using ETicaretAPI.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.CustomAttributes
{
    public class AuthorizeDefinitionAttribute:Attribute
    {
        public string Menu { get; set; }//hangi menü yee ait
        public string Definition { get; set; }
        public ActionTypes ActionTypes { get; set; } //endpoit ne iş yapar
    }
}
