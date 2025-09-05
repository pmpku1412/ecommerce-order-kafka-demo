using MediatR;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Core.Application.DTOs.Products.Request;
using TQM.Backoffice.Core.Application.Contracts.Persistence;
using TQM.Backoffice.Core.Application.Contracts.Infrastructure;
using TQM.Backoffice.Domain.Events;

namespace TQM.Backoffice.Core.Application.Commands.Products;

public class UpdateStockCommand : IRequest<BaseResponse<ProductResponse>>
{
    public UpdateStockRequest? Request { get; set; } = new();

    public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, BaseResponse<ProductResponse>>
    {
        private readonly IProductService _productService;
        private readonly IKafkaProducer _kafkaProducer;

        public UpdateStockCommandHandler(IProductService productService, IKafkaProducer kafkaProducer)
        {
            _productService = productService;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<BaseResponse<ProductResponse>> Handle(UpdateStockCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request!;
            var product = _productService.GetProductById(request.ProductId);

            if (product == null)
            {
                return new BaseResponse<ProductResponse>
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            var previousStock = product.Stock;

            // Update stock in shared data service
            _productService.UpdateStock(request.ProductId, request.Stock, request.Reason);

            // Get updated product
            var updatedProduct = _productService.GetProductById(request.ProductId);

            // Publish stock updated event to Kafka
            var stockEvent = new StockUpdatedEvent
            {
                ProductId = request.ProductId,
                ProductName = product.Name,
                PreviousStock = previousStock,
                NewStock = request.Stock,
                QuantityChanged = request.Stock - previousStock,
                UpdatedDate = DateTime.UtcNow,
                Reason = "MANUAL_ADJUSTMENT",
                OrderId = null
            };

            await _kafkaProducer.PublishStockUpdatedAsync(stockEvent);

            return new BaseResponse<ProductResponse>
            {
                Success = true,
                Message = $"Product stock updated successfully. Reason: {request.Reason}",
                ResponseObject = updatedProduct
            };
        }
    }
}
