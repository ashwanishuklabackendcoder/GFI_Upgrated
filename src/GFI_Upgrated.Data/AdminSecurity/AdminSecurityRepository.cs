using System.Data;
using System.Text.Json;
using System.Text;
using GFI_Upgrated.SharedDto.AdminSecurity;
using GFI_Upgrated.SharedDto.Common;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.AdminSecurity;

public interface IAdminSecurityRepository
{
    Task<LoginResultDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<RoleDto>> GetRolesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<RoleDto?> GetRoleByIdAsync(long roleId, CancellationToken cancellationToken = default);
    Task<int> SaveRoleAsync(SaveRoleRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<UserDto>> GetUsersAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(long loginId, CancellationToken cancellationToken = default);
    Task<int> SaveUserAsync(SaveUserRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<StaffDto>> GetStaffsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<StaffDto?> GetStaffByIdAsync(long staffId, CancellationToken cancellationToken = default);
    Task<int> SaveStaffAsync(SaveStaffRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteStaffAsync(long staffId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuDto>> GetMenusAsync(CancellationToken cancellationToken = default);
    Task<MenuDto?> GetMenuByIdAsync(long linkId, CancellationToken cancellationToken = default);
    Task<int> SaveMenuAsync(SaveMenuRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ModuleDto>> GetModulesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuDto>> GetParentMenusAsync(long moduleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuDto>> GetDashboardsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RolePermissionDto>> GetRolePermissionsAsync(long roleId, long moduleId, CancellationToken cancellationToken = default);
    Task<int> SaveRolePermissionsAsync(SaveRolePermissionsRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRoleAssignmentDto>> GetUserRolesAsync(long loginId, CancellationToken cancellationToken = default);
    Task<int> SaveUserRolesAsync(SaveUserRolesRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffLookupDto>> GetActiveStaffAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffLookupDto>> GetUnassignedStaffAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LookupItemDto>> GetMasterDropdownAsync(long parentId, CancellationToken cancellationToken = default);
    Task<PagedResult<DropDownMasterDto>> GetDropDownMastersAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<DropDownMasterDto?> GetDropDownMasterByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> SaveDropDownMasterAsync(SaveDropDownMasterRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<DropDownValueDto>> GetDropDownValuesAsync(long masterId, PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<DropDownValueDto?> GetDropDownValueByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> SaveDropDownValueAsync(SaveDropDownValueRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteDropDownValueAsync(long id, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LanguageDto>> GetLanguagesAsync(CancellationToken cancellationToken = default);
    Task<LanguageDto?> GetLanguageByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> SaveLanguageAsync(SaveLanguageRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<LocalizedResourceDto>> GetLocalizedResourcesAsync(LocalizedResourceListRequest request, CancellationToken cancellationToken = default);
    Task<int> SaveLocalizedResourceAsync(SaveLocalizedResourceRequest request, CancellationToken cancellationToken = default);
    Task<int> SyncLocalizationDefaultsAsync(CancellationToken cancellationToken = default);
    Task<UserLanguagePreferenceDto?> GetUserLanguagePreferenceAsync(long loginId, CancellationToken cancellationToken = default);
    Task<int> SaveUserLanguagePreferenceAsync(SaveUserLanguagePreferenceRequest request, CancellationToken cancellationToken = default);
    Task<LocalizedDictionaryDto> GetLocalizedDictionaryAsync(long languageId, CancellationToken cancellationToken = default);
    Task<LoginResultDto?> GetLoginResultAsync(long loginId, long roleId, CancellationToken cancellationToken = default);
    Task<int> UpsertPageAsync(MenuDto page, CancellationToken cancellationToken = default);
    Task<PagedResult<UserActivityLogDto>> GetUserActivityLogsAsync(string? userName, string? loginName, string? eventName, string? eventModule, PagedRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<LoginLogDto>> GetLoginLogsAsync(string? searchText, long? loginId, DateTime? fromDate, DateTime? toDate, PagedRequest request, CancellationToken cancellationToken = default);
    Task<long> InsertUserActivityLogAsync(UserActivityLogDto log, CancellationToken cancellationToken = default);
}

public sealed class AdminSecurityRepository : IAdminSecurityRepository
{
    private readonly string _connectionString;

    public AdminSecurityRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<LoginResultDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("Z_UsersLoginsCheck", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@LoginName", request.LoginName);
        command.Parameters.AddWithValue("@Password", LegacyCrypto.EncryptString(request.Password));
        command.Parameters.AddWithValue("@IPAddress", request.IpAddress ?? "127.0.0.1");
        command.Parameters.AddWithValue("@Browser", request.Browser ?? string.Empty);
        command.Parameters.AddWithValue("@OperatingSystem", request.OperatingSystem ?? string.Empty);
        command.Parameters.AddWithValue("@ComputerName", request.ComputerName ?? string.Empty);

        var statusParameter = command.Parameters.Add("@IsError", SqlDbType.SmallInt);
        statusParameter.Direction = ParameterDirection.Output;

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var table = new DataTable();
        table.Load(reader);

        if (table.Rows.Count == 0)
        {
            return null;
        }

        var row = table.Rows[0];
        var loginId = row.SafeLong("LoginID", "LoginId");
        var roleId = row.SafeLong("RoleID", "RoleId");

        var result = await GetLoginResultAsync(loginId, roleId, cancellationToken);
        if (result != null)
        {
            result.Status = Convert.ToInt32(statusParameter.Value ?? 0);
        }
        return result;
    }

    public async Task<LoginResultDto?> GetLoginResultAsync(long loginId, long roleId, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_UsersLoginsDetails", new[]
        {
            new SqlParameter("@LoginID", SqlDbType.BigInt) { Value = loginId }
        }, cancellationToken);

        if (table.Rows.Count == 0) return null;

        var row = table.Rows[0];
        
        // Secondary fetch for RoleName and IsAdmin status from Z_UsersRoles
        var isAdmin = false;
        var roleNameFromDb = string.Empty;
        var dashboardPath = string.Empty;
        if (roleId > 0)
        {
            var roleTable = await ExecuteDataTableRawAsync("SELECT RoleName, IsAdmin FROM Z_UsersRoles WHERE RoleID = @RoleID", new[] { new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = roleId } }, cancellationToken);
            if (roleTable.Rows.Count > 0)
            {
                roleNameFromDb = roleTable.Rows[0].SafeString("RoleName");
                isAdmin = roleTable.Rows[0].SafeBool("IsAdmin");
            }
        }

        IReadOnlyList<MenuDto> menus = Array.Empty<MenuDto>();
        if (loginId > 0 && roleId > 0)
        {
            var allMenus = await GetMenusAsync(cancellationToken);
            var permissions = await GetRolePermissionsAsync(roleId, 0, cancellationToken);

            foreach (var menu in allMenus)
            {
                var perm = permissions.FirstOrDefault(p => p.LinkID == menu.LinkId);
                if (perm != null)
                {
                    menu.IsView = perm.ViewPer;
                    menu.IsInsert = perm.InsertPer;
                    menu.IsUpdate = perm.UpdatePer;
                    menu.IsDelete = perm.DeletePer;
                }
                else
                {
                    menu.IsView = false;
                    menu.IsInsert = false;
                    menu.IsUpdate = false;
                    menu.IsDelete = false;
                }
            }

            var fullTree = BuildMenuTree(allMenus);
            
            // Only return root menus that are either authorized themselves OR have at least one authorized child
            menus = fullTree
                .Where(root => root.IsView || root.SubMenus.Any(child => child.IsView))
                .ToList();

            // Dynamically find a dashboard path from authorized menus
            var firstDashboard = allMenus.FirstOrDefault(m => m.IsDashboard && m.IsView);
            dashboardPath = firstDashboard?.PagePath ?? string.Empty;
        }

        UserLanguagePreferenceDto? userLanguage = null;
        try
        {
            userLanguage = loginId > 0
                ? await GetUserLanguagePreferenceAsync(loginId, cancellationToken)
                : await GetDefaultLanguagePreferenceAsync(cancellationToken);
        }
        catch
        {
            userLanguage = null;
        }

        return new LoginResultDto
        {
            LoginId = loginId,
            LoginName = row.SafeString("LoginName"),
            FirstName = row.SafeString("FirstName"),
            LastName = row.SafeString("LastName"),
            RoleId = roleId,
            RoleName = !string.IsNullOrWhiteSpace(roleNameFromDb) ? roleNameFromDb : row.SafeString("RoleName"),
            IsAdmin = isAdmin,
            DashboardPath = !string.IsNullOrWhiteSpace(dashboardPath) ? dashboardPath : "/admin",
            LanguageId = userLanguage?.LanguageId ?? 0,
            CultureName = userLanguage?.CultureName,
            Menus = menus
        };
    }

    private static List<MenuDto> BuildMenuTree(IEnumerable<MenuDto> flatList)
    {
        var list = flatList.ToList();
        var lookup = list.ToLookup(m => m.ParentId);
        var roots = list.Where(m => m.ParentId == 0 || !list.Any(p => p.LinkId == m.ParentId)).ToList();
        foreach (var item in list)
        {
            item.SubMenus = lookup[item.LinkId].OrderBy(m => m.SequenceNo).ToList();
        }
        return roots.OrderBy(m => m.SequenceNo).ToList();
    }

    public async Task<PagedResult<RoleDto>> GetRolesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@RoleID", SqlDbType.BigInt) { Value = 0L },
            new("@RoleName", SqlDbType.VarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn) && !string.IsNullOrWhiteSpace(request.SortType))
        {
            parameters.Add(new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType });
            parameters.Add(new SqlParameter("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn });
        }

        var table = await ExecuteDataTableAsync("Z_UsersRolesList", parameters, cancellationToken);
        return new PagedResult<RoleDto>
        {
            CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
            TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
            Items = table.AsEnumerable().Select(MapRole).ToList()
        };
    }

    public async Task<RoleDto?> GetRoleByIdAsync(long roleId, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_UsersRolesList", new[]
        {
            new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = roleId }
        }, cancellationToken);

        return table.Rows.Count == 0 ? null : MapRole(table.Rows[0]);
    }

    public async Task<int> SaveRoleAsync(SaveRoleRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = request.RoleId },
            new SqlParameter("@RoleName", SqlDbType.NVarChar, 400) { Value = request.RoleName.Trim() },
            new SqlParameter("@Description", SqlDbType.NVarChar, 1000) { Value = (object?)request.Description ?? DBNull.Value },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = request.IsActive },
            new SqlParameter("@IsAdmin", SqlDbType.Bit) { Value = request.IsAdmin },
            new SqlParameter("@ModuleID", SqlDbType.BigInt) { Value = request.ModuleId },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 400) { Value = request.CreatedBy },
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 400) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Z_UsersRolesModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@LoginID", SqlDbType.BigInt) { Value = 0L },
            new("@LoginName", SqlDbType.VarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn) && !string.IsNullOrWhiteSpace(request.SortType))
        {
            parameters.Add(new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType });
            parameters.Add(new SqlParameter("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn });
        }

        var table = await ExecuteDataTableAsync("Z_UsersLoginsList", parameters, cancellationToken);
        return new PagedResult<UserDto>
        {
            CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
            TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
            Items = table.AsEnumerable().Select(MapUser).ToList()
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(long loginId, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_UsersLoginsDetails_select", new[]
        {
            new SqlParameter("@LoginID", SqlDbType.BigInt) { Value = loginId }
        }, cancellationToken);

        return table.Rows.Count == 0 ? null : MapUser(table.Rows[0]);
    }

    public async Task<int> SaveUserAsync(SaveUserRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@LoginID", SqlDbType.BigInt) { Value = request.LoginId },
            new SqlParameter("@StaffID", SqlDbType.BigInt) { Value = request.StaffId },
            new SqlParameter("@LoginName", SqlDbType.NVarChar, 200) { Value = request.LoginName.Trim() },
            new SqlParameter("@Password", SqlDbType.VarChar, 3000) { Value = LegacyCrypto.EncryptString(request.Password) },
            new SqlParameter("@ForgotEmail", SqlDbType.NVarChar, 200) { Value = (object?)request.ForgotEmail ?? DBNull.Value },
            new SqlParameter("@IPAddress", SqlDbType.VarChar, 50) { Value = request.IpAddress ?? "127.0.0.1" },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = request.IsActive },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.CreatedBy },
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Z_UsersLoginsModify", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);

        // SYNC: If RoleId is provided, ensure it's saved in the UserRoles mapping table as the default role.
        if (result > 0 && request.RoleId > 0)
        {
            await SaveUserRolesAsync(new SaveUserRolesRequest
            {
                LoginId = request.LoginId > 0 ? request.LoginId : result,
                Roles = new List<UserRoleAssignmentDto>
                {
                    new UserRoleAssignmentDto { RoleId = request.RoleId, IsDefault = true }
                }
            }, cancellationToken);
        }

        return result;
    }

    public async Task<PagedResult<StaffDto>> GetStaffsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@StaffId", SqlDbType.BigInt) { Value = 0L },
            new("@StaffFirstName", SqlDbType.VarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn) && !string.IsNullOrWhiteSpace(request.SortType))
        {
            parameters.Add(new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType });
            parameters.Add(new SqlParameter("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn });
        }

        var table = await ExecuteDataTableAsync("A_HrStaffMasterList", parameters, cancellationToken);
        return new PagedResult<StaffDto>
        {
            CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
            TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
            Items = table.AsEnumerable().Select(MapStaff).ToList()
        };
    }

    public async Task<StaffDto?> GetStaffByIdAsync(long staffId, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("A_HrStaffMasterList", new[]
        {
            new SqlParameter("@StaffId", SqlDbType.BigInt) { Value = staffId }
        }, cancellationToken);

        return table.Rows.Count == 0 ? null : MapStaff(table.Rows[0]);
    }

    public async Task<int> SaveStaffAsync(SaveStaffRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@StaffId", SqlDbType.BigInt) { Value = request.StaffId },
            new SqlParameter("@Status", SqlDbType.BigInt) { Value = request.Status },
            new SqlParameter("@EmailIDOfficial", SqlDbType.NVarChar, 200) { Value = request.EmailIDOfficial.Trim() },
            new SqlParameter("@DOB", SqlDbType.DateTime) { Value = request.DOB.HasValue ? request.DOB.Value : DBNull.Value },
            new SqlParameter("@Gender", SqlDbType.BigInt) { Value = request.Gender },
            new SqlParameter("@StaffNumber", SqlDbType.NVarChar, 200) { Value = (object?)request.StaffNumber?.Trim() ?? DBNull.Value },
            new SqlParameter("@EmailIDPersonal", SqlDbType.NVarChar, 200) { Value = (object?)request.EmailIDPersonal?.Trim() ?? DBNull.Value },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.CreatedBy },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@StaffFirstName", SqlDbType.NVarChar, 200) { Value = request.StaffFirstName.Trim() },
            new SqlParameter("@StaffLastName", SqlDbType.NVarChar, 200) { Value = request.StaffLastName.Trim() },
            new SqlParameter("@HasLogin", SqlDbType.Bit) { Value = request.HasLogin },
            new SqlParameter("@Photo", SqlDbType.NVarChar, 200) { Value = (object?)request.Photo?.Trim() ?? DBNull.Value },
            new SqlParameter("@StaffSalutation", SqlDbType.NVarChar, 100) { Value = (object?)request.StaffSalutation?.Trim() ?? DBNull.Value },
            new SqlParameter("@MobileNo", SqlDbType.NVarChar, 200) { Value = (object?)request.MobileNo?.Trim() ?? DBNull.Value },
            new SqlParameter("@NIN", SqlDbType.NVarChar, 100) { Value = (object?)request.NIN?.Trim() ?? DBNull.Value },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("HRStaffPersonalDetailModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteStaffAsync(long staffId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 500) { Value = staffId.ToString() },
            new SqlParameter("@OprType", SqlDbType.Int) { Value = 1 },
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 200) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("A_HrStaffMasterOperation", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<IReadOnlyList<MenuDto>> GetMenusAsync(CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_UsersMenuList", new[]
        {
            new SqlParameter("@LinkID", SqlDbType.BigInt) { Value = 0L },
            new SqlParameter("@PageHeading", SqlDbType.VarChar, 200) { Value = string.Empty },
            new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = 1, Direction = ParameterDirection.InputOutput },
            new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = 5000 },
            new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = "ASC" },
            new SqlParameter("@SortColumn", SqlDbType.VarChar, 200) { Value = "PageHeading" },
            new SqlParameter("@ModuleID", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@MenuName", SqlDbType.NVarChar, 500) { Value = string.Empty },
            new SqlParameter("@PagePath", SqlDbType.NVarChar, 500) { Value = string.Empty },
            new SqlParameter("@ActualName", SqlDbType.NVarChar, 500) { Value = string.Empty },
            new SqlParameter("@LevelNo", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@SequenceNo", SqlDbType.Int) { Value = 0 }
        }, cancellationToken);

        return table.AsEnumerable().Select(MapMenu).ToList();
    }

    public async Task<MenuDto?> GetMenuByIdAsync(long linkId, CancellationToken cancellationToken = default)
    {
        var menus = await GetMenusAsync(cancellationToken);
        return menus.FirstOrDefault(x => x.LinkId == linkId);
    }

    public async Task<int> SaveMenuAsync(SaveMenuRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@LinkID", SqlDbType.BigInt) { Value = request.LinkId },
            new SqlParameter("@ModuleID", SqlDbType.BigInt) { Value = request.ModuleId },
            new SqlParameter("@PageHeading", SqlDbType.NVarChar, 200) { Value = request.PageHeading.Trim() },
            new SqlParameter("@ParentID", SqlDbType.BigInt) { Value = request.ParentId },
            new SqlParameter("@PagePath", SqlDbType.NVarChar, 200) { Value = (object?)request.PagePath ?? DBNull.Value },
            new SqlParameter("@ActualName", SqlDbType.NVarChar, 200) { Value = (object?)request.ActualName ?? DBNull.Value },
            new SqlParameter("@IsView", SqlDbType.Bit) { Value = request.IsView },
            new SqlParameter("@LevelNo", SqlDbType.Int) { Value = request.LevelNo },
            new SqlParameter("@SequenceNo", SqlDbType.Int) { Value = request.SequenceNo },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 400) { Value = request.CreatedBy },
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 400) { Value = request.UpdatedBy },
            new SqlParameter("@IsDashboard", SqlDbType.Bit) { Value = request.IsDashboard },
            new SqlParameter("@ShowInMenu", SqlDbType.Bit) { Value = request.ShowInMenu },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Z_UsersMenuModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[14].Value ?? 0);
    }

    public async Task<IReadOnlyList<ModuleDto>> GetModulesAsync(CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_UserModule_Select", Array.Empty<SqlParameter>(), cancellationToken);
        return table.AsEnumerable().Select(row => new ModuleDto
        {
            ModuleId = row.SafeLong("ModuleID"),
            ModuleName = row.SafeString("ModuleName", "Module")
        }).ToList();
    }

    public async Task<IReadOnlyList<MenuDto>> GetParentMenusAsync(long moduleId, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_UsersMenuParent_ModuleID", new[]
        {
            new SqlParameter("@ModuleID", SqlDbType.Int) { Value = moduleId }
        }, cancellationToken);

        return table.AsEnumerable().Select(MapMenu).ToList();
    }

    public async Task<IReadOnlyList<MenuDto>> GetDashboardsAsync(CancellationToken cancellationToken = default)
    {
        var menus = await GetMenusAsync(cancellationToken);
        return menus.Where(x => x.IsDashboard).ToList();
    }

    public async Task<IReadOnlyList<RolePermissionDto>> GetRolePermissionsAsync(long roleId, long moduleId, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Z_UsersRoleForm WHERE RoleID = @RoleID";
        var parameters = new[] { new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = roleId } };
        
        var permTable = await ExecuteDataTableRawAsync(sql, parameters, cancellationToken);
        var allMenus = await GetMenusAsync(cancellationToken);

        var list = permTable.AsEnumerable().Select(row => 
        {
            var linkId = row.SafeLong("LinkID");
            var menu = allMenus.FirstOrDefault(m => m.LinkId == linkId);
            
            return new RolePermissionDto
            {
                RoleFormID = row.SafeLong("RoleFormID"),
                RoleID = row.SafeLong("RoleID"),
                LinkID = linkId,
                ViewPer = row.SafeBool("ViewPer"),
                InsertPer = row.SafeBool("InsertPer"),
                UpdatePer = row.SafeBool("UpdatePer"),
                DeletePer = row.SafeBool("DeletePer"),
                PageHeading = menu?.PageHeading,
                DisplayName = menu?.DisplayName,
                IconClass = menu?.IconClass,
                PagePath = menu?.PagePath,
                ParentID = menu?.ParentId ?? 0,
                SequenceNo = menu?.SequenceNo ?? 0,
                ModuleID = menu?.ModuleId ?? 0
            };
        }).ToList();

        if (moduleId > 0)
        {
            return list.Where(x => x.ModuleID == moduleId).ToList();
        }
        return list;
    }

    public async Task<int> SaveRolePermissionsAsync(SaveRolePermissionsRequest request, CancellationToken cancellationToken = default)
    {
        int totalAffected = 0;
        foreach (var p in request.Permissions)
        {
            var sql = @"
                IF EXISTS (SELECT 1 FROM Z_UsersRoleForm WHERE RoleID = @RoleID AND LinkID = @LinkID)
                BEGIN
                    UPDATE Z_UsersRoleForm 
                    SET ViewPer = @ViewPer, 
                        InsertPer = @InsertPer, 
                        UpdatePer = @UpdatePer, 
                        DeletePer = @DeletePer
                    WHERE RoleID = @RoleID AND LinkID = @LinkID;
                END
                ELSE
                BEGIN
                    INSERT INTO Z_UsersRoleForm (RoleID, LinkID, ViewPer, InsertPer, UpdatePer, DeletePer)
                    VALUES (@RoleID, @LinkID, @ViewPer, @InsertPer, @UpdatePer, @DeletePer);
                END";

            var parameters = new[]
            {
                new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = request.RoleID },
                new SqlParameter("@LinkID", SqlDbType.BigInt) { Value = p.LinkID },
                new SqlParameter("@ViewPer", SqlDbType.Bit) { Value = p.ViewPer },
                new SqlParameter("@InsertPer", SqlDbType.Bit) { Value = p.InsertPer },
                new SqlParameter("@UpdatePer", SqlDbType.Bit) { Value = p.UpdatePer },
                new SqlParameter("@DeletePer", SqlDbType.Bit) { Value = p.DeletePer }
            };

            var affected = await ExecuteNonQueryRawAsync(sql, parameters, cancellationToken);
            totalAffected += affected;
        }

        return totalAffected;
    }

    public async Task<IReadOnlyList<UserRoleAssignmentDto>> GetUserRolesAsync(long loginId, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_UsersRolesMoreListV1", new[]
        {
            new SqlParameter("@LoginID", SqlDbType.BigInt) { Value = loginId },
            new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = 1, Direction = ParameterDirection.InputOutput },
            new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = 5000 },
            new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = "ASC" },
            new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = "RoleName" }
        }, cancellationToken);

        return table.AsEnumerable().Select(row => new UserRoleAssignmentDto
        {
            UserRoleId = row.SafeLong("UserRoleID"),
            RoleId = row.SafeLong("RoleID"),
            LoginId = row.SafeLong("LoginID"),
            IsDefault = row.SafeBool("IsDefault"),
            RoleName = row.SafeString("RoleName"),
            LoginName = row.SafeString("LoginName")
        }).ToList();
    }

    public async Task<int> SaveUserRolesAsync(SaveUserRolesRequest request, CancellationToken cancellationToken = default)
    {
        var xml = new System.Text.StringBuilder("<Roles>");
        foreach (var role in request.Roles)
        {
            xml.Append("<Role>");
            xml.Append("<RoleID>").Append(role.RoleId).Append("</RoleID>");
            xml.Append("<IsDefault>").Append(role.IsDefault ? "1" : "0").Append("</IsDefault>");
            xml.Append("</Role>");
        }
        xml.Append("</Roles>");

        var parameters = new[]
        {
            new SqlParameter("@LoginID", SqlDbType.BigInt) { Value = request.LoginId },
            new SqlParameter("@RolesXML", SqlDbType.Xml) { Value = xml.ToString() },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.Roles.FirstOrDefault()?.LoginName ?? "System" },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Z_UsersRolesMoreBulkSave", parameters, cancellationToken);
        var savedCount = Convert.ToInt32(parameters[^1].Value ?? 0);

        return savedCount;
    }

    public async Task<IReadOnlyList<StaffLookupDto>> GetActiveStaffAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetStaffsAsync(new PagedRequest
        {
            CurrentPage = 1,
            RecordPerPage = 5000,
            SortColumn = "StaffFirstName",
            SortType = "ASC"
        }, null, cancellationToken);

        return result.Items
            .Where(row => row.Status == 1)
            .Select(row => new StaffLookupDto
            {
                StaffId = row.StaffId,
                StaffName = row.StaffName,
                Email = row.EmailIDOfficial,
                Status = row.Status,
                IsActive = row.Status == 1
            })
            .ToList();
    }

    public async Task<IReadOnlyList<StaffLookupDto>> GetUnassignedStaffAsync(CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("GetStaffLoginsNotExists", Array.Empty<SqlParameter>(), cancellationToken);
        return table.AsEnumerable().Select(row => new StaffLookupDto
        {
            StaffId = row.SafeLong("StaffID"),
            StaffName = row.SafeString("StaffName", "Name"),
            Email = row.SafeString("Email", "ForgotEmail"),
            Status = row.SafeInt("Status", "StatusID"),
            IsActive = row.SafeBool("IsActive")
        }).ToList();
    }

    public async Task<IReadOnlyList<LookupItemDto>> GetMasterDropdownAsync(long parentId, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_MasterDropDownByParentID", new[]
        {
            new SqlParameter("@ParentID", SqlDbType.BigInt) { Value = parentId }
        }, cancellationToken);

        return table.AsEnumerable().Select(row => new LookupItemDto
        {
            Value = row.SafeLong("UniqueID", "Value", "ID"),
            Text = row.SafeString("DisplayText", "Text", "Name")
        }).ToList();
    }

    public async Task<PagedResult<DropDownMasterDto>> GetDropDownMastersAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@UniqueID", SqlDbType.BigInt) { Value = 0L },
            new("@DisplayText", SqlDbType.NVarChar, 1000) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn) && !string.IsNullOrWhiteSpace(request.SortType))
        {
            parameters.Add(new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType });
            parameters.Add(new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = request.SortColumn });
        }

        var table = await ExecuteDataTableAsync("Z_MasterDropDownList", parameters, cancellationToken);
        var items = table.AsEnumerable().Select(row => new DropDownMasterDto
        {
            Id = row.SafeLong("UniqueID", "Id"),
            DisplayText = row.SafeString("DisplayText"),
            IsActive = row.SafeBool("Status", "IsActive"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            ValuesCount = row.SafeInt("ParentIDCount", "ValuesCount")
        }).ToList();

        return new PagedResult<DropDownMasterDto>
        {
            CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
            TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
            Items = items
        };
    }

    public async Task<DropDownMasterDto?> GetDropDownMasterByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_MasterDropDownList", new[]
        {
            new SqlParameter("@UniqueID", SqlDbType.BigInt) { Value = id },
            new SqlParameter("@DisplayText", SqlDbType.NVarChar, 1000) { Value = string.Empty },
            new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = 1, Direction = ParameterDirection.InputOutput },
            new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = 1 },
            new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = "ASC" },
            new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = "UniqueID" }
        }, cancellationToken);

        return table.Rows.Count == 0 ? null : new DropDownMasterDto
        {
            Id = table.Rows[0].SafeLong("UniqueID", "Id"),
            DisplayText = table.Rows[0].SafeString("DisplayText"),
            IsActive = table.Rows[0].SafeBool("Status", "IsActive"),
            CreatedDate = table.Rows[0].SafeDateTime("CreatedDate"),
            ValuesCount = table.Rows[0].SafeInt("ParentIDCount", "ValuesCount")
        };
    }

    public async Task<int> SaveDropDownMasterAsync(SaveDropDownMasterRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@UniqueID", SqlDbType.BigInt) { Value = request.Id },
            new SqlParameter("@ParentID", SqlDbType.BigInt) { Value = request.ParentId },
            new SqlParameter("@DisplayText", SqlDbType.NVarChar, 200) { Value = request.DisplayText.Trim() },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.CreatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Z_MasterDropDownModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<PagedResult<DropDownValueDto>> GetDropDownValuesAsync(long masterId, PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@UniqueID", SqlDbType.BigInt) { Value = 0L },
            new("@DisplayText", SqlDbType.NVarChar, 1000) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new("@SortOrd", SqlDbType.VarChar, 20) { Value = string.IsNullOrWhiteSpace(request.SortType) ? "ASC" : request.SortType.ToUpperInvariant() },
            new("@SortColumn", SqlDbType.VarChar, 50) { Value = string.IsNullOrWhiteSpace(request.SortColumn) ? "UniqueID" : request.SortColumn },
            new("@ParentID", SqlDbType.BigInt) { Value = masterId }
        };

        var table = await ExecuteDataTableAsync("Z_DropDownList", parameters, cancellationToken);
        var items = table.AsEnumerable().Select(row => new DropDownValueDto
        {
            Id = row.SafeLong("UniqueID", "Id"),
            DropDownMasterId = row.SafeLong("ParentID", "DropDownMasterId"),
            DisplayText = row.SafeString("DisplayText"),
            IsActive = row.SafeBool("Status", "IsActive", "IsEditable", "IsShow"),
            CreatedDate = row.SafeDateTime("CreatedDate")
        }).ToList();

        return new PagedResult<DropDownValueDto>
        {
            CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
            TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
            Items = items
        };
    }

    public async Task<DropDownValueDto?> GetDropDownValueByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("Z_DropDownList", new[]
        {
            new SqlParameter("@UniqueID", SqlDbType.BigInt) { Value = id },
            new SqlParameter("@DisplayText", SqlDbType.NVarChar, 1000) { Value = string.Empty },
            new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = 1, Direction = ParameterDirection.InputOutput },
            new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = 1 },
            new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = "ASC" },
            new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = "UniqueID" },
            new SqlParameter("@ParentID", SqlDbType.BigInt) { Value = 0L }
        }, cancellationToken);

        return table.Rows.Count == 0 ? null : new DropDownValueDto
        {
            Id = table.Rows[0].SafeLong("UniqueID", "Id"),
            DropDownMasterId = table.Rows[0].SafeLong("ParentID", "DropDownMasterId"),
            DisplayText = table.Rows[0].SafeString("DisplayText"),
            IsActive = table.Rows[0].SafeBool("Status", "IsActive", "IsEditable", "IsShow"),
            CreatedDate = table.Rows[0].SafeDateTime("CreatedDate")
        };
    }

    public async Task<int> SaveDropDownValueAsync(SaveDropDownValueRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@UniqueID", SqlDbType.BigInt) { Value = request.Id },
            new SqlParameter("@ParentID", SqlDbType.BigInt) { Value = request.DropDownMasterId },
            new SqlParameter("@DisplayText", SqlDbType.NVarChar, 200) { Value = request.DisplayText.Trim() },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.CreatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Z_MasterDropDownModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteDropDownValueAsync(long id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 500) { Value = id.ToString() },
            new SqlParameter("@OprType", SqlDbType.Int) { Value = 1 },
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 200) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Z_DropDownOperation", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<IReadOnlyList<LanguageDto>> GetLanguagesAsync(CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var sql = @"
            SELECT Id, CultureName, DisplayName, Country, Region, IsDefaultLanguage
            FROM Localization.[Language]
            ORDER BY IsDefaultLanguage DESC, DisplayName ASC, Id DESC;";

        var table = await ExecuteDataTableRawAsync(sql, Array.Empty<SqlParameter>(), cancellationToken);
        return table.AsEnumerable().Select(MapLanguage).ToList();
    }

    public async Task<LanguageDto?> GetLanguageByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var sql = @"
            SELECT Id, CultureName, DisplayName, Country, Region, IsDefaultLanguage
            FROM Localization.[Language]
            WHERE Id = @Id;";

        var table = await ExecuteDataTableRawAsync(sql, new[]
        {
            new SqlParameter("@Id", SqlDbType.BigInt) { Value = id }
        }, cancellationToken);

        return table.Rows.Count == 0 ? null : MapLanguage(table.Rows[0]);
    }

    public async Task<int> SaveLanguageAsync(SaveLanguageRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var sql = @"
            DECLARE @SavedId BIGINT = @Id;

            IF EXISTS (SELECT 1 FROM Localization.[Language] WHERE Id = @Id AND @Id > 0)
            BEGIN
                UPDATE Localization.[Language]
                SET CultureName = @CultureName,
                    DisplayName = @DisplayName,
                    Country = @Country,
                    Region = @Region,
                    IsDefaultLanguage = @IsDefaultLanguage
                WHERE Id = @Id;
            END
            ELSE
            BEGIN
                INSERT INTO Localization.[Language] (CultureName, DisplayName, Country, Region, IsDefaultLanguage)
                VALUES (@CultureName, @DisplayName, @Country, @Region, @IsDefaultLanguage);

                SET @SavedId = SCOPE_IDENTITY();
            END

            IF @IsDefaultLanguage = 1
            BEGIN
                UPDATE Localization.[Language]
                SET IsDefaultLanguage = CASE WHEN Id = @SavedId THEN 1 ELSE 0 END;
            END
            ELSE IF NOT EXISTS (SELECT 1 FROM Localization.[Language] WHERE IsDefaultLanguage = 1 AND Id <> @SavedId)
            BEGIN
                UPDATE Localization.[Language]
                SET IsDefaultLanguage = CASE WHEN Id = @SavedId THEN 1 ELSE 0 END;
            END

            SELECT CAST(@SavedId AS INT) AS SavedId;";

        var table = await ExecuteDataTableRawAsync(sql, new[]
        {
            new SqlParameter("@Id", SqlDbType.BigInt) { Value = request.Id },
            new SqlParameter("@CultureName", SqlDbType.NVarChar, 255) { Value = request.CultureName.Trim() },
            new SqlParameter("@DisplayName", SqlDbType.NVarChar, 255) { Value = request.DisplayName.Trim() },
            new SqlParameter("@Country", SqlDbType.NVarChar, 255) { Value = request.Country.Trim() },
            new SqlParameter("@Region", SqlDbType.NVarChar, 255) { Value = request.Region.Trim() },
            new SqlParameter("@IsDefaultLanguage", SqlDbType.Bit) { Value = request.IsDefaultLanguage }
        }, cancellationToken);

        return table.Rows.Count == 0 ? 0 : Convert.ToInt32(table.Rows[0]["SavedId"]);
    }

    public async Task<PagedResult<LocalizedResourceDto>> GetLocalizedResourcesAsync(LocalizedResourceListRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var languageId = request.LanguageId > 0 ? request.LanguageId : await GetDefaultLanguageIdAsync(cancellationToken);
        var defaultLanguageId = await GetDefaultLanguageIdAsync(cancellationToken);
        var normalizedSearch = request.SearchText?.Trim() ?? string.Empty;

        var sortColumn = request.SortColumn?.Trim().ToLowerInvariant() switch
        {
            "key" => "KeyName",
            "defaultvalue" => "DefaultValue",
            "value" => "ResourceValue",
            "updatedon" => "UpdatedOn",
            _ => "KeyName"
        };

        var sortDirection = string.Equals(request.SortType, "DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        var offset = Math.Max(0, (request.CurrentPage - 1) * request.RecordPerPage);

        var cteSql = @"
            ;WITH DefaultResources AS
            (
                SELECT
                    r.[Key] AS KeyName,
                    r.Value AS DefaultValue,
                    r.Comment AS DefaultComment
                FROM Localization.[Resource] r
                WHERE r.LanguageId = @DefaultLanguageId
            ),
            SelectedResources AS
            (
                SELECT
                    r.Id,
                    r.LanguageId,
                    r.[Key] AS KeyName,
                    r.Value,
                    r.Comment,
                    r.UpdatedOn
                FROM Localization.[Resource] r
                WHERE r.LanguageId = @LanguageId
            )";

        var fromSql = @"
            FROM DefaultResources dr
            INNER JOIN Localization.[Language] lang ON lang.Id = @LanguageId
            LEFT JOIN SelectedResources sr ON sr.KeyName = dr.KeyName";

        var whereSql = @"
            WHERE
                (@SearchText = '' OR
                 dr.KeyName LIKE '%' + @SearchText + '%' OR
                 dr.DefaultValue LIKE '%' + @SearchText + '%' OR
                 ISNULL(sr.Value, '') LIKE '%' + @SearchText + '%' OR
                 ISNULL(sr.Comment, '') LIKE '%' + @SearchText + '%')
                AND (@ShowMissingOnly = 0 OR (@LanguageId <> @DefaultLanguageId AND (sr.Id IS NULL OR ISNULL(sr.Value, '') = '')))";

        var countSql = cteSql + @"
            SELECT COUNT(1) AS TotalRecord
            " + fromSql + whereSql + ";";

        var dataSql = cteSql + @"
            SELECT
                ISNULL(sr.Id, 0) AS ResourceId,
                @LanguageId AS LanguageId,
                lang.CultureName,
                dr.KeyName,
                dr.DefaultValue,
                CASE WHEN @LanguageId = @DefaultLanguageId THEN dr.DefaultValue ELSE ISNULL(sr.Value, '') END AS ResourceValue,
                CASE WHEN @LanguageId = @DefaultLanguageId THEN dr.DefaultComment ELSE sr.Comment END AS ResourceComment,
                sr.UpdatedOn,
                CASE WHEN @LanguageId = @DefaultLanguageId THEN 0 WHEN sr.Id IS NULL OR ISNULL(sr.Value, '') = '' THEN 1 ELSE 0 END AS IsMissing
            " + fromSql + whereSql + @"
            ORDER BY " + sortColumn + " " + sortDirection + @"
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var parameters = new[]
        {
            new SqlParameter("@LanguageId", SqlDbType.BigInt) { Value = languageId },
            new SqlParameter("@DefaultLanguageId", SqlDbType.BigInt) { Value = defaultLanguageId },
            new SqlParameter("@SearchText", SqlDbType.NVarChar, 1000) { Value = normalizedSearch },
            new SqlParameter("@ShowMissingOnly", SqlDbType.Bit) { Value = request.ShowMissingOnly },
            new SqlParameter("@Offset", SqlDbType.Int) { Value = offset },
            new SqlParameter("@PageSize", SqlDbType.Int) { Value = request.RecordPerPage }
        };

        var totalTable = await ExecuteDataTableRawAsync(countSql, parameters, cancellationToken);
        var dataTable = await ExecuteDataTableRawAsync(dataSql, parameters, cancellationToken);

        return new PagedResult<LocalizedResourceDto>
        {
            CurrentPage = request.CurrentPage,
            TotalRecord = totalTable.Rows.Count == 0 ? 0 : totalTable.Rows[0].SafeInt("TotalRecord"),
            Items = dataTable.AsEnumerable().Select(MapLocalizedResource).ToList()
        };
    }

    public async Task<int> SaveLocalizedResourceAsync(SaveLocalizedResourceRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var languageId = request.LanguageId > 0 ? request.LanguageId : await GetDefaultLanguageIdAsync(cancellationToken);
        var sql = @"
            DECLARE @SavedId BIGINT;

            IF EXISTS (SELECT 1 FROM Localization.[Resource] WHERE LanguageId = @LanguageId AND [Key] = @KeyName)
            BEGIN
                UPDATE Localization.[Resource]
                SET Value = @Value,
                    Comment = @Comment,
                    UpdatedOn = GETUTCDATE()
                WHERE LanguageId = @LanguageId AND [Key] = @KeyName;

                SELECT @SavedId = Id
                FROM Localization.[Resource]
                WHERE LanguageId = @LanguageId AND [Key] = @KeyName;
            END
            ELSE
            BEGIN
                INSERT INTO Localization.[Resource] (LanguageId, [Key], Value, Comment, UpdatedOn)
                VALUES (@LanguageId, @KeyName, @Value, @Comment, GETUTCDATE());

                SET @SavedId = SCOPE_IDENTITY();
            END

            SELECT CAST(@SavedId AS INT) AS SavedId;";

        var table = await ExecuteDataTableRawAsync(sql, new[]
        {
            new SqlParameter("@LanguageId", SqlDbType.BigInt) { Value = languageId },
            new SqlParameter("@KeyName", SqlDbType.NVarChar, 450) { Value = request.Key.Trim() },
            new SqlParameter("@Value", SqlDbType.NVarChar, -1) { Value = request.Value ?? string.Empty },
            new SqlParameter("@Comment", SqlDbType.NVarChar, -1) { Value = (object?)request.Comment ?? DBNull.Value }
        }, cancellationToken);

        return table.Rows.Count == 0 ? 0 : Convert.ToInt32(table.Rows[0]["SavedId"]);
    }

    public async Task<int> SyncLocalizationDefaultsAsync(CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var defaultLanguageId = await GetDefaultLanguageIdAsync(cancellationToken);
        var synced = 0;

        foreach (var item in LocalizationDefaults.SeedItems)
        {
            var sql = @"
                IF NOT EXISTS (SELECT 1 FROM Localization.[Resource] WHERE LanguageId = @LanguageId AND [Key] = @KeyName)
                BEGIN
                    INSERT INTO Localization.[Resource] (LanguageId, [Key], Value, Comment, UpdatedOn)
                    VALUES (@LanguageId, @KeyName, @Value, @Comment, GETUTCDATE());
                    SELECT 1 AS Synced;
                END
                ELSE
                BEGIN
                    SELECT 0 AS Synced;
                END";

            var table = await ExecuteDataTableRawAsync(sql, new[]
            {
                new SqlParameter("@LanguageId", SqlDbType.BigInt) { Value = defaultLanguageId },
                new SqlParameter("@KeyName", SqlDbType.NVarChar, 450) { Value = item.Key },
                new SqlParameter("@Value", SqlDbType.NVarChar, -1) { Value = item.Value },
                new SqlParameter("@Comment", SqlDbType.NVarChar, -1) { Value = (object?)item.Comment ?? DBNull.Value }
            }, cancellationToken);

            if (table.Rows.Count > 0 && table.Rows[0].SafeInt("Synced") == 1)
            {
                synced++;
            }
        }

        return synced;
    }

    public async Task<UserLanguagePreferenceDto?> GetUserLanguagePreferenceAsync(long loginId, CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var sql = @"
            SELECT TOP 1
                ul.LoginID,
                l.Id AS LanguageId,
                l.CultureName,
                l.DisplayName
            FROM Localization.UserLanguage ul
            INNER JOIN Localization.[Language] l ON l.Id = ul.LanguageId
            WHERE ul.LoginID = @LoginID;";

        var table = await ExecuteDataTableRawAsync(sql, new[]
        {
            new SqlParameter("@LoginID", SqlDbType.BigInt) { Value = loginId }
        }, cancellationToken);

        if (table.Rows.Count > 0)
        {
            return MapUserLanguagePreference(table.Rows[0]);
        }

        return await GetDefaultLanguagePreferenceAsync(cancellationToken);
    }

    public async Task<int> SaveUserLanguagePreferenceAsync(SaveUserLanguagePreferenceRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var sql = @"
            IF EXISTS (SELECT 1 FROM Localization.UserLanguage WHERE LoginID = @LoginID)
            BEGIN
                UPDATE Localization.UserLanguage
                SET LanguageId = @LanguageId,
                    UpdatedOn = GETUTCDATE()
                WHERE LoginID = @LoginID;
            END
            ELSE
            BEGIN
                INSERT INTO Localization.UserLanguage (LoginID, LanguageId, UpdatedOn)
                VALUES (@LoginID, @LanguageId, GETUTCDATE());
            END;

            SELECT 1 AS Saved;";

        var table = await ExecuteDataTableRawAsync(sql, new[]
        {
            new SqlParameter("@LoginID", SqlDbType.BigInt) { Value = request.LoginId },
            new SqlParameter("@LanguageId", SqlDbType.BigInt) { Value = request.LanguageId }
        }, cancellationToken);

        return table.Rows.Count == 0 ? 0 : table.Rows[0].SafeInt("Saved");
    }

    public async Task<LocalizedDictionaryDto> GetLocalizedDictionaryAsync(long languageId, CancellationToken cancellationToken = default)
    {
        await EnsureLocalizationSchemaAsync(cancellationToken);

        var selectedLanguageId = languageId > 0 ? languageId : await GetDefaultLanguageIdAsync(cancellationToken);
        var defaultLanguageId = await GetDefaultLanguageIdAsync(cancellationToken);

        var language = await GetLanguageByIdAsync(selectedLanguageId, cancellationToken)
            ?? await GetLanguageByIdAsync(defaultLanguageId, cancellationToken)
            ?? new LanguageDto { Id = defaultLanguageId, CultureName = "en-IN", DisplayName = "English" };

        var sql = @"
            SELECT
                d.[Key] AS KeyName,
                d.Value AS DefaultValue,
                s.Value AS SelectedValue
            FROM Localization.[Resource] d
            LEFT JOIN Localization.[Resource] s
                ON s.LanguageId = @LanguageId AND s.[Key] = d.[Key]
            WHERE d.LanguageId = @DefaultLanguageId;";

        var table = await ExecuteDataTableRawAsync(sql, new[]
        {
            new SqlParameter("@LanguageId", SqlDbType.BigInt) { Value = selectedLanguageId },
            new SqlParameter("@DefaultLanguageId", SqlDbType.BigInt) { Value = defaultLanguageId }
        }, cancellationToken);

        var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (DataRow row in table.Rows)
        {
            var key = row.SafeString("KeyName");
            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            var selectedValue = row.SafeString("SelectedValue");
            var defaultValue = row.SafeString("DefaultValue");
            dictionary[key] = string.IsNullOrWhiteSpace(selectedValue) ? defaultValue : selectedValue;
        }

        return new LocalizedDictionaryDto
        {
            LanguageId = language.Id,
            CultureName = language.CultureName,
            DefaultLanguageId = defaultLanguageId,
            Entries = dictionary
        };
    }

    private async Task EnsureLocalizationSchemaAsync(CancellationToken cancellationToken)
    {
        var sql = @"
            IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Localization')
            BEGIN
                EXEC('CREATE SCHEMA Localization');
            END;

            IF OBJECT_ID('Localization.[Language]', 'U') IS NULL
            BEGIN
                CREATE TABLE Localization.[Language]
                (
                    Id BIGINT IDENTITY(1,1) NOT NULL,
                    CultureName NVARCHAR(255) NOT NULL,
                    DisplayName NVARCHAR(255) NOT NULL,
                    Country NVARCHAR(255) NOT NULL,
                    Region NVARCHAR(255) NOT NULL,
                    IsDefaultLanguage BIT NOT NULL CONSTRAINT DF_Localization_Language_IsDefault DEFAULT(0),
                    CONSTRAINT PK_Localization_Language PRIMARY KEY CLUSTERED (Id DESC)
                );
            END;

            IF OBJECT_ID('Localization.[Resource]', 'U') IS NULL
            BEGIN
                CREATE TABLE Localization.[Resource]
                (
                    Id BIGINT IDENTITY(1,1) NOT NULL,
                    LanguageId BIGINT NOT NULL,
                    [Key] NVARCHAR(450) NOT NULL,
                    Value NVARCHAR(MAX) NOT NULL,
                    Comment NVARCHAR(MAX) NULL,
                    UpdatedOn DATETIME NULL,
                    CONSTRAINT PK_Localization_Resource PRIMARY KEY CLUSTERED (Id DESC)
                );
            END;

            IF EXISTS
            (
                SELECT 1
                FROM sys.columns c
                WHERE c.object_id = OBJECT_ID('Localization.[Resource]')
                  AND c.name = 'Key'
                  AND (c.max_length = -1 OR c.max_length > 900)
            )
            BEGIN
                IF EXISTS (SELECT 1 FROM Localization.[Resource] WHERE LEN([Key]) > 450)
                BEGIN
                    THROW 51000, 'Localization.Resource.[Key] contains values longer than 450 characters and cannot be indexed. Shorten those keys before continuing.', 1;
                END;

                ALTER TABLE Localization.[Resource]
                ALTER COLUMN [Key] NVARCHAR(450) NOT NULL;
            END;

            IF OBJECT_ID('Localization.UserLanguage', 'U') IS NULL
            BEGIN
                CREATE TABLE Localization.UserLanguage
                (
                    LoginID BIGINT NOT NULL,
                    LanguageId BIGINT NOT NULL,
                    UpdatedOn DATETIME NULL,
                    CONSTRAINT PK_Localization_UserLanguage PRIMARY KEY CLUSTERED (LoginID)
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Localization_Language_CultureName' AND object_id = OBJECT_ID('Localization.[Language]'))
            BEGIN
                CREATE UNIQUE INDEX UX_Localization_Language_CultureName ON Localization.[Language](CultureName);
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Localization_Resource_Language_Key' AND object_id = OBJECT_ID('Localization.[Resource]'))
            BEGIN
                CREATE UNIQUE INDEX UX_Localization_Resource_Language_Key ON Localization.[Resource](LanguageId, [Key]);
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Localization_Language_Default' AND object_id = OBJECT_ID('Localization.[Language]'))
            BEGIN
                CREATE UNIQUE INDEX UX_Localization_Language_Default ON Localization.[Language](IsDefaultLanguage) WHERE IsDefaultLanguage = 1;
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Localization_Resource_Language')
            BEGIN
                ALTER TABLE Localization.[Resource]
                ADD CONSTRAINT FK_Localization_Resource_Language
                FOREIGN KEY (LanguageId) REFERENCES Localization.[Language](Id);
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Localization_UserLanguage_Language')
            BEGIN
                ALTER TABLE Localization.UserLanguage
                ADD CONSTRAINT FK_Localization_UserLanguage_Language
                FOREIGN KEY (LanguageId) REFERENCES Localization.[Language](Id);
            END;";

        await ExecuteNonQueryRawAsync(sql, Array.Empty<SqlParameter>(), cancellationToken);

        var checkTable = await ExecuteDataTableRawAsync("SELECT TOP 1 Id FROM Localization.[Language];", Array.Empty<SqlParameter>(), cancellationToken);
        if (checkTable.Rows.Count == 0)
        {
            await ExecuteNonQueryRawAsync(@"
                INSERT INTO Localization.[Language] (CultureName, DisplayName, Country, Region, IsDefaultLanguage)
                VALUES (N'en-IN', N'English', N'India', N'Asia', 1);", Array.Empty<SqlParameter>(), cancellationToken);
        }
    }

    private async Task<long> GetDefaultLanguageIdAsync(CancellationToken cancellationToken)
    {
        var sql = @"
            SELECT TOP 1 Id
            FROM Localization.[Language]
            ORDER BY IsDefaultLanguage DESC, Id ASC;";

        var table = await ExecuteDataTableRawAsync(sql, Array.Empty<SqlParameter>(), cancellationToken);
        return table.Rows.Count == 0 ? 0 : table.Rows[0].SafeLong("Id");
    }

    private async Task<UserLanguagePreferenceDto?> GetDefaultLanguagePreferenceAsync(CancellationToken cancellationToken)
    {
        var sql = @"
            SELECT TOP 1
                CAST(0 AS BIGINT) AS LoginID,
                Id AS LanguageId,
                CultureName,
                DisplayName
            FROM Localization.[Language]
            ORDER BY IsDefaultLanguage DESC, Id ASC;";

        var table = await ExecuteDataTableRawAsync(sql, Array.Empty<SqlParameter>(), cancellationToken);
        return table.Rows.Count == 0 ? null : MapUserLanguagePreference(table.Rows[0]);
    }

    private Task<DataTable> ExecuteDataTableAsync(string storedProcedure, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
        => Common.ResilientSqlExecutor.ExecuteDataTableAsync(_connectionString, storedProcedure, parameters, cancellationToken);

    private Task<int> ExecuteNonQueryAsync(string storedProcedure, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
        => Common.ResilientSqlExecutor.ExecuteNonQueryAsync(_connectionString, storedProcedure, parameters, cancellationToken);

    private Task<DataTable> ExecuteDataTableRawAsync(string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
        => Common.ResilientSqlExecutor.ExecuteDataTableRawAsync(_connectionString, sql, parameters, cancellationToken);

    private Task<int> ExecuteNonQueryRawAsync(string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
        => Common.ResilientSqlExecutor.ExecuteNonQueryRawAsync(_connectionString, sql, parameters, cancellationToken);

    private static RoleDto MapRole(DataRow row) => new()
    {
        RoleId = row.SafeLong("RoleID"),
        SchoolId = row.SafeLong("SchoolID"),
        RoleName = row.SafeString("RoleName"),
        Description = row.SafeString("Description"),
        IsActive = row.SafeBool("IsActive"),
        IsAdmin = row.SafeBool("IsAdmin"),
        ModuleId = row.SafeLong("ModuleID"),
        ModuleName = row.SafeString("ModuleName")
    };

    private static UserDto MapUser(DataRow row) => new()
    {
        LoginId = row.SafeLong("LoginID"),
        SchoolId = row.SafeLong("SchoolID"),
        StaffId = row.SafeLong("StaffID"),
        LoginName = row.SafeString("LoginName"),
        Password = row.SafeString("Password"),
        ForgotEmail = row.SafeString("ForgotEmail"),
        FirstName = row.SafeString("FirstName"),
        LastName = row.SafeString("LastName"),
        IsActive = row.SafeBool("IsActive"),
        LoginType = row.SafeString("LoginType"),
        Browser = row.SafeString("Browser"),
        OperatingSystem = row.SafeString("OperatingSystem"),
        ComputerName = row.SafeString("ComputerName"),
        RoleId = row.SafeLong("RoleID", "RoleId")
    };

    private static StaffDto MapStaff(DataRow row) => new()
    {
        StaffId = row.SafeLong("StaffID", "StaffId"),
        Status = row.SafeInt("Status", "StatusID"),
        StaffSalutation = row.SafeString("StaffSalutation", "StaffSalutationID", "Salutation"),
        StaffSalutationName = row.SafeString("StaffSalutationName"),
        StaffFirstName = row.SafeString("StaffFirstName"),
        StaffLastName = row.SafeString("StaffLastName"),
        StaffNumber = row.SafeString("StaffNumber"),
        NIN = row.SafeString("NIN"),
        DOB = row.Table.Columns.Contains("DOB") && row["DOB"] != DBNull.Value && DateTime.TryParse(row["DOB"].ToString(), out var dob) ? dob : null,
        Gender = row.SafeLong("Gender", "GenderID"),
        GenderName = row.SafeString("GenderName"),
        MobileNo = row.SafeString("MobileNo"),
        EmailIDPersonal = row.SafeString("EmailIDPersonal"),
        EmailIDOfficial = row.SafeString("EmailIDOfficial"),
        Photo = row.SafeString("Photo"),
        HasLogin = row.SafeBool("HasLogin"),
        CreatedBy = row.SafeString("CreatedBy"),
        CreatedDate = row.Table.Columns.Contains("CreatedDate") && row["CreatedDate"] != DBNull.Value && DateTime.TryParse(row["CreatedDate"].ToString(), out var createdDate) ? createdDate : null
    };

    private static LanguageDto MapLanguage(DataRow row) => new()
    {
        Id = row.SafeLong("Id"),
        CultureName = row.SafeString("CultureName"),
        DisplayName = row.SafeString("DisplayName"),
        Country = row.SafeString("Country"),
        Region = row.SafeString("Region"),
        IsDefaultLanguage = row.SafeBool("IsDefaultLanguage")
    };

    private static UserLanguagePreferenceDto MapUserLanguagePreference(DataRow row) => new()
    {
        LoginId = row.SafeLong("LoginID"),
        LanguageId = row.SafeLong("LanguageId"),
        CultureName = row.SafeString("CultureName"),
        DisplayName = row.SafeString("DisplayName")
    };

    private static LocalizedResourceDto MapLocalizedResource(DataRow row) => new()
    {
        ResourceId = row.SafeLong("ResourceId"),
        LanguageId = row.SafeLong("LanguageId"),
        CultureName = row.SafeString("CultureName"),
        Key = row.SafeString("KeyName"),
        DefaultValue = row.SafeString("DefaultValue"),
        Value = row.SafeString("ResourceValue"),
        Comment = row.SafeString("ResourceComment"),
        UpdatedOn = row.SafeDateTime("UpdatedOn"),
        IsMissing = row.SafeBool("IsMissing")
    };

    private static MenuDto MapMenu(DataRow row) => new()
    {
        LinkId = row.SafeLong("LinkID"),
        ModuleId = row.SafeLong("ModuleID"),
        PageHeading = row.SafeString("PageHeading"),
        ParentId = row.SafeLong("ParentID"),
        PagePath = row.SafeString("PagePath"),
        ActualName = row.SafeString("ActualName"),
        IsView = row.SafeBool("ViewPer", "IsView"),
        IsInsert = row.SafeBool("InsertPer", "IsInsert"),
        IsUpdate = row.SafeBool("UpdatePer", "IsUpdate"),
        IsDelete = row.SafeBool("DeletePer", "IsDelete"),
        LevelNo = row.SafeInt("LevelNo"),
        SequenceNo = row.SafeInt("SequenceNo"),
        IsDashboard = row.SafeBool("IsDashboard"),
        ShowInMenu = row.SafeBool("ShowInMenu"),
        IsApp = row.SafeBool("IsApp"),
        ModuleName = row.SafeString("ModuleName"),
        DisplayName = row.SafeString("DisplayName"),
        IconClass = row.SafeString("IconClass")
    };


    public async Task<int> UpsertPageAsync(MenuDto page, CancellationToken cancellationToken = default)
    {
        var sql = @"
            IF EXISTS (SELECT 1 FROM Z_UsersMenu WHERE (ActualName = @ActualName AND ModuleID = @ModuleID) OR (PagePath = @PagePath AND PagePath IS NOT NULL AND PagePath <> ''))
            BEGIN
                UPDATE Z_UsersMenu 
                SET PageHeading = @PageHeading,
                    PagePath = @PagePath,
                    IsView = @IsView,
                    IsDashboard = @IsDashboard,
                    ShowInMenu = @ShowInMenu,
                    UpdatedBy = 'System',
                    UpdatedDate = GETUTCDATE()
                WHERE (ActualName = @ActualName AND ModuleID = @ModuleID) OR (PagePath = @PagePath AND PagePath IS NOT NULL AND PagePath <> '');
            END
            ELSE
            BEGIN
                INSERT INTO Z_UsersMenu (ModuleID, PageHeading, ParentID, PagePath, ActualName, IsView, LevelNo, SequenceNo, IsDashboard, ShowInMenu, CreatedBy, CreatedDate, IsApp)
                VALUES (@ModuleID, @PageHeading, @ParentID, @PagePath, @ActualName, @IsView, @LevelNo, @SequenceNo, @IsDashboard, @ShowInMenu, 'System', GETUTCDATE(), @IsApp);
            END";

        var parameters = new[]
        {
            new SqlParameter("@ModuleID", SqlDbType.BigInt) { Value = page.ModuleId },
            new SqlParameter("@PageHeading", SqlDbType.NVarChar, 200) { Value = page.PageHeading },
            new SqlParameter("@ParentID", SqlDbType.BigInt) { Value = page.ParentId },
            new SqlParameter("@PagePath", SqlDbType.NVarChar, 200) { Value = (object?)page.PagePath ?? DBNull.Value },
            new SqlParameter("@ActualName", SqlDbType.NVarChar, 200) { Value = (object?)page.ActualName ?? DBNull.Value },
            new SqlParameter("@IsView", SqlDbType.Bit) { Value = page.IsView },
            new SqlParameter("@LevelNo", SqlDbType.Int) { Value = page.LevelNo },
            new SqlParameter("@SequenceNo", SqlDbType.Int) { Value = page.SequenceNo },
            new SqlParameter("@IsDashboard", SqlDbType.Bit) { Value = page.IsDashboard },
            new SqlParameter("@ShowInMenu", SqlDbType.Bit) { Value = page.ShowInMenu },
            new SqlParameter("@IsApp", SqlDbType.Bit) { Value = page.IsApp }
        };

        return await ExecuteNonQueryRawAsync(sql, parameters, cancellationToken);
    }

    public async Task<PagedResult<UserActivityLogDto>> GetUserActivityLogsAsync(string? userName, string? loginName, string? eventName, string? eventModule, PagedRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@UserName", SqlDbType.VarChar, 200) { Value = userName ?? string.Empty },
            new("@LoginName", SqlDbType.VarChar, 200) { Value = loginName ?? string.Empty },
            new("@EventName", SqlDbType.VarChar, 200) { Value = eventName ?? string.Empty },
            new("@EventModule", SqlDbType.VarChar, 200) { Value = eventModule ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn) && !string.IsNullOrWhiteSpace(request.SortType))
        {
            parameters.Add(new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType });
            parameters.Add(new SqlParameter("@SortColumn", SqlDbType.VarChar, 200) { Value = request.SortColumn });
        }

        var table = await ExecuteDataTableAsync("Z_UsersActivityLogList", parameters, cancellationToken);
        
        var items = table.AsEnumerable().Select(row => new UserActivityLogDto
        {
            LogId = row.SafeLong("LogID"),
            UserName = row.SafeString("UserName"),
            LoginName = row.SafeString("LoginName"),
            DT = row.Table.Columns.Contains("DT") && row["DT"] != DBNull.Value ? Convert.ToDateTime(row["DT"]) : null,
            EventName = row.SafeString("EventName"),
            EventModule = row.SafeString("EventModule"),
            RefKey = row.SafeString("RefKey"),
            Remarks = row.SafeString("Remarks"),
            Url = row.SafeString("Url")
        }).ToList();

        return new PagedResult<UserActivityLogDto>
        {
            CurrentPage = Convert.ToInt32(parameters[4].Value ?? request.CurrentPage),
            TotalRecord = Convert.ToInt32(parameters[6].Value ?? 0),
            Items = items
        };
    }

    public async Task<PagedResult<LoginLogDto>> GetLoginLogsAsync(string? searchText, long? loginId, DateTime? fromDate, DateTime? toDate, PagedRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@LoginLogID", SqlDbType.BigInt) { Value = 0L },
            new("@Title", SqlDbType.VarChar, 300) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new("@LoginID", SqlDbType.BigInt) { Value = loginId ?? 0L },
            new("@FromDate", SqlDbType.DateTime) { Value = fromDate ?? (object)DBNull.Value },
            new("@ToDate", SqlDbType.DateTime) { Value = toDate ?? (object)DBNull.Value }
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn) && !string.IsNullOrWhiteSpace(request.SortType))
        {
            parameters.Add(new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType });
            parameters.Add(new SqlParameter("@SortColumn", SqlDbType.VarChar, 200) { Value = request.SortColumn });
        }

        var table = await ExecuteDataTableAsync("Z_UsersLoginsLogList", parameters, cancellationToken);
        
        var items = table.AsEnumerable().Select(row => new LoginLogDto
        {
            LoginLogID = row.SafeLong("LoginLogID"),
            LoginName = row.SafeString("LoginName"),
            UserName = row.SafeString("UserName"),
            MasterName = row.SafeString("MasterName"),
            LoginType = row.SafeString("LoginType"),
            IpAddress = row.SafeString("IpAddress"),
            LoginDateTime = row.Table.Columns.Contains("LoginDateTime") && row["LoginDateTime"] != DBNull.Value ? Convert.ToDateTime(row["LoginDateTime"]) : null,
            LogoutDateTime = row.Table.Columns.Contains("LogoutDateTime") && row["LogoutDateTime"] != DBNull.Value ? Convert.ToDateTime(row["LogoutDateTime"]) : null,
            BrowserName = row.SafeString("BrowserName"),
            OperatingSystem = row.SafeString("OperatingSystem"),
            ComputerName = row.SafeString("ComputerName")
        }).ToList();

        return new PagedResult<LoginLogDto>
        {
            CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
            TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
            Items = items
        };
    }

    public async Task<long> InsertUserActivityLogAsync(UserActivityLogDto log, CancellationToken cancellationToken = default)
    {
        var outputParam = new SqlParameter("@LogID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
        var parameters = new List<SqlParameter>
        {
            new("@UserName", SqlDbType.NVarChar, 200) { Value = !string.IsNullOrEmpty(log.UserName) ? log.UserName : "System" },
            new("@DT", SqlDbType.DateTime) { Value = log.DT ?? DateTime.Now },
            new("@EventName", SqlDbType.NVarChar, 500) { Value = !string.IsNullOrEmpty(log.EventName) ? log.EventName : "Activity" },
            new("@EventModule", SqlDbType.NVarChar, 500) { Value = !string.IsNullOrEmpty(log.EventModule) ? log.EventModule : "System" },
            new("@RefKey", SqlDbType.NVarChar, 500) { Value = !string.IsNullOrEmpty(log.RefKey) ? log.RefKey : "0" },
            new("@Remarks", SqlDbType.NVarChar, -1) { Value = log.Remarks ?? string.Empty },
            new("@Url", SqlDbType.NVarChar, -1) { Value = log.Url ?? string.Empty },
            new("@LoginID", SqlDbType.BigInt) { Value = log.LoginId ?? (object)DBNull.Value },
            outputParam
        };

        await ExecuteNonQueryAsync("Users_ActivityLog_Insert", parameters, cancellationToken);
        
        return Convert.ToInt64(outputParam.Value ?? 0L);
    }
}

internal static class LegacyCrypto
{
    private static readonly string EncryptKey = "ce!nf0c0m";

    public static string EncryptString(string value)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(EncryptKey));
        using var tripleDes = System.Security.Cryptography.TripleDES.Create();
        tripleDes.Key = key;
        tripleDes.Mode = System.Security.Cryptography.CipherMode.ECB;
        tripleDes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

        var data = System.Text.Encoding.UTF8.GetBytes(value);
        using var transform = tripleDes.CreateEncryptor();
        var result = transform.TransformFinalBlock(data, 0, data.Length);
        return Convert.ToBase64String(result);
    }
}

internal static class DataRowExtensions
{
    public static DateTime? SafeDateTime(this IDataRecord record, params string[] names)
    {
        foreach (var name in names)
        {
            var ordinal = -1;
            try
            {
                ordinal = record.GetOrdinal(name);
            }
            catch
            {
                // ignore missing columns
            }

            if (ordinal >= 0 && !record.IsDBNull(ordinal) && DateTime.TryParse(record.GetValue(ordinal).ToString(), out var value))
            {
                return value;
            }
        }

        return null;
    }

    public static DateTime? SafeDateTime(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && DateTime.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
        }

        return null;
    }

    public static long SafeLong(this IDataRecord record, params string[] names)
    {
        foreach (var name in names)
        {
            var ordinal = -1;
            try
            {
                ordinal = record.GetOrdinal(name);
            }
            catch
            {
                // ignore missing columns
            }

            if (ordinal >= 0 && !record.IsDBNull(ordinal) && long.TryParse(record.GetValue(ordinal).ToString(), out var value))
            {
                return value;
            }
        }

        return 0;
    }

    public static int SafeInt(this IDataRecord record, params string[] names)
    {
        foreach (var name in names)
        {
            var ordinal = -1;
            try
            {
                ordinal = record.GetOrdinal(name);
            }
            catch
            {
                // ignore missing columns
            }

            if (ordinal >= 0 && !record.IsDBNull(ordinal) && int.TryParse(record.GetValue(ordinal).ToString(), out var value))
            {
                return value;
            }
        }

        return 0;
    }

    public static bool SafeBool(this IDataRecord record, params string[] names)
    {
        foreach (var name in names)
        {
            var ordinal = -1;
            try
            {
                ordinal = record.GetOrdinal(name);
            }
            catch
            {
                // ignore missing columns
            }

            if (ordinal >= 0 && !record.IsDBNull(ordinal))
            {
                var raw = record.GetValue(ordinal);
                if (bool.TryParse(raw.ToString(), out var value))
                {
                    return value;
                }

                if (int.TryParse(raw.ToString(), out var numeric))
                {
                    return numeric != 0;
                }
            }
        }

        return false;
    }

    public static string SafeString(this IDataRecord record, params string[] names)
    {
        foreach (var name in names)
        {
            var ordinal = -1;
            try
            {
                ordinal = record.GetOrdinal(name);
            }
            catch
            {
                // ignore missing columns
            }

            if (ordinal >= 0 && !record.IsDBNull(ordinal))
            {
                return Convert.ToString(record.GetValue(ordinal)) ?? string.Empty;
            }
        }

        return string.Empty;
    }

    public static long SafeLong(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && long.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
        }

        return 0;
    }

    public static int SafeInt(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && int.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
        }

        return 0;
    }

    public static bool SafeBool(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && bool.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }

            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && int.TryParse(row[name].ToString(), out var numeric))
            {
                return numeric != 0;
            }
        }

        return false;
    }

    public static string SafeString(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value)
            {
                return Convert.ToString(row[name]) ?? string.Empty;
            }
        }

        return string.Empty;
    }
}
