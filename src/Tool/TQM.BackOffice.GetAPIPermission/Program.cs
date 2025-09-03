using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TQM.BackOffice.GetAPIPermission; // Note: actual namespace depends on the project name.

internal class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");

        var configuration = builder.Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ApiFunctionService, ApiFunctionService>()
                .BuildServiceProvider()
                .GetService<ApiFunctionService>()?
                .SysFunctionProcess();
    }
}