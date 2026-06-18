using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IWarehouseService
{
    Task<PagedResult<WarehouseDto>> GetWarehousesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<WarehouseDto?> GetWarehouseByIdAsync(long warehouseId, CancellationToken cancellationToken = default);
    Task<int> SaveWarehouseAsync(SaveWarehouseRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteWarehouseAsync(long warehouseId, string updatedBy, CancellationToken cancellationToken = default);
}

public sealed class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _repository;

    public WarehouseService(IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<WarehouseDto>> GetWarehousesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetWarehousesAsync(request, searchText, cancellationToken);

    public Task<WarehouseDto?> GetWarehouseByIdAsync(long warehouseId, CancellationToken cancellationToken = default)
        => _repository.GetWarehouseByIdAsync(warehouseId, cancellationToken);

    public Task<int> SaveWarehouseAsync(SaveWarehouseRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveWarehouseAsync(request, cancellationToken);

    public Task<int> DeleteWarehouseAsync(long warehouseId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteWarehouseAsync(warehouseId, updatedBy, cancellationToken);
}
