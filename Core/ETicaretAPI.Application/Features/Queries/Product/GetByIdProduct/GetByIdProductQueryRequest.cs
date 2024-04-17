using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queryies.Product.GetByIdProduct
{
    public class GetByIdProductQueryRequest: IRequest<GetByIdProductQueryResponse>
    {
        public string Id { get; set; }
    }
}
