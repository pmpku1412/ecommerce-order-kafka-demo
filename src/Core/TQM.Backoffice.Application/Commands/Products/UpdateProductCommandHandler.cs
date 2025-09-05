using MediatR;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.Backoffice.Core.Application.Commands.Products;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, BaseResponse<ProductResponse>>
{
    private readonly IProductService _productService;

    public UpdateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<BaseResponse<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _productService.GetProductById(request.ProductId);
        
        if (product == null)
        {
            return new BaseResponse<ProductResponse>
            {
                Success = false,
                Message = "Product not found"
            };
        }

        // Create updated product
        var updatedProduct = new ProductResponse
        {
            Id = request.ProductId,
            Name = request.Name,
            Price = request.Price,
            Category = request.Category,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Stock = product.Stock, // Keep existing stock
            CreatedAt = product.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            Status = product.Status
        };

        // Update in shared data service
        _productService.UpdateProduct(request.ProductId, updatedProduct);

        return new BaseResponse<ProductResponse>
        {
            Success = true,
            Message = "Product updated successfully",
            ResponseObject = updatedProduct
        };
    }
}
