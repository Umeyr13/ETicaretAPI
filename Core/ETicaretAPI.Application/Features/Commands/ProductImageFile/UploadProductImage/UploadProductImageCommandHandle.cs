using ETicaretAPI.Application.Abstractions.Stroge;
using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage
{
    public class UploadProductImageCommandHandle : IRequestHandler<UploadProductImageCommandRequest, UploadProductImageCommandResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductWriteRepository _productWriteRepository;
        readonly IStorageService _storageService;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        public UploadProductImageCommandHandle(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository, IStorageService storageService, IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _storageService = storageService;
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        public async Task<UploadProductImageCommandResponse> Handle(UploadProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            #region
            //  var datas = await _storageService.UploadAsync("files",Request.Form.Files);
            ////var datas = await _fileService.UploadAsync("resource/product-images",Request.Form.Files);
            //await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d=> new ProductImageFile()
            //{
            //   FileName = d.fileName,
            //   Path = d.pathOrContainerName,
            //   Storage = _storageService.StorageName

            // }).ToList());

            //await _productImageFileWriteRepository.SaveAsync();
            //  return Ok();
            #endregion
            
            List<(string fileName, string path)> result = await _storageService.UploadAsync("photo-images", request.Files);
            Domain.Entities.Product product = await _productReadRepository.GetByIdAsync(request.Id);
            await _productImageFileWriteRepository.AddRangeAsync(result.Select(r => new Domain.Entities.ProductImageFile
            {
                FileName = r.fileName,
                Path = r.path,
                Storage = _storageService.StorageName,
                Product = new List<Domain.Entities.Product>() { product }
            }).ToList());

            await _productImageFileWriteRepository.SaveAsync();
            return new();
        }
    }
}
