using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[ApiController]
[Route("api/store/raw-materials")]
public class RawMaterialController : ControllerBase
{
    private readonly IRawMaterialService _service;

    public RawMaterialController(IRawMaterialService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<RawMaterialDto>>>> GetRawMaterials([FromQuery] RawMaterialListRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.GetRawMaterialsAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<RawMaterialDto>> { Success = true, Data = result });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<RawMaterialDto>>> GetRawMaterialById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetRawMaterialByIdAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<RawMaterialDto> { Success = true, Data = result });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveRawMaterial(SaveRawMaterialRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveRawMaterialAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Item saved successfully.", Data = result });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<bool>>> DeleteRawMaterial(long id, [FromQuery] string deletedBy, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteRawMaterialAsync(id, deletedBy, cancellationToken);
        if (result)
        {
            return Ok(new ApiEnvelope<bool> { Success = true, Message = "Item deleted successfully.", Data = true });
        }
        else
        {
            return Ok(new ApiEnvelope<bool> { Success = false, Message = "Cannot delete raw material because it is currently linked to transactions, recipes (BOM), or stock batches.", Data = false });
        }
    }

    [HttpGet("{itemId:long}/details")]
    public async Task<ActionResult<ApiEnvelope<RawMaterialDetailDto>>> GetRawMaterialDetail(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetRawMaterialDetailAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<RawMaterialDetailDto> { Success = true, Data = result });
    }

    [HttpPost("details")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveRawMaterialDetail(RawMaterialDetailDto request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveRawMaterialDetailAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Details saved successfully.", Data = result });
    }

    [HttpGet("{itemId:long}/vendors")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<RawMaterialVendorDto>>>> GetRawMaterialVendors(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetRawMaterialVendorsAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<RawMaterialVendorDto>> { Success = true, Data = result });
    }

    [HttpPost("vendors")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveRawMaterialVendor(RawMaterialVendorDto request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveRawMaterialVendorAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Vendor saved successfully.", Data = result });
    }

    [HttpGet("{itemId:long}/batches")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<RawMaterialBatchDto>>>> GetRawMaterialBatches(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetRawMaterialBatchesAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<RawMaterialBatchDto>> { Success = true, Data = result });
    }

    [HttpPost("batches")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveRawMaterialBatch(RawMaterialBatchDto request, [FromQuery] string? deletedBatchIds, CancellationToken cancellationToken)
    {
        var result = await _service.SaveRawMaterialBatchAsync(request, deletedBatchIds, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Batch saved successfully.", Data = result });
    }

    [HttpGet("accounts-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<AccountLookupDto>>>> GetAccountsLookup(CancellationToken cancellationToken)
    {
        var result = await _service.GetAccountsLookupAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<AccountLookupDto>> { Success = true, Data = result });
    }

    [HttpGet("currencies-lookup")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<CurrencyLookupDto>>>> GetCurrenciesLookup(CancellationToken cancellationToken)
    {
        var result = await _service.GetCurrenciesLookupAsync(cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<CurrencyLookupDto>> { Success = true, Data = result });
    }
}
