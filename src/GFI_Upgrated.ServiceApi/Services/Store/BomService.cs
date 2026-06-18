using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Store;

using GFI_Upgrated.Data.Store;

public sealed class BomService : IBomService
{
    private readonly IBomRepository _repository;

    public BomService(IBomRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<BomDto>> GetBomsAsync(BomListRequest request, CancellationToken cancellationToken = default)
    {
        return _repository.GetBomsAsync(request, cancellationToken);
    }

    public Task<BomDto?> GetBomByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _repository.GetBomByIdAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<BomItemDto>> GetBomItemsAsync(long bomId, CancellationToken cancellationToken = default)
    {
        return _repository.GetBomItemsAsync(bomId, cancellationToken);
    }

    public Task<int> SaveBomAsync(SaveBomRequest request, CancellationToken cancellationToken = default)
    {
        return _repository.SaveBomAsync(request, cancellationToken);
    }

    public Task<bool> DeleteBomAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteBomAsync(id, deletedBy, cancellationToken);
    }

    public Task<IReadOnlyList<RawMaterialDto>> GetItemsForBomLookupAsync(int? itemTypeId = null, CancellationToken cancellationToken = default)
    {
        return _repository.GetItemsForBomLookupAsync(itemTypeId, cancellationToken);
    }
}
