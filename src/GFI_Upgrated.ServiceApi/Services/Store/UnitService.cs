using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IUnitService
{
    Task<PagedResult<UnitDto>> GetUnitsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<UnitDto?> GetUnitByIdAsync(long unitId, CancellationToken cancellationToken = default);
    Task<int> SaveUnitAsync(SaveUnitRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteUnitAsync(long unitId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BaseUnitLookupDto>> GetBaseUnitsLookupAsync(CancellationToken cancellationToken = default);
}

public sealed class UnitService : IUnitService
{
    private readonly IUnitRepository _repository;

    public UnitService(IUnitRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<UnitDto>> GetUnitsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetUnitsAsync(request, searchText, cancellationToken);

    public Task<UnitDto?> GetUnitByIdAsync(long unitId, CancellationToken cancellationToken = default)
        => _repository.GetUnitByIdAsync(unitId, cancellationToken);

    public Task<int> SaveUnitAsync(SaveUnitRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveUnitAsync(request, cancellationToken);

    public Task<int> DeleteUnitAsync(long unitId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteUnitAsync(unitId, updatedBy, cancellationToken);

    public Task<IReadOnlyList<BaseUnitLookupDto>> GetBaseUnitsLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetBaseUnitsLookupAsync(cancellationToken);
}
