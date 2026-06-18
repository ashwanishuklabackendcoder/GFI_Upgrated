using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[ApiController]
[Route("api/store/finished-products")]
public class FinishedProductController : ControllerBase
{
    private readonly IFinishedProductService _service;

    public FinishedProductController(IFinishedProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiEnvelope<PagedResult<FinishedProductDto>>>> GetFinishedProducts([FromQuery] FinishedProductListRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.GetFinishedProductsAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<FinishedProductDto>> { Success = true, Data = result });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<FinishedProductDto>>> GetFinishedProductById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetFinishedProductByIdAsync(id, cancellationToken);
        return Ok(new ApiEnvelope<FinishedProductDto> { Success = true, Data = result });
    }

    [HttpPost]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveFinishedProduct(SaveFinishedProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveFinishedProductAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Item saved successfully.", Data = result });
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiEnvelope<bool>>> DeleteFinishedProduct(long id, [FromQuery] string deletedBy, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteFinishedProductAsync(id, deletedBy, cancellationToken);
        return Ok(new ApiEnvelope<bool> { Success = true, Message = "Item deleted successfully.", Data = result });
    }

    [HttpGet("{itemId:long}/details")]
    public async Task<ActionResult<ApiEnvelope<FinishedProductDetailDto>>> GetFinishedProductDetail(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetFinishedProductDetailAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<FinishedProductDetailDto> { Success = true, Data = result });
    }

    [HttpPost("details")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveFinishedProductDetail(FinishedProductDetailDto request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveFinishedProductDetailAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Details saved successfully.", Data = result });
    }

    [HttpGet("{itemId:long}/vendors")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<FinishedProductVendorDto>>>> GetFinishedProductVendors(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetFinishedProductVendorsAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<FinishedProductVendorDto>> { Success = true, Data = result });
    }

    [HttpPost("vendors")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveFinishedProductVendor(FinishedProductVendorDto request, CancellationToken cancellationToken)
    {
        var result = await _service.SaveFinishedProductVendorAsync(request, cancellationToken);
        return Ok(new ApiEnvelope<int> { Success = true, Message = "Vendor saved successfully.", Data = result });
    }

    [HttpGet("{itemId:long}/batches")]
    public async Task<ActionResult<ApiEnvelope<IReadOnlyList<FinishedProductBatchDto>>>> GetFinishedProductBatches(long itemId, CancellationToken cancellationToken)
    {
        var result = await _service.GetFinishedProductBatchesAsync(itemId, cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyList<FinishedProductBatchDto>> { Success = true, Data = result });
    }

    [HttpPost("batches")]
    public async Task<ActionResult<ApiEnvelope<int>>> SaveFinishedProductBatch(FinishedProductBatchDto request, [FromQuery] string? deletedBatchIds, CancellationToken cancellationToken)
    {
        var result = await _service.SaveFinishedProductBatchAsync(request, deletedBatchIds, cancellationToken);
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
