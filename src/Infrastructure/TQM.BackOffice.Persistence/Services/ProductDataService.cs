using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Domain.Entities;

namespace TQM.BackOffice.Persistence.Services;

public static class ProductDataService
{
    private static readonly List<ProductResponse> _products = new()
    {
        new ProductResponse
        {
            Id = "PROD001",
            Name = "Laptop Dell XPS 13",
            Price = 45000.00m,
            Stock = 10,
            Category = "Electronics",
            ImageUrl = "https://placehold.jp/1c1c1c/ffffff/300x200.png?text=Laptop%20Dell%20XPS%2013",
            Description = "High-performance laptop with Intel Core i7",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-5),
            Status = "Active"
        },
        new ProductResponse
        {
            Id = "PROD002",
            Name = "iPhone 15 Pro",
            Price = 35000.00m,
            Stock = 15,
            Category = "Electronics",
            ImageUrl = "https://placehold.jp/1c1c1c/ffffff/300x200.png?text=iPhone%2015%20Pro",
            Description = "Latest iPhone model with advanced features",
            CreatedAt = DateTime.UtcNow.AddDays(-25),
            UpdatedAt = DateTime.UtcNow.AddDays(-3),
            Status = "Active"
        },
        new ProductResponse
        {
            Id = "PROD003",
            Name = "Samsung Galaxy S24",
            Price = 28000.00m,
            Stock = 20,
            Category = "Electronics",
            ImageUrl = "https://placehold.jp/1c1c1c/ffffff/300x200.png?text=Samsung%20Galaxy%20S24",
            Description = "Android flagship phone with excellent camera",
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            Status = "Active"
        },
        new ProductResponse
        {
            Id = "PROD004",
            Name = "MacBook Air M2",
            Price = 42000.00m,
            Stock = 8,
            Category = "Electronics",
            ImageUrl = "https://placehold.jp/1c1c1c/ffffff/300x200.png?text=MacBook%20Air%20M2",
            Description = "Apple laptop with M2 chip and long battery life",
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            Status = "Active"
        },
        new ProductResponse
        {
            Id = "PROD005",
            Name = "AirPods Pro",
            Price = 8500.00m,
            Stock = 25,
            Category = "Electronics",
            ImageUrl = "https://placehold.jp/1c1c1c/ffffff/300x200.png?text=AirPods%20Pro",
            Description = "Wireless earbuds with noise cancellation",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow,
            Status = "Active"
        },
        new ProductResponse
        {
            Id = "PROD006",
            Name = "iPad Pro 12.9",
            Price = 38000.00m,
            Stock = 12,
            Category = "Electronics",
            ImageUrl = "https://placehold.jp/1c1c1c/ffffff/300x200.png?text=iPad%20Pro%2012.9",
            Description = "Professional tablet with M2 chip",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow,
            Status = "Active"
        }
    };

    public static List<ProductResponse> GetAllProducts()
    {
        return _products.Where(p => p.Status == "Active").ToList();
    }

    public static ProductResponse? GetProductById(string productId)
    {
        return _products.FirstOrDefault(p => p.Id == productId && p.Status == "Active");
    }

    public static void AddProduct(ProductResponse product)
    {
        _products.Add(product);
    }

    public static bool UpdateProduct(string productId, ProductResponse updatedProduct)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId && p.Status == "Active");
        if (product == null) return false;

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        product.Category = updatedProduct.Category;
        product.Description = updatedProduct.Description;
        product.ImageUrl = updatedProduct.ImageUrl;
        product.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public static bool UpdateStock(string productId, int newStock, string reason = "")
    {
        var product = _products.FirstOrDefault(p => p.Id == productId && p.Status == "Active");
        if (product == null) return false;

        product.Stock = newStock;
        product.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public static bool ReduceStock(string productId, int quantity)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId && p.Status == "Active");
        if (product == null) return false;

        if (product.Stock < quantity) return false;

        product.Stock -= quantity;
        product.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public static bool DeleteProduct(string productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId && p.Status == "Active");
        if (product == null) return false;

        product.Status = "Deleted";
        product.UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public static List<string> GetCategories()
    {
        return _products
            .Where(p => p.Status == "Active")
            .Select(p => p.Category)
            .Distinct()
            .ToList();
    }

    public static bool CheckStockAvailability(string productId, int requiredQuantity)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId && p.Status == "Active");
        return product != null && product.Stock >= requiredQuantity;
    }
}
