using ETicaretAPI.Application.Consts;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using ETicaretAPI.Application.Features.Commands.Order.CompleteOrder;
using ETicaretAPI.Application.Features.Commands.Order.CreateOrder;
using ETicaretAPI.Application.Features.Queries.Order.GetAllOrders;
using ETicaretAPI.Application.Features.Queries.Order.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes ="Admin")]
    public class OrdersController : ControllerBase
    {
        readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionTypes = ActionTypes.Reading, Definition = "Get All Orders")]
        public async Task<ActionResult> GetAllOrders([FromQuery]GetAllOrdersQueryRequest getAllOrdersQueryRequest)
        {
            GetAllOrdersQueryResponse response = await _mediator.Send(getAllOrdersQueryRequest);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionTypes = ActionTypes.Reading, Definition = "Get Order By Id")]
        public async Task<ActionResult> GetOrderById([FromRoute]GetOrderByIdQueryRequest getOrderByIdQueryRequest )
        {
            GetOrderByIdQueryResponse response = await _mediator.Send(getOrderByIdQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionTypes = ActionTypes.Writing, Definition = "Create Order")]
        public async Task<ActionResult> CreateOrder(CreateOrderCommandRequest createOrderCommandRequest)
        {
            CreateOrderCommandResponse response = await _mediator.Send(createOrderCommandRequest);
            return Ok(response);
        }

        [HttpGet("complete-order/{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionTypes = ActionTypes.Updating, Definition = "Complete Order")]
        public async Task<IActionResult> CopletedOrder([FromRoute]CompleteOrderCommandRequest completeOrderCommandRequest)
        {
            CompleteOrderCommandResponse response = await _mediator.Send(completeOrderCommandRequest);
            return Ok(response);
        }

    }
}
