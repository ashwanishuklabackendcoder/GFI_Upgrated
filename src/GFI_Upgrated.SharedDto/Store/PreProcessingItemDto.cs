using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class PreProcessingItemDto
{
    public long ItemStockUsedID { get; set; }
    public long UsedForId { get; set; } // PreProcessingId
    public long ItemStockByBatchId { get; set; }
    public long ItemID { get; set; }
    public string? ItemName { get; set; }
    public string? BatchNo { get; set; }
    public double Quantity { get; set; }
    public string? Description { get; set; }
}

public sealed class SavePreProcessingItemRequest
{
    public long ItemStockUsedID { get; set; }

    [Required]
    public long UsedForId { get; set; } // PreProcessingId

    [Required]
    public long ItemStockByBatchId { get; set; }

    [Required]
    public long ItemID { get; set; }

    public int UsedFor { get; set; } = 2; // 2 = Pre-Processing

    [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public double Quantity { get; set; }

    public string? Description { get; set; }
    public string UpdatedBy { get; set; } = "System";
}

public sealed class BomItemDetailDto
{
    public long BomId { get; set; }
    public string? BomName { get; set; }
    public long ItemID { get; set; }
    public string? ItemName { get; set; }
    public double Quantity { get; set; } // Quantity per unit of BOM
    public long UnitId { get; set; }
}

public sealed class BomLookupDto
{
    public long BomId { get; set; }
    public string? BomName { get; set; }
    public long ItemTypeId { get; set; }
}

public sealed class WarehouseLookupDto
{
    public long WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
}

public sealed class AvailableBatchDto
{
    public long ItemStockByBatchId { get; set; }
    public string? BatchNo { get; set; }
    public double FinalQuantityLeft { get; set; }
    public string? ExpiryDate { get; set; }
    public string? WarehouseName { get; set; }
}
