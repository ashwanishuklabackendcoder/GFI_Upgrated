using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using GFI_Upgrated.Data.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/production")]
public sealed class ProductionController : ControllerBase
{
    private readonly IProductionService _service;

    public ProductionController(IProductionService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<ProductionDto>>>> GetProductionList([FromQuery] ProductionListRequest request, CancellationToken cancellationToken)
    {
            var result = await _service.GetProductionListAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<ProductionDto>>
            {
                Success = true,
                Message = "Production records loaded successfully.",
                Data = result
            });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<ProductionDto>>> GetProductionById(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetProductionByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<ProductionDto>
            {
                Success = result is not null,
                Message = result is null ? "Production record not found." : "Production record loaded successfully.",
                Data = result
            });
    }

    [HttpPost("save")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveProduction([FromBody] SaveProductionRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

            var id = await _service.SaveProductionAsync(request, cancellationToken);
            
            if (id > 0)
            {
                var dto = await _service.GetProductionByIdAsync(id, cancellationToken);
                if (dto != null)
                {
                    HttpContext.Items["EntityName"] = $"{dto.BomName} (Batch: {dto.BatchNo})";
                }
            }

            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Production record saved successfully." : "Production record save failed.",
                Data = id
            });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteProduction(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
            var dto = await _service.GetProductionByIdAsync(id, cancellationToken);
            if (dto != null)
            {
                HttpContext.Items["EntityName"] = $"{dto.BomName} (Batch: {dto.BatchNo})";
            }

            var result = await _service.DeleteProductionAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Production record deleted successfully." : "Production record delete failed.",
                Data = result
            });
    }

    [HttpGet("{id:long}/items")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<PreProcessingItemDto>>>> GetProductionItems(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetProductionItemsAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<PreProcessingItemDto>>
            {
                Success = true,
                Message = "Production items loaded successfully.",
                Data = result
            });
    }

    [HttpPost("items/save")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveProductionItem([FromBody] SavePreProcessingItemRequest request, CancellationToken cancellationToken)
    {
            var id = await _service.SaveProductionItemAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Item saved successfully." : "Item save failed.",
                Data = id
            });
    }

    [HttpDelete("items/{itemStockUsedId:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteProductionItem(long itemStockUsedId, CancellationToken cancellationToken)
    {
            var result = await _service.DeleteProductionItemAsync(itemStockUsedId, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Item deleted successfully." : "Item delete failed.",
                Data = result
            });
    }

    [HttpPost("{id:long}/finalize")]
    public async Task<ActionResult<ApiEnvelope<int>>> FinalizeStockUpdate(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
            var result = await _service.FinalizeStockUpdateAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Stock finalized successfully." : "Stock finalization failed.",
                Data = result
            });
    }

    [HttpGet("countries-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<CountryLookupDto>>>> GetCountriesLookup(CancellationToken cancellationToken)
    {
            var result = await _service.GetCountriesLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<CountryLookupDto>>
            {
                Success = true,
                Message = "Countries lookup loaded successfully.",
                Data = result
            });
    }

    [HttpGet("skus-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<SkuLookupDto>>>> GetSkusLookup(CancellationToken cancellationToken)
    {
            var result = await _service.GetSkusLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<SkuLookupDto>>
            {
                Success = true,
                Message = "SKUs lookup loaded successfully.",
                Data = result
            });
    }

    [HttpGet("kettles-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<KettleLookupDto>>>> GetKettlesLookup(CancellationToken cancellationToken)
    {
            var result = await _service.GetKettlesLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<KettleLookupDto>>
            {
                Success = true,
                Message = "Kettles lookup loaded successfully.",
                Data = result
            });
    }
}

