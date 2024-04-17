using ETicaretAPI.Application.RequestParameters;
using MediatR;

namespace ETicaretAPI.Application.Features.Queryies.Product.GetAllProduct
{
    public class GetAllProductQueryRequest : IRequest<GetAllProductQueryResponse>//geriye döndürelecek sınıfı bildirdik. MediatR
    {
        //public Pagination pagination { get; set; }
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 5;
    }
}
