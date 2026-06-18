using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class SkuDto
{
    public long SkuId { get; set; }
    public string SkuName { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public long UnitId { get; set; }
    public string? UnitName { get; set; }
    public double QuantityPerColli { get; set; }
    public bool IsActive { get; set; }
    public string? Remarks { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveSkuRequest : IValidatableObject
{
    public long SkuId { get; set; }

    [Required(ErrorMessage = "SKU Name is required.")]
    [StringLength(100, ErrorMessage = "SKU Name cannot exceed 100 characters.")]
    public string SkuName { get; set; } = string.Empty;

    [Range(1, long.MaxValue, ErrorMessage = "Please select a valid Unit.")]
    public long UnitId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be non-negative.")]
    public double Quantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Quantity Per Colli must be non-negative.")]
    public double QuantityPerColli { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
    public string? Remarks { get; set; }

    public string UpdatedBy { get; set; } = "System";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(SkuName))
        {
            yield return new ValidationResult("SKU Name is required.", new[] { nameof(SkuName) });
        }
    }
}

public sealed class UnitLookupDto
{
    public long UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
}
