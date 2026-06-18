using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[ApiController]
[Route("api/store/semi-finished-products")]
public class SemiFinishedProductController : ControllerBase
{
    private readonly ISemiFinishedProductService _service;

    public SemiFinishedProductController(ISemiFinishedProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<SemiFinishedProductDto>>>> GetSemiFinishedProducts([FromQuery] SemiFinishedProductListRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.GetSemiFinishedProductsAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<SemiFinishedProductDto>> { Success = true, Data = result });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<SemiFinishedProductDto>>> GetSemiFinishedProductById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetSemiFinishedProductByIdAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<SemiFinishedProductDto> { Success = true, Data = result });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveSemiFinishedProduct(SaveSemiFinishedProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveSemiFinishedProductAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Item saved successfully.", Data = result });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<bool>>> DeleteSemiFinishedProduct(long id, [FromQuery] string deletedBy, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteSemiFinishedProductAsync(id, deletedBy, cancellationToken);
        if (result)
        {
            return Ok(new ApiEnvelope<bool> { Success = true, Message = "Item deleted successfully.", Data = true });
        }
        else
        {
            return Ok(new ApiEnvelope<bool> { Success = false, Message = "Cannot delete semi-finished product because it is currently linked to transactions, recipes (BOM), or stock batches.", Data = false });
        }
    }

    [HttpGet("{itemId:long}/details")]
    public async Task<ActionResult<ApiEnvelope<SemiFinishedProductDetailDto>>> GetSemiFinishedProductDetail(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetSemiFinishedProductDetailAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<SemiFinishedProductDetailDto> { Success = true, Data = result });
    }

    [HttpPost("details")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveSemiFinishedProductDetail(SemiFinishedProductDetailDto request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveSemiFinishedProductDetailAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Details saved successfully.", Data = result });
    }

    [HttpGet("{itemId:long}/vendors")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<SemiFinishedProductVendorDto>>>> GetSemiFinishedProductVendors(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetSemiFinishedProductVendorsAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<SemiFinishedProductVendorDto>> { Success = true, Data = result });
    }

    [HttpPost("vendors")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveSemiFinishedProductVendor(SemiFinishedProductVendorDto request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveSemiFinishedProductVendorAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Vendor saved successfully.", Data = result });
    }

    [HttpGet("{itemId:long}/batches")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<SemiFinishedProductBatchDto>>>> GetSemiFinishedProductBatches(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetSemiFinishedProductBatchesAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<SemiFinishedProductBatchDto>> { Success = true, Data = result });
    }

    [HttpPost("batches")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveSemiFinishedProductBatch(SemiFinishedProductBatchDto request, [FromQuery] string? deletedBatchIds, CancellationToken cancellationToken)
    {
        var result = await _service.SaveSemiFinishedProductBatchAsync(request, deletedBatchIds, cancellationToken);
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
