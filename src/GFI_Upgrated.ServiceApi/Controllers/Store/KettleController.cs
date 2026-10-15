using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/kettles")]
public sealed class KettleController : ControllerBase
{
    private readonly IKettleService _service;

    public KettleController(IKettleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<KettleDto>>>> GetKettles([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
            var result = await _service.GetKettlesAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<KettleDto>>
            {
                Success = true,
                Message = "Kettles loaded successfully.",
                Data = result
            });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<KettleDto>>> GetKettleById(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetKettleByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<KettleDto>
            {
                Success = result is not null,
                Message = result is null ? "Kettle not found." : "Kettle loaded successfully.",
                Data = result
            });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveKettle([FromBody] SaveKettleRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

            var id = await _service.SaveKettleAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Kettle saved successfully." : (id == -1 ? "Kettle number already exists." : "Kettle save failed."),
                Data = id
            });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteKettle(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
            var dto = await _service.GetKettleByIdAsync(id, cancellationToken);
            if (dto != null) HttpContext.Items["EntityName"] = dto.KettleNumber;

            var result = await _service.DeleteKettleAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Kettle deleted successfully." : "This record cannot be deleted because it is currently in use by other records in the system.",
                Data = result
            });
    }
}

