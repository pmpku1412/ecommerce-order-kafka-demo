// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using TQM.Backoffice.Domain.ValueObjects;

namespace TQM.BackOffice.Identity.Helpers;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissionsClaim = context.User.Claims.SingleOrDefault(c => c.Type == ClaimType.Permissions.Value);
        // If user does not have the scope claim, get out of here
        if (permissionsClaim == null) return Task.CompletedTask;
        if (permissionsClaim.Value.ThisPermissionIsAllowed(requirement.PermissionName)) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

