using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using TQM.Backoffice.Domain.Enums;

namespace TQM.BackOffice.GetAPIPermission;

public class ApiFunctionService
{

    private readonly IConfiguration _configuration;

    public ApiFunctionService(IConfiguration configuration) => _configuration = configuration;

    public void SysFunctionProcess()
    {
        string functionName = string.Empty, functionCode = string.Empty, functionGroup = string.Empty, functionDesc = string.Empty, httpMethod = string.Empty;
        var permissions = Enum.GetValues(typeof(PermissionEnum));

        if (permissions.Length > 0)
        {
            string connectionString = _configuration["ConnectionStrings:ESMDataAccess"] ?? string.Empty;
            OracleConnection oracleConn = new(connectionString);
            try
            {
                if (oracleConn.State == ConnectionState.Closed) oracleConn.Open();
                oracleConn.ActionName = "ESM";
                string query = "DELETE FROM ESM.SYSFUNCTION";
                oracleConn.Execute(query);

                string insert = @"INSERT INTO ESM.SYSFUNCTION(FUNCTIONCODE,FUNCTIONNAME,FUNCTIONGROUP,FUNCTIONDESC,HTTPMETHOD) 
                                    VALUES (:FUNCTIONCODE,:FUNCTIONNAME,:FUNCTIONGROUP,:FUNCTIONDESC,:HTTPMETHOD)";

                foreach (var permission in permissions)
                {
                    functionName = permission.ToString() ?? string.Empty;
                    functionCode = string.Format("0x{0:x3}", (short)permission);

                    object[] valueAttributes = GetEnumAttributes(functionName);
                    functionGroup = ((DisplayAttribute)valueAttributes[0]).GroupName ?? string.Empty;
                    functionDesc = ((DisplayAttribute)valueAttributes[0]).Description ?? string.Empty;
                    httpMethod = ((DisplayAttribute)valueAttributes[0]).Name ?? string.Empty;

                    DynamicParameters parameters = new();
                    parameters.Add("FUNCTIONCODE", functionCode);
                    parameters.Add("FUNCTIONNAME", functionName);
                    parameters.Add("FUNCTIONGROUP", functionGroup);
                    parameters.Add("FUNCTIONDESC", functionDesc);
                    parameters.Add("HTTPMETHOD", httpMethod);

                    oracleConn.Execute(insert, parameters);
                }

            }
            catch (System.Exception ex) { var xx = ex.Message; throw; }
        }

        static object[] GetEnumAttributes(string functionName)
        {
            var permissionEnum = typeof(PermissionEnum);
            var memberInfos = permissionEnum.GetMember(functionName);
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == permissionEnum);
            var valueAttributes = enumValueMemberInfo?.GetCustomAttributes(typeof(DisplayAttribute), false) ?? Array.Empty<object>();
            return valueAttributes;
        }
    }
}
