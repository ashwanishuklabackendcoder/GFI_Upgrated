using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class BrandDto
{
    public long BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? Remarks { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveBrandRequest : IValidatableObject
{
    public long BrandId { get; set; }

    [Required(ErrorMessage = "Brand Name is required.")]
    [StringLength(400, ErrorMessage = "Brand Name cannot exceed 400 characters.")]
    public string BrandName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
    public string? Remarks { get; set; }

    public string UpdatedBy { get; set; } = "System";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(BrandName))
        {
            yield return new ValidationResult("Brand Name is required.", new[] { nameof(BrandName) });
        }
    }
}
