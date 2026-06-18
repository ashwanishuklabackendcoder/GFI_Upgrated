using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IReportRepository
{
    Task<PagedResult<ItemStockReportDto>> GetItemStockReportAsync(ItemStockReportRequest request, CancellationToken cancellationToken = default);
    
    Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByBatchNoAsync(string batchNo, int page, int size, string sortType, CancellationToken cancellationToken = default);
    
    Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByItemAsync(long itemId, int page, int size, string sortType, CancellationToken cancellationToken = default);
    
    Task<PagedResult<ItemStockByBatchReportDto>> GetItemStockByBatchReportAsync(long? itemStockByBatchId, long? stockById, long? itemId, int page, int size, string sortCol, string sortOrd, CancellationToken cancellationToken = default);
}
