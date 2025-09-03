using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using TQM.Backoffice.Application.Contracts.Infrastructure;
using TQM.Backoffice.Application.RESTApi.Line.Contracts;
using TQM.BackOffice.RESTApi.Helpers;
using TQM.BackOffice.RESTApi.Line;
using TQM.Backoffice.Application.RESTApi.JasperReport.Contracts;
using TQM.BackOffice.RESTApi.JasperReport;
using TQM.BackOffice.RESTApi.Services;

namespace TQM.BackOffice.RESTApi;

public static class WebServiceRegistration
{
    public static IServiceCollection ConfigureRESTApiServices(this IServiceCollection services)
    {
        services.AddScoped<ILineNotification, LineNotification>();
        services.AddScoped<IJasperReportExecutions, JasperReportExecutions>();
        services.AddScoped<IRestClient, RestClient>();
        services.AddScoped<IInvoker, Invoker>();
        
        // Add Kafka services
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddHostedService<StockUpdateConsumer>();
        
        return services;
    }
}
