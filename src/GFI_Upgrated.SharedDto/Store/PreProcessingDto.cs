using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class PreProcessingDto
{
    public long PreProcessingId { get; set; }
    public long BomId { get; set; }
    public string? BomName { get; set; }
    public int BomQty { get; set; }
    public string? ProcessingDate { get; set; } // SP returns as string formatted 106
    public double QuantityMade { get; set; }
    public long UnitMade { get; set; }
    public string? UnitName { get; set; }
    public string? BatchNumberMade { get; set; }
    public string? ExpiryDate { get; set; } // SP returns as string formatted 106
    public string? ProcessEmployees { get; set; }
    public string? Remarks { get; set; }
    public string? DocumentUpload { get; set; }
    public long WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? ItemName { get; set; } // Final Product Name
    public int IsComplete { get; set; } // 0 or 1 from SP
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SavePreProcessingRequest
{
    public long PreProcessingId { get; set; }

    [Required(ErrorMessage = "BOM is required")]
    public long BomId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "BOM Quantity must be at least 1")]
    public int BomQty { get; set; }

    [Required(ErrorMessage = "Processing Date is required")]
    public DateTime? ProcessingDate { get; set; }

    public double QuantityMade { get; set; }
    public long UnitMade { get; set; }
    public string? BatchNumberMade { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? ProcessEmployees { get; set; }
    public string? Remarks { get; set; }
    public string? DocumentUpload { get; set; }
    public long WarehouseId { get; set; }
    public string UpdatedBy { get; set; } = "System";
}
