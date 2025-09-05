using MediatR;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.Contracts.Persistence;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;

namespace TQM.Backoffice.Core.Application.Queries.Products;

public class GetProductsQuery : IRequest<BaseResponse<List<ProductResponse>>>
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, BaseResponse<List<ProductResponse>>>
    {
        private readonly IProductService _productService;

        public GetProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<BaseResponse<List<ProductResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = _productService.GetAllProducts();
            
            return new BaseResponse<List<ProductResponse>>
            {
                Success = true,
                Message = "Products retrieved successfully",
                ResponseObject = products
            };
        }
    }
}
