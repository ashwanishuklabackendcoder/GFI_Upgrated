using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface ISkuRepository
{
    Task<PagedResult<SkuDto>> GetSkusAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<SkuDto?> GetSkuByIdAsync(long skuId, CancellationToken cancellationToken = default);
    Task<int> SaveSkuAsync(SaveSkuRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteSkuAsync(long skuId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnitLookupDto>> GetUnitsLookupAsync(CancellationToken cancellationToken = default);
}
