using MediatR;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Core.Application.DTOs.Products.Request;

namespace TQM.Backoffice.Core.Application.Commands.Products;

public class UpdateProductCommand : IRequest<BaseResponse<ProductResponse>>
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public static UpdateProductCommand FromRequest(string productId, UpdateProductRequest request)
    {
        return new UpdateProductCommand
        {
            ProductId = productId,
            Name = request.Name,
            Price = request.Price,
            Category = request.Category,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };
    }
}
