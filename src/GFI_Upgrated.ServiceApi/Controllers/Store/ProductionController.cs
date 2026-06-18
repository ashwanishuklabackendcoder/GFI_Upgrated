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
        try
        {
            var result = await _service.GetProductionListAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<ProductionDto>>
            {
                Success = true,
                Message = "Production records loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<ProductionDto>>
            {
                Success = false,
                Message = $"Error loading production list: {ex.Message}"
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<ProductionDto>>> GetProductionById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetProductionByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<ProductionDto>
            {
                Success = result is not null,
                Message = result is null ? "Production record not found." : "Production record loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<ProductionDto>
            {
                Success = false,
                Message = $"Error loading production details: {ex.Message}"
            });
        }
    }

    [HttpPost("save")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveProduction([FromBody] SaveProductionRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

        try
        {
            var id = await _service.SaveProductionAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Production record saved successfully." : "Production record save failed.",
                Data = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error saving production record: {ex.Message}"
            });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteProduction(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeleteProductionAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Production record deleted successfully." : "Production record delete failed.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error deleting production record: {ex.Message}"
            });
        }
    }

    [HttpGet("{id:long}/items")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<PreProcessingItemDto>>>> GetProductionItems(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetProductionItemsAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<PreProcessingItemDto>>
            {
                Success = true,
                Message = "Production items loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<IReadOnlyList<PreProcessingItemDto>>
            {
                Success = false,
                Message = $"Error loading production items: {ex.Message}"
            });
        }
    }

    [HttpPost("items/save")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveProductionItem([FromBody] SavePreProcessingItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _service.SaveProductionItemAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Item saved successfully." : "Item save failed.",
                Data = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error saving production item: {ex.Message}"
            });
        }
    }

    [HttpDelete("items/{itemStockUsedId:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteProductionItem(long itemStockUsedId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeleteProductionItemAsync(itemStockUsedId, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Item deleted successfully." : "Item delete failed.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error deleting item: {ex.Message}"
            });
        }
    }

    [HttpPost("{id:long}/finalize")]
    public async Task<ActionResult<ApiEnvelope<int>>> FinalizeStockUpdate(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.FinalizeStockUpdateAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Stock finalized successfully." : "Stock finalization failed.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error committing production stock: {ex.Message}"
            });
        }
    }

    [HttpGet("countries-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<CountryLookupDto>>>> GetCountriesLookup(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetCountriesLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<CountryLookupDto>>
            {
                Success = true,
                Message = "Countries lookup loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<IReadOnlyList<CountryLookupDto>>
            {
                Success = false,
                Message = $"Error loading countries lookup: {ex.Message}"
            });
        }
    }

    [HttpGet("skus-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<SkuLookupDto>>>> GetSkusLookup(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetSkusLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<SkuLookupDto>>
            {
                Success = true,
                Message = "SKUs lookup loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<IReadOnlyList<SkuLookupDto>>
            {
                Success = false,
                Message = $"Error loading SKUs lookup: {ex.Message}"
            });
        }
    }

    [HttpGet("kettles-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<KettleLookupDto>>>> GetKettlesLookup(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetKettlesLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<KettleLookupDto>>
            {
                Success = true,
                Message = "Kettles lookup loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<IReadOnlyList<KettleLookupDto>>
            {
                Success = false,
                Message = $"Error loading kettles lookup: {ex.Message}"
            });
        }
    }
}
