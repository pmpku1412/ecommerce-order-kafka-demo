using RestSharp;
using Newtonsoft.Json;
using System.Net;
using Microsoft.Extensions.Configuration;
using TQM.Backoffice.Application.RESTApi.JasperReport.Contracts;
using TQM.BackOffice.RESTApi.Helpers;
using TQM.Backoffice.Application.DTOs.Common;

namespace TQM.BackOffice.RESTApi.JasperReport;

public class JasperReportExecutions : IJasperReportExecutions
{
    private readonly IInvoker _invoker;
    private readonly IConfiguration _configuration;

    public JasperReportExecutions(IInvoker invoker, IConfiguration configuration)
    {
        _invoker = invoker;
        _configuration = configuration;
    }
    public async Task<ReportExecutionServiceInfo> SubmitReportExecutions(ReportExecutionRequest request)
    {
        try
        {
            Dictionary<string, string> header = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Authorization", "Basic " + _configuration["JasperReportSettings:AuthorizedToken"] }
                };

            var response = await _invoker.PostAsync(header, request, _configuration["JasperReportSettings:JasperExecuteUrl"]);

            //Get Execute Response Content to Our list
            ReportExecutionServiceInfo result = new ReportExecutionServiceInfo();
            var t = System.Text.Json.JsonSerializer.Deserialize<ReportExecutionResponse>(response.Content);
            if (t != null)
            {
                result.ExportId = t.exports.Select(x => x.id).First();
                result.ReportUrl = t.reportURI;
                result.RequestId = t.requestId;
                result.Status = t.requestId;
            }
            return result;

        }
        catch (System.Exception) { throw; }
    }


    private async Task<ReportExecutionResponse> JasperExecute(ReportExecutionRequest request)
    {
        try
        {
            Dictionary<string, string> header = new()
                {
                    { "Authorization", "Basic " + _configuration["JasperReportSettings:AuthorizedToken"] },
                    { "Content-Type", "application/json" }
                };

            var response = await _invoker.PostAsync(header, request, _configuration["JasperReportSettings:JasperExecuteUrl"]);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var output = JsonConvert.DeserializeObject<ReportExecutionResponse>(response.Content) ?? new ReportExecutionResponse();
                var sessionCookie = response.Cookies.SingleOrDefault(x => x.Name == "JSESSIONID");
                string t = await JasperOutput(output.requestId, output.exports.First().id, sessionCookie);

                return output;
            }
            else
                return new ReportExecutionResponse();
        }
        catch (System.Exception) { throw; }
    }


    private async Task<string> JasperOutput(string requestId, string exportId, RestResponseCookie? sessionCookie = null)
    {
        try
        {
            CookieContainer _cookieJar = new();
            Dictionary<string, string> header = new()
                {
                    { "Authorization", "Basic " + _configuration["JasperReportSettings:AuthorizedToken"] },
                };

            if (sessionCookie is not null)
                _cookieJar.Add(new Cookie(sessionCookie.Name, sessionCookie.Value, sessionCookie.Path, sessionCookie.Domain));


            string url = _configuration["JasperReportSettings:JasperOutputUrl"];
            url = url.Replace("{requestID}", requestId);
            url = url.Replace("{exportID}", exportId);
            
            var zzz = await _invoker.GetAsync(header, new object(), url, _cookieJar);
            return zzz.Content;
        }
        catch (System.Exception) { throw; }
    }


    public async Task<string> JasperReportExecution(ReportExecutionRequest request)
    {
        try
        {
            Dictionary<string, string> header = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Authorization", "Basic " + _configuration["JasperReportSettings:AuthorizedToken"] }
                };

            var response = await _invoker.PostAsync(header, request, _configuration["JasperReportSettings:JasperExecuteUrl"]);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var output = JsonConvert.DeserializeObject<ReportExecutionResponse>(response.Content) ?? new ReportExecutionResponse();
                var sessionCookie = response.Cookies.SingleOrDefault(x => x.Name == "JSESSIONID");

                //Polling loop check for fetching status

                //Output
                string t = await JasperOutput(output.requestId, output.exports.First().id, sessionCookie);

                return t;

            }
            else
            {
                return "";
            }

        }
        catch (System.Exception) { throw; }
    }

}