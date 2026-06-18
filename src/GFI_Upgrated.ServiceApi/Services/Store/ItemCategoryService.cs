using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IItemCategoryService
{
    Task<PagedResult<ItemCategoryDto>> GetItemCategoriesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<ItemCategoryDto?> GetItemCategoryByIdAsync(long itemCatId, CancellationToken cancellationToken = default);
    Task<int> SaveItemCategoryAsync(SaveItemCategoryRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteItemCategoryAsync(long itemCatId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ParentCategoryLookupDto>> GetParentCategoriesLookupAsync(CancellationToken cancellationToken = default);
}

public sealed class ItemCategoryService : IItemCategoryService
{
    private readonly IItemCategoryRepository _repository;

    public ItemCategoryService(IItemCategoryRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<ItemCategoryDto>> GetItemCategoriesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetItemCategoriesAsync(request, searchText, cancellationToken);

    public Task<ItemCategoryDto?> GetItemCategoryByIdAsync(long itemCatId, CancellationToken cancellationToken = default)
        => _repository.GetItemCategoryByIdAsync(itemCatId, cancellationToken);

    public Task<int> SaveItemCategoryAsync(SaveItemCategoryRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveItemCategoryAsync(request, cancellationToken);

    public Task<int> DeleteItemCategoryAsync(long itemCatId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteItemCategoryAsync(itemCatId, updatedBy, cancellationToken);

    public Task<IReadOnlyList<ParentCategoryLookupDto>> GetParentCategoriesLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetParentCategoriesLookupAsync(cancellationToken);
}
