using MediatR;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.Backoffice.Core.Application.Commands.Products;

public class DeleteProductCommand : IRequest<BaseResponse<string>>
{
    public string ProductId { get; set; } = string.Empty;

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, BaseResponse<string>>
    {
        private readonly IProductService _productService;

        public DeleteProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<BaseResponse<string>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = _productService.GetProductById(request.ProductId);
            
            if (product == null)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            // Soft delete in shared data service
            _productService.DeleteProduct(request.ProductId);

            return new BaseResponse<string>
            {
                Success = true,
                Message = "Product deleted successfully",
                ResponseObject = request.ProductId
            };
        }
    }
}
