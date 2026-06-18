using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/skus")]
public sealed class SkuController : ControllerBase
{
    private readonly ISkuService _service;

    public SkuController(ISkuService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<SkuDto>>>> GetSkus([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetSkusAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<SkuDto>>
            {
                Success = true,
                Message = "SKUs loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<SkuDto>>
            {
                Success = false,
                Message = $"Error loading SKUs: {ex.Message}"
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<SkuDto>>> GetSkuById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetSkuByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<SkuDto>
            {
                Success = result is not null,
                Message = result is null ? "SKU not found." : "SKU loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<SkuDto>
            {
                Success = false,
                Message = $"Error loading SKU: {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveSku([FromBody] SaveSkuRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

        try
        {
            var id = await _service.SaveSkuAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "SKU saved successfully." : (id == -1 ? "SKU name already exists." : "SKU save failed."),
                Data = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error saving SKU: {ex.Message}"
            });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteSku(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeleteSkuAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "SKU deleted successfully." : (result == -1 ? "Cannot delete SKU because it is currently linked to production records." : "SKU delete failed."),
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error deleting SKU: {ex.Message}"
            });
        }
    }

    [HttpGet("units-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<UnitLookupDto>>>> GetUnitsLookup(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetUnitsLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<UnitLookupDto>>
            {
                Success = true,
                Message = "Units loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<IReadOnlyList<UnitLookupDto>>
            {
                Success = false,
                Message = $"Error loading units: {ex.Message}"
            });
        }
    }
}
