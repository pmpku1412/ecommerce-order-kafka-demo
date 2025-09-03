using Microsoft.AspNetCore.Mvc;
using API.Controllers;
using TQM.Backoffice.Core.Application.DTOs.MasterData.Response;
using TQM.Backoffice.Core.Application.Queries;
namespace TQM.BackOffice.API.Controllers;

[Route("[controller]")]
[Authorize, EnableCors, ApiController]
public class MasterDataController : ApiControllerBase
{
    [HttpPost("GetMasterdata")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<List<MasterDataResponse>>>> GetMasterData(GetMasterdata query) => Ok(await Mediator.Send(query));
}
