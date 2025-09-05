using Microsoft.AspNetCore.Mvc;
using API.Controllers;
using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Core.Application.Commands.Orders;
using TQM.Backoffice.Core.Application.Queries.Orders;

namespace TQM.BackOffice.API.Controllers;

[Route("[controller]")]
[Authorize, EnableCors, ApiController]
public class OrderController : ApiControllerBase
{
    [HttpPost("CreateOrder")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<OrderResponse>>> CreateOrder(CreateOrderCommand command) 
        => Ok(await Mediator.Send(command));

    [HttpGet("GetOrder/{orderId}")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<OrderResponse>>> GetOrder(int orderId)
    {
        var query = new GetOrderQuery { OrderId = orderId };
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("GetAllOrders")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<List<OrderResponse>>>> GetAllOrders()
    {
        var query = new GetAllOrdersQuery();
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("GetOrdersByCustomer/{customerId}")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<List<OrderResponse>>>> GetOrdersByCustomer(string customerId)
    {
        var query = new GetOrdersByCustomerQuery { CustomerId = customerId };
        return Ok(await Mediator.Send(query));
    }
}
