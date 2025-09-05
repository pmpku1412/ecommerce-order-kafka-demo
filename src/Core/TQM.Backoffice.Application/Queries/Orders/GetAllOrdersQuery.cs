using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.Backoffice.Core.Application.Queries.Orders;

public class GetAllOrdersQuery : IRequest<BaseResponse<List<OrderResponse>>>
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, BaseResponse<List<OrderResponse>>>
    {
        private readonly IOrderService _orderService;

        public GetAllOrdersHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseResponse<List<OrderResponse>>> Handle(GetAllOrdersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var response = new BaseResponse<List<OrderResponse>>();
                var orders = await _orderService.GetAllOrdersAsync();

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
