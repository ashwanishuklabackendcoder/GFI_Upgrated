using GFI_Upgrated.ServiceApi.Services.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/reports")]
public sealed class ReportController : ControllerBase
{
    private readonly IReportService _service;

    public ReportController(IReportService service)
    {
        _service = service;
    }

    [HttpGet("item-stock")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<ItemStockReportDto>>>> GetItemStockReport([FromQuery] ItemStockReportRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetItemStockReportAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<ItemStockReportDto>>
            {
                Success = true,
                Message = "Item stock report loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<ItemStockReportDto>>
            {
                Success = false,
                Message = $"Error loading item stock report: {ex.Message}"
            });
        }
    }

    [HttpGet("batch-wise/by-number")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<BatchWiseItemDto>>>> GetBatchWiseItemsByBatchNo([FromQuery] string batchNo, [FromQuery] int page, [FromQuery] int size, [FromQuery] string sortType, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetBatchWiseItemsByBatchNoAsync(batchNo, page, size, sortType, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<BatchWiseItemDto>>
            {
                Success = true,
                Message = "Batch-wise items report by batch number loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<BatchWiseItemDto>>
            {
                Success = false,
                Message = $"Error loading batch-wise items report by batch number: {ex.Message}"
            });
        }
    }

    [HttpGet("batch-wise/by-item")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<BatchWiseItemDto>>>> GetBatchWiseItemsByItem([FromQuery] long itemId, [FromQuery] int page, [FromQuery] int size, [FromQuery] string sortType, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetBatchWiseItemsByItemAsync(itemId, page, size, sortType, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<BatchWiseItemDto>>
            {
                Success = true,
                Message = "Batch-wise items report by item loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<BatchWiseItemDto>>
            {
                Success = false,
                Message = $"Error loading batch-wise items report by item: {ex.Message}"
            });
        }
    }

    [HttpGet("stock-by-batch")]
    public async Task<ActionResult<ApiEnvelope<PagedResult<ItemStockByBatchReportDto>>>> GetItemStockByBatchReport([FromQuery] long? itemStockByBatchId, [FromQuery] long? stockById, [FromQuery] long? itemId, [FromQuery] int page, [FromQuery] int size, [FromQuery] string sortCol, [FromQuery] string sortOrd, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.GetItemStockByBatchReportAsync(itemStockByBatchId, stockById, itemId, page, size, sortCol, sortOrd, cancellationToken);
            return Ok(new ApiEnvelope<PagedResult<ItemStockByBatchReportDto>>
            {
                Success = true,
                Message = "Item stock by batch report loaded successfully.",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiEnvelope<PagedResult<ItemStockByBatchReportDto>>
            {
                Success = false,
                Message = $"Error loading item stock by batch report: {ex.Message}"
            });
        }
    }
}
