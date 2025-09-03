namespace TQM.Backoffice.Application.DTOs.Common;

// DTOS for Jaspersoft report execution
// *becuase we use this for sending&retrive in JSON
// *, then properties name should be JSON style name (first small case)

// 1. ReportExecutionRequest: Report Execution Request sending to Jasper Server
//    - sub1-ReportParameters
//    - sub2-ReportParameter
// 2. Report Execution Request's response


public class ReportExecutionRequest
{
    public string reportUnitUri { get; set; } = string.Empty;
    public bool async { get; set; }
    public bool freshData { get; set; }
    public bool saveDataSnapshot { get; set; }
    public string outputFormat { get; set; } = string.Empty;
    public bool interactive { get; set; }
    public bool ignorePagination { get; set; }
    public string pages { get; set; }  = string.Empty; //1-5

    public ReportParameters Parameter { get; set; } = new();
}

public class ReportParameters
{
    public List<ReportParameter> reportParameter { get; set; } = new();
}

public class ReportParameter
{
    public String name { get; set; } = string.Empty;
    public List<String> value { get; set; } = new();

}

// 2. Report Execution Request's response
public class ReportExecutionResponse
{
    public string status { get; set; } = string.Empty;
    public string requestId { get; set; } = string.Empty;
    public string reportURI { get; set; } = string.Empty;
    public string cookiesSession { get; set; } = string.Empty;
    public List<ReportExecutionResponseExports> exports { get; set; } = new();
}

public class ReportExecutionResponseExports
{
    public string status { get; set; } = string.Empty;
    public string id { get; set; } = string.Empty;
}
