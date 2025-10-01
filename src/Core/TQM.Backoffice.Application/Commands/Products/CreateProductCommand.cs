using MediatR;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Core.Application.DTOs.Products.Request;
using TQM.Backoffice.Core.Application.Contracts.Persistence;
using TQM.Backoffice.Domain.Events;
using TQM.Backoffice.Core.Application.Contracts.Infrastructure;

namespace TQM.Backoffice.Core.Application.Commands.Products;

public class CreateProductCommand : IRequest<BaseResponse<ProductResponse>>
{
    public CreateProductRequest? Request { get; set; } = new();
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, BaseResponse<ProductResponse>>
    {
        private readonly IProductService _productService;
        private readonly IKafkaProducer _kafkaProducer;
        
        public CreateProductCommandHandler(IProductService productService, IKafkaProducer kafkaProducer)
        {
            _productService = productService;
            _kafkaProducer = kafkaProducer;
        }
        public async Task<BaseResponse<ProductResponse>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var request = command.Request;
                // Generate new product ID
                var productId = $"PROD{DateTime.Now:yyyyMMddHHmmss}";

                // Create product response
                var product = new ProductResponse
                {
                    Id = productId,
                    Name = request.Name,
                    Price = request.Price,
                    Stock = request.Stock,
                    Category = request.Category,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = "Active"
                };

                // In a real application, this would save to database
                // For now, we'll just return the created product
                _productService.AddProduct(product);

                // Get updated product
                var updatedProduct = _productService.GetProductById(productId);

                // Publish product created event to Kafka
                var productCreatedEvent = new ProductCreatedEvent
                {
                    ProductId = productId,
                    ProductName = updatedProduct.Name,
                    Price = updatedProduct.Price,
                    InitialStock = updatedProduct.Stock,
                    Category = updatedProduct.Category,
                    Description = updatedProduct.Description,
                    ImageUrl = updatedProduct.ImageUrl,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System" // หรือ user ที่สร้าง
                };

                await _kafkaProducer.PublishProductCreatedAsync(productCreatedEvent);

                return new BaseResponse<ProductResponse>
                {
                    Success = true,
                    Message = "Product created successfully",
                    ResponseObject = product
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductResponse>
                {
                    Success = false,
                    Message = $"Error creating product: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
