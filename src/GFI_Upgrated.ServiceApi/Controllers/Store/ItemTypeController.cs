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
            var result = await _service.GetItemTypesAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<ItemTypeDto>>
            {
                Success = true,
                Message = "Item Types loaded successfully.",
                Data = result
            });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<ItemTypeDto>>> GetItemTypeById(long id, CancellationToken cancellationToken)
    {
            var result = await _service.GetItemTypeByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<ItemTypeDto>
            {
                Success = result is not null,
                Message = result is null ? "Item Type not found." : "Item Type loaded successfully.",
                Data = result
            });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveItemType([FromBody] SaveItemTypeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

            var id = await _service.SaveItemTypeAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Item Type saved successfully." : (id == -1 ? "Item Type name already exists." : "Item Type save failed."),
                Data = id
            });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteItemType(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
            var dto = await _service.GetItemTypeByIdAsync(id, cancellationToken);
            if (dto != null) HttpContext.Items["EntityName"] = dto.ItemTypeName;

            var result = await _service.DeleteItemTypeAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Item Type deleted successfully." : "This record cannot be deleted because it is currently in use by other records in the system.",
                Data = result
            });
    }

    [HttpGet("parent-types-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<ParentTypeLookupDto>>>> GetParentTypesLookup(CancellationToken cancellationToken)
    {
            var result = await _service.GetParentTypesLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<ParentTypeLookupDto>>
            {
                Success = true,
                Message = "Parent types loaded successfully.",
                Data = result
            });
    }
}

