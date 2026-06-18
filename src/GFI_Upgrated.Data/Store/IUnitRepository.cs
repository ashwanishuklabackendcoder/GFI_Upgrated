using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IUnitRepository
{
    Task<PagedResult<UnitDto>> GetUnitsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<UnitDto?> GetUnitByIdAsync(long unitId, CancellationToken cancellationToken = default);
    Task<int> SaveUnitAsync(SaveUnitRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteUnitAsync(long unitId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BaseUnitLookupDto>> GetBaseUnitsLookupAsync(CancellationToken cancellationToken = default);
}
