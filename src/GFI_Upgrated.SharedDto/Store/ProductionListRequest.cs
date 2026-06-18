namespace GFI_Upgrated.SharedDto.Store;

public sealed class ProductionListRequest
{
    public int CurrentPage { get; set; } = 1;
    public int RecordPerPage { get; set; } = 25;
    public string SortColumn { get; set; } = "ProductionId";
    public string SortType { get; set; } = "DESC";
    
    public long? ProductionId { get; set; }
    public string? Remarks { get; set; }
    public long? BomId { get; set; }
    public long? SkuId { get; set; }
}
