using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IWarehouseRepository
{
    Task<PagedResult<WarehouseDto>> GetWarehousesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<WarehouseDto?> GetWarehouseByIdAsync(long warehouseId, CancellationToken cancellationToken = default);
    Task<int> SaveWarehouseAsync(SaveWarehouseRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteWarehouseAsync(long warehouseId, string updatedBy, CancellationToken cancellationToken = default);
}
