using System.Text.Json;
using GFI_Upgrated.SharedDto.AdminSecurity;
using GFI_Upgrated.UI.Services;
using Microsoft.JSInterop;

namespace GFI_Upgrated.UI.State;

public sealed class LocalizationState
{
    private const string DictionaryStorageKey = "gfi.localization.dictionary";
    private const string LanguageStorageKey = "gfi.localization.language";

    private Dictionary<string, string> _entries = new(StringComparer.OrdinalIgnoreCase);

    public event Action? OnChange;

    public IReadOnlyDictionary<string, string> Entries => _entries;
    public IReadOnlyList<LanguageDto> Languages { get; private set; } = Array.Empty<LanguageDto>();
    public long CurrentLanguageId { get; private set; }
    public string? CurrentCultureName { get; private set; }

    public string Text(string key, string fallback)
    {
        if (!string.IsNullOrWhiteSpace(key) && _entries.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return fallback;
    }

    public string Format(string key, string fallback, params object[] args)
    {
        var template = Text(key, fallback);
        return args.Length == 0 ? template : string.Format(template, args);
    }

    public async Task InitializeAsync(AppSessionState sessionState, AdminSecurityApiClient apiClient, IJSRuntime jsRuntime)
    {
        await LoadLanguagesAsync(apiClient);

        try
        {
            var cachedLanguage = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", LanguageStorageKey);
            var cachedDictionary = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", DictionaryStorageKey);

            if (long.TryParse(cachedLanguage, out var languageId) && !string.IsNullOrWhiteSpace(cachedDictionary))
            {
                var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(cachedDictionary, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed is not null)
                {
                    _entries = new Dictionary<string, string>(parsed, StringComparer.OrdinalIgnoreCase);
                    CurrentLanguageId = languageId;
                    CurrentCultureName = sessionState.CurrentUser?.CultureName;
                }
            }
        }
        catch
        {
            // Ignore cache issues and fall through to API load.
        }

        if (_entries.Count == 0 || (sessionState.CurrentUser?.LanguageId ?? 0) != CurrentLanguageId)
        {
            var preferredLanguageId = sessionState.CurrentUser?.LanguageId ?? 0;
            await LoadDictionaryAsync(apiClient, jsRuntime, preferredLanguageId);
        }

        OnChange?.Invoke();
    }

    public async Task LoadLanguagesAsync(AdminSecurityApiClient apiClient)
    {
        Languages = await apiClient.GetLanguagesAsync();
        OnChange?.Invoke();
    }

    public async Task SetLanguageAsync(AppSessionState sessionState, AdminSecurityApiClient apiClient, IJSRuntime jsRuntime, long languageId)
    {
        if (languageId <= 0)
        {
            return;
        }

        if (sessionState.CurrentUser is not null)
        {
            await apiClient.SaveUserLanguagePreferenceAsync(new SaveUserLanguagePreferenceRequest
            {
                LoginId = sessionState.CurrentUser.LoginId,
                LanguageId = languageId
            });
        }

        await LoadDictionaryAsync(apiClient, jsRuntime, languageId);

        var selected = Languages.FirstOrDefault(x => x.Id == languageId);
        await sessionState.UpdateLanguageAsync(jsRuntime, languageId, selected?.CultureName);
        OnChange?.Invoke();
    }

    public async Task ClearAsync(IJSRuntime jsRuntime)
    {
        _entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Languages = Array.Empty<LanguageDto>();
        CurrentLanguageId = 0;
        CurrentCultureName = null;

        await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", DictionaryStorageKey);
        await jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", LanguageStorageKey);
        OnChange?.Invoke();
    }

    private async Task LoadDictionaryAsync(AdminSecurityApiClient apiClient, IJSRuntime jsRuntime, long languageId)
    {
        var dictionary = await apiClient.GetLocalizedDictionaryAsync(languageId);
        if (dictionary is null)
        {
            return;
        }

        _entries = new Dictionary<string, string>(dictionary.Entries, StringComparer.OrdinalIgnoreCase);
        CurrentLanguageId = dictionary.LanguageId;
        CurrentCultureName = dictionary.CultureName;

        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", LanguageStorageKey, CurrentLanguageId.ToString());
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", DictionaryStorageKey, JsonSerializer.Serialize(_entries));
    }
}
