using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IBomService
{
    Task<PagedResult<BomDto>> GetBomsAsync(BomListRequest request, CancellationToken cancellationToken = default);
    Task<BomDto?> GetBomByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BomItemDto>> GetBomItemsAsync(long bomId, CancellationToken cancellationToken = default);
    Task<int> SaveBomAsync(SaveBomRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteBomAsync(long id, string deletedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RawMaterialDto>> GetItemsForBomLookupAsync(int? itemTypeId = null, CancellationToken cancellationToken = default);
}
