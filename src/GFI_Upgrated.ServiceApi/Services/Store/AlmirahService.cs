using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IAlmirahService
{
    Task<PagedResult<AlmirahDto>> GetAlmirahsAsync(PagedRequest request, string? searchText, long? warehouseId, CancellationToken cancellationToken = default);
    Task<AlmirahDto?> GetAlmirahByIdAsync(long almirahId, CancellationToken cancellationToken = default);
    Task<int> SaveAlmirahAsync(SaveAlmirahRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteAlmirahAsync(long almirahId, string updatedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AlmirahDto>> GetShelvesByAlmirahIdAsync(long almirahId, CancellationToken cancellationToken = default);
}

public sealed class AlmirahService : IAlmirahService
{
    private readonly IAlmirahRepository _repository;

    public AlmirahService(IAlmirahRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<AlmirahDto>> GetAlmirahsAsync(PagedRequest request, string? searchText, long? warehouseId, CancellationToken cancellationToken = default)
        => _repository.GetAlmirahsAsync(request, searchText, warehouseId, cancellationToken);

    public Task<AlmirahDto?> GetAlmirahByIdAsync(long almirahId, CancellationToken cancellationToken = default)
        => _repository.GetAlmirahByIdAsync(almirahId, cancellationToken);

    public Task<int> SaveAlmirahAsync(SaveAlmirahRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveAlmirahAsync(request, cancellationToken);

    public Task<int> DeleteAlmirahAsync(long almirahId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteAlmirahAsync(almirahId, updatedBy, cancellationToken);

    public Task<IReadOnlyList<AlmirahDto>> GetShelvesByAlmirahIdAsync(long almirahId, CancellationToken cancellationToken = default)
        => _repository.GetShelvesByAlmirahIdAsync(almirahId, cancellationToken);
}
