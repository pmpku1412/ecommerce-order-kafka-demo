using TQM.Backoffice.Core.Application.DTOs.Orders.Request;
using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.Contracts.Infrastructure;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.Backoffice.Core.Application.Commands.Orders;

public class CreateOrderCommand : IRequest<BaseResponse<OrderResponse>>
{
    public CreateOrderRequest Request { get; set; } = new();

    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, BaseResponse<OrderResponse>>
    {
        private readonly IOrderService _orderService;
        private readonly IKafkaProducer _kafkaProducer;

        public CreateOrderHandler(IOrderService orderService, IKafkaProducer kafkaProducer)
        {
            _orderService = orderService;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<BaseResponse<OrderResponse>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var response = new BaseResponse<OrderResponse>();
                
                // สร้าง order
                var order = await _orderService.CreateOrderAsync(command.Request);
                
                // Publish event ไป Kafka
                await _kafkaProducer.PublishOrderCreatedAsync(order);
                
                response.ResponseObject = order;
                response.Success = true;
                response.Message = "Order created successfully";

                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse<OrderResponse>() { Message = ex.Message };
            }
        }
    }
}
