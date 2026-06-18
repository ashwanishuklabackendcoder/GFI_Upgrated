using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IPreProcessingRepository
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
