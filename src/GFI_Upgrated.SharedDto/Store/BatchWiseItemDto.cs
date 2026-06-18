namespace GFI_Upgrated.SharedDto.Store;

public sealed class BatchWiseItemDto
{
    public long Id { get; set; }
    public string? BatchNo { get; set; }
    public string? ProcessingDate { get; set; }
    public string? ExpiryDate { get; set; }
    public string? ItemName { get; set; }
    public string? AccountName { get; set; }
}
