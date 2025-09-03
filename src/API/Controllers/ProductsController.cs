using Microsoft.AspNetCore.Mvc;
using TQM.Backoffice.Application.Contracts.Persistence;
using TQM.Backoffice.Domain.Entities;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ApiControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>List of products</returns>
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAllProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{productId}")]
    public async Task<ActionResult<Product>> GetProduct(string productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            return NotFound($"Product with ID {productId} not found");
        }

        return Ok(product);
    }
}
