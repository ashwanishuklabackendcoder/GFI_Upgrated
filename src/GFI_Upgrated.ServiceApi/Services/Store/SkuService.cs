using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface ISkuService
{
    Task<PagedResult<SkuDto>> GetSkusAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<SkuDto?> GetSkuByIdAsync(long skuId, CancellationToken cancellationToken = default);
    Task<int> SaveSkuAsync(SaveSkuRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteSkuAsync(long skuId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnitLookupDto>> GetUnitsLookupAsync(CancellationToken cancellationToken = default);
}

public sealed class SkuService : ISkuService
{
    private readonly ISkuRepository _repository;

    public SkuService(ISkuRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<SkuDto>> GetSkusAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetSkusAsync(request, searchText, cancellationToken);

    public Task<SkuDto?> GetSkuByIdAsync(long skuId, CancellationToken cancellationToken = default)
        => _repository.GetSkuByIdAsync(skuId, cancellationToken);

    public Task<int> SaveSkuAsync(SaveSkuRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveSkuAsync(request, cancellationToken);

    public Task<int> DeleteSkuAsync(long skuId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteSkuAsync(skuId, updatedBy, cancellationToken);

    public Task<IReadOnlyList<UnitLookupDto>> GetUnitsLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetUnitsLookupAsync(cancellationToken);
}
