using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IPreProcessingService
{
    Task<PagedResult<PreProcessingDto>> GetPreProcessingListAsync(PreProcessingListRequest request, CancellationToken cancellationToken = default);
    Task<PreProcessingDto?> GetPreProcessingByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> SavePreProcessingAsync(SavePreProcessingRequest request, CancellationToken cancellationToken = default);
    Task<int> DeletePreProcessingAsync(long id, string updatedBy, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<PreProcessingItemDto>> GetPreProcessingItemsAsync(long preProcessingId, CancellationToken cancellationToken = default);
    Task<int> SavePreProcessingItemAsync(SavePreProcessingItemRequest request, CancellationToken cancellationToken = default);
    Task<int> DeletePreProcessingItemAsync(long itemStockUsedId, CancellationToken cancellationToken = default);
    Task<int> FinalizeStockUpdateAsync(long preProcessingId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BomItemDetailDto>> GetBomItemsAsync(long bomId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<BomLookupDto>> GetBomsLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WarehouseLookupDto>> GetWarehousesLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AvailableBatchDto>> GetAvailableBatchesAsync(long itemId, CancellationToken cancellationToken = default);
}

public sealed class PreProcessingService : IPreProcessingService
{
    private readonly IPreProcessingRepository _repository;

    public PreProcessingService(IPreProcessingRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<PreProcessingDto>> GetPreProcessingListAsync(PreProcessingListRequest request, CancellationToken cancellationToken = default)
        => _repository.GetPreProcessingListAsync(request, cancellationToken);

    public Task<PreProcessingDto?> GetPreProcessingByIdAsync(long id, CancellationToken cancellationToken = default)
        => _repository.GetPreProcessingByIdAsync(id, cancellationToken);

    public Task<int> SavePreProcessingAsync(SavePreProcessingRequest request, CancellationToken cancellationToken = default)
        => _repository.SavePreProcessingAsync(request, cancellationToken);

    public Task<int> DeletePreProcessingAsync(long id, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeletePreProcessingAsync(id, updatedBy, cancellationToken);

    public Task<IReadOnlyList<PreProcessingItemDto>> GetPreProcessingItemsAsync(long preProcessingId, CancellationToken cancellationToken = default)
        => _repository.GetPreProcessingItemsAsync(preProcessingId, cancellationToken);

    public Task<int> SavePreProcessingItemAsync(SavePreProcessingItemRequest request, CancellationToken cancellationToken = default)
        => _repository.SavePreProcessingItemAsync(request, cancellationToken);

    public Task<int> DeletePreProcessingItemAsync(long itemStockUsedId, CancellationToken cancellationToken = default)
        => _repository.DeletePreProcessingItemAsync(itemStockUsedId, cancellationToken);

    public Task<int> FinalizeStockUpdateAsync(long preProcessingId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.FinalizeStockUpdateAsync(preProcessingId, updatedBy, cancellationToken);

    public Task<IReadOnlyList<BomItemDetailDto>> GetBomItemsAsync(long bomId, CancellationToken cancellationToken = default)
        => _repository.GetBomItemsAsync(bomId, cancellationToken);

    public Task<IReadOnlyList<BomLookupDto>> GetBomsLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetBomsLookupAsync(cancellationToken);

    public Task<IReadOnlyList<WarehouseLookupDto>> GetWarehousesLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetWarehousesLookupAsync(cancellationToken);

    public Task<IReadOnlyList<AvailableBatchDto>> GetAvailableBatchesAsync(long itemId, CancellationToken cancellationToken = default)
        => _repository.GetAvailableBatchesAsync(itemId, cancellationToken);
}
