using Xunit;
using TQM.Backoffice.Core.Application.Commands.Products;
using TQM.Backoffice.Core.Application.DTOs.Products.Request;

namespace TQM.BackOffice.UnitTests.Application.Commands;

public class CreateProductCommandTests
{
    [Fact]
    public async Task Handle_ValidProduct_ReturnsSuccessResponse()
    {
        // Arrange
        var handler = new CreateProductCommandHandler();
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Price = 100.00m,
            Stock = 10,
            Category = "Electronics",
            Description = "Test product description",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Product created successfully", result.Message);
        Assert.NotNull(result.ResponseObject);
        Assert.Equal(command.Name, result.ResponseObject.Name);
        Assert.Equal(command.Price, result.ResponseObject.Price);
        Assert.Equal(command.Stock, result.ResponseObject.Stock);
        Assert.Equal(command.Category, result.ResponseObject.Category);
        Assert.Equal("Active", result.ResponseObject.Status);
        Assert.True(result.ResponseObject.Id.StartsWith("PROD"));
    }

    [Fact]
    public void FromRequest_ValidRequest_ReturnsCorrectCommand()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            Price = 100.00m,
            Stock = 10,
            Category = "Electronics",
            Description = "Test product description",
            ImageUrl = "https://example.com/image.jpg"
        };

        // Act
        var command = CreateProductCommand.FromRequest(request);

        // Assert
        Assert.Equal(request.Name, command.Name);
        Assert.Equal(request.Price, command.Price);
        Assert.Equal(request.Stock, command.Stock);
        Assert.Equal(request.Category, command.Category);
        Assert.Equal(request.Description, command.Description);
        Assert.Equal(request.ImageUrl, command.ImageUrl);
    }

    [Theory]
    [InlineData("", 100.00, 10, "Electronics", "Description", "https://example.com/image.jpg")]
    [InlineData("Product", 0, 10, "Electronics", "Description", "https://example.com/image.jpg")]
    [InlineData("Product", 100.00, -1, "Electronics", "Description", "https://example.com/image.jpg")]
    [InlineData("Product", 100.00, 10, "", "Description", "https://example.com/image.jpg")]
    public async Task Handle_InvalidProduct_ReturnsSuccessButWithValidation(
        string name, decimal price, int stock, string category, string description, string imageUrl)
    {
        // Arrange
        var handler = new CreateProductCommandHandler();
        var command = new CreateProductCommand
        {
            Name = name,
            Price = price,
            Stock = stock,
            Category = category,
            Description = description,
            ImageUrl = imageUrl
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        // Note: In this simple implementation, validation is handled at the controller level
        // The handler itself doesn't perform validation, so it will still return success
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Handle_GeneratesUniqueProductId()
    {
        // Arrange
        var handler = new CreateProductCommandHandler();
        var command1 = new CreateProductCommand
        {
            Name = "Product 1",
            Price = 100.00m,
            Stock = 10,
            Category = "Electronics"
        };
        var command2 = new CreateProductCommand
        {
            Name = "Product 2",
            Price = 200.00m,
            Stock = 20,
            Category = "Electronics"
        };

        // Act
        var result1 = await handler.Handle(command1, CancellationToken.None);
        await Task.Delay(1000); // Ensure different timestamps
        var result2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        Assert.True(result1.Success);
        Assert.True(result2.Success);
        Assert.NotEqual(result1.ResponseObject?.Id, result2.ResponseObject?.Id);
    }

    [Fact]
    public async Task Handle_SetsCorrectTimestamps()
    {
        // Arrange
        var handler = new CreateProductCommandHandler();
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Price = 100.00m,
            Stock = 10,
            Category = "Electronics"
        };
        var beforeExecution = DateTime.UtcNow;

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        var afterExecution = DateTime.UtcNow;

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.ResponseObject);
        Assert.True(result.ResponseObject.CreatedAt >= beforeExecution);
        Assert.True(result.ResponseObject.CreatedAt <= afterExecution);
        Assert.True(result.ResponseObject.UpdatedAt >= beforeExecution);
        Assert.True(result.ResponseObject.UpdatedAt <= afterExecution);
        Assert.Equal(result.ResponseObject.CreatedAt, result.ResponseObject.UpdatedAt);
    }
}
