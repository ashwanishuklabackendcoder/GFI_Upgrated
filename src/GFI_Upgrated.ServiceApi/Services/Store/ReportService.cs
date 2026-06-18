using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IReportService
{
    Task<PagedResult<ItemStockReportDto>> GetItemStockReportAsync(ItemStockReportRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByBatchNoAsync(string batchNo, int page, int size, string sortType, CancellationToken cancellationToken = default);
    Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByItemAsync(long itemId, int page, int size, string sortType, CancellationToken cancellationToken = default);
    Task<PagedResult<ItemStockByBatchReportDto>> GetItemStockByBatchReportAsync(long? itemStockByBatchId, long? stockById, long? itemId, int page, int size, string sortCol, string sortOrd, CancellationToken cancellationToken = default);
}

public sealed class ReportService : IReportService
{
    private readonly IReportRepository _repository;

    public ReportService(IReportRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<ItemStockReportDto>> GetItemStockReportAsync(ItemStockReportRequest request, CancellationToken cancellationToken = default)
        => _repository.GetItemStockReportAsync(request, cancellationToken);

    public Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByBatchNoAsync(string batchNo, int page, int size, string sortType, CancellationToken cancellationToken = default)
        => _repository.GetBatchWiseItemsByBatchNoAsync(batchNo, page, size, sortType, cancellationToken);

    public Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByItemAsync(long itemId, int page, int size, string sortType, CancellationToken cancellationToken = default)
        => _repository.GetBatchWiseItemsByItemAsync(itemId, page, size, sortType, cancellationToken);

    public Task<PagedResult<ItemStockByBatchReportDto>> GetItemStockByBatchReportAsync(long? itemStockByBatchId, long? stockById, long? itemId, int page, int size, string sortCol, string sortOrd, CancellationToken cancellationToken = default)
        => _repository.GetItemStockByBatchReportAsync(itemStockByBatchId, stockById, itemId, page, size, sortCol, sortOrd, cancellationToken);
}
