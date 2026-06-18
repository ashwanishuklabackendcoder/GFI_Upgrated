using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class AlmirahDto
{
    public long AlmirahShelfID { get; set; }
    public string AlmirahShelfName { get; set; } = string.Empty;
    public int? NoShelf { get; set; }
    public long WarehouseID { get; set; }
    public string? WarehouseName { get; set; }
    public string? AlmirahLocation { get; set; }
    public string? Description { get; set; }
    public long? ParentId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveAlmirahRequest
{
    public long AlmirahShelfID { get; set; }

    [Required(ErrorMessage = "Almirah Name is required.")]
    [StringLength(200, ErrorMessage = "Almirah Name cannot exceed 200 characters.")]
    public string AlmirahShelfName { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Number of shelves must be between 0 and 100.")]
    public int NoShelf { get; set; }

    [Required(ErrorMessage = "Warehouse is required.")]
    public long WarehouseID { get; set; }

    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
    public string? AlmirahLocation { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    public long? ParentId { get; set; }
    public string UpdatedBy { get; set; } = "System";
}
