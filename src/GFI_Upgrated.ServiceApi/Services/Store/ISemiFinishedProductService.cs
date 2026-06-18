using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface ISemiFinishedProductService
{
    Task<PagedResult<SemiFinishedProductDto>> GetSemiFinishedProductsAsync(SemiFinishedProductListRequest request, CancellationToken cancellationToken = default);
    Task<SemiFinishedProductDto?> GetSemiFinishedProductByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> SaveSemiFinishedProductAsync(SaveSemiFinishedProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteSemiFinishedProductAsync(long id, string deletedBy, CancellationToken cancellationToken = default);

    Task<SemiFinishedProductDetailDto?> GetSemiFinishedProductDetailAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveSemiFinishedProductDetailAsync(SemiFinishedProductDetailDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SemiFinishedProductVendorDto>> GetSemiFinishedProductVendorsAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveSemiFinishedProductVendorAsync(SemiFinishedProductVendorDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteSemiFinishedProductVendorAsync(long vendorId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SemiFinishedProductBatchDto>> GetSemiFinishedProductBatchesAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveSemiFinishedProductBatchAsync(SemiFinishedProductBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountLookupDto>> GetAccountsLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CurrencyLookupDto>> GetCurrenciesLookupAsync(CancellationToken cancellationToken = default);
}
