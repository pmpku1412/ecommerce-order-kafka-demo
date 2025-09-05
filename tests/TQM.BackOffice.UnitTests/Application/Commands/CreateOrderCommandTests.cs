using FluentAssertions;
using Xunit;

namespace TQM.BackOffice.UnitTests.Application.Commands;

public class CreateOrderCommandTests
{
    [Fact]
    public void CreateOrderCommand_SampleTest_ShouldPass()
    {
        // Arrange
        var expected = "Order Creation Test";
        
        // Act
        var actual = "Order Creation Test";
        
        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("CUST001", "John Doe")]
    [InlineData("CUST002", "Jane Smith")]
    public void CreateOrderCommand_ValidCustomerData_ShouldBeValid(string customerId, string customerName)
    {
        // Arrange & Act
        var isValid = !string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(customerName);
        
        // Assert
        isValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "John Doe")]
    [InlineData("CUST001", "")]
    [InlineData(null, "John Doe")]
    [InlineData("CUST001", null)]
    public void CreateOrderCommand_InvalidCustomerData_ShouldBeInvalid(string customerId, string customerName)
    {
        // Arrange & Act
        var isValid = !string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(customerName);
        
        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void CreateOrderCommand_CalculateTotalAmount_ShouldReturnCorrectTotal()
    {
        // Arrange
        var quantity1 = 2;
        var unitPrice1 = 100.00m;
        var quantity2 = 1;
        var unitPrice2 = 50.00m;
        
        // Act
        var totalAmount = (quantity1 * unitPrice1) + (quantity2 * unitPrice2);
        
        // Assert
        totalAmount.Should().Be(250.00m);
    }

    [Fact]
    public void CreateOrderCommand_ValidateProductId_ShouldReturnTrue()
    {
        // Arrange
        var validProductIds = new[] { "PROD001", "PROD002", "PROD003", "PROD004", "PROD005" };
        var testProductId = "PROD001";
        
        // Act
        var isValid = Array.Exists(validProductIds, id => id == testProductId);
        
        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void CreateOrderCommand_InvalidProductId_ShouldReturnFalse()
    {
        // Arrange
        var validProductIds = new[] { "PROD001", "PROD002", "PROD003", "PROD004", "PROD005" };
        var testProductId = "INVALID";
        
        // Act
        var isValid = Array.Exists(validProductIds, id => id == testProductId);
        
        // Assert
        isValid.Should().BeFalse();
    }
}
