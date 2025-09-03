using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TQM.Backoffice.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ILogAdapter, LogAdapter>();
        services.AddScoped<IHelperGetStaff, GetStaff>();
        services.AddMediatR(Assembly.GetExecutingAssembly());
        return services;
    }
}
