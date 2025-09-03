using TQM.Backoffice.Application.DTOs.Task;

namespace TQM.Backoffice.Application.Infrastructure.Persistence.Contracts;

public interface ITaskService
{
    Task<IEnumerable<ActionAlert>> GetActionAlert();
}
