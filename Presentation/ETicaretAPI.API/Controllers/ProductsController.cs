using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;
using ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.ChangeShowCaseImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ETicaretAPI.Application.Features.Queryies.Product.GetAllProduct;
using ETicaretAPI.Application.Features.Queryies.Product.GetByIdProduct;
using ETicaretAPI.Application.Features.Queryies.ProductImageFile.GetProductImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #region
        //  await  _productWriteRepository.AddRangeAsync(new()
        //   {
        //      new () { Id =Guid.NewGuid(), Name ="Product 1", Price=100, CreateDate= DateTime.UtcNow, Stock = 10  },
        //      new () { Id =Guid.NewGuid(), Name ="Product 2", Price=200, CreateDate= DateTime.UtcNow, Stock = 20  },
        //      new () { Id =Guid.NewGuid(), Name ="Product 3", Price=300, CreateDate= DateTime.UtcNow, Stock = 30  }
        //   });
        //var count =   await _productWriteRepository.SaveAsync();

        //Product p = await _productReadRepository.GetByIdAsync("c8384245-ac81-4929-acd8-3d9c2dca62d7", false);
        // p.Name = "Merhmet";
        // await _productWriteRepository.SaveAsync();
        //var cutomerId =Guid.NewGuid();
        //await _customerWriteRepository.AddAsync(new Customer {Id=cutomerId,Name="Muhiddin" });

        //await _orderWriteRepository.AddAsync(new() { Description = "asdas", Address = "istanbul,Beylikdüzü", CustomerId = cutomerId   });

        //await _orderWriteRepository.AddAsync(new() { Description = "ghjsdasdas", Address = "istanbul,Avcılar",CustomerId = cutomerId });

        //await _orderWriteRepository.AddAsync(new() { Description = "sdfdasdasddas", Address = "istanbul,Esenyurt", CustomerId = cutomerId });
        //await _orderWriteRepository.SaveAsync();

        //Order order = await _orderReadRepository.GetByIdAsync("2683bf71-0a52-42eb-92fb-da237660d9f7");
        //order.Address = "Ankara";
        //await _orderWriteRepository.SaveAsync();
        #endregion

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);

        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] GetByIdProductQueryRequest getByIdProductQueryRequest)
        {
            GetByIdProductQueryResponse response = await _mediator.Send(getByIdProductQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return Ok((int)HttpStatusCode.Created);//201
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> Put([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
        {
            UpdateProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);
            return Ok();
        }

        [HttpDelete("{Id}")]//root data dan id yi yakalamak için
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] RemoveProductCommandRequest removeProductCommandRequest)
        {
            RemoveProductCommandResponse response = await _mediator.Send(removeProductCommandRequest);
            return Ok();
        }

        [HttpPost("[action]")] //query string olarak id gelir
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> Upload([FromQuery] UploadProductImageCommandRequest uploadProductImageCommandRequest)
        {
            uploadProductImageCommandRequest.Files = Request.Form.Files;
            UploadProductImageCommandResponse response = await _mediator.Send(uploadProductImageCommandRequest);
            return Ok();
        }

        [HttpGet("[action]/{Id}")]//  burada "/id " diye belirttiğimiz için root data olarak geliyor
        public async Task<IActionResult> GetProductImages([FromRoute] GetProductImagesQueryRequest getProductImagesQueryRequest)
        {
            List<GetProductImagesQueryResponse> response = await _mediator.Send(getProductImagesQueryRequest);
            return Ok(response);
        }

        [HttpDelete("[action]/{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] RemoveProductImageCommandRequest removeProductImageCommandRequest, [FromQuery] string ImageId)
        {
            removeProductImageCommandRequest.ImageId = ImageId;
            RemoveProductImageCommandResponse response = await _mediator.Send(removeProductImageCommandRequest);
            return Ok();


        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> ChangeShowCaseImage([FromQuery] ChangeShowCaseImageRequest changeShowCaseImageRequest)
        {
            ChangeShowCaseImageResponse response = await _mediator.Send(changeShowCaseImageRequest);
            return Ok(response);
        }
    }
}
