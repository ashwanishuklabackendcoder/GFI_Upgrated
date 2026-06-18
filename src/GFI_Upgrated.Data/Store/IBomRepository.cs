using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.Data.Store;

public interface IBomRepository
{
    Task<PagedResult<BomDto>> GetBomsAsync(BomListRequest request, CancellationToken cancellationToken = default);
    Task<BomDto?> GetBomByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BomItemDto>> GetBomItemsAsync(long bomId, CancellationToken cancellationToken = default);
    Task<int> SaveBomAsync(SaveBomRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteBomAsync(long id, string deletedBy, CancellationToken cancellationToken = default);
    
    // Lookups
    Task<IReadOnlyList<RawMaterialDto>> GetItemsForBomLookupAsync(int? itemTypeId = null, CancellationToken cancellationToken = default);
}
