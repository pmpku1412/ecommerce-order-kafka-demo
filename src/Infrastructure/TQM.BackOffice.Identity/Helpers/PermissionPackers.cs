// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Globalization;

namespace TQM.BackOffice.Identity.Helpers;

public static class PermissionPackers
{
    public const char packType = 'H';
    public const int packedSize = 3;

    public static string FormDefaultPackPrefix()
    {
        return $"{packType}{packedSize:D1}-";
    }

    public static string PackPermissionsIntoString(this IEnumerable<PermissionEnum> permissions)
    {
        return permissions.Aggregate(FormDefaultPackPrefix(), (s, permission) => s + ((int)permission).ToString("X3"));
    }

    public static IEnumerable<PermissionEnum> UnpackPermissionsFromString(this string packedPermissions)
    {
        return packedPermissions.UnpackPermissionValuesFromString().Select(x => ((PermissionEnum)x));
    }

    public static IEnumerable<int> UnpackPermissionValuesFromString(this string packedPermissions)
    {
        var packPrefix = FormDefaultPackPrefix();
        if (packedPermissions == null) throw new ArgumentNullException(nameof(packedPermissions));
        if (!packedPermissions.StartsWith(packPrefix))
            throw new InvalidOperationException("The format of the packed permissions is wrong" +
                                                $" - should start with {packPrefix}");

        int index = packPrefix.Length;
        while (index < packedPermissions.Length)
        {
            yield return int.Parse(packedPermissions.Substring(index, packedSize), NumberStyles.HexNumber);
            index += packedSize;
        }
    }

    public static PermissionEnum? FindPermissionViaName(this string permissionName)
    {
        return Enum.TryParse(permissionName, out PermissionEnum permission) ? permission : null;
    }
}
