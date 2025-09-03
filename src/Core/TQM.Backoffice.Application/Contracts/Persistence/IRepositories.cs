using TQM.Backoffice.Domain.Entities;

namespace TQM.Backoffice.Application.Contracts.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(string productId);
    Task<List<Product>> GetAllAsync();
    Task UpdateStockAsync(string productId, int newStock);
}

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(string orderId);
    Task<List<Order>> GetAllAsync();
}
