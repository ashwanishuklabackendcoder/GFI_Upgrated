using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/units")]
public sealed class UnitController : ControllerBase
{
    private readonly IUnitService _service;

    public UnitController(IUnitService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<UnitDto>>>> GetUnits([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetUnitsAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<UnitDto>>
            {
                Success = true,
                Message = "Units loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<UnitDto>>
            {
                Success = false,
                Message = $"Error loading units: {ex.Message}"
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<UnitDto>>> GetUnitById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetUnitByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<UnitDto>
            {
                Success = result is not null,
                Message = result is null ? "Unit not found." : "Unit loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<UnitDto>
            {
                Success = false,
                Message = $"Error loading unit: {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveUnit([FromBody] SaveUnitRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

        try
        {
            var id = await _service.SaveUnitAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Unit saved successfully." : (id == -1 ? "Unit name already exists." : "Unit save failed."),
                Data = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error saving unit: {ex.Message}"
            });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteUnit(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeleteUnitAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Unit deleted successfully." : (result == -1 ? "Cannot delete Unit because it is currently linked to SKUs or BOMs." : "Unit delete failed."),
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error deleting unit: {ex.Message}"
            });
        }
    }

    [HttpGet("base-units-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<BaseUnitLookupDto>>>> GetBaseUnitsLookup(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetBaseUnitsLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<BaseUnitLookupDto>>
            {
                Success = true,
                Message = "Base units loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<IReadOnlyList<BaseUnitLookupDto>>
            {
                Success = false,
                Message = $"Error loading base units: {ex.Message}"
            });
        }
    }
}
