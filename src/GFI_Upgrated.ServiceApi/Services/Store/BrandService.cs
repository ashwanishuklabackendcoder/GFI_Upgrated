using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IBrandService
{
    Task<PagedResult<BrandDto>> GetBrandsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default);
    Task<BrandDto?> GetBrandByIdAsync(long brandId, CancellationToken cancellationToken = default);
    Task<int> SaveBrandAsync(SaveBrandRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteBrandAsync(long brandId, string updatedBy, CancellationToken cancellationToken = default);
}

public sealed class BrandService : IBrandService
{
    private readonly IBrandRepository _repository;

    public BrandService(IBrandRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<BrandDto>> GetBrandsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
        => _repository.GetBrandsAsync(request, searchText, cancellationToken);

    public Task<BrandDto?> GetBrandByIdAsync(long brandId, CancellationToken cancellationToken = default)
        => _repository.GetBrandByIdAsync(brandId, cancellationToken);

    public Task<int> SaveBrandAsync(SaveBrandRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveBrandAsync(request, cancellationToken);

    public Task<int> DeleteBrandAsync(long brandId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteBrandAsync(brandId, updatedBy, cancellationToken);
}
