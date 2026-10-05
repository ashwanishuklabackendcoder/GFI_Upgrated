namespace GFI_Upgrated.SharedDto.AdminSecurity;

public sealed class GeneralSettingItemDto
{
    public string ConfigKey { get; set; } = string.Empty;
    public string ConfigValue { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class GeneralSettingsDto
{
    public string CompanyName { get; set; } = "GFI Nuvotrace";
    public string CompanyAddress { get; set; } = string.Empty;
    public string CompanyLogo { get; set; } = "/assets/img/branding/nuvotrace-horizontal-logo.png";
    public string TopbarLogo { get; set; } = "/assets/img/branding/nuvotrace-horizontal-logo.png";
    public string LoginPageLogo { get; set; } = "/assets/img/branding/nuvotrace-custom-logo.png";
    public string DefaultCurrency { get; set; } = "SRD";
    public string DateFormat { get; set; } = "MM/DD/YYYY";
    public int DecimalDigits { get; set; } = 2;
    public Dictionary<string, string> CustomSettings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class UpdateGeneralSettingsRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CompanyLogo { get; set; } = string.Empty;
    public string TopbarLogo { get; set; } = string.Empty;
    public string LoginPageLogo { get; set; } = string.Empty;
    public string DefaultCurrency { get; set; } = "SRD";
    public string DateFormat { get; set; } = "MM/DD/YYYY";
    public int DecimalDigits { get; set; } = 2;
    public Dictionary<string, string>? AdditionalSettings { get; set; }
}
