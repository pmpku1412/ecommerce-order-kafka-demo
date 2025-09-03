using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Net;

namespace TQM.BackOffice.Persistence.Helpers;

public enum DataAccessTypeEnum
{
    SECURITY,
    UBILOG,
    PRINTSERV,
    PRINTSERVDEV,
    CONMFLOW,
    INSUREDB,
    SALE,
    CLAIM,
    LOGGING,
    ESM,
    XININSURE,
    ESM_DEV,
    ESM_PRE,
    ESM_PRO,
    ESM_TQMSALE,
    ESM_TQMSALE_DEV,
    ESM_TQMSALE_PRE,
    ESM_TQMSALE_PRO,
    HRM,
    BUDGET,
    CONISERVICE,
    CONCHATCENTER,
    DSR,
    TQMSALE127
}

public interface IDBAdapter
{
    OracleConnection GetConn(DataAccessTypeEnum dataAccess, string? staffCode = null);
    SqlConnection GetConnSQL(DataAccessTypeEnum dataAccess, string? staffCode = null);
    string? GetStaffCode();
    string? GetStaffDBLoginCode();
    string? GetStaffInfo(string type);
    string? GetActionName();
    Task<string> GetQueryStringFromProp(IEnumerable<Tuple<string, object?>> props, string query, DynamicParameters parameters);
    Task<string> GetInsertQueryStringFromProp(IEnumerable<Tuple<string, object?>> props, string TableNameWithSchema, DynamicParameters parameters);
    Task<string> GetInsertQueryStringFromProp2(IEnumerable<Tuple<string, object?, object?>> props, string TableNameWithSchema, DynamicParameters parameters);
    Task<string> GetUpdateQueryStringFromProp(IEnumerable<Tuple<string, object?, object?>> propsUpdatePart, IEnumerable<Tuple<string, object?, object?>> propsWherePart, string TableNameWithSchema, DynamicParameters parameters);
    public string? GetIPAddress();
    public string? GetHostName();
}

public class DBAdapter : IDBAdapter
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public DBAdapter(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        ActionName = string.Empty;
        IPAddress = string.Empty;
        HostName = string.Empty;
    }

    public string ActionName;
    public string IPAddress;
    public string HostName;

    public async Task<string> GetQueryStringFromProp(IEnumerable<Tuple<string, object?>> props, string query, DynamicParameters parameters)
    {
        return await Task.Run(() =>
        {
            if (!props.Any()) throw new Exception("ไม่มีข้อมูลที่จะนำมา update");

            var last = props.Last();
            foreach (var prop in props)
            {
                string propValStr = prop.Item2?.ToString() ?? string.Empty;
                var parseSuccess = long.TryParse(prop.Item2?.ToString(), out long propValNum);
                object? propValue = propValStr == "NULL" || (propValNum == 0L && parseSuccess) ? null : prop.Item2;
                query += $" {prop.Item1} = :{prop.Item1}";
                if (!prop.Equals(last)) query += ",";
                parameters.Add(prop.Item1, propValue);
            }

            return query;

        });
    }

    public async Task<string> GetInsertQueryStringFromProp(IEnumerable<Tuple<string, object?>> props, string TableNameWithSchema, DynamicParameters parameters)
    {
        return await Task.Run(() =>
        {
            if (!props.Any()) throw new Exception("ไม่มีข้อมูลที่จะนำมา Insert");

            var last = props.Last();

            string query = $@"INSERT INTO {TableNameWithSchema}";

            // Column List

            query += @"(";
            foreach (var prop in props)
            {
                // Get Value
                //string propValStr = prop.Item2?.ToString() ?? string.Empty;
                //var parseSuccess = long.TryParse(prop.Item2?.ToString(), out long propValNum);
                //object? propValue = propValStr == "NULL" || (propValNum == 0L && parseSuccess) ? null : prop.Item2;
                query += $" {prop.Item1}";
                if (!prop.Equals(last)) 
                    query += ",";
            }
            query += @")";

            query += @" VALUES(";
            // Values List
            foreach (var prop in props)
            {
                string propValStr = prop.Item2?.ToString() ?? string.Empty;
                var parseSuccess = long.TryParse(prop.Item2?.ToString(), out long propValNum);
                object? propValue = propValStr == "NULL" || (propValNum == 0L && parseSuccess) ? null : prop.Item2;
                query += $":{prop.Item1}";
                if (!prop.Equals(last)) query += ",";
                parameters.Add(prop.Item1, propValue);
            }
            query += @")";

            return query;

        });
    }
    
    public async Task<string> GetInsertQueryStringFromProp2(IEnumerable<Tuple<string, object?, object?>> props, string TableNameWithSchema, DynamicParameters parameters)
    {
        return await Task.Run(() =>
        {
            if (!props.Any()) throw new Exception("ไม่มีข้อมูลที่จะนำมา Insert");

            var last = props.Last();

            string query = $@"INSERT INTO {TableNameWithSchema}";

            // Column List

            query += @"(";
            foreach (var prop in props)
            {
                query += $" {prop.Item1}";
                if (!prop.Equals(last)) 
                    query += ",";
            }
            query += @")";

            query += @" VALUES(";
            // Values List
            foreach (var prop in props)
            {
                string propValStr = prop.Item2?.ToString() ?? string.Empty;
                var parseSuccess = long.TryParse(prop.Item2?.ToString(), out long propValNum);
                object? propValue = propValStr == "NULL" || (propValNum == 0L && parseSuccess) ? null : prop.Item2;
                query += $":{prop.Item1}";
                if (!prop.Equals(last)) 
                    query += ",";
                
                if ((prop.Item3 != prop.Item2) && (propValue == null))    // Assign not null value like 0 in nullable field
                    parameters.Add(prop.Item1, prop.Item2);
                else
                    parameters.Add(prop.Item1, propValue);
            }
            query += @")";

            return query;

        });
    }
    

    public async Task<string> GetUpdateQueryStringFromProp(IEnumerable<Tuple<string, object?, object?>> propsUpdatePart, IEnumerable<Tuple<string, object?, object?>> propsWherePart, string TableNameWithSchema, DynamicParameters parameters)
    {
        return await Task.Run(() =>
        {
            if ( (!propsUpdatePart.Any()) || (!propsWherePart.Any())) throw new Exception("ไม่มีข้อมูลที่จะนำมา Update");

            var lastUpdate = propsUpdatePart.Last();
            var lastWhere = propsWherePart.Last();
            var firstWhere = propsWherePart.First();

            string query = $@"UPDATE {TableNameWithSchema}";

            // Update Part

            query += @" SET ";
            foreach (var prop in propsUpdatePart)
            {
                query += $" {prop.Item1} = :S_{prop.Item1}";
                if (!prop.Equals(lastUpdate)) 
                    query += " ,";

                string propValStr = prop.Item2?.ToString() ?? string.Empty;
                var parseSuccess = long.TryParse(prop.Item2?.ToString(), out long propValNum);
                object? propValue = propValStr == "NULL" || (propValNum == 0L && parseSuccess) ? null : prop.Item2;
                parameters.Add("S_" + prop.Item1, propValue);
            }
            query += @"";

            // WHERE Part

            query += @" WHERE ";
            foreach (var prop in propsWherePart)
            {
                string propValStr = prop.Item2?.ToString() ?? string.Empty;
                var parseSuccess = long.TryParse(prop.Item2?.ToString(), out long propValNum);
                object? propValue = propValStr == "NULL" || (propValNum == 0L && parseSuccess) ? null : prop.Item2;
                

                if (propValue != null)
                {
                    if (prop.Equals(firstWhere)) 
                        query += $" {prop.Item1} = :W_{prop.Item1}";
                    else
                        query += $" AND  {prop.Item1} = :W_{prop.Item1}";

                    parameters.Add("W_" + prop.Item1, propValue);
                }
                else
                {
                    if (prop.Equals(firstWhere)) 
                        query += $" {prop.Item1} IS NULL";
                    else
                        query += $" AND  {prop.Item1} IS NULL ";            
                }                
            }
            query += @"";

            return query;

        });
    }
    

    public OracleConnection GetConn(DataAccessTypeEnum dataAccess, string? staffCode = null)
    {
        try
        {
            string connectionString = string.Empty;

            switch (dataAccess)
            {
                case DataAccessTypeEnum.SECURITY:
                    connectionString = _configuration["ConnectionStrings:SecurityDataAccess"];
                    break;
                case DataAccessTypeEnum.SALE:
                    connectionString = _configuration["ConnectionStrings:SaleDataAccess"];
                    break;
                case DataAccessTypeEnum.LOGGING:
                    connectionString = _configuration["ConnectionStrings:LoggingAccess"];
                    break;
                case DataAccessTypeEnum.CLAIM:
                    connectionString = _configuration["ConnectionStrings:ClaimDataAccess"];
                    break;
                case DataAccessTypeEnum.INSUREDB:
                    connectionString = _configuration["ConnectionStrings:InsureDBAccess"];
                    break;
                case DataAccessTypeEnum.PRINTSERV:
                    connectionString = _configuration["ConnectionStrings:PrintServDataAccess"];
                    break;
                case DataAccessTypeEnum.ESM:
                    connectionString = _configuration["ConnectionStrings:ESMDataAccess"];
                    break;
                case DataAccessTypeEnum.XININSURE:
                    connectionString = _configuration["ConnectionStrings:XininsureDataAccess"];
                    break;
                case DataAccessTypeEnum.ESM_DEV:
                    connectionString = _configuration["ConnectionStrings:ESMDevDataAccess"];
                    break;
                case DataAccessTypeEnum.ESM_PRE:
                    connectionString = _configuration["ConnectionStrings:ESMDataAccess_129"];
                    break;
                case DataAccessTypeEnum.ESM_PRO:
                    connectionString = _configuration["ConnectionStrings:ESMDataAccess_PRO"];
                    break;
                case DataAccessTypeEnum.ESM_TQMSALE:
                    connectionString = _configuration["ConnectionStrings:TQMSALEDataAccess"];
                    break;
                case DataAccessTypeEnum.ESM_TQMSALE_DEV:
                    connectionString = _configuration["ConnectionStrings:TQMSALEDataAccess_DEV"];
                    break;
                case DataAccessTypeEnum.ESM_TQMSALE_PRE:
                    connectionString = _configuration["ConnectionStrings:TQMSALEDataAccess_PRE"];
                    break;
                case DataAccessTypeEnum.ESM_TQMSALE_PRO:
                    connectionString = _configuration["ConnectionStrings:TQMSALEDataAccess_PRO"];
                    break;
                case DataAccessTypeEnum.CONISERVICE:
                    connectionString = _configuration["ConnectionStrings:CONISERVICEAccess"];
                    break;
                case DataAccessTypeEnum.CONCHATCENTER:
                    connectionString = _configuration["ConnectionStrings:CONCHATCENTERAccess"];
                    break;
                case DataAccessTypeEnum.DSR:
                    connectionString = _configuration["ConnectionStrings:DSRAccess"];
                    break;
                case DataAccessTypeEnum.TQMSALE127:
                    connectionString = _configuration["ConnectionStrings:TQMSALE127Access"];
                    break;
                default:
                    break;
            }

            if ((_configuration["ForceUseMainConnect"] ?? "YES").ToUpper() != "YES")
            {
                switch(dataAccess)
                {
                    case DataAccessTypeEnum.ESM:
                    {
                        switch (_configuration["Env"].ToUpper())
                        {
                            case "DEV" : connectionString = _configuration["ConnectionStrings:ESMDevDataAccess"]; break;
                            case "PRE" : connectionString = _configuration["ConnectionStrings:ESMDataAccess_129"]; break;
                            case "PRO" : connectionString = _configuration["ConnectionStrings:ESMDataAccess_PRO"]; break;
                        }
                    } break;
                    case DataAccessTypeEnum.ESM_TQMSALE:
                    {
                        switch (_configuration["Env"].ToUpper())
                        {
                            case "DEV" : connectionString = _configuration["ConnectionStrings:TQMSALEDataAccess_DEV"]; break;
                            case "PRE" : connectionString = _configuration["ConnectionStrings:TQMSALEDataAccess_PRE"]; break;
                            case "PRO" : connectionString = _configuration["ConnectionStrings:TQMSALEDataAccess_PRO"]; break;
                        }
                    } break;
                }
            }

            if (string.IsNullOrEmpty(connectionString)) throw new Exception("Connection Not Found");

            OracleConnection oracleConn = new(connectionString);
            if (oracleConn.State == ConnectionState.Closed) oracleConn.Open();
            oracleConn.ActionName = ActionName = staffCode ?? GetStaffCode() ?? string.Empty;

            // Force To Use Only IP at 28/02/2023
            string HostName = GetIPAddress() ?? string.Empty;

            oracleConn.ModuleName = HostName;
            return oracleConn;
        }
        catch (System.Exception) { throw; }
    }

    public SqlConnection GetConnSQL(DataAccessTypeEnum dataAccess, string? staffCode = null)
    {
        try
        {
            string connectionString = string.Empty;

            switch (dataAccess)
            {
                case DataAccessTypeEnum.HRM:
                    connectionString = _configuration["ConnectionStrings:HRMDataAccess"];
                    break;
                case DataAccessTypeEnum.BUDGET:
                    connectionString = _configuration["ConnectionStrings:BGDataAccess"];
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(connectionString)) throw new Exception("Connection Not Found");

            SqlConnection sqlConn = new(connectionString);
            if (sqlConn.State == ConnectionState.Closed) sqlConn.Open();
            return sqlConn;
        }
        catch (System.Exception) { throw; }
    }

    public string? GetStaffCode()
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.Where(c => c.Type == "StaffCode").Select(x => x.Value).FirstOrDefault();
        }
        catch (System.Exception) { throw; }
    }

    public string? GetStaffDBLoginCode()
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.Where(c => c.Type == "DbLogin").Select(x => x.Value).FirstOrDefault();
        }
        catch (System.Exception) { throw; }
    }

    public string? GetStaffInfo(string type)
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.Where(c => c.Type == type).Select(x => x.Value).FirstOrDefault();
        }
        catch (System.Exception) { throw; }
    }

    public string? GetActionName()
    {
        return ActionName;
    }

    public string? GetIPAddress()
    {
        try
        {
            //this.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var user = _httpContextAccessor.HttpContext?.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return user?.ToString();
        }
        catch (System.Exception) { throw; }
    }
    public string? GetHostName()
    {
        try
        {
            String? IPAddress = GetIPAddress();

            try
            {
                if (string.IsNullOrEmpty(IPAddress) == false)
                    return Dns.GetHostEntry(IPAddress).HostName.ToString();
                else
                    return IPAddress;
            }
            catch (System.Exception) 
            { 
                return IPAddress;
            }
        }
        catch (System.Exception)
        {
            return String.Empty;
        }
    }
}
