using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IKettleRepository
{
    Task<PagedResult<KettleDto>> GetKettlesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<KettleDto?> GetKettleByIdAsync(long kettleId, CancellationToken cancellationToken = default);
    Task<int> SaveKettleAsync(SaveKettleRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteKettleAsync(long kettleId, string updatedBy, CancellationToken cancellationToken = default);
}
