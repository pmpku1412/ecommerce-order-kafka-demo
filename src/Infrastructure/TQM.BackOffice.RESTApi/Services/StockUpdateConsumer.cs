using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TQM.Backoffice.Application.Contracts.Persistence;
using TQM.Backoffice.Domain.Events;

namespace TQM.BackOffice.RESTApi.Services;

public class StockUpdateConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockUpdateConsumer> _logger;

    public StockUpdateConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<StockUpdateConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration.GetConnectionString("Kafka") ?? "localhost:9092",
            GroupId = "stock-update-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 6000,
            HeartbeatIntervalMs = 3000
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka consumer error: {Error}", e.Reason))
            .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForKafkaTopicAsync(stoppingToken);
        
        _consumer.Subscribe("orders");
        _logger.LogInformation("Stock Update Consumer started, subscribed to 'orders' topic");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    
                    if (consumeResult?.Message != null)
                    {
                        await ProcessOrderCreatedEvent(consumeResult.Message.Value);
                        _consumer.Commit(consumeResult);
                        
                        _logger.LogInformation("Processed message from partition {Partition}, offset {Offset}", 
                            consumeResult.Partition, consumeResult.Offset);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Wait before retry
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in consumer");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Wait before retry
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Stock Update Consumer");
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Stock Update Consumer stopped");
        }
    }

    private async Task WaitForKafkaTopicAsync(CancellationToken stoppingToken)
    {
        var maxRetries = 30; // 30 retries = 5 minutes max wait
        var retryCount = 0;
        
        while (retryCount < maxRetries && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var adminClient = new AdminClientBuilder(new AdminClientConfig 
                { 
                    BootstrapServers = "localhost:9092" 
                }).Build();
                
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                var topicExists = metadata.Topics.Any(t => t.Topic == "orders");
                
                if (topicExists)
                {
                    _logger.LogInformation("Kafka topic 'orders' is available");
                    return;
                }
                
                _logger.LogInformation("Waiting for Kafka topic 'orders' to be available... (attempt {Attempt}/{MaxRetries})", 
                    retryCount + 1, maxRetries);
                
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                retryCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check Kafka topic availability (attempt {Attempt}/{MaxRetries})", 
                    retryCount + 1, maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                retryCount++;
            }
        }
        
        if (retryCount >= maxRetries)
        {
            throw new InvalidOperationException("Kafka topic 'orders' is not available after maximum wait time");
        }
    }

    private async Task ProcessOrderCreatedEvent(string messageValue)
    {
        try
        {
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(messageValue, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });

            if (orderEvent == null)
            {
                _logger.LogWarning("Failed to deserialize order event");
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

            _logger.LogInformation("Processing order created event for order: {OrderId}", orderEvent.OrderId);

            // Update stock for each product in the order
            foreach (var item in orderEvent.Items)
            {
                var product = await productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    var newStock = Math.Max(0, product.StockQuantity - item.Quantity);
                    await productRepository.UpdateStockAsync(item.ProductId, newStock);
                    
                    _logger.LogInformation("Updated stock for product {ProductId}: {OldStock} -> {NewStock}", 
                        item.ProductId, product.StockQuantity, newStock);
                }
                else
                {
                    _logger.LogWarning("Product {ProductId} not found for stock update", item.ProductId);
                }
            }

            _logger.LogInformation("Successfully processed order event: {OrderId}", orderEvent.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order created event: {Message}", messageValue);
            throw;
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
