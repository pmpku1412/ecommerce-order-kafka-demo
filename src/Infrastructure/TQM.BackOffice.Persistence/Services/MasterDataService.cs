using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQM.Backoffice.Application.Responses;
using TQM.Backoffice.Core.Application.DTOs.MasterData.Request;
using TQM.Backoffice.Core.Application.DTOs.MasterData.Response;

namespace TQM.BackOffice.Persistence.Services
{
    public class MasterdataService : IMasterdataService
    {
        private readonly IDBAdapter _adapter;
        public MasterdataService(IDBAdapter adapter)
        {
            _adapter = adapter;
        }

        public async Task<List<MasterDataResponse>> GetMasterdata(MasterDataRequest request)
        {
            using var connection = _adapter.GetConn(DataAccessTypeEnum.XININSURE);

            try
            {
                string extraCondition = "";
                if (string.Equals(request.COLUMNNAME, "SALEPIORITY", StringComparison.OrdinalIgnoreCase))
                {
                    extraCondition = "AND s.BYTECODE < '9'";
                }

                string query = $@"SELECT
                                    s.BYTECODE AS Id,
	                                s.BYTEDES AS Title
	                             FROM XININSURE.SYSBYTEDES s
	                             WHERE
                                    s.COLUMNNAME = :COLUMNNAME
                                    AND s.TABLENAME = :TABLENAME
                                    {extraCondition}
	                             ORDER BY s.BYTECODE ASC
                                 ";

                var data = await connection.QueryAsync<MasterDataResponse>(query, new
                {
                    COLUMNNAME = request.COLUMNNAME?.ToUpper(),
                    TABLENAME = request.TABLENAME?.ToUpper()
                });

                return data.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
