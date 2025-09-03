using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TQM.Backoffice.Application.Contracts.Infrastructure;

namespace TQM.BackOffice.RESTApi.Services;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = configuration.GetConnectionString("Kafka") ?? "localhost:9092",
            Acks = Acks.Leader,
            EnableIdempotence = true,
            MessageTimeoutMs = 5000,
            RequestTimeoutMs = 5000
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Error}", e.Reason))
            .Build();
    }

    public async Task PublishAsync<T>(string topic, string key, T message) where T : class
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = jsonMessage,
                Timestamp = Timestamp.Default
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage);
            
            _logger.LogInformation("Message published to Kafka. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}", 
                result.Topic, result.Partition, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to Kafka topic: {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
