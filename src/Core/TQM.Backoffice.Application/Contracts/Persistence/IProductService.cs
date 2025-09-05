using TQM.Backoffice.Core.Application.DTOs.Products.Response;

namespace TQM.Backoffice.Core.Application.Contracts.Persistence;

public interface IProductService
{
    List<ProductResponse> GetAllProducts();
    ProductResponse? GetProductById(string productId);
    void AddProduct(ProductResponse product);
    void UpdateProduct(string productId, ProductResponse product);
    void UpdateStock(string productId, int newStock, string reason);
    void ReduceStock(string productId, int quantity);
    void DeleteProduct(string productId);
    List<string> GetCategories();
    bool CheckStockAvailability(string productId, int requiredQuantity);
}
