using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IBrandRepository
{
    Task<PagedResult<BrandDto>> GetBrandsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<BrandDto?> GetBrandByIdAsync(long brandId, CancellationToken cancellationToken = default);
    Task<int> SaveBrandAsync(SaveBrandRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteBrandAsync(long brandId, string updatedBy, CancellationToken cancellationToken = default);
}
