using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class UnitDto
{
    public long UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public double? ConversionValue { get; set; }
    public long? BaseUnit { get; set; }
    public string? BaseName { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveUnitRequest
{
    public long UnitId { get; set; }

    [Required(ErrorMessage = "Unit Name is required.")]
    [StringLength(100, ErrorMessage = "Unit Name cannot exceed 100 characters.")]
    public string UnitName { get; set; } = string.Empty;

    public double? ConversionValue { get; set; }
    public long? BaseUnit { get; set; }
    public bool IsActive { get; set; } = true;
    public string UpdatedBy { get; set; } = "System";
}

public sealed class BaseUnitLookupDto
{
    public long UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
}
