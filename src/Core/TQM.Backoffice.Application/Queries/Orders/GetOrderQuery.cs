using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.Backoffice.Core.Application.Queries.Orders;

public class GetOrderQuery : IRequest<BaseResponse<OrderResponse>>
{
    public int OrderId { get; set; }

    public class GetOrderHandler : IRequestHandler<GetOrderQuery, BaseResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;

        public GetOrderHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseResponse<OrderResponse>> Handle(GetOrderQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var response = new BaseResponse<OrderResponse>();
                var order = await _orderService.GetOrderByIdAsync(query.OrderId);
                
                if (order == null)
                {
                    response.Success = false;
                    response.Message = "Order not found";
                    return response;
                }

                response.ResponseObject = order;
                response.Success = true;
                response.Message = "Success";

                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse<OrderResponse>() { Message = ex.Message };
            }
        }
    }
}
