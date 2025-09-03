using Microsoft.AspNetCore.Http;

namespace TQM.Backoffice.Application.Helpers;

public interface IHelperGetStaff
{
    string? GetStaffCode();
    string? GetStaffInfo(string type);
}
public class GetStaff : IHelperGetStaff
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetStaff(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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

    public string? GetStaffInfo(string type)
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Claims.Where(c => c.Type.ToUpper() == type.ToUpper()).Select(x => x.Value).FirstOrDefault();
        }
        catch (System.Exception) { throw; }
    }

}


