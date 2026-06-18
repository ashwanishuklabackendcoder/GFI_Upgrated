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
        try
        {
            var result = await _service.GetWarehousesAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<WarehouseDto>>
            {
                Success = true,
                Message = "Warehouses loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<WarehouseDto>>
            {
                Success = false,
                Message = $"Error loading warehouses: {ex.Message}"
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<WarehouseDto>>> GetWarehouseById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetWarehouseByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<WarehouseDto>
            {
                Success = result is not null,
                Message = result is null ? "Warehouse not found." : "Warehouse loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<WarehouseDto>
            {
                Success = false,
                Message = $"Error loading warehouse: {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveWarehouse([FromBody] SaveWarehouseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

        try
        {
            var id = await _service.SaveWarehouseAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Warehouse saved successfully." : (id == -1 ? "Warehouse name already exists." : "Warehouse save failed."),
                Data = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error saving warehouse: {ex.Message}"
            });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteWarehouse(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeleteWarehouseAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Warehouse deleted successfully." : (result == -1 ? "Cannot delete Warehouse because it is currently linked to production, preprocessing, or shelves." : "Warehouse delete failed."),
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error deleting warehouse: {ex.Message}"
            });
        }
    }
}
