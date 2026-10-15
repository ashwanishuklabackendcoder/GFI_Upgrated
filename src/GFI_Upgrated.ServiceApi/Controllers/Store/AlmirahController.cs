using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/almirahs")]
public sealed class AlmirahController : ControllerBase
{
    private readonly IAlmirahService _service;

    public AlmirahController(IAlmirahService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<AlmirahDto>>>> GetAlmirahs([FromQuery] PagedRequest request, [FromQuery] string? searchText, [FromQuery] long? warehouseId, CancellationToken cancellationToken)
    {
            var result = await _service.GetAlmirahsAsync(request, searchText, warehouseId, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<AlmirahDto>>
            {
                Success = true,
                Message = "Almirahs loaded successfully.",
                Data = result
            });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<AlmirahDto>>> GetAlmirahById(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetAlmirahByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<AlmirahDto>
            {
                Success = result is not null,
                Message = result is null ? "Almirah not found." : "Almirah loaded successfully.",
                Data = result
            });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveAlmirah([FromBody] SaveAlmirahRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

            var id = await _service.SaveAlmirahAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Almirah saved successfully." : (id == -1 ? "Almirah name already exists." : "Almirah save failed."),
                Data = id
            });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteAlmirah(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
            var dto = await _service.GetAlmirahByIdAsync(id, cancellationToken);
            if (dto != null) HttpContext.Items["EntityName"] = dto.AlmirahShelfName;

            var result = await _service.DeleteAlmirahAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Almirah deleted successfully." : "This record cannot be deleted because it is currently in use by other records in the system.",
                Data = result
            });
    }

    [HttpGet("{id:long}/shelves")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<AlmirahDto>>>> GetShelves(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetShelvesByAlmirahIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<AlmirahDto>>
            {
                Success = true,
                Message = "Shelves loaded successfully.",
                Data = result
            });
    }
}

