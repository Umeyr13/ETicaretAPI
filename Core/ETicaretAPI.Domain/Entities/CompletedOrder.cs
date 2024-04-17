using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class CompletedOrder: BaseEntity
    {
        public Guid OrderId { get; set; }//bu tabloda ıd si olan sipariş tamamlanmıştır
        public Order Order { get; set; }
    }
}
