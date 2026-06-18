using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IFinishedProductService
{
    Task<PagedResult<FinishedProductDto>> GetFinishedProductsAsync(FinishedProductListRequest request, CancellationToken cancellationToken = default);
    Task<FinishedProductDto?> GetFinishedProductByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> SaveFinishedProductAsync(SaveFinishedProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteFinishedProductAsync(long id, string deletedBy, CancellationToken cancellationToken = default);

    Task<FinishedProductDetailDto?> GetFinishedProductDetailAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveFinishedProductDetailAsync(FinishedProductDetailDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FinishedProductVendorDto>> GetFinishedProductVendorsAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveFinishedProductVendorAsync(FinishedProductVendorDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteFinishedProductVendorAsync(long vendorId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FinishedProductBatchDto>> GetFinishedProductBatchesAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveFinishedProductBatchAsync(FinishedProductBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountLookupDto>> GetAccountsLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CurrencyLookupDto>> GetCurrenciesLookupAsync(CancellationToken cancellationToken = default);
}
