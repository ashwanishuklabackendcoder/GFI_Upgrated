using GFI_Upgrated.SharedDto.AdminSecurity;
using Microsoft.JSInterop;
using System.Text.Json;

namespace GFI_Upgrated.UI.State;

public sealed class AppSessionState
{
    public LoginResultDto? CurrentUser { get; private set; }

    private readonly IJSRuntime _jsRuntime;
    private DateTime _lastRefreshTime = DateTime.MinValue;
    private bool _isRefreshing = false;

    public AppSessionState(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }


    public string DateFormat { get; private set; } = "MM/DD/YYYY";
    public string DatePickerFormat => DateFormat.ToLower() == "mm/dd/yyyy" ? "MM/dd/yyyy" : "dd/MM/yyyy";
    public string DefaultCurrency { get; private set; } = "SRD";
    public string NameFormat { get; private set; } = "FN_LN";

    public string CurrentUserAuditName => 
        CurrentUser != null && !string.IsNullOrWhiteSpace(CurrentUser.FullName) 
            ? CurrentUser.FullName 
            : (CurrentUser?.LoginName ?? "System");

    public string FormatDate(DateTime? date)
    {
        if (date == null) return string.Empty;
        return FormatDate(date.Value);
    }

    public string FormatDate(DateTime date)
    {
        if (DateFormat == "MM/DD/YYYY")
        {
            return date.ToString("MM/dd/yyyy");
        }
        return date.ToString("MM/dd/yyyy"); // default/fallback to USA format
    }

    public string FormatDate(string? dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr)) return string.Empty;
        
        // Strip time if present (e.g. "30/01/2027 00:00:00")
        var cleanDateStr = dateStr.Trim();
        if (cleanDateStr.Contains(" "))
        {
            cleanDateStr = cleanDateStr.Split(' ')[0];
        }

        // Try exact parsing matching the API Server's serialization format (dd/MM/yyyy) first
        string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "yyyy/MM/dd", "d/M/yyyy", "M/d/yyyy" };
        if (DateTime.TryParseExact(cleanDateStr, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date))
        {
            return FormatDate(date);
        }

        // Generic fallback parse if exact formats did not match
        if (DateTime.TryParse(cleanDateStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
        {
            return FormatDate(date);
        }

        return cleanDateStr;
    }

    public string FormatCurrency(double amount) => $"{DefaultCurrency} {amount:N2}";
    public string FormatCurrency(decimal amount) => $"{DefaultCurrency} {amount:N2}";

    public string FormatName(string? firstName, string? lastName)
    {
        firstName = (firstName ?? "").Trim();
        lastName = (lastName ?? "").Trim();
        if (NameFormat == "LN_FN")
        {
            return $"{lastName} {firstName}".Trim();
        }
        return $"{firstName} {lastName}".Trim();
    }

    public bool IsDarkMode { get; private set; }
    public event Action? OnChange;
    public event Action? OnSessionExpired;

    public void NotifySessionExpired()
    {
        OnSessionExpired?.Invoke();
    }

    public bool IsLoggedIn => CurrentUser is not null;

    public bool IsAdmin => CurrentUser?.IsAdmin ?? false;

    public bool HasPermission(string path, string action)
    {
        if (CurrentUser == null) return false;
        
        // Priority Override: Disabled as requested to restrict access for Admin roles who do not have permissions
        // if (IsAdmin) return true;

        // Normalize path for comparison
        var normalizedPath = path.ToLower().Trim('/').Replace("\\", "/");
        
        // Find if the menu exists in the backend-returned authorized tree
        var menu = FindMenu(CurrentUser.Menus, normalizedPath);

        Console.WriteLine($"[RBAC Trace] HasPermission: Path='{path}', Normalized='{normalizedPath}', Action='{action}'. Menu Found='{menu?.PageHeading ?? "null"}', IsView={menu?.IsView}");

        if (menu != null)
        {
            var allowed = action.ToLower() switch
            {
                "view" => menu.IsView,
                "insert" => menu.IsInsert,
                "update" => menu.IsUpdate,
                "delete" => menu.IsDelete,
                "export" => menu.IsExport,
                "print" => menu.IsPrint,
                "approve" => menu.IsApprove,
                _ => false
            };
            Console.WriteLine($"[RBAC Trace] Permission Result for '{normalizedPath}' is {allowed} (View={menu.IsView})");
            return allowed;
        }

        // Trace for debugging access denied issues
        Console.WriteLine($"[RBAC Trace] Permission Denied: Path='{normalizedPath}', Action='{action}'. No matching menu found.");
        return false;
    }
    public MenuDto GetPermissions(string path)
    {
        if (CurrentUser == null) return new MenuDto();
        var normalizedPath = path.ToLower().Trim('/').Replace("\\", "/");
        return FindMenu(CurrentUser.Menus, normalizedPath) ?? new MenuDto();
    }

    private static readonly Dictionary<string, string> RouteGroupMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "admin/almirah", "almirah" },
        { "admin/w_masteralmirahshelfaddlist", "almirah" },
        { "store/w_masteralmirahshelfaddlist", "almirah" },
        { "store/almirah", "almirah" },
        { "store/bom", "bom" },
        { "admin/bom", "bom" },
        { "store/w_masterbomitemslist", "bom" },
        { "admin/w_masterbomitemslist", "bom" },
        { "store/w_masterbomaddlist", "bom" },
        { "store/w_masterbrandsaddlist", "brands" },
        { "store/brands", "brands" },
        { "admin/w_masterbrandsaddlist", "brands" },
        { "admin/brands", "brands" },
        { "store/finished-products", "finished-products" },
        { "store/w_masteritemproductlist", "finished-products" },
        { "admin/w_masteritemproductlist", "finished-products" },
        { "admin/finished-products", "finished-products" },
        { "admin/item-category", "item-category" },
        { "store/w_itemcategoryaddlist", "item-category" },
        { "store/item-category", "item-category" },
        { "admin/w_itemcategoryaddlist", "item-category" },
        { "store/reports/item-stock-by-batch", "item-stock" },
        { "admin/reports/item-stock-by-batch", "item-stock" },
        { "store/itemstocklist", "item-stock" },
        { "admin/itemstocklist", "item-stock" },
        { "admin/item-type", "item-type" },
        { "admin/w_itemtypeaddlist", "item-type" },
        { "store/w_itemtypeaddlist", "item-type" },
        { "store/item-type", "item-type" },
        { "admin/kettle", "kettle" },
        { "store/w_masterkettlelist", "kettle" },
        { "store/kettle", "kettle" },
        { "admin/w_masterkettlelist", "kettle" },
        { "store/w_masteritemprelist", "preprocessing" },
        { "store/preprocessing", "preprocessing" },
        { "admin/w_masteritemprelist", "preprocessing" },
        { "admin/preprocessing", "preprocessing" },
        { "store/w_preprocessinglist", "preprocessing" },
        { "admin/pre-processing", "preprocessing" },
        { "admin/production-pre-processing", "preprocessing" },
        { "admin/production", "production" },
        { "store/production", "production" },
        { "store/w_productionlist", "production" },
        { "admin/w_productionlist", "production" },
        { "store/rawmaterials", "rawmaterials" },
        { "store/w_masteritemlist", "rawmaterials" },
        { "admin/w_masteritemlist", "rawmaterials" },
        { "admin/rawmaterials", "rawmaterials" },
        { "admin/raw-materials", "rawmaterials" },
        { "store/semi-finished-products", "semi-finished-products" },
        { "store/w_masteritempreprocessedlist", "semi-finished-products" },
        { "admin/w_masteritempreprocessedlist", "semi-finished-products" },
        { "admin/semi-finished-products", "semi-finished-products" },
        { "admin/status", "status" },
        { "store/w_masterstatusaddlist", "status" },
        { "store/status", "status" },
        { "admin/w_masterstatusaddlist", "status" },
        { "admin/warehouse", "warehouse" },
        { "store/w_warehouseaddlist", "warehouse" },
        { "store/warehouse", "warehouse" },
        { "admin/w_warehouseaddlist", "warehouse" },
        { "admin/units", "units" },
        { "admin/w_masterunitaddlist", "units" },
        { "store/w_masterunitaddlist", "units" },
        { "store/units", "units" },
        { "admin/user-roles", "user-roles" },
        { "admin/assign-roles", "user-roles" },
        { "admin/users", "users" },
        { "admin/logins", "users" },
        { "admin/menus", "menus" },
        { "admin/menu", "menus" },
        { "admin/purchase", "purchase" },
        { "store/w_purchasemasterlist", "purchase" },
        { "admin/currency", "currency" },
        { "acc/mastercurrency", "currency" },
        { "acc/customerorderlist", "customer-order" },
        { "admin/customer-orders", "customer-order" },
        { "admin/customer-order", "customer-order" },
        { "store/reports/batch-wise-items", "batch-wise-items" },
        { "admin/reports/batch-wise-items", "batch-wise-items" },
        { "store/reportbatchwiseitems", "batch-wise-items" },
        { "admin/reportbatchwiseitems", "batch-wise-items" },
        { "admin/item-write-off", "item-write-off" },
        { "admin/z_usersactivitylog", "user-logs" },
        { "admin/loginslog", "login-logs" },
        { "admin/drop-down-editor", "drop-down-editor" },
        { "admin/drop-down-values", "drop-down-editor" },
        { "admin/drop-down", "drop-down-editor" },
        { "admin/resource-editor", "resource-editor" },
        { "acc/accountmaster", "customers-suppliers" },
        { "acc/customer-supplier", "customers-suppliers" }
    };

    private string GetResourceKey(string route)
    {
        var normalized = route.ToLower().Trim('/').Replace("\\", "/");
        foreach (var kvp in RouteGroupMap)
        {
            var key = kvp.Key.ToLower().Trim('/');
            if (normalized == key || normalized.StartsWith(key + "/"))
            {
                return kvp.Value;
            }
        }
        return normalized;
    }

    private MenuDto? FindMenu(IEnumerable<MenuDto> menus, string normalizedTarget)
    {
        var targetKey = GetResourceKey(normalizedTarget);

        foreach (var m in menus)
        {
            var menuRoute = GetHardmappedRoute(m.PagePath, m.PageHeading).ToLower().Trim('/').Replace("\\", "/");
            var menuKey = GetResourceKey(menuRoute);
            
            Console.WriteLine($"[RBAC Trace] FindMenu: Checking Menu='{m.PageHeading}', PagePath='{m.PagePath}', menuRoute='{menuRoute}', menuKey='{menuKey}', targetKey='{targetKey}'");

            // Match based on resolved resource key or path prefix (excluding base dashboard paths like admin/store/acc)
            if (menuKey == targetKey || 
                (menuRoute != "admin" && menuRoute != "store" && menuRoute != "acc" && 
                 !string.IsNullOrEmpty(menuRoute) && normalizedTarget.StartsWith(menuRoute + "/")))
            {
                Console.WriteLine($"[RBAC Trace] FindMenu: MATCH FOUND! Menu='{m.PageHeading}', IsView={m.IsView}");
                return m;
            }
            
            var found = FindMenu(m.SubMenus, normalizedTarget);
            if (found != null) return found;
        }
        return null;
    }

    public string GetHardmappedRoute(string? dbPath, string heading)
    {
        var cleanHeading = (heading ?? "").Trim().TrimStart('_').ToLower();
        var route = cleanHeading switch
        {
            "dashboard" => "/admin",
            "staff" => "/admin/staff",
            "logins" => "/admin/users",
            "drop down editor" or "drop down" or "dropdown editor" or "dropdown" => "/admin/drop-down-editor",
            "resource editor" => "/admin/resource-editor",
            "user activity logs" => "/admin/user-activity-logs",
            "logins log" => "/admin/logins-log",
            "assign roles" => "/admin/user-roles",
            "permissions" => "/admin/rolepermission",
            "roles" => "/admin/roles",
            "import" => "/admin/import",
            "menu" => "/admin/menus",
            "pre processing" or "preprocessing" => "/store/preprocessing",
            "semi finished products" or "semi-finished products" or "semi finished product" or "semi-finished product" or "semifinishedproducts" or "semifinishedproduct" => "/store/semi-finished-products",
            "finished products" or "finishedproducts" or "finished product" or "finish product" => "/store/finished-products",
            "raw materials" or "rawmaterials" => "/store/rawmaterials",
            "item category" or "itemcategory" => "/admin/item-category",
            "sku" => "/store/w_masterskuaddlist",
            "brands" => "/store/w_masterbrandsaddlist",
            "units" => "/admin/units",
            "kettle" => "/admin/kettle",
            "almirah" => "/admin/almirah",
            "warehouse" => "/admin/warehouse",
            "status" => "/admin/status",
            "purchase" => "/admin/purchase",
            "purchase return" => "/admin/purchase-return",
            "purchase order" => "/admin/purchase-order",
            "purchase request" => "/admin/purchase-request",
            "item write off" => "/admin/item-write-off",
            "production" => "/admin/production",
            "bom" or "store/bom" => "/store/bom",
            "order return" => "/admin/order-return",
            "customer order" => "/admin/customer-order",
            "invoice" => "/admin/invoice",
            "customers & suppliers" => "/admin/customers-suppliers",
            "account master" => "/admin/account-master",
            "account group" => "/admin/account-group",
            "currency" => "/admin/currency",
            "reports" => "/admin/reports",
            "user logs" or "userlogs" or "z_usersactivitylog" => "/admin/z_usersactivitylog",
            "login logs" or "loginlogs" or "loginslog" => "/admin/loginslog",
            _ => null
        };

        if (route != null) return route;

        // DB-First Dynamic Approach: If dbPath is already a valid-looking Blazor route, use it.
        if (!string.IsNullOrWhiteSpace(dbPath) && (dbPath.StartsWith("/") || dbPath.Contains("/")))
        {
            var formatted = FormatHref(dbPath);
            if (!string.IsNullOrWhiteSpace(formatted) && formatted != "#")
            {
                return formatted.TrimEnd('/');
            }
        }

        return FormatHref(dbPath).TrimEnd('/');
    }

    private string FormatHref(string? path)
    {
        if (string.IsNullOrEmpty(path) || path == "#") return "";
        var href = path.Replace(".aspx", "").Replace(".razor", "").Replace("~", "").ToLower();
        if (!href.StartsWith("/")) href = "/" + href;
        if (href.Contains("admin/dashboard")) return "/admin";
        if (href == "/default") return "/admin";
        return href;
    }

    public void SetUser(LoginResultDto? user)
    {
        CurrentUser = user;
        _lastRefreshTime = DateTime.UtcNow;
    }

    public void Clear()
    {
        CurrentUser = null;
    }

    public async Task InitializeAsync(IJSRuntime jsRuntime)
    {
        try
        {
            var json = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "currentUser");
            if (!string.IsNullOrWhiteSpace(json))
            {
                CurrentUser = JsonSerializer.Deserialize<LoginResultDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                _lastRefreshTime = DateTime.UtcNow;
            }

            var dateFormat = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "DateFormat");
            if (!string.IsNullOrWhiteSpace(dateFormat)) DateFormat = dateFormat;

            var defaultCurrency = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "DefaultCurrency");
            if (!string.IsNullOrWhiteSpace(defaultCurrency)) DefaultCurrency = defaultCurrency;

            var nameFormat = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "NameFormat");
            if (!string.IsNullOrWhiteSpace(nameFormat)) NameFormat = nameFormat;

            var isDarkStr = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "isDarkMode");
            if (bool.TryParse(isDarkStr, out var dark))
            {
                IsDarkMode = dark;
                if (dark)
                {
                    await jsRuntime.InvokeVoidAsync("document.body.classList.add", "dark-theme");
                    await jsRuntime.InvokeVoidAsync("document.documentElement.classList.add", "dark-style");
                    await jsRuntime.InvokeVoidAsync("document.documentElement.classList.remove", "light-style");
                }
                else
                {
                    await jsRuntime.InvokeVoidAsync("document.body.classList.remove", "dark-theme");
                    await jsRuntime.InvokeVoidAsync("document.documentElement.classList.remove", "dark-style");
                    await jsRuntime.InvokeVoidAsync("document.documentElement.classList.add", "light-style");
                }
            }
        }
        catch
        {
            // Ignore deserialization errors
        }
    }

    public async Task UpdateUserAsync(IJSRuntime jsRuntime, LoginResultDto? user)
    {
        CurrentUser = user;
        _lastRefreshTime = DateTime.UtcNow;
        if (user is null)
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
            IsDarkMode = false;
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "isDarkMode");
            await jsRuntime.InvokeVoidAsync("document.body.classList.remove", "dark-theme");
            await jsRuntime.InvokeVoidAsync("document.documentElement.classList.remove", "dark-style");
            await jsRuntime.InvokeVoidAsync("document.documentElement.classList.add", "light-style");
        }
        else
        {
            var json = JsonSerializer.Serialize(user);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", json);
        }
        OnChange?.Invoke();
    }

    public async Task UpdateLanguageAsync(IJSRuntime jsRuntime, long languageId, string? cultureName)
    {
        if (CurrentUser is null)
        {
            return;
        }

        CurrentUser.LanguageId = languageId;
        CurrentUser.CultureName = cultureName;

        var json = JsonSerializer.Serialize(CurrentUser);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", json);
        OnChange?.Invoke();
    }

    public async Task ToggleThemeAsync(IJSRuntime jsRuntime)
    {
        IsDarkMode = !IsDarkMode;
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "isDarkMode", IsDarkMode.ToString());
        
        if (IsDarkMode)
        {
            await jsRuntime.InvokeVoidAsync("document.body.classList.add", "dark-theme");
            await jsRuntime.InvokeVoidAsync("document.documentElement.classList.add", "dark-style");
            await jsRuntime.InvokeVoidAsync("document.documentElement.classList.remove", "light-style");
        }
        else
        {
            await jsRuntime.InvokeVoidAsync("document.body.classList.remove", "dark-theme");
            await jsRuntime.InvokeVoidAsync("document.documentElement.classList.remove", "dark-style");
            await jsRuntime.InvokeVoidAsync("document.documentElement.classList.add", "light-style");
        }

        OnChange?.Invoke();
    }

    public async Task RefreshTokenIfNeededAsync(HttpClient httpClient)
    {
        if (CurrentUser == null || _isRefreshing) return;
        if (DateTime.UtcNow - _lastRefreshTime < TimeSpan.FromMinutes(15)) return;

        _isRefreshing = true;
        try
        {
            var url = $"api/admin/security/refresh-session?loginId={CurrentUser.LoginId}&roleId={CurrentUser.RoleId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", CurrentUser.Token);
            
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync();
                var envelope = JsonSerializer.Deserialize<GFI_Upgrated.SharedDto.Common.ApiEnvelope<LoginResultDto>>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (envelope is { Success: true, Data: not null })
                 {
                     await UpdateUserAsync(_jsRuntime, envelope.Data);
                     _lastRefreshTime = DateTime.UtcNow;
                 }
            }
        }
        catch
        {
            // Ignore token refresh call errors to prevent UI disruption
        }
        finally
        {
            _isRefreshing = false;
        }
    }
}
