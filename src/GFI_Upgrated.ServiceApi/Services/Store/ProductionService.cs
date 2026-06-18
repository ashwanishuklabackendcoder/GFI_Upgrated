using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.ServiceApi.Services.Store;

public interface IProductionService
{
    Task<PagedResult<ProductionDto>> GetProductionListAsync(ProductionListRequest request, CancellationToken cancellationToken = default);
    Task<ProductionDto?> GetProductionByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<int> SaveProductionAsync(SaveProductionRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteProductionAsync(long id, string updatedBy, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<PreProcessingItemDto>> GetProductionItemsAsync(long productionId, CancellationToken cancellationToken = default);
    Task<int> SaveProductionItemAsync(SavePreProcessingItemRequest request, CancellationToken cancellationToken = default);
    Task<int> DeleteProductionItemAsync(long itemStockUsedId, CancellationToken cancellationToken = default);
    Task<int> FinalizeStockUpdateAsync(long productionId, string updatedBy, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<CountryLookupDto>> GetCountriesLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SkuLookupDto>> GetSkusLookupAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KettleLookupDto>> GetKettlesLookupAsync(CancellationToken cancellationToken = default);
}

public sealed class ProductionService : IProductionService
{
    private readonly IProductionRepository _repository;

    public ProductionService(IProductionRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<ProductionDto>> GetProductionListAsync(ProductionListRequest request, CancellationToken cancellationToken = default)
        => _repository.GetProductionListAsync(request, cancellationToken);

    public Task<ProductionDto?> GetProductionByIdAsync(long id, CancellationToken cancellationToken = default)
        => _repository.GetProductionByIdAsync(id, cancellationToken);

    public Task<int> SaveProductionAsync(SaveProductionRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveProductionAsync(request, cancellationToken);

    public Task<int> DeleteProductionAsync(long id, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.DeleteProductionAsync(id, updatedBy, cancellationToken);

    public Task<IReadOnlyList<PreProcessingItemDto>> GetProductionItemsAsync(long productionId, CancellationToken cancellationToken = default)
        => _repository.GetProductionItemsAsync(productionId, cancellationToken);

    public Task<int> SaveProductionItemAsync(SavePreProcessingItemRequest request, CancellationToken cancellationToken = default)
        => _repository.SaveProductionItemAsync(request, cancellationToken);

    public Task<int> DeleteProductionItemAsync(long itemStockUsedId, CancellationToken cancellationToken = default)
        => _repository.DeleteProductionItemAsync(itemStockUsedId, cancellationToken);

    public Task<int> FinalizeStockUpdateAsync(long productionId, string updatedBy, CancellationToken cancellationToken = default)
        => _repository.FinalizeStockUpdateAsync(productionId, updatedBy, cancellationToken);

    public Task<IReadOnlyList<CountryLookupDto>> GetCountriesLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetCountriesLookupAsync(cancellationToken);

    public Task<IReadOnlyList<SkuLookupDto>> GetSkusLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetSkusLookupAsync(cancellationToken);

    public Task<IReadOnlyList<KettleLookupDto>> GetKettlesLookupAsync(CancellationToken cancellationToken = default)
        => _repository.GetKettlesLookupAsync(cancellationToken);
}
