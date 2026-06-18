using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/item-types")]
public sealed class ItemTypeController : ControllerBase
{
    private readonly IItemTypeService _service;

    public ItemTypeController(IItemTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<ItemTypeDto>>>> GetItemTypes([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetItemTypesAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<ItemTypeDto>>
            {
                Success = true,
                Message = "Item Types loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<ItemTypeDto>>
            {
                Success = false,
                Message = $"Error loading item types: {ex.Message}"
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<ItemTypeDto>>> GetItemTypeById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetItemTypeByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<ItemTypeDto>
            {
                Success = result is not null,
                Message = result is null ? "Item Type not found." : "Item Type loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<ItemTypeDto>
            {
                Success = false,
                Message = $"Error loading item type: {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveItemType([FromBody] SaveItemTypeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

        try
        {
            var id = await _service.SaveItemTypeAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Item Type saved successfully." : (id == -1 ? "Item Type name already exists." : "Item Type save failed."),
                Data = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error saving item type: {ex.Message}"
            });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteItemType(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeleteItemTypeAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Item Type deleted successfully." : "Cannot delete Item Type because it is currently linked to items or is not editable.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error deleting item type: {ex.Message}"
            });
        }
    }

    [HttpGet("parent-types-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<ParentTypeLookupDto>>>> GetParentTypesLookup(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetParentTypesLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<ParentTypeLookupDto>>
            {
                Success = true,
                Message = "Parent types loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<IReadOnlyList<ParentTypeLookupDto>>
            {
                Success = false,
                Message = $"Error loading parent types: {ex.Message}"
            });
        }
    }
}
