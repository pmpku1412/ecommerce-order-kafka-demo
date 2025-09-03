using Microsoft.Extensions.Configuration;
using TQM.Backoffice.Application.RESTApi.Line.Contracts;
using TQM.BackOffice.RESTApi.Helpers;

namespace TQM.BackOffice.RESTApi.Line;

public class LineNotification : ILineNotification
{
    private readonly IInvoker _invoker;
    private readonly IConfiguration _configuration;
    public LineNotification(IInvoker invoker, IConfiguration configuration)
    {
        _invoker = invoker;
        _configuration = configuration;
    }

    public async Task SubmitLineGroup(string message)
    {
        try
        {
            if (_configuration["LineSettings:LineNotiStatus"] == "OPEN")
            {
                Dictionary<string, string> header = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                };
                await _invoker.PostAsync(header, new { message = message, token = _configuration["LineSettings:NewCoreToken"] }, _configuration["LineSettings:LineNotifyUrl"]);
            }
        }
        catch (System.Exception) { throw; }
    }
}
