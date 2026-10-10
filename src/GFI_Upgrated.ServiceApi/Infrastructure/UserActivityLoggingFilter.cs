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

        // Check if the result is an ApiEnvelope and Success is false
        if (resultContext.Result is Microsoft.AspNetCore.Mvc.ObjectResult objResult)
        {
            var value = objResult.Value;
            if (value != null)
            {
                var successProp = value.GetType().GetProperty("Success");
                if (successProp != null && successProp.GetValue(value) is bool success && !success)
                {
                    return; // Action failed logically, do not log
                }
            }
        }

        try
        {
            var user = context.HttpContext.User;
            var userName = user.FindFirst("FullName")?.Value
                         ?? user.Identity?.Name 
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

            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "System";
            var actionName = context.RouteData.Values["action"]?.ToString() ?? "Action";

            bool isUpdate = (method == "PUT" || method == "PATCH");
            if (method == "POST")
            {
                foreach (var arg in context.ActionArguments.Values)
                {
                    if (arg == null || arg is string) continue;
                    var type = arg.GetType();
                    // Look for common ID property names
                    var idPropNames = new[] { "Id", $"{controllerName}Id", "ItemId", "ItemCatId", "BomId" }; // Added common ones
                    foreach (var propName in idPropNames)
                    {
                        var prop = type.GetProperty(propName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                        if (prop != null)
                        {
                            // if it has an ID > 0, and it's the primary entity, it's an update
                            // Note: For things like PreProcessing where BomId > 0, we need to be careful if PreProcessingId == 0.
                            // Let's specifically check for the exact ID matching the controller or generic 'Id' or 'ItemId'.
                            if (propName.Equals($"{controllerName}Id", StringComparison.OrdinalIgnoreCase) || 
                                propName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                                propName.Equals("ItemId", StringComparison.OrdinalIgnoreCase) ||
                                propName.Equals("ItemCatId", StringComparison.OrdinalIgnoreCase) ||
                                propName.Equals("AlmirahShelfID", StringComparison.OrdinalIgnoreCase))
                            {
                                var val = prop.GetValue(arg);
                                if (val is long lVal && lVal > 0) isUpdate = true;
                                if (val is int iVal && iVal > 0) isUpdate = true;
                                if (isUpdate) break;
                            }
                        }
                    }
                    if (isUpdate) break;
                }
            }

            var eventName = method switch
            {
                "POST" => isUpdate ? "UPDATE" : "INSERT",
                "PUT" or "PATCH" => "UPDATE",
                "DELETE" => "DELETE",
                _ => method
            };

            // Extract proper record name/value from request arguments
            string? entityName = null;
            
            if (context.HttpContext.Items.TryGetValue("EntityName", out var customEntity) && customEntity is string sCustom)
            {
                entityName = sCustom;
            }
            
            if (string.IsNullOrWhiteSpace(entityName))
            {
                foreach (var arg in context.ActionArguments)
                {
                    if (arg.Value == null) continue;
                    
                    if (arg.Key.Equals("updatedBy", StringComparison.OrdinalIgnoreCase) ||
                        arg.Key.Equals("createdBy", StringComparison.OrdinalIgnoreCase) ||
                        arg.Key.Equals("deletedBy", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    entityName = ExtractItemName(arg.Value);
                    if (!string.IsNullOrWhiteSpace(entityName)) break;
                }
            }

            // Build clean user-friendly Remark text showing proper values without IDs
            var friendlyAction = method switch
            {
                "POST" => isUpdate ? "Updated" : "Created new",
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
                "Production" => "Production Record",
                "PreProcessing" => "Pre-Processing Record",
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
                DT = DateTime.UtcNow,
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
            "ItemName", "Name", "Title", "Code", "BatchNo", "BatchNumberMade",
            "RequestNumber", "VoucherNumber", "InvoiceNo", "InvoiceNumber", "OrderNumber", "OrderNo"
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
