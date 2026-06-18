using GFI_Upgrated.ServiceApi.Services;
using GFI_Upgrated.SharedDto.AdminSecurity;
using GFI_Upgrated.SharedDto.Common;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace GFI_Upgrated.ServiceApi.Controllers.Admin;

[ApiController]
[Route("api/admin/security")]
public sealed class SecurityController : ControllerBase
{
    private readonly IAdminSecurityService _service;
    private readonly IConfiguration _configuration;
    private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
    private readonly IMemoryCache _memoryCache;

    public SecurityController(IAdminSecurityService service, IConfiguration configuration, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IMemoryCache memoryCache)
    {
        _service = service;
        _configuration = configuration;
        _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        _memoryCache = memoryCache;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiEnvelope<LoginResultDto>>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.LoginAsync(request, cancellationToken);
            if (result is null)
            {
                return Ok(new ApiEnvelope<LoginResultDto>
                {
                    Success = false,
                    Message = "Invalid login credentials."
                });
            }

            if (result != null)
            {
                result.Token = GenerateJwtToken(result);
                await _service.InsertUserActivityLogAsync(new UserActivityLogDto
                {
                    UserName = $"{result.FirstName} {result.LastName}".Trim(),
                    LoginName = result.LoginName,
                    DT = DateTime.Now,
                    EventName = "Login",
                    EventModule = "Security",
                    RefKey = result.LoginId.ToString(),
                    Remarks = $"User {result.LoginName} logged in successfully.",
                    Url = "/login",
                    LoginId = result.LoginId
                }, cancellationToken);
            }

            return Ok(new ApiEnvelope<LoginResultDto>
            {
                Success = true,
                Message = "Login successful.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred during login for user {LoginName}", request?.LoginName);
            return StatusCode(500, new ApiEnvelope<LoginResultDto>
            {
                Success = false,
                Message = $"Server Error: {ex.Message}. Details: {ex.InnerException?.Message ?? "None"}"
            });
        }
    }

    [HttpGet("roles")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<RoleDto>>>> GetRoles([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        var result = await _service.GetRolesAsync(request, searchText, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<RoleDto>>
        {
            Success = true,
            Message = "Roles loaded.",
            Data = result
        });
    }

    [HttpGet("roles/{roleId:long}")]
    public async Task<ActionResult<ApiEnvelope<RoleDto?>>> GetRoleById(long roleId, CancellationToken cancellationToken)
    {
        var result = await _service.GetRoleByIdAsync(roleId, cancellationToken);
        return Ok(new ApiEnvelope<RoleDto?>
        {
            Success = result is not null,
            Message = result is null ? "Role not found." : "Role loaded.",
            Data = result
        });
    }

    [HttpPost("roles")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveRole([FromBody] SaveRoleRequest request, CancellationToken cancellationToken)
    {
        var id = await _service.SaveRoleAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "Role saved successfully." : "Role save failed.",
            Data = id
        });
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<UserDto>>>> GetUsers([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        var result = await _service.GetUsersAsync(request, searchText, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<UserDto>>
        {
            Success = true,
            Message = "Users loaded.",
            Data = result
        });
    }

    [HttpGet("users/{loginId:long}")]
    public async Task<ActionResult<ApiEnvelope<UserDto?>>> GetUserById(long loginId, CancellationToken cancellationToken)
    {
        var result = await _service.GetUserByIdAsync(loginId, cancellationToken);
        return Ok(new ApiEnvelope<UserDto?>
        {
            Success = result is not null,
            Message = result is null ? "User not found." : "User loaded.",
            Data = result
        });
    }

    [HttpPost("users")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveUser([FromBody] SaveUserRequest request, CancellationToken cancellationToken)
    {
        var id = await _service.SaveUserAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "User saved successfully." : "User save failed.",
            Data = id
        });
    }

    [HttpGet("staff")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<StaffDto>>>> GetStaff([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        var result = await _service.GetStaffsAsync(request, searchText, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<StaffDto>>
        {
            Success = true,
            Message = "Staff loaded.",
            Data = result
        });
    }

    [HttpGet("staff/{staffId:long}")]
    public async Task<ActionResult<ApiEnvelope<StaffDto?>>> GetStaffById(long staffId, CancellationToken cancellationToken)
    {
        var result = await _service.GetStaffByIdAsync(staffId, cancellationToken);
        return Ok(new ApiEnvelope<StaffDto?>
        {
            Success = result is not null,
            Message = result is null ? "Staff not found." : "Staff loaded.",
            Data = result
        });
    }

    [HttpPost("staff")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveStaff([FromBody] SaveStaffRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).Where(message => !string.IsNullOrWhiteSpace(message)));
            return Ok(new ApiEnvelope<int>
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(message) ? "Invalid staff data." : message
            });
        }

        var id = await _service.SaveStaffAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "Staff saved successfully." : "Staff save failed.",
            Data = id
        });
    }

    [HttpDelete("staff/{staffId:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteStaff(long staffId, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteStaffAsync(staffId, updatedBy ?? "System", cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = result >= 0,
            Message = result >= 0 ? "Staff deleted successfully." : "Staff delete failed.",
            Data = result
        });
    }

    [HttpGet("menus")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<MenuDto>>>> GetMenus(CancellationToken cancellationToken)
    {
        var result = await _service.GetMenusAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<MenuDto>>
        {
            Success = true,
            Message = "Menus loaded.",
            Data = result
        });
    }

    [HttpGet("menus/{linkId:long}")]
    public async Task<ActionResult<ApiEnvelope<MenuDto?>>> GetMenuById(long linkId, CancellationToken cancellationToken)
    {
        var result = await _service.GetMenuByIdAsync(linkId, cancellationToken);
        return Ok(new ApiEnvelope<MenuDto?>
        {
            Success = result is not null,
            Message = result is null ? "Menu not found." : "Menu loaded.",
            Data = result
        });
    }

    [HttpPost("menus")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveMenu([FromBody] SaveMenuRequest request, CancellationToken cancellationToken)
    {
        var id = await _service.SaveMenuAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "Menu saved successfully." : "Menu save failed.",
            Data = id
        });
    }

    [HttpGet("modules")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<ModuleDto>>>> GetModules(CancellationToken cancellationToken)
    {
        var result = await _service.GetModulesAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<ModuleDto>>
        {
            Success = true,
            Message = "Modules loaded.",
            Data = result
        });
    }

    [HttpGet("modules/{moduleId:long}/parent-menus")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<MenuDto>>>> GetParentMenus(long moduleId, CancellationToken cancellationToken)
    {
        var result = await _service.GetParentMenusAsync(moduleId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<MenuDto>>
        {
            Success = true,
            Message = "Parent menus loaded.",
            Data = result
        });
    }

    [HttpGet("dashboards")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<MenuDto>>>> GetDashboards(CancellationToken cancellationToken)
    {
        var result = await _service.GetDashboardsAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<MenuDto>>
        {
            Success = true,
            Message = "Dashboards loaded.",
            Data = result
        });
    }

    [HttpGet("roles/{roleId:long}/modules/{moduleId:long}/permissions")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<RolePermissionDto>>>> GetRolePermissions(long roleId, long moduleId, CancellationToken cancellationToken)
    {
        var result = await _service.GetRolePermissionsAsync(roleId, moduleId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<RolePermissionDto>>
        {
            Success = true,
            Message = "Role permissions loaded.",
            Data = result
        });
    }

    [HttpPost("roles/{roleId:long}/modules/{moduleId:long}/permissions")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveRolePermissions(long roleId, long moduleId, [FromBody] SaveRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        request.RoleID = roleId;
        request.ModuleID = moduleId;
        var savedCount = await _service.SaveRolePermissionsAsync(request, cancellationToken);
        
        // Invalidate permission cache for this role
        _memoryCache.Remove($"Permission_{roleId}");

        return Ok(new ApiEnvelope<int>
        {
            Success = savedCount >= 0,
            Message = "Role permissions saved.",
            Data = savedCount
        });
    }

    [HttpGet("users/{loginId:long}/roles")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<UserRoleAssignmentDto>>>> GetUserRoles(long loginId, CancellationToken cancellationToken)
    {
        var result = await _service.GetUserRolesAsync(loginId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<UserRoleAssignmentDto>>
        {
            Success = true,
            Message = "User roles loaded.",
            Data = result
        });
    }

    [HttpPost("users/{loginId:long}/roles")]
    [GFI_Upgrated.ServiceApi.Infrastructure.RequirePermission("Admin", "Assign Roles", "update")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveUserRoles(long loginId, [FromBody] SaveUserRolesRequest request, CancellationToken cancellationToken)
    {
        request.LoginId = loginId;
        var savedCount = await _service.SaveUserRolesAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = savedCount > 0,
            Message = "User roles saved.",
            Data = savedCount
        });
    }

    [HttpGet("staff/unassigned")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<StaffLookupDto>>>> GetUnassignedStaff(CancellationToken cancellationToken)
    {
        var result = await _service.GetUnassignedStaffAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<StaffLookupDto>>
        {
            Success = true,
            Message = "Staff loaded.",
            Data = result
        });
    }

    [HttpGet("staff/active")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<StaffLookupDto>>>> GetActiveStaff(CancellationToken cancellationToken)
    {
        var result = await _service.GetActiveStaffAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<StaffLookupDto>>
        {
            Success = true,
            Message = "Active staff loaded.",
            Data = result
        });
    }

    [HttpGet("lookups/master-dropdown/{parentId:long}")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<LookupItemDto>>>> GetMasterDropdown(long parentId, CancellationToken cancellationToken)
    {
        var result = await _service.GetMasterDropdownAsync(parentId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<LookupItemDto>>
        {
            Success = true,
            Message = "Lookup loaded.",
            Data = result
        });
    }

    [HttpGet("dropdown-masters")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<DropDownMasterDto>>>> GetDropDownMasters([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        var result = await _service.GetDropDownMastersAsync(request, searchText, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<DropDownMasterDto>>
        {
            Success = true,
            Message = "Dropdown masters loaded.",
            Data = result
        });
    }

    [HttpGet("dropdown-masters/{id:long}")]
    public async Task<ActionResult<ApiEnvelope<DropDownMasterDto?>>> GetDropDownMasterById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetDropDownMasterByIdAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<DropDownMasterDto?>
        {
            Success = result is not null,
            Message = result is null ? "Dropdown master not found." : "Dropdown master loaded.",
            Data = result
        });
    }

    [HttpPost("dropdown-masters")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveDropDownMaster([FromBody] SaveDropDownMasterRequest request, CancellationToken cancellationToken)
    {
        var id = await _service.SaveDropDownMasterAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "Dropdown master saved successfully." : "Dropdown master save failed.",
            Data = id
        });
    }

    [HttpGet("dropdown-masters/{masterId:long}/values")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<DropDownValueDto>>>> GetDropDownValues(long masterId, [FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        var result = await _service.GetDropDownValuesAsync(masterId, request, searchText, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<DropDownValueDto>>
        {
            Success = true,
            Message = "Dropdown values loaded.",
            Data = result
        });
    }

    [HttpGet("dropdown-values/{id:long}")]
    public async Task<ActionResult<ApiEnvelope<DropDownValueDto?>>> GetDropDownValueById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetDropDownValueByIdAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<DropDownValueDto?>
        {
            Success = result is not null,
            Message = result is null ? "Dropdown value not found." : "Dropdown value loaded.",
            Data = result
        });
    }

    [HttpPost("dropdown-values")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveDropDownValue([FromBody] SaveDropDownValueRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).Where(message => !string.IsNullOrWhiteSpace(message)));
            return Ok(new ApiEnvelope<int>
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(message) ? "Invalid dropdown value data." : message
            });
        }

        var id = await _service.SaveDropDownValueAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "Dropdown value saved successfully." : "Dropdown value save failed.",
            Data = id
        });
    }

    [HttpDelete("dropdown-values/{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteDropDownValue(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteDropDownValueAsync(id, updatedBy ?? "System", cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = result >= 0,
            Message = result >= 0 ? "Dropdown value deleted successfully." : "Dropdown value delete failed.",
            Data = result
        });
    }

    [HttpGet("refresh-session")]
    public async Task<ActionResult<ApiEnvelope<LoginResultDto>>> RefreshSession([FromQuery] long loginId, [FromQuery] long roleId, CancellationToken cancellationToken)
    {
        var result = await _service.GetLoginResultAsync(loginId, roleId, cancellationToken);
        if (result is null)
        {
            return Ok(new ApiEnvelope<LoginResultDto>
            {
                Success = false,
                Message = "Session refresh failed."
            });
        }

        if (result != null)
        {
            result.Token = GenerateJwtToken(result);
        }

        return Ok(new ApiEnvelope<LoginResultDto>
        {
            Success = true,
            Message = "Session refreshed successfully.",
            Data = result
        });
    }

    [HttpPost("discover-routes")]
    public async Task<ActionResult<ApiEnvelope<int>>> DiscoverRoutes(CancellationToken cancellationToken)
    {
        var actions = _actionDescriptorCollectionProvider.ActionDescriptors.Items;
        int count = 0;

        foreach (var action in actions)
        {
            if (action is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor ad)
            {
                // Only discover actual module controllers, skip system/security if needed
                if (ad.ControllerName == "Security" || ad.ControllerName == "Health") continue;

                var page = new MenuDto
                {
                    ModuleId = 1, // Default to a general module if not specified
                    PageHeading = ad.ActionName,
                    ActualName = $"{ad.ControllerName}_{ad.ActionName}",
                    PagePath = ad.AttributeRouteInfo?.Template ?? $"api/{ad.ControllerName}/{ad.ActionName}",
                    IsView = true,
                    ShowInMenu = false, // Discovered routes are usually API endpoints, not menu items
                    IsApp = true
                };

                await _service.UpsertPageAsync(page, cancellationToken);
                count++;
            }
        }

        return Ok(new ApiEnvelope<int>
        {
            Success = true,
            Message = $"{count} routes discovered and updated.",
            Data = count
        });
    }

    [HttpGet("user-activity-logs")]
    [GFI_Upgrated.ServiceApi.Infrastructure.RequirePermission("Admin", "_UserLogs", "view")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<UserActivityLogDto>>>> GetUserActivityLogs(
        [FromQuery] string? userName,
        [FromQuery] string? loginName,
        [FromQuery] string? eventName,
        [FromQuery] string? eventModule,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortCol = "DT",
        [FromQuery] string sortOrd = "DESC",
        CancellationToken cancellationToken = default)
    {
        var request = new PagedRequest
        {
            CurrentPage = page,
            RecordPerPage = pageSize,
            SortColumn = sortCol,
            SortType = sortOrd
        };

        var result = await _service.GetUserActivityLogsAsync(userName, loginName, eventName, eventModule, request, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<UserActivityLogDto>>
        {
            Success = true,
            Message = "User activity logs loaded.",
            Data = result
        });
    }

    [HttpGet("login-logs")]
    [GFI_Upgrated.ServiceApi.Infrastructure.RequirePermission("Admin", "_MyLoginLogs", "view")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<LoginLogDto>>>> GetLoginLogs(
        [FromQuery] string? searchText,
        [FromQuery] long? loginId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortCol = "Z_UsersLoginsLog.LoginLogID",
        [FromQuery] string sortOrd = "DESC",
        CancellationToken cancellationToken = default)
    {
        var request = new PagedRequest
        {
            CurrentPage = page,
            RecordPerPage = pageSize,
            SortColumn = sortCol,
            SortType = sortOrd
        };

        var result = await _service.GetLoginLogsAsync(searchText, loginId, fromDate, toDate, request, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<LoginLogDto>>
        {
            Success = true,
            Message = "Login logs loaded.",
            Data = result
        });
    }

    [HttpPost("user-activity-logs")]
    public async Task<ActionResult<ApiEnvelope<long>>> InsertUserActivityLog([FromBody] UserActivityLogDto log, CancellationToken cancellationToken)
    {
        var id = await _service.InsertUserActivityLogAsync(log, cancellationToken);
        return Ok(new ApiEnvelope<long>
        {
            Success = id > 0,
            Message = id > 0 ? "User activity log recorded successfully." : "Failed to record log.",
            Data = id
        });
    }

    private string GenerateJwtToken(LoginResultDto user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var key = Encoding.ASCII.GetBytes(jwtKey);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.LoginId.ToString()),
            new Claim(ClaimTypes.Name, user.LoginName),
            new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            new Claim("IsAdmin", user.IsAdmin.ToString().ToLower())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = jwtSettings["Issuer"] ?? "GFI_Upgrated",
            Audience = jwtSettings["Audience"] ?? "GFI_Upgrated_UI",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
