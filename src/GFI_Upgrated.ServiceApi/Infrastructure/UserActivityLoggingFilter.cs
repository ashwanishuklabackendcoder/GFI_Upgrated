using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Text.Json;
using GFI_Upgrated.SharedDto.AdminSecurity;
using GFI_Upgrated.ServiceApi.Services;

namespace GFI_Upgrated.ServiceApi.Infrastructure;

public class UserActivityLoggingFilter : IAsyncActionFilter
{
    private readonly IAdminSecurityService _securityService;

    public UserActivityLoggingFilter(IAdminSecurityService securityService)
    {
        _securityService = securityService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        // Only log state-modifying HTTP methods (POST, PUT, DELETE, PATCH)
        var method = context.HttpContext.Request.Method.ToUpper();
        if (method == "GET" || method == "HEAD" || method == "OPTIONS")
        {
            return;
        }

        // Skip logging if request resulted in unhandled exception or status >= 400
        if (resultContext.Exception != null && !resultContext.ExceptionHandled)
        {
            return;
        }

        var statusCode = context.HttpContext.Response.StatusCode;
        if (statusCode >= 400)
        {
            return;
        }

        try
        {
            var user = context.HttpContext.User;
            var userName = user.Identity?.Name 
                         ?? user.FindFirst(ClaimTypes.Name)?.Value 
                         ?? user.FindFirst("LoginName")?.Value 
                         ?? "System";

            long? loginId = null;
            var loginIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                            ?? user.FindFirst("LoginId")?.Value;
            if (long.TryParse(loginIdClaim, out var parsedLoginId))
            {
                loginId = parsedLoginId;
            }

            var eventName = method switch
            {
                "POST" => "INSERT",
                "PUT" or "PATCH" => "UPDATE",
                "DELETE" => "DELETE",
                _ => method
            };

            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "System";
            var actionName = context.RouteData.Values["action"]?.ToString() ?? "Action";

            // Extract proper record name/value from request arguments
            string? entityName = null;
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg == null) continue;
                entityName = ExtractItemName(arg);
                if (!string.IsNullOrWhiteSpace(entityName)) break;
            }

            // Build clean user-friendly Remark text showing proper values without IDs
            var friendlyAction = method switch
            {
                "POST" => "Created new",
                "PUT" or "PATCH" => "Updated",
                "DELETE" => "Deleted",
                _ => method
            };

            var friendlyEntity = controllerName switch
            {
                "Security" => "User Security / Role Assignment",
                "Staff" => "Staff Record",
                "Users" or "User" => "User Account",
                "RawMaterial" => "Raw Material",
                "FinishedProduct" => "Finished Product",
                "SemiFinishedProduct" => "Semi-Finished Product",
                "Purchase" => "Purchase Record",
                "ItemCategory" => "Item Category",
                "Brand" => "Brand Record",
                "Sku" => "SKU Record",
                "Unit" => "Unit Record",
                "Warehouse" => "Warehouse Record",
                "Kettle" => "Kettle Record",
                "Almirah" => "Almirah Record",
                "Status" => "Status Record",
                _ => controllerName
            };

            string remarks;
            if (!string.IsNullOrWhiteSpace(entityName))
            {
                remarks = $"{friendlyAction} {friendlyEntity} - {entityName}";
            }
            else
            {
                remarks = $"{friendlyAction} {friendlyEntity}";
            }

            var url = $"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}";

            // Extract Reference Key (RefKey) from route parameters if available
            string refKey = "0";
            foreach (var key in new[] { "id", "loginId", "roleId", "staffId", "itemId", "batchId", "orderId" })
            {
                if (context.RouteData.Values.TryGetValue(key, out var val) && val != null)
                {
                    refKey = val.ToString() ?? "0";
                    break;
                }
            }

            var logEntry = new UserActivityLogDto
            {
                UserName = userName,
                LoginId = loginId,
                DT = DateTime.Now,
                EventName = eventName,
                EventModule = controllerName,
                RefKey = refKey,
                Remarks = remarks,
                Url = url
            };

            await _securityService.InsertUserActivityLogAsync(logEntry);
        }
        catch
        {
            // Logging failure should never break the primary HTTP request flow
        }
    }

    private static string? ExtractItemName(object? model)
    {
        if (model == null) return null;

        var type = model.GetType();

        if (model is string s && !string.IsNullOrWhiteSpace(s) && s.Length < 100)
        {
            return s;
        }

        var propNames = new[]
        {
            "StaffName", "StaffFirstName", "LoginName", "UserName",
            "RawMaterialName", "ProductName", "FinishedProductName", "SemiFinishedProductName",
            "CategoryName", "ItemCategoryName", "BrandName", "UnitName", "RoleName",
            "ItemName", "Name", "Title", "Code"
        };

        foreach (var propName in propNames)
        {
            var prop = type.GetProperty(propName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (prop != null)
            {
                var val = prop.GetValue(model)?.ToString();
                if (!string.IsNullOrWhiteSpace(val))
                {
                    if (propName.Equals("StaffFirstName", StringComparison.OrdinalIgnoreCase))
                    {
                        var lastNameProp = type.GetProperty("StaffLastName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                        var lastName = lastNameProp?.GetValue(model)?.ToString();
                        if (!string.IsNullOrWhiteSpace(lastName))
                        {
                            return $"{val} {lastName}";
                        }
                    }
                    return val;
                }
            }
        }

        return null;
    }
}
