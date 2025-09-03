using System.ComponentModel;

namespace TQM.BackOffice.Identity.Helpers;

public static class PermissionChecker
{
    public static bool ThisPermissionIsAllowed(this string packedPermissions, string permissionName)
    {
        var usersPermissions = packedPermissions.UnpackPermissionsFromString().ToArray();

        if (!Enum.TryParse(permissionName, true, out PermissionEnum permissionToCheck))
            throw new InvalidEnumArgumentException($"{permissionName} could not be converted to a {nameof(PermissionEnum)}.");

        return usersPermissions.Contains(permissionToCheck);
    }
}
