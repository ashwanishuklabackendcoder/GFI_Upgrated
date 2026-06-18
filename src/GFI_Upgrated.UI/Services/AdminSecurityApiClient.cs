using System.Net.Http.Json;
using System.Text.Json;
using GFI_Upgrated.SharedDto.AdminSecurity;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.UI.State;

namespace GFI_Upgrated.UI.Services;

public sealed class AdminSecurityApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly AppSessionState _sessionState;

    public AdminSecurityApiClient(HttpClient httpClient, AppSessionState sessionState)
    {
        _httpClient = httpClient;
        _sessionState = sessionState;
    }

    public async Task<LoginResultDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<LoginRequest, LoginResultDto>("api/admin/security/login", request, cancellationToken);

    public async Task<PagedResult<RoleDto>> GetRolesAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<RoleDto>>(BuildQuery("api/admin/security/roles", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<RoleDto>();

    public async Task<RoleDto?> GetRoleByIdAsync(long roleId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<RoleDto>($"api/admin/security/roles/{roleId}", cancellationToken);

    public async Task<int> SaveRoleAsync(SaveRoleRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveRoleRequest, int>("api/admin/security/roles", request, cancellationToken);

    public async Task<PagedResult<UserDto>> GetUsersAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<UserDto>>(BuildQuery("api/admin/security/users", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<UserDto>();

    public async Task<UserDto?> GetUserByIdAsync(long loginId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<UserDto>($"api/admin/security/users/{loginId}", cancellationToken);

    public async Task<int> SaveUserAsync(SaveUserRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveUserRequest, int>("api/admin/security/users", request, cancellationToken);

    public async Task<PagedResult<StaffDto>> GetStaffsAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<StaffDto>>(BuildQuery("api/admin/security/staff", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<StaffDto>();

    public async Task<StaffDto?> GetStaffByIdAsync(long staffId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<StaffDto>($"api/admin/security/staff/{staffId}", cancellationToken);

    public async Task<int> SaveStaffAsync(SaveStaffRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveStaffRequest, int>("api/admin/security/staff", request, cancellationToken);

    public async Task<int> DeleteStaffAsync(long staffId, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/admin/security/staff/{staffId}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    public async Task<IReadOnlyList<MenuDto>> GetMenusAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<MenuDto>>("api/admin/security/menus", cancellationToken) ?? Array.Empty<MenuDto>();

    public async Task<MenuDto?> GetMenuByIdAsync(long linkId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<MenuDto>($"api/admin/security/menus/{linkId}", cancellationToken);

    public async Task<int> SaveMenuAsync(SaveMenuRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveMenuRequest, int>("api/admin/security/menus", request, cancellationToken);

    public async Task<IReadOnlyList<ModuleDto>> GetModulesAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<ModuleDto>>("api/admin/security/modules", cancellationToken) ?? Array.Empty<ModuleDto>();

    public async Task<IReadOnlyList<MenuDto>> GetParentMenusAsync(long moduleId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<MenuDto>>($"api/admin/security/modules/{moduleId}/parent-menus", cancellationToken) ?? Array.Empty<MenuDto>();

    public async Task<IReadOnlyList<MenuDto>> GetDashboardsAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<MenuDto>>("api/admin/security/dashboards", cancellationToken) ?? Array.Empty<MenuDto>();

    public async Task<IReadOnlyList<RolePermissionDto>> GetRolePermissionsAsync(long roleId, long moduleId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<RolePermissionDto>>($"api/admin/security/roles/{roleId}/modules/{moduleId}/permissions", cancellationToken) ?? Array.Empty<RolePermissionDto>();

    public async Task<int> SaveRolePermissionsAsync(long roleId, long moduleId, SaveRolePermissionsRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveRolePermissionsRequest, int>($"api/admin/security/roles/{roleId}/modules/{moduleId}/permissions", request, cancellationToken);

    public async Task<IReadOnlyList<UserRoleAssignmentDto>> GetUserRolesAsync(long loginId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<UserRoleAssignmentDto>>($"api/admin/security/users/{loginId}/roles", cancellationToken) ?? Array.Empty<UserRoleAssignmentDto>();

    public async Task<int> SaveUserRolesAsync(long loginId, SaveUserRolesRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveUserRolesRequest, int>($"api/admin/security/users/{loginId}/roles", request, cancellationToken);

    public async Task<LoginResultDto?> RefreshSessionAsync(long loginId, long roleId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<LoginResultDto>($"api/admin/security/refresh-session?loginId={loginId}&roleId={roleId}", cancellationToken);

    public async Task<IReadOnlyList<StaffLookupDto>> GetUnassignedStaffAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<StaffLookupDto>>("api/admin/security/staff/unassigned", cancellationToken) ?? Array.Empty<StaffLookupDto>();

    public async Task<IReadOnlyList<StaffLookupDto>> GetActiveStaffAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<StaffLookupDto>>("api/admin/security/staff/active", cancellationToken) ?? Array.Empty<StaffLookupDto>();

    public async Task<IReadOnlyList<LookupItemDto>> GetMasterDropdownAsync(long parentId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<LookupItemDto>>($"api/admin/security/lookups/master-dropdown/{parentId}", cancellationToken) ?? Array.Empty<LookupItemDto>();

    public async Task<PagedResult<DropDownMasterDto>> GetDropDownMastersAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<DropDownMasterDto>>(BuildQuery("api/admin/security/dropdown-masters", request, ("searchText", searchText)), cancellationToken) ?? new PagedResult<DropDownMasterDto>();

    public async Task<DropDownMasterDto?> GetDropDownMasterByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<DropDownMasterDto>($"api/admin/security/dropdown-masters/{id}", cancellationToken);

    public async Task<int> SaveDropDownMasterAsync(SaveDropDownMasterRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveDropDownMasterRequest, int>("api/admin/security/dropdown-masters", request, cancellationToken);

    public async Task<PagedResult<DropDownValueDto>> GetDropDownValuesAsync(long masterId, PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<DropDownValueDto>>(BuildQuery($"api/admin/security/dropdown-masters/{masterId}/values", request, ("searchText", searchText)), cancellationToken) ?? new PagedResult<DropDownValueDto>();

    public async Task<DropDownValueDto?> GetDropDownValueByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<DropDownValueDto>($"api/admin/security/dropdown-values/{id}", cancellationToken);

    public async Task<int> SaveDropDownValueAsync(SaveDropDownValueRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveDropDownValueRequest, int>("api/admin/security/dropdown-values", request, cancellationToken);

    public async Task<int> DeleteDropDownValueAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/admin/security/dropdown-values/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    private async Task<T?> GetEnvelopeAsync<T>(string url, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var envelope = await _httpClient.GetFromJsonAsync<ApiEnvelope<T>>(url, cancellationToken);
        return envelope is { Success: true } ? envelope.Data : default;
    }

    private void AddAuthHeader()
    {
        if (_sessionState.CurrentUser?.Token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionState.CurrentUser.Token);
        }
    }

    private async Task<TResponse> PostEnvelopeAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with {(int)response.StatusCode} ({response.ReasonPhrase}): {payload}");
        }

        var envelope = string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);

        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    private async Task<TResponse> DeleteEnvelopeAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with {(int)response.StatusCode} ({response.ReasonPhrase}): {payload}");
        }

        var envelope = string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);

        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    public async Task<PagedResult<UserActivityLogDto>> GetUserActivityLogsAsync(
        string? userName,
        string? loginName,
        string? eventName,
        string? eventModule,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var extras = new[]
        {
            ("userName", userName ?? string.Empty),
            ("loginName", loginName ?? string.Empty),
            ("eventName", eventName ?? string.Empty),
            ("eventModule", eventModule ?? string.Empty)
        };
        var url = BuildQuery("api/admin/security/user-activity-logs", request, extras);
        return await GetEnvelopeAsync<PagedResult<UserActivityLogDto>>(url, cancellationToken) ?? new PagedResult<UserActivityLogDto>();
    }

    public async Task<PagedResult<LoginLogDto>> GetLoginLogsAsync(
        string? searchText,
        long? loginId,
        DateTime? fromDate,
        DateTime? toDate,
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var extras = new[]
        {
            ("searchText", searchText ?? string.Empty),
            ("loginId", loginId?.ToString() ?? string.Empty),
            ("fromDate", fromDate?.ToString("o") ?? string.Empty),
            ("toDate", toDate?.ToString("o") ?? string.Empty)
        };
        var url = BuildQuery("api/admin/security/login-logs", request, extras);
        return await GetEnvelopeAsync<PagedResult<LoginLogDto>>(url, cancellationToken) ?? new PagedResult<LoginLogDto>();
    }

    public async Task<long> InsertUserActivityLogAsync(UserActivityLogDto log, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<UserActivityLogDto, long>("api/admin/security/user-activity-logs", log, cancellationToken);

    private static string BuildQuery(string url, PagedRequest request, params (string Name, string? Value)[] extras)
    {
        var query = new List<string>
        {
            $"CurrentPage={Uri.EscapeDataString(request.CurrentPage.ToString())}",
            $"RecordPerPage={Uri.EscapeDataString(request.RecordPerPage.ToString())}"
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn))
        {
            query.Add($"SortColumn={Uri.EscapeDataString(request.SortColumn)}");
        }

        if (!string.IsNullOrWhiteSpace(request.SortType))
        {
            query.Add($"SortType={Uri.EscapeDataString(request.SortType)}");
        }

        foreach (var (name, value) in extras)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                query.Add($"{name}={Uri.EscapeDataString(value)}");
            }
        }

        return $"{url}?{string.Join('&', query)}";
    }
}
