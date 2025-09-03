namespace TQM.Backoffice.Application.RESTApi.Line.Contracts;

public interface ILineNotification
{
    Task SubmitLineGroup(string message);
}
