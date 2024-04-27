using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.Configuration
{
    public class Menu
    {
        public string Name { get; set; }
        public List<Action> Actions { get; set; } = new();//Hangi menü deysek onun altındaki authorize gerektiren bütün action ları topladık
    }
}
