using Confluent.Kafka;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Testcontainers.Kafka;
using TQM.Backoffice.Application.DTOs.Orders.Request;
using TQM.Backoffice.Application.DTOs.Orders.Response;
using TQM.Backoffice.Domain.Events;
using TQM.Backoffice.Infrastructure.Responses;
using Xunit;

namespace TQM.BackOffice.E2ETests;

public class OrderProcessingE2ETests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly KafkaContainer _kafkaContainer;
    private IConsumer<string, string> _kafkaConsumer;

    public OrderProcessingE2ETests(WebApplicationFactory<Program> factory)
    {
        _kafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:7.4.0")
            .WithPortBinding(9092, true)
            .Build();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Override Kafka configuration for testing
                services.Configure<KafkaSettings>(options =>
                {
                    options.BootstrapServers = _kafkaContainer.GetBootstrapAddress();
                });
            });
        });

        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task InitializeAsync()
    {
        await _kafkaContainer.StartAsync();

        // Setup Kafka consumer for verification
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaContainer.GetBootstrapAddress(),
            GroupId = "e2e-test-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    public async Task DisposeAsync()
    {
        _kafkaConsumer?.Dispose();
        await _kafkaContainer.DisposeAsync();
    }

    [Fact]
    public async Task CompleteOrderFlow_ShouldProcessOrderAndPublishEvents()
    {
        // Arrange
        var createOrderRequest = new CreateOrderRequest
        {
            CustomerId = "CUST001",
            CustomerName = "John Doe",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD001", Quantity = 2, UnitPrice = 100.00m },
                new() { ProductId = "PROD002", Quantity = 1, UnitPrice = 50.00m }
            }
        };

        _kafkaConsumer.Subscribe(new[] { "order-events", "stock-events", "notification-events" });

        // Act 1: Create Order
        var createResponse = await _client.PostAsJsonAsync("/api/orders", createOrderRequest, _jsonOptions);

        // Assert 1: Order Creation
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<BaseResponse<OrderResponse>>(createContent, _jsonOptions);

        createResult.Should().NotBeNull();
        createResult.Success.Should().BeTrue();
        createResult.Data.Should().NotBeNull();
        createResult.Data.CustomerId.Should().Be(createOrderRequest.CustomerId);
        createResult.Data.TotalAmount.Should().Be(250.00m);

        var orderId = createResult.Data.Id;

        // Act 2: Verify Order Retrieval
        var getResponse = await _client.GetAsync($"/api/orders/{orderId}");

        // Assert 2: Order Retrieval
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var getResult = JsonSerializer.Deserialize<BaseResponse<OrderResponse>>(getContent, _jsonOptions);

        getResult.Should().NotBeNull();
        getResult.Success.Should().BeTrue();
        getResult.Data.Id.Should().Be(orderId);

        // Act 3: Verify Kafka Events
        var receivedEvents = new List<(string Topic, string Message)>();
        var timeout = DateTime.UtcNow.AddSeconds(30);

        while (DateTime.UtcNow < timeout && receivedEvents.Count < 3)
        {
            try
            {
                var consumeResult = _kafkaConsumer.Consume(TimeSpan.FromSeconds(5));
                if (consumeResult != null)
                {
                    receivedEvents.Add((consumeResult.Topic, consumeResult.Message.Value));
                }
            }
            catch (ConsumeException)
            {
                // Continue trying
            }
        }

        // Assert 3: Kafka Events
        receivedEvents.Should().HaveCountGreaterOrEqualTo(1);

        var orderEvent = receivedEvents.FirstOrDefault(e => e.Topic == "order-events");
        orderEvent.Should().NotBe(default);

        var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderEvent.Message);
        orderCreatedEvent.Should().NotBeNull();
        orderCreatedEvent.OrderId.Should().Be(orderId);
        orderCreatedEvent.CustomerId.Should().Be(createOrderRequest.CustomerId);
        orderCreatedEvent.TotalAmount.Should().Be(250.00m);
        orderCreatedEvent.Items.Should().HaveCount(2);

        // Act 4: Verify Customer Orders
        var customerOrdersResponse = await _client.GetAsync($"/api/orders/customer/{createOrderRequest.CustomerId}");

        // Assert 4: Customer Orders
        customerOrdersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customerOrdersContent = await customerOrdersResponse.Content.ReadAsStringAsync();
        var customerOrdersResult = JsonSerializer.Deserialize<BaseResponse<List<OrderResponse>>>(customerOrdersContent, _jsonOptions);

        customerOrdersResult.Should().NotBeNull();
        customerOrdersResult.Success.Should().BeTrue();
        customerOrdersResult.Data.Should().Contain(o => o.Id == orderId);
    }

    [Fact]
    public async Task MultipleOrdersFlow_ShouldProcessAllOrdersCorrectly()
    {
        // Arrange
        var orders = new List<CreateOrderRequest>
        {
            new()
            {
                CustomerId = "CUST001",
                CustomerName = "John Doe",
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = "PROD001", Quantity = 1, UnitPrice = 100.00m }
                }
            },
            new()
            {
                CustomerId = "CUST002",
                CustomerName = "Jane Smith",
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = "PROD002", Quantity = 2, UnitPrice = 50.00m }
                }
            },
            new()
            {
                CustomerId = "CUST001",
                CustomerName = "John Doe",
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = "PROD003", Quantity = 1, UnitPrice = 75.00m }
                }
            }
        };

        _kafkaConsumer.Subscribe("order-events");
        var createdOrderIds = new List<string>();

        // Act: Create multiple orders
        foreach (var orderRequest in orders)
        {
            var response = await _client.PostAsJsonAsync("/api/orders", orderRequest, _jsonOptions);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BaseResponse<OrderResponse>>(content, _jsonOptions);
            createdOrderIds.Add(result.Data.Id);
        }

        // Assert: Verify all orders were created
        createdOrderIds.Should().HaveCount(3);

        // Verify all orders endpoint
        var allOrdersResponse = await _client.GetAsync("/api/orders");
        allOrdersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var allOrdersContent = await allOrdersResponse.Content.ReadAsStringAsync();
        var allOrdersResult = JsonSerializer.Deserialize<BaseResponse<List<OrderResponse>>>(allOrdersContent, _jsonOptions);

        allOrdersResult.Data.Should().HaveCountGreaterOrEqualTo(3);
        foreach (var orderId in createdOrderIds)
        {
            allOrdersResult.Data.Should().Contain(o => o.Id == orderId);
        }

        // Verify customer-specific orders
        var cust001OrdersResponse = await _client.GetAsync("/api/orders/customer/CUST001");
        var cust001OrdersContent = await cust001OrdersResponse.Content.ReadAsStringAsync();
        var cust001OrdersResult = JsonSerializer.Deserialize<BaseResponse<List<OrderResponse>>>(cust001OrdersContent, _jsonOptions);

        cust001OrdersResult.Data.Should().HaveCount(2);
        cust001OrdersResult.Data.Should().OnlyContain(o => o.CustomerId == "CUST001");

        var cust002OrdersResponse = await _client.GetAsync("/api/orders/customer/CUST002");
        var cust002OrdersContent = await cust002OrdersResponse.Content.ReadAsStringAsync();
        var cust002OrdersResult = JsonSerializer.Deserialize<BaseResponse<List<OrderResponse>>>(cust002OrdersContent, _jsonOptions);

        cust002OrdersResult.Data.Should().HaveCount(1);
        cust002OrdersResult.Data.Should().OnlyContain(o => o.CustomerId == "CUST002");

        // Verify Kafka events for all orders
        var receivedEvents = new List<string>();
        var timeout = DateTime.UtcNow.AddSeconds(30);

        while (DateTime.UtcNow < timeout && receivedEvents.Count < orders.Count)
        {
            try
            {
                var consumeResult = _kafkaConsumer.Consume(TimeSpan.FromSeconds(5));
                if (consumeResult != null)
                {
                    receivedEvents.Add(consumeResult.Message.Value);
                }
            }
            catch (ConsumeException)
            {
                // Continue trying
            }
        }

        receivedEvents.Should().HaveCountGreaterOrEqualTo(orders.Count);

        foreach (var eventMessage in receivedEvents)
        {
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(eventMessage);
            orderEvent.Should().NotBeNull();
            createdOrderIds.Should().Contain(orderEvent.OrderId);
        }
    }

    [Fact]
    public async Task ErrorHandlingFlow_ShouldHandleInvalidRequestsGracefully()
    {
        // Test 1: Invalid product
        var invalidProductRequest = new CreateOrderRequest
        {
            CustomerId = "CUST001",
            CustomerName = "John Doe",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "INVALID", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var invalidProductResponse = await _client.PostAsJsonAsync("/api/orders", invalidProductRequest, _jsonOptions);
        invalidProductResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Test 2: Empty customer ID
        var emptyCustomerRequest = new CreateOrderRequest
        {
            CustomerId = "",
            CustomerName = "John Doe",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD001", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var emptyCustomerResponse = await _client.PostAsJsonAsync("/api/orders", emptyCustomerRequest, _jsonOptions);
        emptyCustomerResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Test 3: Empty items
        var emptyItemsRequest = new CreateOrderRequest
        {
            CustomerId = "CUST001",
            CustomerName = "John Doe",
            Items = new List<CreateOrderItemRequest>()
        };

        var emptyItemsResponse = await _client.PostAsJsonAsync("/api/orders", emptyItemsRequest, _jsonOptions);
        emptyItemsResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Test 4: Non-existent order retrieval
        var nonExistentOrderResponse = await _client.GetAsync("/api/orders/NONEXISTENT");
        nonExistentOrderResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ProductCatalogFlow_ShouldReturnAvailableProducts()
    {
        // Act
        var productsResponse = await _client.GetAsync("/api/orders/products");

        // Assert
        productsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var productsContent = await productsResponse.Content.ReadAsStringAsync();
        var productsResult = JsonSerializer.Deserialize<BaseResponse<List<object>>>(productsContent, _jsonOptions);

        productsResult.Should().NotBeNull();
        productsResult.Success.Should().BeTrue();
        productsResult.Data.Should().NotBeNull();
        productsResult.Data.Should().HaveCount(5);
    }

    [Fact]
    public async Task HighVolumeOrderFlow_ShouldHandleMultipleSimultaneousOrders()
    {
        // Arrange
        var orderTasks = new List<Task<HttpResponseMessage>>();
        var orderCount = 10;

        _kafkaConsumer.Subscribe("order-events");

        // Act: Create multiple orders simultaneously
        for (int i = 1; i <= orderCount; i++)
        {
            var request = new CreateOrderRequest
            {
                CustomerId = $"CUST{i:D3}",
                CustomerName = $"Customer {i}",
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = "PROD001", Quantity = 1, UnitPrice = 100.00m }
                }
            };

            orderTasks.Add(_client.PostAsJsonAsync("/api/orders", request, _jsonOptions));
        }

        var responses = await Task.WhenAll(orderTasks);

        // Assert: All orders should be created successfully
        responses.Should().HaveCount(orderCount);
        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);

        // Verify all orders are retrievable
        var allOrdersResponse = await _client.GetAsync("/api/orders");
        allOrdersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var allOrdersContent = await allOrdersResponse.Content.ReadAsStringAsync();
        var allOrdersResult = JsonSerializer.Deserialize<BaseResponse<List<OrderResponse>>>(allOrdersContent, _jsonOptions);

        allOrdersResult.Data.Should().HaveCountGreaterOrEqualTo(orderCount);

        // Verify Kafka events
        var receivedEventCount = 0;
        var timeout = DateTime.UtcNow.AddSeconds(60);

        while (DateTime.UtcNow < timeout && receivedEventCount < orderCount)
        {
            try
            {
                var consumeResult = _kafkaConsumer.Consume(TimeSpan.FromSeconds(5));
                if (consumeResult != null)
                {
                    receivedEventCount++;
                }
            }
            catch (ConsumeException)
            {
                // Continue trying
            }
        }

        receivedEventCount.Should().BeGreaterOrEqualTo(orderCount);
    }
}

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
}
