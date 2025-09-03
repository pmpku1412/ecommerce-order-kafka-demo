using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        //AddService
        services.AddScoped<IMasterdataService, MasterdataService>();

        return services;
    }
}
