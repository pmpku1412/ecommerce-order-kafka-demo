using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Net.Http.Json;
using System.Text.Json;
using TQM.Backoffice.Application.DTOs.Orders.Request;
using Xunit;

namespace TQM.BackOffice.E2ETests;

public class PerformanceTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public void OrderCreation_LoadTest_ShouldHandleHighThroughput()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000"); // Adjust port as needed

        var scenario = Scenario.Create("create_order_load_test", async context =>
        {
            var customerId = $"CUST{context.ScenarioInfo.ThreadId:D3}";
            var customerName = $"Customer {context.ScenarioInfo.ThreadId}";

            var request = new CreateOrderRequest
            {
                CustomerId = customerId,
                CustomerName = customerName,
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = "PROD001", Quantity = 1, UnitPrice = 100.00m },
                    new() { ProductId = "PROD002", Quantity = 2, UnitPrice = 50.00m }
                }
            };

            var response = await httpClient.PostAsJsonAsync("/api/orders", request, _jsonOptions);

            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(2))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void OrderRetrieval_LoadTest_ShouldHandleHighReadThroughput()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000");

        // Pre-create some orders for testing
        var orderIds = new List<string>();
        for (int i = 1; i <= 100; i++)
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

            var response = httpClient.PostAsJsonAsync("/api/orders", request, _jsonOptions).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var result = JsonSerializer.Deserialize<dynamic>(content);
                // Extract order ID from response (simplified)
                orderIds.Add($"ORDER{i:D3}"); // Assuming predictable ID format
            }
        }

        var scenario = Scenario.Create("get_order_load_test", async context =>
        {
            var randomOrderId = orderIds[Random.Shared.Next(orderIds.Count)];
            var response = await httpClient.GetAsync($"/api/orders/{randomOrderId}");

            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void GetAllOrders_LoadTest_ShouldHandleListRequests()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000");

        var scenario = Scenario.Create("get_all_orders_load_test", async context =>
        {
            var response = await httpClient.GetAsync("/api/orders");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 20, during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void CustomerOrders_LoadTest_ShouldHandleCustomerSpecificRequests()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000");

        var customerIds = new List<string>();
        for (int i = 1; i <= 50; i++)
        {
            customerIds.Add($"CUST{i:D3}");
        }

        var scenario = Scenario.Create("get_customer_orders_load_test", async context =>
        {
            var randomCustomerId = customerIds[Random.Shared.Next(customerIds.Count)];
            var response = await httpClient.GetAsync($"/api/orders/customer/{randomCustomerId}");

            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 30, during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void MixedWorkload_LoadTest_ShouldHandleRealisticTraffic()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000");

        // Scenario 1: Create orders (20% of traffic)
        var createOrderScenario = Scenario.Create("create_orders", async context =>
        {
            var request = new CreateOrderRequest
            {
                CustomerId = $"CUST{Random.Shared.Next(1, 1000):D3}",
                CustomerName = $"Customer {Random.Shared.Next(1, 1000)}",
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = $"PROD{Random.Shared.Next(1, 6):D3}", Quantity = Random.Shared.Next(1, 5), UnitPrice = Random.Shared.Next(50, 200) }
                }
            };

            var response = await httpClient.PostAsJsonAsync("/api/orders", request, _jsonOptions);
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithWeight(20)
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 5, during: TimeSpan.FromMinutes(3))
        );

        // Scenario 2: Get all orders (30% of traffic)
        var getAllOrdersScenario = Scenario.Create("get_all_orders", async context =>
        {
            var response = await httpClient.GetAsync("/api/orders");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithWeight(30)
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 8, during: TimeSpan.FromMinutes(3))
        );

        // Scenario 3: Get customer orders (40% of traffic)
        var getCustomerOrdersScenario = Scenario.Create("get_customer_orders", async context =>
        {
            var customerId = $"CUST{Random.Shared.Next(1, 100):D3}";
            var response = await httpClient.GetAsync($"/api/orders/customer/{customerId}");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithWeight(40)
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(3))
        );

        // Scenario 4: Get products (10% of traffic)
        var getProductsScenario = Scenario.Create("get_products", async context =>
        {
            var response = await httpClient.GetAsync("/api/orders/products");
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithWeight(10)
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 2, during: TimeSpan.FromMinutes(3))
        );

        NBomberRunner
            .RegisterScenarios(createOrderScenario, getAllOrdersScenario, getCustomerOrdersScenario, getProductsScenario)
            .Run();
    }

    [Fact]
    public void StressTest_ShouldIdentifyBreakingPoint()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000");

        var scenario = Scenario.Create("stress_test", async context =>
        {
            var request = new CreateOrderRequest
            {
                CustomerId = $"STRESS_CUST{context.ScenarioInfo.ThreadId:D3}",
                CustomerName = $"Stress Customer {context.ScenarioInfo.ThreadId}",
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = "PROD001", Quantity = 1, UnitPrice = 100.00m }
                }
            };

            var response = await httpClient.PostAsJsonAsync("/api/orders", request, _jsonOptions);
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            // Gradually increase load to find breaking point
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromSeconds(30)),
            Simulation.InjectPerSec(rate: 25, during: TimeSpan.FromSeconds(30)),
            Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromSeconds(30)),
            Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30)),
            Simulation.InjectPerSec(rate: 200, during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void SpikeTest_ShouldHandleSuddenTrafficSpikes()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000");

        var scenario = Scenario.Create("spike_test", async context =>
        {
            var request = new CreateOrderRequest
            {
                CustomerId = $"SPIKE_CUST{context.ScenarioInfo.ThreadId:D3}",
                CustomerName = $"Spike Customer {context.ScenarioInfo.ThreadId}",
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = "PROD001", Quantity = 1, UnitPrice = 100.00m }
                }
            };

            var response = await httpClient.PostAsJsonAsync("/api/orders", request, _jsonOptions);
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            // Normal load
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1)),
            // Sudden spike
            Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30)),
            // Back to normal
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [Fact]
    public void EnduranceTest_ShouldMaintainPerformanceOverTime()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:5000");

        var scenario = Scenario.Create("endurance_test", async context =>
        {
            var request = new CreateOrderRequest
            {
                CustomerId = $"ENDURANCE_CUST{context.ScenarioInfo.ThreadId:D3}",
                CustomerName = $"Endurance Customer {context.ScenarioInfo.ThreadId}",
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = "PROD001", Quantity = 1, UnitPrice = 100.00m }
                }
            };

            var response = await httpClient.PostAsJsonAsync("/api/orders", request, _jsonOptions);
            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            // Sustained load for extended period
            Simulation.InjectPerSec(rate: 20, during: TimeSpan.FromMinutes(10))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
