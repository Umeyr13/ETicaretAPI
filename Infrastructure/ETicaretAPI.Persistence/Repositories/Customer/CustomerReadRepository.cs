using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    public class CustomerReadRepository : ReadRepository<Customer>, ICustomerReadRepository
    {
        /*CustomerReadRepository aslında bir  ReadRepository olduğu için ve biz de ReadRepository<Customer> diyerek ICustomerReadRepository interface inin istediklerini doldurabiliyoruz. Tekrar yazmak yerine ReadRepository<Customer> dan dolu bir şekilde türetmiş olduk. (soyut yapılanma)
         * Ctor da base context göndermeliyiz o ayrı zaten. Onu ReadRepository istiyor
         */

        public CustomerReadRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
