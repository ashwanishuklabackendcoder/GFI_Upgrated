using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/brands")]
public sealed class BrandController : ControllerBase
{
    private readonly IBrandService _service;

    public BrandController(IBrandService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<BrandDto>>>> GetBrands([FromQuery] PagedRequest request, [FromQuery] string? searchText, CancellationToken cancellationToken)
    {
            var result = await _service.GetBrandsAsync(request, searchText, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<BrandDto>>
            {
                Success = true,
                Message = "Brands loaded successfully.",
                Data = result
            });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<BrandDto>>> GetBrandById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetBrandByIdAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<BrandDto>
        {
            Success = result is not null,
            Message = result is null ? "Brand not found." : "Brand loaded successfully.",
            Data = result
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveBrand([FromBody] SaveBrandRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Ok(new ApiEnvelope<int> { Success = false, Message = message });
        }

        var id = await _service.SaveBrandAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = id > 0,
            Message = id > 0 ? "Brand saved successfully." : (id == -1 ? "Brand name already exists." : "Brand save failed."),
            Data = id
        });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<int>>> DeleteBrand(long id, [FromQuery] string? updatedBy, CancellationToken cancellationToken)
    {
        var dto = await _service.GetBrandByIdAsync(id, cancellationToken);
        if (dto != null) HttpContext.Items["EntityName"] = dto.BrandName;

        var result = await _service.DeleteBrandAsync(id, updatedBy ?? "System", cancellationToken);
        return Ok(new ApiEnvelope<int>
        {
            Success = result > 0,
            Message = result > 0 ? "Brand deleted successfully." : "This record cannot be deleted because it is currently in use by other records in the system.",
            Data = result
        });
    }
}

