using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TQM.Backoffice.Application.Contracts.Persistence;
using TQM.BackOffice.Persistence.Services;

namespace TQM.BackOffice.Persistence;

public static class PersistenceServicesRegistration
{
    public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IDBAdapter, DBAdapter>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMailServiceX, MailService>();
        
        // Add new repositories
        services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        
        return services;
    }
}
