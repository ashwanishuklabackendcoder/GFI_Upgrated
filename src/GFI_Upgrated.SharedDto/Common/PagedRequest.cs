namespace GFI_Upgrated.SharedDto.Common;

public sealed class PagedRequest
{
    public int CurrentPage { get; set; } = 1;
    public int RecordPerPage { get; set; } = 25;
    public string? SortColumn { get; set; }
    public string? SortType { get; set; }
}

