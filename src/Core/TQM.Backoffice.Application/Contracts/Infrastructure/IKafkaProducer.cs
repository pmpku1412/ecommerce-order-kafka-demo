using TQM.Backoffice.Domain.Events;

namespace TQM.Backoffice.Application.Contracts.Infrastructure;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, string key, T message) where T : class;
}
