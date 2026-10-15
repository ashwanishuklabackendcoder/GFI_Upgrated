using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/warehouses")]
public sealed class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _service;

    public WarehouseController(IWarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<WarehouseDto>>>> GetWarehouses([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
            var result = await _service.GetWarehousesAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<WarehouseDto>>
            {
                Success = true,
                Message = "Warehouses loaded successfully.",
                Data = result
            });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<WarehouseDto>>> GetWarehouseById(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetWarehouseByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<WarehouseDto>
            {
                Success = result is not null,
                Message = result is null ? "Warehouse not found." : "Warehouse loaded successfully.",
                Data = result
            });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveWarehouse([FromBody] SaveWarehouseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

            var id = await _service.SaveWarehouseAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Warehouse saved successfully." : (id == -1 ? "Warehouse name already exists." : "Warehouse save failed."),
                Data = id
            });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteWarehouse(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
            var dto = await _service.GetWarehouseByIdAsync(id, cancellationToken);
            if (dto != null) HttpContext.Items["EntityName"] = dto.WarehouseName;

            var result = await _service.DeleteWarehouseAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Warehouse deleted successfully." : "This record cannot be deleted because it is currently in use by other records in the system.",
                Data = result
            });
    }
}

