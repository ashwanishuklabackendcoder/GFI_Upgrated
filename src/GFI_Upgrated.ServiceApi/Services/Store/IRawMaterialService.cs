using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IRawMaterialService
{
    Task<PagedResult<RawMaterialDto>> GetRawMaterialsAsync(RawMaterialListRequest request, CancellationToken cancellationToken = default);
    Task<RawMaterialDto?> GetRawMaterialByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> SaveRawMaterialAsync(SaveRawMaterialRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteRawMaterialAsync(long id, string deletedBy, CancellationToken cancellationToken = default);

    Task<RawMaterialDetailDto?> GetRawMaterialDetailAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveRawMaterialDetailAsync(RawMaterialDetailDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RawMaterialVendorDto>> GetRawMaterialVendorsAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveRawMaterialVendorAsync(RawMaterialVendorDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteRawMaterialVendorAsync(long vendorId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RawMaterialBatchDto>> GetRawMaterialBatchesAsync(long itemId, CancellationToken cancellationToken = default);
    Task<int> SaveRawMaterialBatchAsync(RawMaterialBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountLookupDto>> GetAccountsLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CurrencyLookupDto>> GetCurrenciesLookupAsync(CancellationToken cancellationToken = default);
}
