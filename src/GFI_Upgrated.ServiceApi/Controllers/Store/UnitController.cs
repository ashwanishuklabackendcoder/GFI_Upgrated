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
            var result = await _service.GetUnitsAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<UnitDto>>
            {
                Success = true,
                Message = "Units loaded successfully.",
                Data = result
            });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<UnitDto>>> GetUnitById(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetUnitByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<UnitDto>
            {
                Success = result is not null,
                Message = result is null ? "Unit not found." : "Unit loaded successfully.",
                Data = result
            });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveUnit([FromBody] SaveUnitRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

            var id = await _service.SaveUnitAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Unit saved successfully." : (id == -1 ? "Unit name already exists." : "Unit save failed."),
                Data = id
            });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteUnit(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
            var dto = await _service.GetUnitByIdAsync(id, cancellationToken);
            if (dto != null) HttpContext.Items["EntityName"] = dto.UnitName;

            var result = await _service.DeleteUnitAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Unit deleted successfully." : "This record cannot be deleted because it is currently in use by other records in the system.",
                Data = result
            });
    }

    [HttpGet("base-units-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<BaseUnitLookupDto>>>> GetBaseUnitsLookup(CancellationToken cancellationToken)
    {
            var result = await _service.GetBaseUnitsLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<BaseUnitLookupDto>>
            {
                Success = true,
                Message = "Base units loaded successfully.",
                Data = result
            });
    }
}

