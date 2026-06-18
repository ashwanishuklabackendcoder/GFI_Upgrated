using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IItemTypeRepository
{
    Task<PagedResult<ItemTypeDto>> GetItemTypesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<ItemTypeDto?> GetItemTypeByIdAsync(long itemTypeId, CancellationToken cancellationToken = default);
    Task<int> SaveItemTypeAsync(SaveItemTypeRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteItemTypeAsync(long itemTypeId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ParentTypeLookupDto>> GetParentTypesLookupAsync(CancellationToken cancellationToken = default);
}
