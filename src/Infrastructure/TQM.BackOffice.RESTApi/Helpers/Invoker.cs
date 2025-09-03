using System.Net;
using RestSharp;

namespace TQM.BackOffice.RESTApi.Helpers;

public interface IInvoker
{
    Task<IRestResponse<T>> GetAsync<T>(Dictionary<string, string> header, T data, string resource, CookieContainer? sessionCookie = null);
    Task<IRestResponse<T>> PostAsync<T>(Dictionary<string, string> header, T data, string resource);
}

public class Invoker : IInvoker
{
    private readonly IRestClient _restClient;

    public Invoker(IRestClient restClient) => _restClient = restClient;

    public async Task<IRestResponse<T>> GetAsync<T>(Dictionary<string, string> header, T data, string resource, CookieContainer? sessionCookie = null)
    {
        // if(sessionCookie is not null){}
        if(sessionCookie != null){
            _restClient.CookieContainer = sessionCookie;
        }
        return await _restClient.ExecuteGetAsync<T>(GenRequest(header, data, resource));
    }

    public async Task<IRestResponse<T>> PostAsync<T>(Dictionary<string, string> header, T data, string resource)
    {
        return await _restClient.ExecutePostAsync<T>(GenRequest(header, data, resource));
    }

    private static RestRequest GenRequest<T>(Dictionary<string, string> header, T data, string resource)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(data);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            RestRequest request = new RestRequest(resource);
            foreach (var pair in header) { request.AddHeader(pair.Key, pair.Value); }

            if (header.TryGetValue("Content-Type", out string? headerValue))
            {
                if (headerValue == "application/x-www-form-urlencoded")
                    request.AddParameter(headerValue, CryptoHelper.ConvertToQueryString(data), ParameterType.RequestBody);
                else if (headerValue == "application/json")
                    request.AddJsonBody(data);
            }

            return request;
        }
        catch (System.Exception) { throw; }
    }
}
