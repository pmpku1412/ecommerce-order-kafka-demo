using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TQM.BackOffice.Persistence.Services;
using TQM.Backoffice.Core.Application.Contracts.Infrastructure;
using TQM.Backoffice.Core.Application.Contracts.Persistence;

namespace TQM.BackOffice.Persistence;

public static class PersistenceServicesRegistration
{
    public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IDBAdapter, DBAdapter>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMailServiceX, MailService>();

        //AddService
        services.AddScoped<IMasterdataService, MasterdataService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IKafkaProducer, KafkaProducer>();
        services.AddSingleton<KafkaConsumerService>();

        return services;
    }
}
