namespace GFI_Upgrated.SharedDto.Store;

public sealed class ItemStockReportDto
{
    public long StockID { get; set; }
    public long ItemID { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public double OpeningQuantity { get; set; }
    public double PurchasedQuantity { get; set; }
    public double IssuedQuantity { get; set; }
    public double RemovedQuantity { get; set; }
    public double FinalStock { get; set; }
}
