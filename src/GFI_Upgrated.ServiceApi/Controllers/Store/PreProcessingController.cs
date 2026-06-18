using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/preprocessing")]
public sealed class PreProcessingController : ControllerBase
{
    private readonly IPreProcessingService _service;

    public PreProcessingController(IPreProcessingService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<PreProcessingDto>>>> GetPreProcessingList([FromQuery] PreProcessingListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetPreProcessingListAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<PreProcessingDto>>
            {
                Success = true,
                Message = "Pre-Processing records loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<PreProcessingDto>>
            {
                Success = false,
                Message = $"Error loading pre-processing list: {ex.Message}"
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<PreProcessingDto>>> GetPreProcessingById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetPreProcessingByIdAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<PreProcessingDto>
        {
            Success = result is not null,
            Message = result is null ? "Pre-Processing record not found." : "Pre-Processing record loaded successfully.",
            Data = result
        });
    }

    [HttpPost("save")]
    public async Task<ActionResult<ApiEnvelope<int>>> SavePreProcessing([FromBody] SavePreProcessingRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

        var id = await _service.SavePreProcessingAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "Pre-Processing record saved successfully." : "Pre-Processing record save failed.",
            Data = id
        });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeletePreProcessing(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        var result = await _service.DeletePreProcessingAsync(id, updatedBy ?? "System", cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = result > 0,
            Message = result > 0 ? "Pre-Processing record deleted successfully." : "Pre-Processing record delete failed.",
            Data = result
        });
    }

    [HttpGet("{id:long}/items")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<PreProcessingItemDto>>>> GetPreProcessingItems(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetPreProcessingItemsAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<PreProcessingItemDto>>
        {
            Success = true,
            Message = "Pre-Processing items loaded successfully.",
            Data = result
        });
    }

    [HttpPost("items/save")]
    public async Task<ActionResult<ApiEnvelope<int>>> SavePreProcessingItem([FromBody] SavePreProcessingItemRequest request, CancellationToken cancellationToken)
    {
        var id = await _service.SavePreProcessingItemAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "Item saved successfully." : "Item save failed.",
            Data = id
        });
    }

    [HttpDelete("items/{itemStockUsedId:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeletePreProcessingItem(long itemStockUsedId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeletePreProcessingItemAsync(itemStockUsedId, cancellationToken);
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
        var result = await _service.FinalizeStockUpdateAsync(id, updatedBy ?? "System", cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = result > 0,
            Message = result > 0 ? "Stock finalized successfully." : "Stock finalization failed.",
            Data = result
        });
    }

    [HttpGet("bom/{bomId:long}/items")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<BomItemDetailDto>>>> GetBomItems(long bomId, CancellationToken cancellationToken)
    {
        var result = await _service.GetBomItemsAsync(bomId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<BomItemDetailDto>>
        {
            Success = true,
            Message = "BOM items loaded successfully.",
            Data = result
        });
    }

    [HttpGet("boms-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<BomLookupDto>>>> GetBomsLookup(CancellationToken cancellationToken)
    {
        var result = await _service.GetBomsLookupAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<BomLookupDto>>
        {
            Success = true,
            Message = "BOMs loaded successfully.",
            Data = result
        });
    }

    [HttpGet("warehouses-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<WarehouseLookupDto>>>> GetWarehousesLookup(CancellationToken cancellationToken)
    {
        var result = await _service.GetWarehousesLookupAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<WarehouseLookupDto>>
        {
            Success = true,
            Message = "Warehouses loaded successfully.",
            Data = result
        });
    }

    [HttpGet("available-batches/{itemId:long}")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<AvailableBatchDto>>>> GetAvailableBatches(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetAvailableBatchesAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<AvailableBatchDto>>
        {
            Success = true,
            Message = "Available batches loaded successfully.",
            Data = result
        });
    }
}
