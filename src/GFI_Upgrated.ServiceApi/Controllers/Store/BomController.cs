using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[ApiController]
[Route("api/store/bom")]
public class BomController : ControllerBase
{
    private readonly IBomService _service;

    public BomController(IBomService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<BomDto>>>> GetBoms([FromQuery] BomListRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.GetBomsAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<BomDto>> { Success = true, Data = result });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<BomDto>>> GetBomById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetBomByIdAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<BomDto> { Success = true, Data = result });
    }

    [HttpGet("{id:long}/items")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<BomItemDto>>>> GetBomItems(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetBomItemsAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<BomItemDto>> { Success = true, Data = result });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveBom(SaveBomRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveBomAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "BOM saved successfully.", Data = result });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<bool>>> DeleteBom(long id, [FromQuery] string deletedBy, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteBomAsync(id, deletedBy, cancellationToken);
        if (result)
        {
            return Ok(new ApiEnvelope<bool> { Success = true, Message = "BOM deleted successfully.", Data = true });
        }
        else
        {
            return Ok(new ApiEnvelope<bool> { Success = false, Message = "Cannot delete BOM because it is currently linked to production entries or other records.", Data = false });
        }
    }

    [HttpGet("items-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<RawMaterialDto>>>> GetItemsLookup([FromQuery] int? itemTypeId, CancellationToken cancellationToken)
    {
        var result = await _service.GetItemsForBomLookupAsync(itemTypeId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<RawMaterialDto>> { Success = true, Data = result });
    }
}
