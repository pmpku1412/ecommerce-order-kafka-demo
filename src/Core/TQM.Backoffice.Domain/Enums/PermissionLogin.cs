namespace TQM.Backoffice.Domain.Enums;

public enum PermissionLogin
{
    /// <summary>
    /// login Pass
    /// </summary>
    Pass = 1,
    /// <summary>
    /// password expire
    /// </summary>
    Password_expire = 2,
    /// <summary>
    /// password expire or forcechangepassword
    /// </summary>
    Password_expire_forcechange = 3,
    /// <summary>
    /// login Pass with password near expiration date
    /// </summary>
    Pass2 = 30,
    /// <summary>
    /// password expire
    /// </summary>
    Password_expire2 = 31,
    /// <summary>
    /// password expire
    /// </summary>
    Userpass_Incorrect = -1,
    /// <summary>
    /// User Password not correct
    /// </summary>
    Userpass_Incorrect2 = -2,
    /// <summary>
    /// User Password not correct
    /// </summary>
    Password_expire3 = -3,
    /// <summary>
    /// password expire
    /// </summary>
    Password_expire4 = -4,
    /// <summary>
    /// login outside area
    /// </summary>
    outside_area = -5,
    /// <summary>
    /// Staff Exit
    /// </summary>
    staff_exit = -6,
    /// <summary>
    /// login Pass on LDAP
    /// </summary>
    Pass_ldap = 0,

    Pass_PRODUCTAPI = 5
}
