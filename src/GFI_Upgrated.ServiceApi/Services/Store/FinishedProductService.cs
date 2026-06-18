using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public class FinishedProductService : IFinishedProductService
{
    private readonly IFinishedProductRepository _repository;

    public FinishedProductService(IFinishedProductRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<FinishedProductDto>> GetFinishedProductsAsync(FinishedProductListRequest request, CancellationToken cancellationToken = default)
        => _repository.GetFinishedProductsAsync(request, cancellationToken);

    public Task<FinishedProductDto?> GetFinishedProductByIdAsync(long id, CancellationToken cancellationToken = default)
        => _repository.GetFinishedProductByIdAsync(id, cancellationToken);

    public Task<int> SaveFinishedProductAsync(SaveFinishedProductRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveFinishedProductAsync(request, cancellationToken);

    public Task<bool> DeleteFinishedProductAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteFinishedProductAsync(id, deletedBy, cancellationToken);

    public Task<FinishedProductDetailDto?> GetFinishedProductDetailAsync(long itemId, CancellationToken cancellationToken = default)
        => _repository.GetFinishedProductDetailAsync(itemId, cancellationToken);

    public Task<int> SaveFinishedProductDetailAsync(FinishedProductDetailDto request, CancellationToken cancellationToken = default)
        => _repository.SaveFinishedProductDetailAsync(request, cancellationToken);

    public Task<IReadOnlyList<FinishedProductVendorDto>> GetFinishedProductVendorsAsync(long itemId, CancellationToken cancellationToken = default)
        => _repository.GetFinishedProductVendorsAsync(itemId, cancellationToken);

    public Task<int> SaveFinishedProductVendorAsync(FinishedProductVendorDto request, CancellationToken cancellationToken = default)
        => _repository.SaveFinishedProductVendorAsync(request, cancellationToken);

    public Task<bool> DeleteFinishedProductVendorAsync(long vendorId, CancellationToken cancellationToken = default)
        => _repository.DeleteFinishedProductVendorAsync(vendorId, cancellationToken);

    public Task<IReadOnlyList<FinishedProductBatchDto>> GetFinishedProductBatchesAsync(long itemId, CancellationToken cancellationToken = default)
        => _repository.GetFinishedProductBatchesAsync(itemId, cancellationToken);

    public Task<int> SaveFinishedProductBatchAsync(FinishedProductBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default)
        => _repository.SaveFinishedProductBatchAsync(request, deletedBatchIds, cancellationToken);

    public Task<IReadOnlyList<AccountLookupDto>> GetAccountsLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetAccountsLookupAsync(cancellationToken);

    public Task<IReadOnlyList<CurrencyLookupDto>> GetCurrenciesLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetCurrenciesLookupAsync(cancellationToken);
}
