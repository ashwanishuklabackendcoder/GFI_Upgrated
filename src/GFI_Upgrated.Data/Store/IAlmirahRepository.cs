using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IAlmirahRepository
{
    Task<PagedResult<AlmirahDto>> GetAlmirahsAsync(PagedRequest request, string? searchText, long? warehouseId, CancellationToken cancellationToken = default);
    Task<AlmirahDto?> GetAlmirahByIdAsync(long almirahId, CancellationToken cancellationToken = default);
    Task<int> SaveAlmirahAsync(SaveAlmirahRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteAlmirahAsync(long almirahId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AlmirahDto>> GetShelvesByAlmirahIdAsync(long almirahId, CancellationToken cancellationToken = default);
}
