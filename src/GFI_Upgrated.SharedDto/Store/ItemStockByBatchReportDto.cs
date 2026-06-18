namespace GFI_Upgrated.SharedDto.Store;

public sealed class ItemStockByBatchReportDto
{
    public long ItemStockByBatchId { get; set; }
    public int StockById { get; set; }
    public long IdFrom { get; set; }
    public long ItemId { get; set; }
    public double Quantity { get; set; }
    public int Unit { get; set; }
    public string? BatchNo { get; set; }
    public string? ExpiryDate { get; set; }
    public long WarehouseId { get; set; }
    public double FinalQuantityLeft { get; set; }
    
    public string? ItemName { get; set; }
    public string? UnitName { get; set; }
    public string? WarehouseName { get; set; }
}
