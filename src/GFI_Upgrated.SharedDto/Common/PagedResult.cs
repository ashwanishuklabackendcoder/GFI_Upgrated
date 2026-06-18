namespace GFI_Upgrated.SharedDto.Common;

public sealed class PagedResult<T>
{
    public int CurrentPage { get; set; }
    public int TotalRecord { get; set; }
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
}

