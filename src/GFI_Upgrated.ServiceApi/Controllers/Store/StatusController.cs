using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/statuses")]
public sealed class StatusController : ControllerBase
{
    private readonly IStatusService _service;

    public StatusController(IStatusService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<StatusDto>>>> GetStatuses([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
            var result = await _service.GetStatusesAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<StatusDto>>
            {
                Success = true,
                Message = "Statuses loaded successfully.",
                Data = result
            });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<StatusDto>>> GetStatusById(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetStatusByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<StatusDto>
            {
                Success = result is not null,
                Message = result is null ? "Status not found." : "Status loaded successfully.",
                Data = result
            });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveStatus([FromBody] SaveStatusRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

            var id = await _service.SaveStatusAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Status saved successfully." : (id == -1 ? "Status name already exists." : "Status save failed."),
                Data = id
            });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteStatus(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
            var dto = await _service.GetStatusByIdAsync(id, cancellationToken);
            if (dto != null) HttpContext.Items["EntityName"] = dto.StatusName;

            var result = await _service.DeleteStatusAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Status deleted successfully." : "This record cannot be deleted because it is currently in use by other records in the system.",
                Data = result
            });
    }
}

