using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class ItemTypeDto
{
    public long ItemTypeId { get; set; }
    public string ItemTypeName { get; set; } = string.Empty;
    public bool IsMainType { get; set; }
    public bool IsEditable { get; set; }
    public long? MainTypeId { get; set; }
    public string? ParentItemTypeName { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveItemTypeRequest
{
    public long ItemTypeId { get; set; }

    [Required(ErrorMessage = "Item Type Name is required.")]
    [StringLength(200, ErrorMessage = "Item Type Name cannot exceed 200 characters.")]
    public string ItemTypeName { get; set; } = string.Empty;

    public bool IsMainType { get; set; }
    public long? MainTypeId { get; set; }
    public bool IsActive { get; set; } = true;
    public string UpdatedBy { get; set; } = "System";
}

public sealed class ParentTypeLookupDto
{
    public long ItemTypeId { get; set; }
    public string ItemTypeName { get; set; } = string.Empty;
}
