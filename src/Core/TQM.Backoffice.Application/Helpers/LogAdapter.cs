using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TQM.Backoffice.Application.Helpers;

public interface ILogAdapter
{
    Task<string> GetCurrentMethod();
    Task WriteLog(string message);
    Task WriteTextFileRequest(string Method, object request, object resposnes, string useRequest);
}

public class LogAdapter : ILogAdapter
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public async Task<string> GetCurrentMethod()
    {
        return await Task.Run(() =>
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1) ?? new StackFrame();
            return sf.GetMethod()?.Name ?? string.Empty;
        });
    }

    public async Task WriteLog(string message)
    {
        await Task.Run(() =>
        {
            string folderDate = DateTime.Now.ToString("yyyyMM") + @"\";
            string pathLog = AppDomain.CurrentDomain.BaseDirectory + "/Log/" + folderDate;

            if (!Directory.Exists(pathLog)) Directory.CreateDirectory(pathLog);
            using StreamWriter writer = new StreamWriter(pathLog + DateTime.Today.ToString("ddMMyyyy") + ".txt", true);
            writer.WriteLine(DateTime.Now.ToString() + " : " + message);
            writer.Close();

        });
    }
    public async Task WriteTextFileRequest(string Method, object request, object resposnes, string useRequest)
    {
        try
        {
            await Task.Run(() =>
                    {
                        string textRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                        string textResponse = Newtonsoft.Json.JsonConvert.SerializeObject(resposnes);
                        string folderDate = DateTime.Now.ToString("yyyyMM") + @"\";
                        string pathLog = AppDomain.CurrentDomain.BaseDirectory + "/LogUseApi/" + folderDate;

                        if (!Directory.Exists(pathLog)) Directory.CreateDirectory(pathLog);
                        using StreamWriter writer = new StreamWriter(pathLog + DateTime.Today.ToString("ddMMyyyy") + ".txt", true);
                        writer.WriteLine(DateTime.Now.ToString() + ": Method :" + Method + " : UseRequest : " + useRequest + " : " + " Request : " + textRequest + ": Response" + textResponse);
                        writer.Close();

                    });
        }
        catch
        {

        }

    }
}
