using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateProduct
{

    public class UpdateProductCommandHandle : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductWriteRepository _productWriteRepository;
        readonly ILogger<UpdateProductCommandHandle> _logger;
        public UpdateProductCommandHandle(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, ILogger<UpdateProductCommandHandle> logger = null)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _logger = logger;
        }

        public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Product products = await _productReadRepository.GetByIdAsync(request.Id);
            products.Stock = request.Stock;
            products.Name = request.Name;
            products.Price = request.Price;
            await _productWriteRepository.SaveAsync();
            
            return new();
        }
    }
}
