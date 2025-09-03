using TQM.Backoffice.Application.Contracts.Persistence;
using TQM.Backoffice.Domain.Entities;

namespace TQM.BackOffice.Persistence.Services;

public class InMemoryProductRepository : IProductRepository
{
    private static readonly List<Product> _products = new()
    {
        new Product { ProductId = "P001", ProductName = "iPhone 15", Price = 30000, StockQuantity = 50, Category = "Electronics" },
        new Product { ProductId = "P002", ProductName = "Samsung Galaxy S24", Price = 25000, StockQuantity = 30, Category = "Electronics" },
        new Product { ProductId = "P003", ProductName = "MacBook Pro", Price = 80000, StockQuantity = 20, Category = "Electronics" },
        new Product { ProductId = "P004", ProductName = "iPad Air", Price = 20000, StockQuantity = 40, Category = "Electronics" },
        new Product { ProductId = "P005", ProductName = "AirPods Pro", Price = 8000, StockQuantity = 100, Category = "Accessories" }
    };

    public Task<Product?> GetByIdAsync(string productId)
    {
        var product = _products.FirstOrDefault(p => p.ProductId == productId);
        return Task.FromResult(product);
    }

    public Task<List<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.ToList());
    }

    public Task UpdateStockAsync(string productId, int newStock)
    {
        var product = _products.FirstOrDefault(p => p.ProductId == productId);
        if (product != null)
        {
            product.StockQuantity = newStock;
        }
        return Task.CompletedTask;
    }
}

public class InMemoryOrderRepository : IOrderRepository
{
    private static readonly List<Order> _orders = new();

    public Task<Order> CreateAsync(Order order)
    {
        _orders.Add(order);
        return Task.FromResult(order);
    }

    public Task<Order?> GetByIdAsync(string orderId)
    {
        var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
        return Task.FromResult(order);
    }

    public Task<List<Order>> GetAllAsync()
    {
        return Task.FromResult(_orders.ToList());
    }
}
