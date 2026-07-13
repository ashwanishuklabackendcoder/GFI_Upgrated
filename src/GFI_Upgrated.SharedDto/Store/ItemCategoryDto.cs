using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class ItemCategoryDto
{
    public long ItemCatId { get; set; }
    public string ItemCatName { get; set; } = string.Empty;
    public bool IsMainCategory { get; set; }
    public long? MainCategoryId { get; set; }
    public string? ParentItemCatName { get; set; }
    public bool IsActive { get; set; }
    public bool IsEditable { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveItemCategoryRequest : IValidatableObject
{
    public long ItemCatId { get; set; }

    [Required(ErrorMessage = "Category Name is required.")]
    [StringLength(200, ErrorMessage = "Category Name cannot exceed 200 characters.")]
    public string ItemCatName { get; set; } = string.Empty;

    public bool IsMainCategory { get; set; }
    public long? MainCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public string UpdatedBy { get; set; } = "System";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsMainCategory && !MainCategoryId.HasValue)
        {
            yield return new ValidationResult("Parent category is required when 'Is Main Category' is unchecked.", new[] { nameof(MainCategoryId) });
        }
    }
}

public sealed class ParentCategoryLookupDto
{
    public long ItemCatId { get; set; }
    public string ItemCatName { get; set; } = string.Empty;
}
