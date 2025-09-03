using TQM.Backoffice.Application.DTOs.Common;
using TQM.Backoffice.Core.Application.DTOs.MasterData.Response;
using TQM.Backoffice.Core.Application.DTOs.MasterData.Request;

namespace TQM.Backoffice.Core.Application.Queries;

public class GetMasterdata : IRequest<BaseResponse<List<MasterDataResponse>>>
{
    public MasterDataRequest Request { get; set; } = new();

    public class MasterdataHandler : IRequestHandler<GetMasterdata, BaseResponse<List<MasterDataResponse>>>
    {
        private readonly IMasterdataService _Imasterdataservice;

        public MasterdataHandler(IMasterdataService masterdataservice)
        {
            _Imasterdataservice = masterdataservice;
        }
        public async Task<BaseResponse<List<MasterDataResponse>>> Handle(GetMasterdata command, CancellationToken cancellationToken)
        {
            try
            {
                var response = new BaseResponse<List<MasterDataResponse>>();
                response.ResponseObject = await _Imasterdataservice.GetMasterdata(command.Request);
                response.Success = true;
                response.Message = "Success";

                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<MasterDataResponse>>() { Message = ex.Message };
            }
        }
    }
}
