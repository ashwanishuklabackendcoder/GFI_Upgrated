using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace GFI_Upgrated.ServiceApi.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    public string Module { get; }
    public string Page { get; }
    public string Action { get; }

    public RequirePermissionAttribute(string module, string page, string action)
    {
        Module = module;
        Page = page;
        Action = action;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var isAdminClaim = user.FindFirst("IsAdmin")?.Value 
                        ?? user.FindFirst("isadmin")?.Value;
        var roleIdClaim = user.FindFirst(ClaimTypes.Role)?.Value 
                       ?? user.FindFirst("role")?.Value 
                       ?? user.FindFirst("RoleId")?.Value
                       ?? user.FindFirst("roleId")?.Value;

        var isSuperAdmin = string.Equals(isAdminClaim, "true", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(roleIdClaim, "1");

        // Super Admin or Admin bypasses individual permission checks
        if (isSuperAdmin)
        {
            return;
        }

        if (!long.TryParse(roleIdClaim, out var roleId))
        {
            if (user.Identity?.IsAuthenticated == true)
            {
                return;
            }
            context.Result = new ForbidResult();
            return;
        }

        var securityService = context.HttpContext.RequestServices.GetRequiredService<GFI_Upgrated.ServiceApi.Services.IAdminSecurityService>();
        var cache = context.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
        
        System.Diagnostics.Debug.WriteLine($"[RBAC] Checking access for {user.Identity.Name} to {Page} Action: {Action}");

        var cacheKey = $"Permission_{roleId}";
        if (!cache.TryGetValue(cacheKey, out System.Collections.Generic.IReadOnlyList<GFI_Upgrated.SharedDto.AdminSecurity.RolePermissionDto>? permissions) || permissions == null)
        {
            permissions = await securityService.GetRolePermissionsAsync(roleId, 0, context.HttpContext.RequestAborted);
            var cacheOptions = new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };
            cache.Set(cacheKey, permissions, cacheOptions);
        }

        if (roleId == 1)
        {
            return;
        }

        // Find the matching page by PageHeading, PagePath, or page alias
        var pagePerm = permissions.FirstOrDefault(p =>
               string.Equals(p.PageHeading, Page, StringComparison.OrdinalIgnoreCase)
            || string.Equals(p.PagePath?.Trim('/'), Page.Trim('/'), StringComparison.OrdinalIgnoreCase)
            || (string.Equals(Page, "Assign Roles", StringComparison.OrdinalIgnoreCase) &&
                (string.Equals(p.PageHeading, "User Roles", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.PagePath?.Trim('/'), "admin/user-roles", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.PagePath?.Trim('/'), "admin/assign-roles", StringComparison.OrdinalIgnoreCase)))
            || (string.Equals(Page, "User Roles", StringComparison.OrdinalIgnoreCase) &&
                (string.Equals(p.PageHeading, "Assign Roles", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.PagePath?.Trim('/'), "admin/user-roles", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.PagePath?.Trim('/'), "admin/assign-roles", StringComparison.OrdinalIgnoreCase)))
        );

        if (pagePerm == null)
        {
            System.Diagnostics.Debug.WriteLine($"[RBAC] Access DENIED: Page '{Page}' not found in role permissions.");
            context.Result = new ForbidResult();
            return;
        }

        bool hasAccess = Action.ToLower() switch
        {
            "view" => pagePerm.ViewPer,
            "insert" => pagePerm.InsertPer,
            "update" => pagePerm.UpdatePer,
            "delete" => pagePerm.DeletePer,
            "export" => pagePerm.ExportPer,
            "print" => pagePerm.PrintPer,
            "approve" => pagePerm.ApprovePer,
            _ => false
        };

        if (!hasAccess)
        {
            System.Diagnostics.Debug.WriteLine($"[RBAC] Access DENIED for {user.Identity.Name} to {Page} Action {Action}. Permission flag is FALSE.");
            context.Result = new ForbidResult();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[RBAC] Access GRANTED for {user.Identity.Name} to {Page} Action {Action}.");
        }
    }
}
