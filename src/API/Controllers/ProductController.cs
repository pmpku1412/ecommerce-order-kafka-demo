using Microsoft.AspNetCore.Mvc;
using API.Controllers;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Core.Application.DTOs.Products.Request;
using TQM.Backoffice.Core.Application.Commands.Products;
using TQM.Backoffice.Core.Application.Queries.Products;

namespace TQM.BackOffice.API.Controllers;

[Route("[controller]")]
[Authorize, EnableCors, ApiController]
public class ProductController : ApiControllerBase
{
    [HttpGet("GetProducts")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<List<ProductResponse>>>> GetProducts()
    {
        var query = new GetProductsQuery();
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("GetProduct/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<ProductResponse>>> GetProduct(string productId)
    {
        var query = new GetProductQuery { ProductId = productId };
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("CreateProduct")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<ProductResponse>>> CreateProduct(CreateProductCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPut("UpdateProduct/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<ProductResponse>>> UpdateProduct(string productId, UpdateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new BaseResponse<ProductResponse>
            {
                Success = false,
                Message = "Invalid product data",
                Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
            });
        }

        var command = UpdateProductCommand.FromRequest(productId, request);
        return Ok(await Mediator.Send(command));
    }

    [HttpPut("UpdateStock")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<ProductResponse>>> UpdateStock(UpdateStockCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpDelete("DeleteProduct/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<string>>> DeleteProduct(string productId)
    {
        var command = new DeleteProductCommand { ProductId = productId };
        return Ok(await Mediator.Send(command));
    }

    [HttpGet("GetCategories")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<List<string>>>> GetCategories()
    {
        var query = new GetCategoriesQuery();
        return Ok(await Mediator.Send(query));
    }
}
