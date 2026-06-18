using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public class SemiFinishedProductService : ISemiFinishedProductService
{
    private readonly ISemiFinishedProductRepository _repository;

    public SemiFinishedProductService(ISemiFinishedProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SemiFinishedProductDto>> GetSemiFinishedProductsAsync(SemiFinishedProductListRequest request, CancellationToken cancellationToken = default)
        => await _repository.GetSemiFinishedProductsAsync(request, cancellationToken);

    public async Task<SemiFinishedProductDto?> GetSemiFinishedProductByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _repository.GetSemiFinishedProductByIdAsync(id, cancellationToken);

    public async Task<int> SaveSemiFinishedProductAsync(SaveSemiFinishedProductRequest request, CancellationToken cancellationToken = default)
        => await _repository.SaveSemiFinishedProductAsync(request, cancellationToken);

    public async Task<bool> DeleteSemiFinishedProductAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
        => await _repository.DeleteSemiFinishedProductAsync(id, deletedBy, cancellationToken);

    public async Task<SemiFinishedProductDetailDto?> GetSemiFinishedProductDetailAsync(long itemId, CancellationToken cancellationToken = default)
        => await _repository.GetSemiFinishedProductDetailAsync(itemId, cancellationToken);

    public async Task<int> SaveSemiFinishedProductDetailAsync(SemiFinishedProductDetailDto request, CancellationToken cancellationToken = default)
        => await _repository.SaveSemiFinishedProductDetailAsync(request, cancellationToken);

    public async Task<IReadOnlyList<SemiFinishedProductVendorDto>> GetSemiFinishedProductVendorsAsync(long itemId, CancellationToken cancellationToken = default)
        => await _repository.GetSemiFinishedProductVendorsAsync(itemId, cancellationToken);

    public async Task<int> SaveSemiFinishedProductVendorAsync(SemiFinishedProductVendorDto request, CancellationToken cancellationToken = default)
        => await _repository.SaveSemiFinishedProductVendorAsync(request, cancellationToken);

    public async Task<bool> DeleteSemiFinishedProductVendorAsync(long vendorId, CancellationToken cancellationToken = default)
        => await _repository.DeleteSemiFinishedProductVendorAsync(vendorId, cancellationToken);

    public async Task<IReadOnlyList<SemiFinishedProductBatchDto>> GetSemiFinishedProductBatchesAsync(long itemId, CancellationToken cancellationToken = default)
        => await _repository.GetSemiFinishedProductBatchesAsync(itemId, cancellationToken);

    public async Task<int> SaveSemiFinishedProductBatchAsync(SemiFinishedProductBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default)
        => await _repository.SaveSemiFinishedProductBatchAsync(request, deletedBatchIds, cancellationToken);

    public async Task<IReadOnlyList<AccountLookupDto>> GetAccountsLookupAsync(CancellationToken cancellationToken = default)
        => await _repository.GetAccountsLookupAsync(cancellationToken);

    public async Task<IReadOnlyList<CurrencyLookupDto>> GetCurrenciesLookupAsync(CancellationToken cancellationToken = default)
        => await _repository.GetCurrenciesLookupAsync(cancellationToken);
}
