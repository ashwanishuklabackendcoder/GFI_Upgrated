using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IItemCategoryRepository
{
    Task<PagedResult<ItemCategoryDto>> GetItemCategoriesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<ItemCategoryDto?> GetItemCategoryByIdAsync(long itemCatId, CancellationToken cancellationToken = default);
    Task<int> SaveItemCategoryAsync(SaveItemCategoryRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteItemCategoryAsync(long itemCatId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ParentCategoryLookupDto>> GetParentCategoriesLookupAsync(CancellationToken cancellationToken = default);
}
