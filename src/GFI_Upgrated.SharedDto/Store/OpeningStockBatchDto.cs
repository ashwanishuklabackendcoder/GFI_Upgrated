using System;

namespace GFI_Upgrated.SharedDto.Store;

public class OpeningStockBatchDto
{
    public long ItemStockByBatchId { get; set; }
    public string BatchNo { get; set; } = string.Empty;
    public long WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public double Amount { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
