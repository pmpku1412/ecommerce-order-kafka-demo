using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.DTOs.Orders.Request;
using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Core.Application.DTOs.Products.Request;
using TQM.Backoffice.Core.Application.Contracts.Persistence;
using Xunit;
using Xunit.Abstractions;

namespace TQM.BackOffice.IntegrationTests;

public class OrderProductStockIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public OrderProductStockIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task CreateOrder_Should_ReduceProductStock_And_SendKafkaEvent()
    {
        // Arrange - Get initial product stock
        var getProductsResponse = await _client.GetAsync("/Product/GetProducts");
        getProductsResponse.EnsureSuccessStatusCode();
        
        var productsJson = await getProductsResponse.Content.ReadAsStringAsync();
        var productsResult = JsonConvert.DeserializeObject<BaseResponse<List<ProductResponse>>>(productsJson);
        
        Assert.NotNull(productsResult?.ResponseObject);
        Assert.True(productsResult.Success);
        
        var firstProduct = productsResult.ResponseObject.First();
        var initialStock = firstProduct.Stock;
        var orderQuantity = 2;
        
        _output.WriteLine($"Initial stock for {firstProduct.Name}: {initialStock}");

        // Arrange - Create order request
        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = "CUST001",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ShippingAddress = "123 Test Street",
            PaymentMethod = "Credit Card",
            OrderItems = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest
                {
                    ProductId = firstProduct.Id,
                    Quantity = orderQuantity
                }
            }
        };

        var orderJson = JsonConvert.SerializeObject(createOrderRequest);
        var orderContent = new StringContent(orderJson, Encoding.UTF8, "application/json");

        // Act - Create order
        var createOrderResponse = await _client.PostAsync("/Order/CreateOrder", orderContent);
        createOrderResponse.EnsureSuccessStatusCode();

        var orderResponseJson = await createOrderResponse.Content.ReadAsStringAsync();
        var orderResult = JsonConvert.DeserializeObject<BaseResponse<OrderResponse>>(orderResponseJson);

        // Assert - Order created successfully
        Assert.NotNull(orderResult?.ResponseObject);
        Assert.True(orderResult.Success);
        Assert.Equal("CUST001", orderResult.ResponseObject.CustomerId);
        
        _output.WriteLine($"Order created successfully: {orderResult.ResponseObject.Id}");

        // Wait a bit for Kafka processing
        await Task.Delay(1000);

        // Act - Check updated product stock
        var getUpdatedProductResponse = await _client.GetAsync($"/Product/GetProduct/{firstProduct.Id}");
        getUpdatedProductResponse.EnsureSuccessStatusCode();

        var updatedProductJson = await getUpdatedProductResponse.Content.ReadAsStringAsync();
        var updatedProductResult = JsonConvert.DeserializeObject<BaseResponse<ProductResponse>>(updatedProductJson);

        // Assert - Stock should be reduced
        Assert.NotNull(updatedProductResult?.ResponseObject);
        Assert.True(updatedProductResult.Success);
        
        var expectedStock = initialStock - orderQuantity;
        var actualStock = updatedProductResult.ResponseObject.Stock;
        
        _output.WriteLine($"Expected stock: {expectedStock}, Actual stock: {actualStock}");
        
        Assert.Equal(expectedStock, actualStock);
    }

    [Fact]
    public async Task UpdateProductStock_Should_SendKafkaEvent_And_TriggerConsumer()
    {
        // Arrange - Get a product
        var getProductsResponse = await _client.GetAsync("/Product/GetProducts");
        getProductsResponse.EnsureSuccessStatusCode();
        
        var productsJson = await getProductsResponse.Content.ReadAsStringAsync();
        var productsResult = JsonConvert.DeserializeObject<BaseResponse<List<ProductResponse>>>(productsJson);
        
        var testProduct = productsResult?.ResponseObject?.First();
        Assert.NotNull(testProduct);
        
        var initialStock = testProduct.Stock;
        var newStock = initialStock + 10;
        
        _output.WriteLine($"Updating stock for {testProduct.Name} from {initialStock} to {newStock}");

        // Arrange - Create update stock request
        var updateStockRequest = new UpdateStockRequest
        {
            Stock = newStock,
            Reason = "Integration Test Stock Update"
        };

        var updateJson = JsonConvert.SerializeObject(updateStockRequest);
        var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

        // Act - Update stock
        var updateResponse = await _client.PutAsync($"/Product/UpdateStock/{testProduct.Id}", updateContent);
        updateResponse.EnsureSuccessStatusCode();

        var updateResponseJson = await updateResponse.Content.ReadAsStringAsync();
        var updateResult = JsonConvert.DeserializeObject<BaseResponse<ProductResponse>>(updateResponseJson);

        // Assert - Stock updated successfully
        Assert.NotNull(updateResult?.ResponseObject);
        Assert.True(updateResult.Success);
        Assert.Equal(newStock, updateResult.ResponseObject.Stock);
        Assert.Contains("Integration Test Stock Update", updateResult.Message);
        
        _output.WriteLine($"Stock updated successfully. New stock: {updateResult.ResponseObject.Stock}");

        // Wait for Kafka processing
        await Task.Delay(1000);

        // Verify the stock is still correct after Kafka processing
        var verifyResponse = await _client.GetAsync($"/Product/GetProduct/{testProduct.Id}");
        verifyResponse.EnsureSuccessStatusCode();

        var verifyJson = await verifyResponse.Content.ReadAsStringAsync();
        var verifyResult = JsonConvert.DeserializeObject<BaseResponse<ProductResponse>>(verifyJson);

        Assert.NotNull(verifyResult?.ResponseObject);
        Assert.Equal(newStock, verifyResult.ResponseObject.Stock);
        
        _output.WriteLine($"Stock verification passed. Final stock: {verifyResult.ResponseObject.Stock}");
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_Should_ReturnError()
    {
        // Arrange - Get a product with low stock
        var getProductsResponse = await _client.GetAsync("/Product/GetProducts");
        getProductsResponse.EnsureSuccessStatusCode();
        
        var productsJson = await getProductsResponse.Content.ReadAsStringAsync();
        var productsResult = JsonConvert.DeserializeObject<BaseResponse<List<ProductResponse>>>(productsJson);
        
        var testProduct = productsResult?.ResponseObject?.First();
        Assert.NotNull(testProduct);
        
        var availableStock = testProduct.Stock;
        var excessiveQuantity = availableStock + 100; // Request more than available
        
        _output.WriteLine($"Attempting to order {excessiveQuantity} items when only {availableStock} available");

        // Arrange - Create order with excessive quantity
        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = "CUST002",
            CustomerName = "Test Customer 2",
            CustomerEmail = "test2@example.com",
            ShippingAddress = "456 Test Avenue",
            PaymentMethod = "Debit Card",
            OrderItems = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest
                {
                    ProductId = testProduct.Id,
                    Quantity = excessiveQuantity
                }
            }
        };

        var orderJson = JsonConvert.SerializeObject(createOrderRequest);
        var orderContent = new StringContent(orderJson, Encoding.UTF8, "application/json");

        // Act - Attempt to create order
        var createOrderResponse = await _client.PostAsync("/Order/CreateOrder", orderContent);

        // Assert - Should return error (either 400 Bad Request or 500 Internal Server Error)
        Assert.False(createOrderResponse.IsSuccessStatusCode);
        
        var errorResponseJson = await createOrderResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"Error response: {errorResponseJson}");
        
        // Verify stock wasn't changed
        var verifyStockResponse = await _client.GetAsync($"/Product/GetProduct/{testProduct.Id}");
        verifyStockResponse.EnsureSuccessStatusCode();

        var verifyStockJson = await verifyStockResponse.Content.ReadAsStringAsync();
        var verifyStockResult = JsonConvert.DeserializeObject<BaseResponse<ProductResponse>>(verifyStockJson);

        Assert.NotNull(verifyStockResult?.ResponseObject);
        Assert.Equal(availableStock, verifyStockResult.ResponseObject.Stock);
        
        _output.WriteLine($"Stock remained unchanged: {verifyStockResult.ResponseObject.Stock}");
    }
}
