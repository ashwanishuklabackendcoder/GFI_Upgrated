using System.ComponentModel.DataAnnotations;
using GFI_Upgrated.SharedDto.Common;

namespace GFI_Upgrated.SharedDto.AdminSecurity;

public sealed class LanguageDto
{
    public long Id { get; set; }
    public string CultureName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool IsDefaultLanguage { get; set; }
}

public sealed class SaveLanguageRequest : IValidatableObject
{
    public long Id { get; set; }

    [Required]
    public string CultureName { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    public string Region { get; set; } = string.Empty;

    public bool IsDefaultLanguage { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(CultureName))
        {
            yield return new ValidationResult("Culture name is required.", new[] { nameof(CultureName) });
        }

        if (string.IsNullOrWhiteSpace(DisplayName))
        {
            yield return new ValidationResult("Display name is required.", new[] { nameof(DisplayName) });
        }
    }
}

public sealed class LocalizedResourceDto
{
    public long ResourceId { get; set; }
    public long LanguageId { get; set; }
    public string CultureName { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string DefaultValue { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool IsMissing { get; set; }
}

public sealed class SaveLocalizedResourceRequest : IValidatableObject
{
    public long ResourceId { get; set; }
    public long LanguageId { get; set; }

    [Required]
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
    public string? Comment { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LanguageId <= 0)
        {
            yield return new ValidationResult("Language is required.", new[] { nameof(LanguageId) });
        }

        if (string.IsNullOrWhiteSpace(Key))
        {
            yield return new ValidationResult("Resource key is required.", new[] { nameof(Key) });
        }
    }
}

public sealed class LocalizedResourceListRequest
{
    public int CurrentPage { get; set; } = 1;
    public int RecordPerPage { get; set; } = 25;
    public string? SortColumn { get; set; }
    public string? SortType { get; set; }
    public long LanguageId { get; set; }
    public string? SearchText { get; set; }
    public bool ShowMissingOnly { get; set; }
}

public sealed class UserLanguagePreferenceDto
{
    public long LoginId { get; set; }
    public long LanguageId { get; set; }
    public string CultureName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public sealed class SaveUserLanguagePreferenceRequest
{
    public long LoginId { get; set; }
    public long LanguageId { get; set; }
}

public sealed class LocalizedDictionaryDto
{
    public long LanguageId { get; set; }
    public string CultureName { get; set; } = string.Empty;
    public long DefaultLanguageId { get; set; }
    public Dictionary<string, string> Entries { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
