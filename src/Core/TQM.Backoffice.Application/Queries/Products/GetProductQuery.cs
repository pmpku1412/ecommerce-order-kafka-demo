using MediatR;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.Contracts.Persistence;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;

namespace TQM.Backoffice.Core.Application.Queries.Products;

public class GetProductQuery : IRequest<BaseResponse<ProductResponse>>
{
    public string ProductId { get; set; } = string.Empty;

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, BaseResponse<ProductResponse>>
    {
        private readonly IProductService _productService;

        public GetProductQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<BaseResponse<ProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
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

            return new BaseResponse<ProductResponse>
            {
                Success = true,
                Message = "Product retrieved successfully",
                ResponseObject = product
            };
        }
    }

}
