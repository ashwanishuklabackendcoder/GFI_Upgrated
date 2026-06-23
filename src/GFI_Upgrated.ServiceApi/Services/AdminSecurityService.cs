using GFI_Upgrated.Data.AdminSecurity;
using GFI_Upgrated.SharedDto.AdminSecurity;
using GFI_Upgrated.SharedDto.Common;

namespace GFI_Upgrated.ServiceApi.Services;

public interface IAdminSecurityService
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

public sealed class AdminSecurityService : IAdminSecurityService
{
    private readonly IAdminSecurityRepository _repository;

    public AdminSecurityService(IAdminSecurityRepository repository)
    {
        _repository = repository;
    }

    public Task<LoginResultDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => _repository.LoginAsync(request, cancellationToken);

    public Task<PagedResult<RoleDto>> GetRolesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetRolesAsync(request, searchText, cancellationToken);

    public Task<RoleDto?> GetRoleByIdAsync(long roleId, CancellationToken cancellationToken = default)
        => _repository.GetRoleByIdAsync(roleId, cancellationToken);

    public Task<int> SaveRoleAsync(SaveRoleRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveRoleAsync(request, cancellationToken);

    public Task<PagedResult<UserDto>> GetUsersAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetUsersAsync(request, searchText, cancellationToken);

    public Task<UserDto?> GetUserByIdAsync(long loginId, CancellationToken cancellationToken = default)
        => _repository.GetUserByIdAsync(loginId, cancellationToken);

    public Task<int> SaveUserAsync(SaveUserRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveUserAsync(request, cancellationToken);

    public Task<PagedResult<StaffDto>> GetStaffsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetStaffsAsync(request, searchText, cancellationToken);

    public Task<StaffDto?> GetStaffByIdAsync(long staffId, CancellationToken cancellationToken = default)
        => _repository.GetStaffByIdAsync(staffId, cancellationToken);

    public Task<int> SaveStaffAsync(SaveStaffRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveStaffAsync(request, cancellationToken);

    public Task<int> DeleteStaffAsync(long staffId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteStaffAsync(staffId, updatedBy, cancellationToken);

    public Task<IReadOnlyList<MenuDto>> GetMenusAsync(CancellationToken cancellationToken = default)
        => _repository.GetMenusAsync(cancellationToken);

    public Task<MenuDto?> GetMenuByIdAsync(long linkId, CancellationToken cancellationToken = default)
        => _repository.GetMenuByIdAsync(linkId, cancellationToken);

    public Task<int> SaveMenuAsync(SaveMenuRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveMenuAsync(request, cancellationToken);

    public Task<IReadOnlyList<ModuleDto>> GetModulesAsync(CancellationToken cancellationToken = default)
        => _repository.GetModulesAsync(cancellationToken);

    public Task<IReadOnlyList<MenuDto>> GetParentMenusAsync(long moduleId, CancellationToken cancellationToken = default)
        => _repository.GetParentMenusAsync(moduleId, cancellationToken);

    public Task<IReadOnlyList<MenuDto>> GetDashboardsAsync(CancellationToken cancellationToken = default)
        => _repository.GetDashboardsAsync(cancellationToken);

    public Task<IReadOnlyList<RolePermissionDto>> GetRolePermissionsAsync(long roleId, long moduleId, CancellationToken cancellationToken = default)
        => _repository.GetRolePermissionsAsync(roleId, moduleId, cancellationToken);

    public Task<int> SaveRolePermissionsAsync(SaveRolePermissionsRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveRolePermissionsAsync(request, cancellationToken);

    public Task<IReadOnlyList<UserRoleAssignmentDto>> GetUserRolesAsync(long loginId, CancellationToken cancellationToken = default)
        => _repository.GetUserRolesAsync(loginId, cancellationToken);

    public Task<int> SaveUserRolesAsync(SaveUserRolesRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveUserRolesAsync(request, cancellationToken);

    public Task<IReadOnlyList<StaffLookupDto>> GetActiveStaffAsync(CancellationToken cancellationToken = default)
        => _repository.GetActiveStaffAsync(cancellationToken);

    public Task<IReadOnlyList<StaffLookupDto>> GetUnassignedStaffAsync(CancellationToken cancellationToken = default)
        => _repository.GetUnassignedStaffAsync(cancellationToken);

    public Task<IReadOnlyList<LookupItemDto>> GetMasterDropdownAsync(long parentId, CancellationToken cancellationToken = default)
        => _repository.GetMasterDropdownAsync(parentId, cancellationToken);

    public Task<PagedResult<DropDownMasterDto>> GetDropDownMastersAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetDropDownMastersAsync(request, searchText, cancellationToken);

    public Task<DropDownMasterDto?> GetDropDownMasterByIdAsync(long id, CancellationToken cancellationToken = default)
        => _repository.GetDropDownMasterByIdAsync(id, cancellationToken);

    public Task<int> SaveDropDownMasterAsync(SaveDropDownMasterRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveDropDownMasterAsync(request, cancellationToken);

    public Task<PagedResult<DropDownValueDto>> GetDropDownValuesAsync(long masterId, PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetDropDownValuesAsync(masterId, request, searchText, cancellationToken);

    public Task<DropDownValueDto?> GetDropDownValueByIdAsync(long id, CancellationToken cancellationToken = default)
        => _repository.GetDropDownValueByIdAsync(id, cancellationToken);

    public Task<int> SaveDropDownValueAsync(SaveDropDownValueRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveDropDownValueAsync(request, cancellationToken);

    public Task<int> DeleteDropDownValueAsync(long id, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteDropDownValueAsync(id, updatedBy, cancellationToken);

    public Task<IReadOnlyList<LanguageDto>> GetLanguagesAsync(CancellationToken cancellationToken = default)
        => _repository.GetLanguagesAsync(cancellationToken);

    public Task<LanguageDto?> GetLanguageByIdAsync(long id, CancellationToken cancellationToken = default)
        => _repository.GetLanguageByIdAsync(id, cancellationToken);

    public Task<int> SaveLanguageAsync(SaveLanguageRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveLanguageAsync(request, cancellationToken);

    public Task<PagedResult<LocalizedResourceDto>> GetLocalizedResourcesAsync(LocalizedResourceListRequest request, CancellationToken cancellationToken = default)
        => _repository.GetLocalizedResourcesAsync(request, cancellationToken);

    public Task<int> SaveLocalizedResourceAsync(SaveLocalizedResourceRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveLocalizedResourceAsync(request, cancellationToken);

    public Task<int> SyncLocalizationDefaultsAsync(CancellationToken cancellationToken = default)
        => _repository.SyncLocalizationDefaultsAsync(cancellationToken);

    public Task<UserLanguagePreferenceDto?> GetUserLanguagePreferenceAsync(long loginId, CancellationToken cancellationToken = default)
        => _repository.GetUserLanguagePreferenceAsync(loginId, cancellationToken);

    public Task<int> SaveUserLanguagePreferenceAsync(SaveUserLanguagePreferenceRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveUserLanguagePreferenceAsync(request, cancellationToken);

    public Task<LocalizedDictionaryDto> GetLocalizedDictionaryAsync(long languageId, CancellationToken cancellationToken = default)
        => _repository.GetLocalizedDictionaryAsync(languageId, cancellationToken);

    public Task<LoginResultDto?> GetLoginResultAsync(long loginId, long roleId, CancellationToken cancellationToken = default)
        => _repository.GetLoginResultAsync(loginId, roleId, cancellationToken);

    public Task<int> UpsertPageAsync(MenuDto page, CancellationToken cancellationToken = default)
        => _repository.UpsertPageAsync(page, cancellationToken);

    public Task<PagedResult<UserActivityLogDto>> GetUserActivityLogsAsync(string? userName, string? loginName, string? eventName, string? eventModule, PagedRequest request, CancellationToken cancellationToken = default)
        => _repository.GetUserActivityLogsAsync(userName, loginName, eventName, eventModule, request, cancellationToken);

    public Task<PagedResult<LoginLogDto>> GetLoginLogsAsync(string? searchText, long? loginId, DateTime? fromDate, DateTime? toDate, PagedRequest request, CancellationToken cancellationToken = default)
        => _repository.GetLoginLogsAsync(searchText, loginId, fromDate, toDate, request, cancellationToken);

    public Task<long> InsertUserActivityLogAsync(UserActivityLogDto log, CancellationToken cancellationToken = default)
        => _repository.InsertUserActivityLogAsync(log, cancellationToken);
}
