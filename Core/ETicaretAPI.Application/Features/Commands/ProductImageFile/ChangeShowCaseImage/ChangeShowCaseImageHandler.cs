using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.ChangeShowCaseImage
{
    public class ChangeShowCaseImageHandler : IRequestHandler<ChangeShowCaseImageRequest, ChangeShowCaseImageResponse>
    {
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;

        public ChangeShowCaseImageHandler(IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        public async Task<ChangeShowCaseImageResponse> Handle(ChangeShowCaseImageRequest request, CancellationToken cancellationToken)
        {
            var query = _productImageFileWriteRepository.Table
                .Include(p => p.Product)
                .SelectMany(p => p.Product, (pif, p) => new
                {
                    pif,
                    p
                });
            var data = await query.FirstOrDefaultAsync(p => p.p.Id == Guid.Parse(request.ProductId) && p.pif.Showcase);
            if (data != null)
            data.pif.Showcase = false;

            var image = await query.FirstOrDefaultAsync(p => p.pif.Id == Guid.Parse(request.ImageId));
            if (image != null)
            image.pif.Showcase = true;

            
            await _productImageFileWriteRepository.SaveAsync();

            return new ChangeShowCaseImageResponse();

        }
    }
}
