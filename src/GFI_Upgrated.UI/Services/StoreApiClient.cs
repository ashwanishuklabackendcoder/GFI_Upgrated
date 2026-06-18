using System.Net.Http.Json;
using System.Text.Json;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using GFI_Upgrated.SharedDto.Purchase;
using GFI_Upgrated.UI.State;

namespace GFI_Upgrated.UI.Services;

public sealed class StoreApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly AppSessionState _sessionState;

    public StoreApiClient(HttpClient httpClient, AppSessionState sessionState)
    {
        _httpClient = httpClient;
        _sessionState = sessionState;
    }

    public async Task<PagedResult<BrandDto>> GetBrandsAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<BrandDto>>(BuildQuery("api/store/brands", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<BrandDto>();

    public async Task<BrandDto?> GetBrandByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<BrandDto>($"api/store/brands/{id}", cancellationToken);

    public async Task<int> SaveBrandAsync(SaveBrandRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveBrandRequest, int>("api/store/brands", request, cancellationToken);

    public async Task<int> DeleteBrandAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/brands/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    // SKU Methods
    public async Task<PagedResult<SkuDto>> GetSkusAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<SkuDto>>(BuildQuery("api/store/skus", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<SkuDto>();

    public async Task<SkuDto?> GetSkuByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<SkuDto>($"api/store/skus/{id}", cancellationToken);

    public async Task<int> SaveSkuAsync(SaveSkuRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveSkuRequest, int>("api/store/skus", request, cancellationToken);

    public async Task<int> DeleteSkuAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/skus/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    public async Task<IReadOnlyList<UnitLookupDto>> GetUnitsLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<UnitLookupDto>>("api/store/skus/units-lookup", cancellationToken)
           ?? Array.Empty<UnitLookupDto>();

    // Unit Methods
    public async Task<PagedResult<UnitDto>> GetUnitsAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<UnitDto>>(BuildQuery("api/store/units", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<UnitDto>();

    public async Task<UnitDto?> GetUnitByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<UnitDto>($"api/store/units/{id}", cancellationToken);

    public async Task<int> SaveUnitAsync(SaveUnitRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveUnitRequest, int>("api/store/units", request, cancellationToken);

    public async Task<int> DeleteUnitAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/units/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    public async Task<IReadOnlyList<BaseUnitLookupDto>> GetBaseUnitsLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<BaseUnitLookupDto>>("api/store/units/base-units-lookup", cancellationToken)
           ?? Array.Empty<BaseUnitLookupDto>();

    // Warehouse Methods
    public async Task<PagedResult<WarehouseDto>> GetWarehousesAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<WarehouseDto>>(BuildQuery("api/store/warehouses", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<WarehouseDto>();

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<WarehouseDto>($"api/store/warehouses/{id}", cancellationToken);

    public async Task<int> SaveWarehouseAsync(SaveWarehouseRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveWarehouseRequest, int>("api/store/warehouses", request, cancellationToken);

    public async Task<int> DeleteWarehouseAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/warehouses/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    // Kettle Methods
    public async Task<PagedResult<KettleDto>> GetKettlesAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<KettleDto>>(BuildQuery("api/store/kettles", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<KettleDto>();

    public async Task<KettleDto?> GetKettleByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<KettleDto>($"api/store/kettles/{id}", cancellationToken);

    public async Task<int> SaveKettleAsync(SaveKettleRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveKettleRequest, int>("api/store/kettles", request, cancellationToken);

    public async Task<int> DeleteKettleAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/kettles/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    // Item Category Methods
    public async Task<PagedResult<ItemCategoryDto>> GetItemCategoriesAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<ItemCategoryDto>>(BuildQuery("api/store/item-categories", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<ItemCategoryDto>();

    public async Task<ItemCategoryDto?> GetItemCategoryByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<ItemCategoryDto>($"api/store/item-categories/{id}", cancellationToken);

    public async Task<int> SaveItemCategoryAsync(SaveItemCategoryRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveItemCategoryRequest, int>("api/store/item-categories", request, cancellationToken);

    public async Task<int> DeleteItemCategoryAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/item-categories/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    public async Task<IReadOnlyList<ParentCategoryLookupDto>> GetParentCategoriesLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<ParentCategoryLookupDto>>("api/store/item-categories/parent-categories-lookup", cancellationToken)
           ?? Array.Empty<ParentCategoryLookupDto>();

    // Status Methods
    public async Task<PagedResult<StatusDto>> GetStatusesAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<StatusDto>>(BuildQuery("api/store/statuses", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<StatusDto>();

    public async Task<StatusDto?> GetStatusByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<StatusDto>($"api/store/statuses/{id}", cancellationToken);

    public async Task<int> SaveStatusAsync(SaveStatusRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveStatusRequest, int>("api/store/statuses", request, cancellationToken);

    public async Task<int> DeleteStatusAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/statuses/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    // Item Type Methods
    public async Task<PagedResult<ItemTypeDto>> GetItemTypesAsync(PagedRequest request, string? searchText = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<ItemTypeDto>>(BuildQuery("api/store/item-types", request, ("searchText", searchText)), cancellationToken)
           ?? new PagedResult<ItemTypeDto>();

    public async Task<ItemTypeDto?> GetItemTypeByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<ItemTypeDto>($"api/store/item-types/{id}", cancellationToken);

    public async Task<int> SaveItemTypeAsync(SaveItemTypeRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveItemTypeRequest, int>("api/store/item-types", request, cancellationToken);

    public async Task<int> DeleteItemTypeAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/item-types/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    public async Task<IReadOnlyList<ParentTypeLookupDto>> GetParentTypesLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<ParentTypeLookupDto>>("api/store/item-types/parent-types-lookup", cancellationToken)
           ?? Array.Empty<ParentTypeLookupDto>();

    // Almirah Methods
    public async Task<PagedResult<AlmirahDto>> GetAlmirahsAsync(PagedRequest request, string? searchText = null, long? warehouseId = null, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<AlmirahDto>>(BuildQuery("api/store/almirahs", request, ("searchText", searchText), ("warehouseId", warehouseId?.ToString())), cancellationToken)
           ?? new PagedResult<AlmirahDto>();

    public async Task<AlmirahDto?> GetAlmirahByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<AlmirahDto>($"api/store/almirahs/{id}", cancellationToken);

    public async Task<int> SaveAlmirahAsync(SaveAlmirahRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveAlmirahRequest, int>("api/store/almirahs", request, cancellationToken);

    public async Task<int> DeleteAlmirahAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/almirahs/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    public async Task<IReadOnlyList<AlmirahDto>> GetShelvesByAlmirahIdAsync(long almirahId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<AlmirahDto>>($"api/store/almirahs/{almirahId}/shelves", cancellationToken)
           ?? Array.Empty<AlmirahDto>();

    // Pre-Processing Methods
    public async Task<PagedResult<PreProcessingDto>> GetPreProcessingListAsync(PreProcessingListRequest request, CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"CurrentPage={request.CurrentPage}",
            $"RecordPerPage={request.RecordPerPage}",
            $"SortColumn={request.SortColumn}",
            $"SortType={request.SortType}"
        };
        if (request.PreProcessingId.HasValue) query.Add($"PreProcessingId={request.PreProcessingId}");
        if (request.BomId.HasValue) query.Add($"BomId={request.BomId}");
        if (!string.IsNullOrWhiteSpace(request.CreatedBy)) query.Add($"CreatedBy={Uri.EscapeDataString(request.CreatedBy)}");

        return await GetEnvelopeAsync<PagedResult<PreProcessingDto>>($"api/store/preprocessing/list?{string.Join('&', query)}", cancellationToken)
               ?? new PagedResult<PreProcessingDto>();
    }

    public async Task<PreProcessingDto?> GetPreProcessingByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PreProcessingDto>($"api/store/preprocessing/{id}", cancellationToken);

    public async Task<int> SavePreProcessingAsync(SavePreProcessingRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SavePreProcessingRequest, int>("api/store/preprocessing/save", request, cancellationToken);

    public async Task<int> DeletePreProcessingAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/preprocessing/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    public async Task<IReadOnlyList<PreProcessingItemDto>> GetPreProcessingItemsAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<PreProcessingItemDto>>($"api/store/preprocessing/{id}/items", cancellationToken)
           ?? Array.Empty<PreProcessingItemDto>();

    public async Task<int> SavePreProcessingItemAsync(SavePreProcessingItemRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SavePreProcessingItemRequest, int>("api/store/preprocessing/items/save", request, cancellationToken);

    public async Task<int> DeletePreProcessingItemAsync(long itemStockUsedId, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/preprocessing/items/{itemStockUsedId}", cancellationToken);

    public async Task<int> FinalizePreProcessingStockAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<object, int>($"api/store/preprocessing/{id}/finalize?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", new { }, cancellationToken);

    public async Task<IReadOnlyList<BomItemDetailDto>> GetBomItemsAsync(long bomId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<BomItemDetailDto>>($"api/store/preprocessing/bom/{bomId}/items", cancellationToken)
           ?? Array.Empty<BomItemDetailDto>();

    public async Task<IReadOnlyList<BomLookupDto>> GetBomsLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<BomLookupDto>>("api/store/preprocessing/boms-lookup", cancellationToken)
           ?? Array.Empty<BomLookupDto>();

    public async Task<IReadOnlyList<WarehouseLookupDto>> GetWarehousesLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<WarehouseLookupDto>>("api/store/preprocessing/warehouses-lookup", cancellationToken)
           ?? Array.Empty<WarehouseLookupDto>();

    public async Task<IReadOnlyList<AvailableBatchDto>> GetAvailableBatchesAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<AvailableBatchDto>>($"api/store/preprocessing/available-batches/{itemId}", cancellationToken)
           ?? Array.Empty<AvailableBatchDto>();

    // Production Methods
    public async Task<PagedResult<ProductionDto>> GetProductionListAsync(ProductionListRequest request, CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"CurrentPage={request.CurrentPage}",
            $"RecordPerPage={request.RecordPerPage}",
            $"SortColumn={request.SortColumn}",
            $"SortType={request.SortType}"
        };
        if (request.ProductionId.HasValue) query.Add($"ProductionId={request.ProductionId}");
        if (request.BomId.HasValue) query.Add($"BomId={request.BomId}");
        if (request.SkuId.HasValue) query.Add($"SkuId={request.SkuId}");
        if (!string.IsNullOrWhiteSpace(request.Remarks)) query.Add($"Remarks={Uri.EscapeDataString(request.Remarks)}");

        return await GetEnvelopeAsync<PagedResult<ProductionDto>>($"api/store/production/list?{string.Join('&', query)}", cancellationToken)
               ?? new PagedResult<ProductionDto>();
    }

    public async Task<ProductionDto?> GetProductionByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<ProductionDto>($"api/store/production/{id}", cancellationToken);

    public async Task<int> SaveProductionAsync(SaveProductionRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveProductionRequest, int>("api/store/production/save", request, cancellationToken);

    public async Task<int> DeleteProductionAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/production/{id}?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", cancellationToken);

    public async Task<IReadOnlyList<PreProcessingItemDto>> GetProductionItemsAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<PreProcessingItemDto>>($"api/store/production/{id}/items", cancellationToken)
           ?? Array.Empty<PreProcessingItemDto>();

    public async Task<int> SaveProductionItemAsync(SavePreProcessingItemRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SavePreProcessingItemRequest, int>("api/store/production/items/save", request, cancellationToken);

    public async Task<int> DeleteProductionItemAsync(long itemStockUsedId, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<int>($"api/store/production/items/{itemStockUsedId}", cancellationToken);

    public async Task<int> FinalizeProductionStockAsync(long id, string? updatedBy = null, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<object, int>($"api/store/production/{id}/finalize?updatedBy={Uri.EscapeDataString(updatedBy ?? "System")}", new { }, cancellationToken);

    public async Task<IReadOnlyList<CountryLookupDto>> GetCountriesLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<CountryLookupDto>>("api/store/production/countries-lookup", cancellationToken)
           ?? Array.Empty<CountryLookupDto>();

    public async Task<IReadOnlyList<SkuLookupDto>> GetSkusLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<SkuLookupDto>>("api/store/production/skus-lookup", cancellationToken)
           ?? Array.Empty<SkuLookupDto>();

    public async Task<IReadOnlyList<KettleLookupDto>> GetKettlesLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<KettleLookupDto>>("api/store/production/kettles-lookup", cancellationToken)
           ?? Array.Empty<KettleLookupDto>();

    // Semi Finished Product Methods
    public async Task<PagedResult<SemiFinishedProductDto>> GetSemiFinishedProductsAsync(SemiFinishedProductListRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"PageNumber={request.PageNumber}",
            $"PageSize={request.PageSize}"
        };
        if (!string.IsNullOrWhiteSpace(request.SortColumn)) queryParams.Add($"SortColumn={Uri.EscapeDataString(request.SortColumn)}");
        if (!string.IsNullOrWhiteSpace(request.SortType)) queryParams.Add($"SortType={Uri.EscapeDataString(request.SortType)}");
        if (!string.IsNullOrWhiteSpace(request.SearchTerm)) queryParams.Add($"SearchTerm={Uri.EscapeDataString(request.SearchTerm)}");

        return await GetEnvelopeAsync<PagedResult<SemiFinishedProductDto>>($"api/store/semi-finished-products?{string.Join('&', queryParams)}", cancellationToken)
               ?? new PagedResult<SemiFinishedProductDto>();
    }

    public async Task<SemiFinishedProductDto?> GetSemiFinishedProductByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<SemiFinishedProductDto>($"api/store/semi-finished-products/{id}", cancellationToken);

    public async Task<int> SaveSemiFinishedProductAsync(SaveSemiFinishedProductRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveSemiFinishedProductRequest, int>("api/store/semi-finished-products", request, cancellationToken);

    public async Task<bool> DeleteSemiFinishedProductAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/store/semi-finished-products/{id}?deletedBy={Uri.EscapeDataString(deletedBy)}", cancellationToken);

    public async Task<SemiFinishedProductDetailDto?> GetSemiFinishedProductDetailAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<SemiFinishedProductDetailDto>($"api/store/semi-finished-products/{itemId}/details", cancellationToken);

    public async Task<int> SaveSemiFinishedProductDetailAsync(SemiFinishedProductDetailDto request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SemiFinishedProductDetailDto, int>("api/store/semi-finished-products/details", request, cancellationToken);

    public async Task<IReadOnlyList<SemiFinishedProductVendorDto>> GetSemiFinishedProductVendorsAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<SemiFinishedProductVendorDto>>($"api/store/semi-finished-products/{itemId}/vendors", cancellationToken)
           ?? Array.Empty<SemiFinishedProductVendorDto>();

    public async Task<int> SaveSemiFinishedProductVendorAsync(SemiFinishedProductVendorDto request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SemiFinishedProductVendorDto, int>("api/store/semi-finished-products/vendors", request, cancellationToken);

    public async Task<IReadOnlyList<SemiFinishedProductBatchDto>> GetSemiFinishedProductBatchesAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<SemiFinishedProductBatchDto>>($"api/store/semi-finished-products/{itemId}/batches", cancellationToken)
           ?? Array.Empty<SemiFinishedProductBatchDto>();

    public async Task<int> SaveSemiFinishedProductBatchAsync(SemiFinishedProductBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SemiFinishedProductBatchDto, int>($"api/store/semi-finished-products/batches{(string.IsNullOrEmpty(deletedBatchIds) ? "" : $"?deletedBatchIds={deletedBatchIds}")}", request, cancellationToken);

    // Raw Material Methods
    public async Task<PagedResult<RawMaterialDto>> GetRawMaterialsAsync(RawMaterialListRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"PageNumber={request.PageNumber}",
            $"PageSize={request.PageSize}"
        };
        if (!string.IsNullOrWhiteSpace(request.SortColumn)) queryParams.Add($"SortColumn={Uri.EscapeDataString(request.SortColumn)}");
        if (!string.IsNullOrWhiteSpace(request.SortType)) queryParams.Add($"SortType={Uri.EscapeDataString(request.SortType)}");
        if (!string.IsNullOrWhiteSpace(request.SearchTerm)) queryParams.Add($"SearchTerm={Uri.EscapeDataString(request.SearchTerm)}");

        return await GetEnvelopeAsync<PagedResult<RawMaterialDto>>($"api/store/raw-materials?{string.Join('&', queryParams)}", cancellationToken)
               ?? new PagedResult<RawMaterialDto>();
    }

    public async Task<RawMaterialDto?> GetRawMaterialByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<RawMaterialDto>($"api/store/raw-materials/{id}", cancellationToken);

    public async Task<int> SaveRawMaterialAsync(SaveRawMaterialRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveRawMaterialRequest, int>("api/store/raw-materials", request, cancellationToken);

    public async Task<bool> DeleteRawMaterialAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/store/raw-materials/{id}?deletedBy={Uri.EscapeDataString(deletedBy)}", cancellationToken);

    public async Task<RawMaterialDetailDto?> GetRawMaterialDetailAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<RawMaterialDetailDto>($"api/store/raw-materials/{itemId}/details", cancellationToken);

    public async Task<int> SaveRawMaterialDetailAsync(RawMaterialDetailDto request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<RawMaterialDetailDto, int>("api/store/raw-materials/details", request, cancellationToken);

    public async Task<IReadOnlyList<RawMaterialVendorDto>> GetRawMaterialVendorsAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<RawMaterialVendorDto>>($"api/store/raw-materials/{itemId}/vendors", cancellationToken)
           ?? Array.Empty<RawMaterialVendorDto>();

    public async Task<int> SaveRawMaterialVendorAsync(RawMaterialVendorDto request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<RawMaterialVendorDto, int>("api/store/raw-materials/vendors", request, cancellationToken);

    public async Task<IReadOnlyList<RawMaterialBatchDto>> GetRawMaterialBatchesAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<RawMaterialBatchDto>>($"api/store/raw-materials/{itemId}/batches", cancellationToken)
           ?? Array.Empty<RawMaterialBatchDto>();

    public async Task<int> SaveRawMaterialBatchAsync(RawMaterialBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<RawMaterialBatchDto, int>($"api/store/raw-materials/batches{(string.IsNullOrEmpty(deletedBatchIds) ? "" : $"?deletedBatchIds={deletedBatchIds}")}", request, cancellationToken);

    public async Task<IReadOnlyList<AccountLookupDto>> GetAccountsLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<AccountLookupDto>>("api/store/raw-materials/accounts-lookup", cancellationToken)
           ?? Array.Empty<AccountLookupDto>();

    public async Task<IReadOnlyList<CurrencyLookupDto>> GetCurrenciesLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<CurrencyLookupDto>>("api/store/raw-materials/currencies-lookup", cancellationToken)
           ?? Array.Empty<CurrencyLookupDto>();

    // Finished Product Methods
    public async Task<PagedResult<FinishedProductDto>> GetFinishedProductsAsync(FinishedProductListRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"PageNumber={request.PageNumber}",
            $"PageSize={request.PageSize}"
        };
        if (!string.IsNullOrWhiteSpace(request.SortColumn)) queryParams.Add($"SortColumn={Uri.EscapeDataString(request.SortColumn)}");
        if (!string.IsNullOrWhiteSpace(request.SortType)) queryParams.Add($"SortType={Uri.EscapeDataString(request.SortType)}");
        if (!string.IsNullOrWhiteSpace(request.SearchTerm)) queryParams.Add($"SearchTerm={Uri.EscapeDataString(request.SearchTerm)}");

        return await GetEnvelopeAsync<PagedResult<FinishedProductDto>>($"api/store/finished-products?{string.Join('&', queryParams)}", cancellationToken)
               ?? new PagedResult<FinishedProductDto>();
    }

    public async Task<FinishedProductDto?> GetFinishedProductByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<FinishedProductDto>($"api/store/finished-products/{id}", cancellationToken);

    public async Task<int> SaveFinishedProductAsync(SaveFinishedProductRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveFinishedProductRequest, int>("api/store/finished-products", request, cancellationToken);

    public async Task<bool> DeleteFinishedProductAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/store/finished-products/{id}?deletedBy={Uri.EscapeDataString(deletedBy)}", cancellationToken);

    public async Task<FinishedProductDetailDto?> GetFinishedProductDetailAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<FinishedProductDetailDto>($"api/store/finished-products/{itemId}/details", cancellationToken);

    public async Task<int> SaveFinishedProductDetailAsync(FinishedProductDetailDto request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<FinishedProductDetailDto, int>("api/store/finished-products/details", request, cancellationToken);

    public async Task<IReadOnlyList<FinishedProductVendorDto>> GetFinishedProductVendorsAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<FinishedProductVendorDto>>($"api/store/finished-products/{itemId}/vendors", cancellationToken)
           ?? Array.Empty<FinishedProductVendorDto>();

    public async Task<int> SaveFinishedProductVendorAsync(FinishedProductVendorDto request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<FinishedProductVendorDto, int>("api/store/finished-products/vendors", request, cancellationToken);

    public async Task<IReadOnlyList<FinishedProductBatchDto>> GetFinishedProductBatchesAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<FinishedProductBatchDto>>($"api/store/finished-products/{itemId}/batches", cancellationToken)
           ?? Array.Empty<FinishedProductBatchDto>();

    public async Task<int> SaveFinishedProductBatchAsync(FinishedProductBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<FinishedProductBatchDto, int>($"api/store/finished-products/batches{(string.IsNullOrEmpty(deletedBatchIds) ? "" : $"?deletedBatchIds={deletedBatchIds}")}", request, cancellationToken);

    // BOM Methods
    public async Task<PagedResult<BomDto>> GetBomsAsync(BomListRequest request, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<PagedResult<BomDto>>(BuildQuery("api/store/bom", new PagedRequest
        {
            CurrentPage = request.PageNumber,
            RecordPerPage = request.PageSize,
            SortColumn = request.SortColumn,
            SortType = request.SortType
        }, ("searchTerm", request.SearchTerm), ("itemTypeId", request.ItemTypeId?.ToString())), cancellationToken)
           ?? new PagedResult<BomDto>();

    public async Task<BomDto?> GetBomByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<BomDto>($"api/store/bom/{id}", cancellationToken);

    public async Task<IReadOnlyList<BomItemDto>> GetBomComponentsAsync(long bomId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<BomItemDto>>($"api/store/bom/{bomId}/items", cancellationToken)
           ?? Array.Empty<BomItemDto>();

    public async Task<int> SaveBomAsync(SaveBomRequest request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<SaveBomRequest, int>("api/store/bom", request, cancellationToken);

    public async Task<bool> DeleteBomAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
    {
        AddAuthHeader();
        var response = await _httpClient.DeleteAsync($"api/store/bom/{id}?deletedBy={deletedBy}", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<bool>>(cancellationToken: cancellationToken);
        return envelope?.Success ?? false;
    }

    public async Task<IReadOnlyList<RawMaterialDto>> GetItemsForBomLookupAsync(int? itemTypeId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<RawMaterialDto>>($"api/store/bom/items-lookup?itemTypeId={itemTypeId}", cancellationToken)
           ?? Array.Empty<RawMaterialDto>();

    // Purchase Request Methods
    public async Task<PagedResult<PurchaseRequestDto>> GetPurchaseRequestsAsync(long? requestId, string? requestNumber, long? requestedBy, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = $"api/purchase/requests?page={page}&pageSize={pageSize}";
        if (requestId.HasValue) query += $"&requestId={requestId}";
        if (!string.IsNullOrEmpty(requestNumber)) query += $"&requestNumber={Uri.EscapeDataString(requestNumber)}";
        if (requestedBy.HasValue) query += $"&requestedBy={requestedBy}";
        return await GetEnvelopeAsync<PagedResult<PurchaseRequestDto>>(query, cancellationToken) ?? new();
    }

    public async Task<List<PurchaseRequestItemDto>> GetPurchaseRequestItemsAsync(long requestId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<List<PurchaseRequestItemDto>>($"api/purchase/requests/{requestId}/items", cancellationToken) ?? new();

    public async Task<long> SavePurchaseRequestAsync(PurchaseRequestDto request, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<PurchaseRequestDto, long>("api/purchase/requests", request, cancellationToken);

    public async Task<bool> DeletePurchaseRequestsAsync(string ids, string deletedBy, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/purchase/requests?ids={ids}&deletedBy={Uri.EscapeDataString(deletedBy)}", cancellationToken);

    // Purchase Order Methods
    public async Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(long? orderId, string? voucherNumber, long? accountId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = $"api/purchase/orders?page={page}&pageSize={pageSize}";
        if (orderId.HasValue) query += $"&orderId={orderId}";
        if (!string.IsNullOrEmpty(voucherNumber)) query += $"&voucherNumber={Uri.EscapeDataString(voucherNumber)}";
        if (accountId.HasValue) query += $"&accountId={accountId}";
        return await GetEnvelopeAsync<PagedResult<PurchaseOrderDto>>(query, cancellationToken) ?? new();
    }

    public async Task<List<PurchaseOrderItemDto>> GetPurchaseOrderItemsAsync(long orderId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<List<PurchaseOrderItemDto>>($"api/purchase/orders/{orderId}/items", cancellationToken) ?? new();

    public async Task<long> SavePurchaseOrderAsync(PurchaseOrderDto order, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<PurchaseOrderDto, long>("api/purchase/orders", order, cancellationToken);

    public async Task<bool> DeletePurchaseOrdersAsync(string ids, string deletedBy, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/purchase/orders?ids={ids}&deletedBy={Uri.EscapeDataString(deletedBy)}", cancellationToken);

    // Purchase (GRN) Methods
    public async Task<PagedResult<PurchaseDto>> GetPurchasesAsync(long? purchaseId, string? voucherNumber, string? invoiceNo, long? accountId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = $"api/purchase/grn?page={page}&pageSize={pageSize}";
        if (purchaseId.HasValue) query += $"&purchaseId={purchaseId}";
        if (!string.IsNullOrEmpty(voucherNumber)) query += $"&voucherNumber={Uri.EscapeDataString(voucherNumber)}";
        if (!string.IsNullOrEmpty(invoiceNo)) query += $"&invoiceNo={Uri.EscapeDataString(invoiceNo)}";
        if (accountId.HasValue) query += $"&accountId={accountId}";
        return await GetEnvelopeAsync<PagedResult<PurchaseDto>>(query, cancellationToken) ?? new();
    }

    public async Task<List<PurchaseItemDto>> GetPurchaseItemsAsync(long purchaseId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<List<PurchaseItemDto>>($"api/purchase/grn/{purchaseId}/items", cancellationToken) ?? new();

    public async Task<long> SavePurchaseAsync(PurchaseDto purchase, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<PurchaseDto, long>("api/purchase/grn", purchase, cancellationToken);

    public async Task<bool> DeletePurchasesAsync(string ids, string deletedBy, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/purchase/grn?ids={ids}&deletedBy={Uri.EscapeDataString(deletedBy)}", cancellationToken);

    // Purchase Return Methods
    public async Task<PagedResult<PurchaseReturnDto>> GetPurchaseReturnsAsync(long? returnId, long? itemId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = $"api/purchase/returns?page={page}&pageSize={pageSize}";
        if (returnId.HasValue) query += $"&returnId={returnId}";
        if (itemId.HasValue) query += $"&itemId={itemId}";
        return await GetEnvelopeAsync<PagedResult<PurchaseReturnDto>>(query, cancellationToken) ?? new();
    }

    public async Task<long> SavePurchaseReturnAsync(PurchaseReturnDto @return, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<PurchaseReturnDto, long>("api/purchase/returns", @return, cancellationToken);

    public async Task<bool> DeletePurchaseReturnsAsync(string ids, string deletedBy, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/purchase/returns?ids={ids}&deletedBy={Uri.EscapeDataString(deletedBy)}", cancellationToken);

    public async Task<IReadOnlyList<PurchaseReturnItemLookupDto>> GetPurchaseReturnItemsLookupAsync(long accountId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<List<PurchaseReturnItemLookupDto>>($"api/purchase/returns/items-lookup?accountId={accountId}", cancellationToken) 
           ?? new List<PurchaseReturnItemLookupDto>();

    public async Task<IReadOnlyList<PurchaseReturnBatchLookupDto>> GetPurchaseReturnBatchesLookupAsync(long itemId, long brandId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<List<PurchaseReturnBatchLookupDto>>($"api/purchase/returns/batches-lookup?itemId={itemId}&brandId={brandId}", cancellationToken) 
           ?? new List<PurchaseReturnBatchLookupDto>();

    // Item Write Off Methods
    public async Task<PagedResult<ItemWriteOffDto>> GetItemWriteOffsAsync(long? writeOffId, long? itemId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = $"api/purchase/write-offs?page={page}&pageSize={pageSize}";
        if (writeOffId.HasValue) query += $"&writeOffId={writeOffId}";
        if (itemId.HasValue) query += $"&itemId={itemId}";
        return await GetEnvelopeAsync<PagedResult<ItemWriteOffDto>>(query, cancellationToken) ?? new();
    }

    public async Task<long> SaveItemWriteOffAsync(ItemWriteOffDto dto, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<ItemWriteOffDto, long>("api/purchase/write-offs", dto, cancellationToken);

    public async Task<bool> DeleteItemWriteOffsAsync(string ids, string deletedBy, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/purchase/write-offs?ids={ids}&deletedBy={Uri.EscapeDataString(deletedBy)}", cancellationToken);

    public async Task<IReadOnlyList<WriteOffBatchLookupDto>> GetWriteOffBatchesLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<List<WriteOffBatchLookupDto>>("api/purchase/write-offs/batches-lookup", cancellationToken)
           ?? new List<WriteOffBatchLookupDto>();

    private async Task<T?> GetEnvelopeAsync<T>(string url, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with {response.StatusCode}: {content}");
        }

        var envelope = JsonSerializer.Deserialize<ApiEnvelope<T>>(content, JsonOptions);
        return envelope is { Success: true } ? envelope.Data : default;
    }

    private void AddAuthHeader()
    {
        if (_sessionState.CurrentUser?.Token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionState.CurrentUser.Token);
        }
    }

    private async Task<TResponse> PostEnvelopeAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with {(int)response.StatusCode} ({response.ReasonPhrase}): {payload}");
        }

        var envelope = string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);

        if (envelope is not null && !envelope.Success)
        {
            throw new Exception(envelope.Message ?? "API post request failed.");
        }

        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    private async Task<TResponse> DeleteEnvelopeAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with {(int)response.StatusCode} ({response.ReasonPhrase}): {payload}");
        }

        var envelope = string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);

        if (envelope is not null && !envelope.Success)
        {
            throw new Exception(envelope.Message ?? "API delete request failed.");
        }

        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    private static string BuildQuery(string url, PagedRequest request, params (string Name, string? Value)[] extras)
    {
        var query = new List<string>
        {
            $"CurrentPage={Uri.EscapeDataString(request.CurrentPage.ToString())}",
            $"RecordPerPage={Uri.EscapeDataString(request.RecordPerPage.ToString())}"
        };

        if (!string.IsNullOrWhiteSpace(request.SortColumn)) query.Add($"SortColumn={Uri.EscapeDataString(request.SortColumn)}");
        if (!string.IsNullOrWhiteSpace(request.SortType)) query.Add($"SortType={Uri.EscapeDataString(request.SortType)}");

        foreach (var (name, value) in extras)
        {
            if (!string.IsNullOrWhiteSpace(value)) query.Add($"{name}={Uri.EscapeDataString(value)}");
        }

        return $"{url}?{string.Join('&', query)}";
    }

    // Report Methods
    public async Task<PagedResult<ItemStockReportDto>> GetItemStockReportAsync(ItemStockReportRequest request, CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"CurrentPage={request.CurrentPage}",
            $"RecordPerPage={request.RecordPerPage}",
            $"SortColumn={request.SortColumn}",
            $"SortType={request.SortType}"
        };
        if (request.StockID.HasValue) query.Add($"StockID={request.StockID}");
        if (request.ItemID.HasValue) query.Add($"ItemID={request.ItemID}");
        if (request.WarehouseID.HasValue) query.Add($"WarehouseID={request.WarehouseID}");
        if (request.UnitId.HasValue) query.Add($"UnitId={request.UnitId}");
        if (!string.IsNullOrWhiteSpace(request.CreatedBy)) query.Add($"CreatedBy={Uri.EscapeDataString(request.CreatedBy)}");

        return await GetEnvelopeAsync<PagedResult<ItemStockReportDto>>($"api/store/reports/item-stock?{string.Join('&', query)}", cancellationToken)
               ?? new PagedResult<ItemStockReportDto>();
    }

    public async Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByBatchNoAsync(string batchNo, int page, int size, string sortType, CancellationToken cancellationToken = default)
    {
        return await GetEnvelopeAsync<PagedResult<BatchWiseItemDto>>($"api/store/reports/batch-wise/by-number?batchNo={Uri.EscapeDataString(batchNo)}&page={page}&size={size}&sortType={Uri.EscapeDataString(sortType)}", cancellationToken)
               ?? new PagedResult<BatchWiseItemDto>();
    }

    public async Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByItemAsync(long itemId, int page, int size, string sortType, CancellationToken cancellationToken = default)
    {
        return await GetEnvelopeAsync<PagedResult<BatchWiseItemDto>>($"api/store/reports/batch-wise/by-item?itemId={itemId}&page={page}&size={size}&sortType={Uri.EscapeDataString(sortType)}", cancellationToken)
               ?? new PagedResult<BatchWiseItemDto>();
    }

    public async Task<PagedResult<ItemStockByBatchReportDto>> GetItemStockByBatchReportAsync(long? itemStockByBatchId, long? stockById, long? itemId, int page, int size, string sortCol, string sortOrd, CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"page={page}",
            $"size={size}",
            $"sortCol={sortCol}",
            $"sortOrd={sortOrd}"
        };
        if (itemStockByBatchId.HasValue) query.Add($"itemStockByBatchId={itemStockByBatchId}");
        if (stockById.HasValue) query.Add($"stockById={stockById}");
        if (itemId.HasValue) query.Add($"itemId={itemId}");

        return await GetEnvelopeAsync<PagedResult<ItemStockByBatchReportDto>>($"api/store/reports/stock-by-batch?{string.Join('&', query)}", cancellationToken)
               ?? new PagedResult<ItemStockByBatchReportDto>();
    }
}
