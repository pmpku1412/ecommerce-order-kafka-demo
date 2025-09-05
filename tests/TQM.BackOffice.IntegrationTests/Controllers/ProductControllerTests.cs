using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using TQM.Backoffice.Core.Application.DTOs.Products.Request;
using TQM.Backoffice.Core.Application.DTOs.Products.Response;
using TQM.Backoffice.Application.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TQM.BackOffice.IntegrationTests.Controllers;

public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessWithProducts()
    {
        // Act
        var response = await _client.GetAsync("/Product/GetProducts");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<ProductResponse>>>();
        
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.ResponseObject);
        Assert.True(result.ResponseObject.Count > 0);
    }

    [Fact]
    public async Task GetProduct_ValidId_ReturnsProduct()
    {
        // Arrange
        var productId = "PROD001"; // Assuming this exists in the mock data

        // Act
        var response = await _client.GetAsync($"/Product/GetProduct/{productId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();
        
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.ResponseObject);
        Assert.Equal(productId, result.ResponseObject.Id);
    }

    [Fact]
    public async Task GetProduct_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidProductId = "INVALID_ID";

        // Act
        var response = await _client.GetAsync($"/Product/GetProduct/{invalidProductId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();
        
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Product not found", result.Message);
    }

    [Fact]
    public async Task CreateProduct_ValidData_ReturnsCreatedProduct()
    {
        // Arrange
        var createRequest = new CreateProductRequest
        {
            Name = "Test Product Integration",
            Price = 999.99m,
            Stock = 50,
            Category = "Test Category",
            Description = "Integration test product",
            ImageUrl = "https://example.com/test-image.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Product/CreateProduct", createRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();
        
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.ResponseObject);
        Assert.Equal(createRequest.Name, result.ResponseObject.Name);
        Assert.Equal(createRequest.Price, result.ResponseObject.Price);
        Assert.Equal(createRequest.Stock, result.ResponseObject.Stock);
        Assert.Equal(createRequest.Category, result.ResponseObject.Category);
        Assert.Equal("Active", result.ResponseObject.Status);
        Assert.True(result.ResponseObject.Id.StartsWith("PROD"));
    }

    [Fact]
    public async Task CreateProduct_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new CreateProductRequest
        {
            Name = "", // Invalid: empty name
            Price = -10, // Invalid: negative price
            Stock = -5, // Invalid: negative stock
            Category = "",
            Description = "Test",
            ImageUrl = "invalid-url" // Invalid URL format
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Product/CreateProduct", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();
        
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.Count > 0);
    }

    [Fact]
    public async Task UpdateProduct_ValidData_ReturnsUpdatedProduct()
    {
        // Arrange
        var productId = "PROD001"; // Assuming this exists
        var updateRequest = new UpdateProductRequest
        {
            Id = productId,
            Name = "Updated Product Name",
            Price = 1500.00m,
            Category = "Updated Category",
            Description = "Updated description",
            ImageUrl = "https://example.com/updated-image.jpg"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/Product/UpdateProduct/{productId}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();
        
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.ResponseObject);
        Assert.Equal(updateRequest.Name, result.ResponseObject.Name);
        Assert.Equal(updateRequest.Price, result.ResponseObject.Price);
        Assert.Equal(updateRequest.Category, result.ResponseObject.Category);
    }

    [Fact]
    public async Task UpdateProduct_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidProductId = "INVALID_ID";
        var updateRequest = new UpdateProductRequest
        {
            Id = invalidProductId,
            Name = "Updated Product",
            Price = 100.00m,
            Category = "Category",
            Description = "Description",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/Product/UpdateProduct/{invalidProductId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStock_ValidData_ReturnsUpdatedProduct()
    {
        // Arrange
        var productId = "PROD001"; // Assuming this exists
        var stockRequest = new UpdateStockRequest
        {
            Id = productId,
            Stock = 100,
            Reason = "Integration test stock update"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/Product/UpdateStock/{productId}", stockRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();
        
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.ResponseObject);
        Assert.Equal(stockRequest.Stock, result.ResponseObject.Stock);
        Assert.Contains(stockRequest.Reason, result.Message);
    }

    [Fact]
    public async Task UpdateStock_InvalidStock_ReturnsBadRequest()
    {
        // Arrange
        var productId = "PROD001";
        var invalidStockRequest = new UpdateStockRequest
        {
            Id = productId,
            Stock = -10, // Invalid: negative stock
            Reason = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/Product/UpdateStock/{productId}", invalidStockRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ValidId_ReturnsSuccess()
    {
        // Arrange - First create a product to delete
        var createRequest = new CreateProductRequest
        {
            Name = "Product to Delete",
            Price = 100.00m,
            Stock = 10,
            Category = "Test",
            Description = "Test product for deletion",
            ImageUrl = "https://example.com/image.jpg"
        };

        var createResponse = await _client.PostAsJsonAsync("/Product/CreateProduct", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();
        var productId = createResult?.ResponseObject?.Id;

        // Act
        var deleteResponse = await _client.DeleteAsync($"/Product/DeleteProduct/{productId}");

        // Assert
        deleteResponse.EnsureSuccessStatusCode();
        var deleteResult = await deleteResponse.Content.ReadFromJsonAsync<BaseResponse<string>>();
        
        Assert.NotNull(deleteResult);
        Assert.True(deleteResult.Success);
        Assert.Equal("Product deleted successfully", deleteResult.Message);

        // Verify product is no longer accessible
        var getResponse = await _client.GetAsync($"/Product/GetProduct/{productId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidProductId = "INVALID_ID";

        // Act
        var response = await _client.DeleteAsync($"/Product/DeleteProduct/{invalidProductId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCategories_ReturnsListOfCategories()
    {
        // Act
        var response = await _client.GetAsync("/Product/GetCategories");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<List<string>>>();
        
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.ResponseObject);
        Assert.True(result.ResponseObject.Count > 0);
        Assert.Contains("Electronics", result.ResponseObject);
    }

    [Fact]
    public async Task ProductWorkflow_CreateUpdateDeleteProduct_WorksCorrectly()
    {
        // Step 1: Create Product
        var createRequest = new CreateProductRequest
        {
            Name = "Workflow Test Product",
            Price = 500.00m,
            Stock = 25,
            Category = "Test Category",
            Description = "Product for workflow testing",
            ImageUrl = "https://example.com/workflow-image.jpg"
        };

        var createResponse = await _client.PostAsJsonAsync("/Product/CreateProduct", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createResult = await createResponse.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();
        var productId = createResult?.ResponseObject?.Id;

        Assert.NotNull(productId);

        // Step 2: Update Product
        var updateRequest = new UpdateProductRequest
        {
            Id = productId,
            Name = "Updated Workflow Product",
            Price = 750.00m,
            Category = "Updated Category",
            Description = "Updated description",
            ImageUrl = "https://example.com/updated-workflow-image.jpg"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/Product/UpdateProduct/{productId}", updateRequest);
        updateResponse.EnsureSuccessStatusCode();
        var updateResult = await updateResponse.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();

        Assert.Equal(updateRequest.Name, updateResult?.ResponseObject?.Name);
        Assert.Equal(updateRequest.Price, updateResult?.ResponseObject?.Price);

        // Step 3: Update Stock
        var stockRequest = new UpdateStockRequest
        {
            Id = productId,
            Stock = 50,
            Reason = "Workflow test stock adjustment"
        };

        var stockResponse = await _client.PutAsJsonAsync($"/Product/UpdateStock/{productId}", stockRequest);
        stockResponse.EnsureSuccessStatusCode();
        var stockResult = await stockResponse.Content.ReadFromJsonAsync<BaseResponse<ProductResponse>>();

        Assert.Equal(stockRequest.Stock, stockResult?.ResponseObject?.Stock);

        // Step 4: Delete Product
        var deleteResponse = await _client.DeleteAsync($"/Product/DeleteProduct/{productId}");
        deleteResponse.EnsureSuccessStatusCode();

        // Step 5: Verify Deletion
        var getResponse = await _client.GetAsync($"/Product/GetProduct/{productId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
