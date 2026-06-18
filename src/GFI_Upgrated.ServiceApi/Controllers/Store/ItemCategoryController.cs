using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/item-categories")]
public sealed class ItemCategoryController : ControllerBase
{
    private readonly IItemCategoryService _service;

    public ItemCategoryController(IItemCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<ItemCategoryDto>>>> GetItemCategories([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetItemCategoriesAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<ItemCategoryDto>>
            {
                Success = true,
                Message = "Item Categories loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<ItemCategoryDto>>
            {
                Success = false,
                Message = $"Error loading item categories: {ex.Message}"
            });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<ItemCategoryDto>>> GetItemCategoryById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetItemCategoryByIdAsync(id, cancellationToken);
            return Ok(new ApiEnvelope<ItemCategoryDto>
            {
                Success = result is not null,
                Message = result is null ? "Item Category not found." : "Item Category loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<ItemCategoryDto>
            {
                Success = false,
                Message = $"Error loading item category: {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveItemCategory([FromBody] SaveItemCategoryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

        try
        {
            var id = await _service.SaveItemCategoryAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = id > 0,
                Message = id > 0 ? "Item Category saved successfully." : (id == -1 ? "Item Category name already exists." : "Item Category save failed."),
                Data = id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error saving item category: {ex.Message}"
            });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteItemCategory(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.DeleteItemCategoryAsync(id, updatedBy ?? "System", cancellationToken);
            return Ok(new ApiEnvelope<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Item Category deleted successfully." : (result == -1 ? "Cannot delete Item Category because it is currently linked to items." : "Item Category delete failed."),
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<int>
            {
                Success = false,
                Message = $"Error deleting item category: {ex.Message}"
            });
        }
    }

    [HttpGet("parent-categories-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<ParentCategoryLookupDto>>>> GetParentCategoriesLookup(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetParentCategoriesLookupAsync(cancellationToken);
            return Ok(new ApiEnvelope<IReadOnlyList<ParentCategoryLookupDto>>
            {
                Success = true,
                Message = "Parent categories loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<IReadOnlyList<ParentCategoryLookupDto>>
            {
                Success = false,
                Message = $"Error loading parent categories: {ex.Message}"
            });
        }
    }
}
