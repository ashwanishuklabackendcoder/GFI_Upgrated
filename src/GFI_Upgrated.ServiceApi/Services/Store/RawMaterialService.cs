using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public class RawMaterialService : IRawMaterialService
{
    private readonly IRawMaterialRepository _repository;

    public RawMaterialService(IRawMaterialRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<RawMaterialDto>> GetRawMaterialsAsync(RawMaterialListRequest request, CancellationToken cancellationToken = default)
        => await _repository.GetRawMaterialsAsync(request, cancellationToken);

    public async Task<RawMaterialDto?> GetRawMaterialByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _repository.GetRawMaterialByIdAsync(id, cancellationToken);

    public async Task<int> SaveRawMaterialAsync(SaveRawMaterialRequest request, CancellationToken cancellationToken = default)
        => await _repository.SaveRawMaterialAsync(request, cancellationToken);

    public async Task<bool> DeleteRawMaterialAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
        => await _repository.DeleteRawMaterialAsync(id, deletedBy, cancellationToken);

    public async Task<RawMaterialDetailDto?> GetRawMaterialDetailAsync(long itemId, CancellationToken cancellationToken = default)
        => await _repository.GetRawMaterialDetailAsync(itemId, cancellationToken);

    public async Task<int> SaveRawMaterialDetailAsync(RawMaterialDetailDto request, CancellationToken cancellationToken = default)
        => await _repository.SaveRawMaterialDetailAsync(request, cancellationToken);

    public async Task<IReadOnlyList<RawMaterialVendorDto>> GetRawMaterialVendorsAsync(long itemId, CancellationToken cancellationToken = default)
        => await _repository.GetRawMaterialVendorsAsync(itemId, cancellationToken);

    public async Task<int> SaveRawMaterialVendorAsync(RawMaterialVendorDto request, CancellationToken cancellationToken = default)
        => await _repository.SaveRawMaterialVendorAsync(request, cancellationToken);

    public async Task<bool> DeleteRawMaterialVendorAsync(long vendorId, CancellationToken cancellationToken = default)
        => await _repository.DeleteRawMaterialVendorAsync(vendorId, cancellationToken);

    public async Task<IReadOnlyList<RawMaterialBatchDto>> GetRawMaterialBatchesAsync(long itemId, CancellationToken cancellationToken = default)
        => await _repository.GetRawMaterialBatchesAsync(itemId, cancellationToken);

    public async Task<int> SaveRawMaterialBatchAsync(RawMaterialBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default)
        => await _repository.SaveRawMaterialBatchAsync(request, deletedBatchIds, cancellationToken);

    public async Task<IReadOnlyList<AccountLookupDto>> GetAccountsLookupAsync(CancellationToken cancellationToken = default)
        => await _repository.GetAccountsLookupAsync(cancellationToken);

    public async Task<IReadOnlyList<CurrencyLookupDto>> GetCurrenciesLookupAsync(CancellationToken cancellationToken = default)
        => await _repository.GetCurrenciesLookupAsync(cancellationToken);
}
