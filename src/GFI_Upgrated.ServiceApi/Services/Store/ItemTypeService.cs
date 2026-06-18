using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IItemTypeService
{
    Task<PagedResult<ItemTypeDto>> GetItemTypesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<ItemTypeDto?> GetItemTypeByIdAsync(long itemTypeId, CancellationToken cancellationToken = default);
    Task<int> SaveItemTypeAsync(SaveItemTypeRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteItemTypeAsync(long itemTypeId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ParentTypeLookupDto>> GetParentTypesLookupAsync(CancellationToken cancellationToken = default);
}

public sealed class ItemTypeService : IItemTypeService
{
    private readonly IItemTypeRepository _repository;

    public ItemTypeService(IItemTypeRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<ItemTypeDto>> GetItemTypesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetItemTypesAsync(request, searchText, cancellationToken);

    public Task<ItemTypeDto?> GetItemTypeByIdAsync(long itemTypeId, CancellationToken cancellationToken = default)
        => _repository.GetItemTypeByIdAsync(itemTypeId, cancellationToken);

    public Task<int> SaveItemTypeAsync(SaveItemTypeRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveItemTypeAsync(request, cancellationToken);

    public Task<int> DeleteItemTypeAsync(long itemTypeId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteItemTypeAsync(itemTypeId, updatedBy, cancellationToken);

    public Task<IReadOnlyList<ParentTypeLookupDto>> GetParentTypesLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetParentTypesLookupAsync(cancellationToken);
}
