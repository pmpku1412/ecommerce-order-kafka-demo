using TQM.Backoffice.Application.DTOs.Mail;
using TQM.Backoffice.Core.Application.DTOs.MasterData.Request;
using TQM.Backoffice.Core.Application.DTOs.MasterData.Response;
namespace TQM.Backoffice.Application.Infrastructure.Persistence.Contracts;

public interface IMasterdataService
{
    Task<List<MasterDataResponse>> GetMasterdata(MasterDataRequest request);
}
