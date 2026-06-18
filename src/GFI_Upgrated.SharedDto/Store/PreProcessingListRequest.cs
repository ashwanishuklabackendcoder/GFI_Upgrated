namespace GFI_Upgrated.SharedDto.Store;

public sealed class PreProcessingListRequest
{
    public int CurrentPage { get; set; } = 1;
    public int RecordPerPage { get; set; } = 25;
    public string SortColumn { get; set; } = "PreProcessingId";
    public string SortType { get; set; } = "DESC";
    
    public long? PreProcessingId { get; set; }
    public string? CreatedBy { get; set; }
    public long? BomId { get; set; }
}
