using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IDashboardRepository
{
    Task<StockDashboardDto> GetStockDashboardAsync(CancellationToken cancellationToken = default);
    Task<ProductionDashboardDto> GetProductionDashboardAsync(string batchNo, CancellationToken cancellationToken = default);
    Task<List<DashboardBatchLookupDto>> GetProductionBatchesAsync(CancellationToken cancellationToken = default);
    Task<SalesDashboardDto> GetSalesDashboardAsync(CancellationToken cancellationToken = default);
}
