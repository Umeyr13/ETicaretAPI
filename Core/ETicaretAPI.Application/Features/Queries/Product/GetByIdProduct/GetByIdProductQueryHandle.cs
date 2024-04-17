using ETicaretAPI.Application.Repositories;
using MediatR;

namespace ETicaretAPI.Application.Features.Queryies.Product.GetByIdProduct
{
    public class GetByIdProductQueryHandle : IRequestHandler<GetByIdProductQueryRequest, GetByIdProductQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository;

        public GetByIdProductQueryHandle(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<GetByIdProductQueryResponse> Handle(GetByIdProductQueryRequest request, CancellationToken cancellationToken)
        {
          ETicaretAPI.Domain.Entities.Product product = await _productReadRepository.GetByIdAsync(request.Id, false);
            return new() { 
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };
        }
    }
}
