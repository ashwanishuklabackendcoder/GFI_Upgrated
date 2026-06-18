using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class WarehouseDto
{
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveWarehouseRequest
{
    public long WarehouseId { get; set; }

    [Required(ErrorMessage = "Warehouse Name is required.")]
    [StringLength(300, ErrorMessage = "Warehouse Name cannot exceed 300 characters.")]
    public string WarehouseName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    public string UpdatedBy { get; set; } = "System";
}
