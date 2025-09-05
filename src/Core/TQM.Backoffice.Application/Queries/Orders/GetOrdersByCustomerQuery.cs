using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.Backoffice.Core.Application.Queries.Orders;

public class GetOrdersByCustomerQuery : IRequest<BaseResponse<List<OrderResponse>>>
{
    public string CustomerId { get; set; } = string.Empty;

    public class GetOrdersByCustomerHandler : IRequestHandler<GetOrdersByCustomerQuery, BaseResponse<List<OrderResponse>>>
    {
        private readonly IOrderService _orderService;

        public GetOrdersByCustomerHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseResponse<List<OrderResponse>>> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var response = new BaseResponse<List<OrderResponse>>();
                var orders = await _orderService.GetOrdersByCustomerIdAsync(query.CustomerId);

                response.ResponseObject = orders;
                response.Success = true;
                response.Message = "Success";

                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<OrderResponse>>() { Message = ex.Message };
            }
        }
    }
}
