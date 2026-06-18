using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IStatusRepository
{
    Task<PagedResult<StatusDto>> GetStatusesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<StatusDto?> GetStatusByIdAsync(long statusId, CancellationToken cancellationToken = default);
    Task<int> SaveStatusAsync(SaveStatusRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteStatusAsync(long statusId, string updatedBy, CancellationToken cancellationToken = default);
}
