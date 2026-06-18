namespace GFI_Upgrated.SharedDto.Store;

public sealed class ItemStockReportRequest
{
    public long? StockID { get; set; }
    public string? CreatedBy { get; set; }
    public long? ItemID { get; set; }
    public long? WarehouseID { get; set; }
    public long? UnitId { get; set; }
    
    public int CurrentPage { get; set; } = 1;
    public int RecordPerPage { get; set; } = 25;
    public string SortColumn { get; set; } = "StockID";
    public string SortType { get; set; } = "DESC";
}
