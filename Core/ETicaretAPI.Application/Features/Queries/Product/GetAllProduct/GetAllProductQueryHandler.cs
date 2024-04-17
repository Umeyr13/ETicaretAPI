using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queryies.Product.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly ILogger<GetAllProductQueryHandler> _logger;
        public GetAllProductQueryHandler(IProductReadRepository productReadRepository, ILogger<GetAllProductQueryHandler> logger = null)
        {
            _productReadRepository = productReadRepository;
            _logger = logger;
        }

        async Task<GetAllProductQueryResponse> IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>.Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ürünler Listelendi");
            var totalProductCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false).Skip(request.Page * request.Size).Take(request.Size)
                .Include(p => p.ProductImageFiles)
                .Select(p => new
            {
                p.Id,
                p.Name,
                p.Stock,
                p.Price,
                p.CreateDate,
                p.UpdatedDate,
                p.ProductImageFiles
            }).ToList();// her sayfa 5 ise 3 sayfa (3*5 taner veri) artı bir sayfa kadar (5) daha getir dedik toplam 20 misal     
            return new()
            {
                Products = products,
                TotalProductCount = totalProductCount
            };

        }
    }
}
