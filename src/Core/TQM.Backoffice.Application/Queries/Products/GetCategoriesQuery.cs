using MediatR;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.Backoffice.Core.Application.Queries.Products;

public class GetCategoriesQuery : IRequest<BaseResponse<List<string>>>
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, BaseResponse<List<string>>>
    {
        private readonly IProductService _productService;

        public GetCategoriesQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<BaseResponse<List<string>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = _productService.GetCategories();

            return new BaseResponse<List<string>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                ResponseObject = categories
            };
        }
    }
}
