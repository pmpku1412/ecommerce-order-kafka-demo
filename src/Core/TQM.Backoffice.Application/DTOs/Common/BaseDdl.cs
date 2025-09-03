namespace TQM.Backoffice.Application.DTOs.Common;

public class BaseDdl
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
}

public class BaseDdlWithRelate : BaseDdl
{
    public string Ref_1 { get; set; } = "";
    public string Ref_2 { get; set; } = "";
    public string Ref_3 { get; set; } = "";

    public string Ref_1_Source { get; set; } = "";
    public string Ref_2_Source { get; set; } = "";
    public string Ref_3_Source { get; set; } = "";

    public string Code { get; set; } = "";
}
