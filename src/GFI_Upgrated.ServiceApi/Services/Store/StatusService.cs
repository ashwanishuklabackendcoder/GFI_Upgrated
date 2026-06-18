using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IStatusService
{
    Task<PagedResult<StatusDto>> GetStatusesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<StatusDto?> GetStatusByIdAsync(long statusId, CancellationToken cancellationToken = default);
    Task<int> SaveStatusAsync(SaveStatusRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteStatusAsync(long statusId, string updatedBy, CancellationToken cancellationToken = default);
}

public sealed class StatusService : IStatusService
{
    private readonly IStatusRepository _repository;

    public StatusService(IStatusRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<StatusDto>> GetStatusesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetStatusesAsync(request, searchText, cancellationToken);

    public Task<StatusDto?> GetStatusByIdAsync(long statusId, CancellationToken cancellationToken = default)
        => _repository.GetStatusByIdAsync(statusId, cancellationToken);

    public Task<int> SaveStatusAsync(SaveStatusRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveStatusAsync(request, cancellationToken);

    public Task<int> DeleteStatusAsync(long statusId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteStatusAsync(statusId, updatedBy, cancellationToken);
}
