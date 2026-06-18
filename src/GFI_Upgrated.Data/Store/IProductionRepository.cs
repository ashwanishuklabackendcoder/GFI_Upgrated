using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;

namespace GFI_Upgrated.Data.Store;

public interface IProductionRepository
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
