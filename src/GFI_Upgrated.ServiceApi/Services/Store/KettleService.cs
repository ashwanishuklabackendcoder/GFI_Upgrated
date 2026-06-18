using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IKettleService
{
    Task<PagedResult<KettleDto>> GetKettlesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<KettleDto?> GetKettleByIdAsync(long kettleId, CancellationToken cancellationToken = default);
    Task<int> SaveKettleAsync(SaveKettleRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteKettleAsync(long kettleId, string updatedBy, CancellationToken cancellationToken = default);
}

public sealed class KettleService : IKettleService
{
    private readonly IKettleRepository _repository;

    public KettleService(IKettleRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<KettleDto>> GetKettlesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetKettlesAsync(request, searchText, cancellationToken);

    public Task<KettleDto?> GetKettleByIdAsync(long kettleId, CancellationToken cancellationToken = default)
        => _repository.GetKettleByIdAsync(kettleId, cancellationToken);

    public Task<int> SaveKettleAsync(SaveKettleRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveKettleAsync(request, cancellationToken);

    public Task<int> DeleteKettleAsync(long kettleId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteKettleAsync(kettleId, updatedBy, cancellationToken);
}
