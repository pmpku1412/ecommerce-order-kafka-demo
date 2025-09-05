using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.BackOffice.Persistence.Services;

public class ProductService : IProductService
{
    public List<ProductResponse> GetAllProducts()
    {
        return ProductDataService.GetAllProducts();
    }

    public ProductResponse? GetProductById(string productId)
    {
        return ProductDataService.GetProductById(productId);
    }

    public void AddProduct(ProductResponse product)
    {
        ProductDataService.AddProduct(product);
    }

    public void UpdateProduct(string productId, ProductResponse product)
    {
        ProductDataService.UpdateProduct(productId, product);
    }

    public void UpdateStock(string productId, int newStock, string reason)
    {
        ProductDataService.UpdateStock(productId, newStock, reason);
    }

    public void ReduceStock(string productId, int quantity)
    {
        ProductDataService.ReduceStock(productId, quantity);
    }

    public void DeleteProduct(string productId)
    {
        ProductDataService.DeleteProduct(productId);
    }

    public List<string> GetCategories()
    {
        return ProductDataService.GetCategories();
    }

    public bool CheckStockAvailability(string productId, int requiredQuantity)
    {
        return ProductDataService.CheckStockAvailability(productId, requiredQuantity);
    }
}
