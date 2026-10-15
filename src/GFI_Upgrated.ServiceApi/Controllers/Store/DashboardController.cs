using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Store;

[Authorize]
[ApiController]
[Route("api/store/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardRepository _repository;

    public DashboardController(IDashboardRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("stock")]
    public async Task<ActionResult<ApiEnvelope<StockDashboardDto>>> GetStockDashboard(CancellationToken cancellationToken)
    {
            var data = await _repository.GetStockDashboardAsync(cancellationToken);
            return Ok(new ApiEnvelope<StockDashboardDto>
            {
                Success = true,
                Message = "Stock dashboard loaded successfully.",
                Data = data
            });
    }

    [HttpGet("production")]
    public async Task<ActionResult<ApiEnvelope<ProductionDashboardDto>>> GetProductionDashboard([FromQuery] string? batchNo, CancellationToken cancellationToken)
    {
            var data = await _repository.GetProductionDashboardAsync(batchNo ?? string.Empty, cancellationToken);
            return Ok(new ApiEnvelope<ProductionDashboardDto>
            {
                Success = true,
                Message = "Production dashboard loaded successfully.",
                Data = data
            });
    }

    [HttpGet("production/batches")]
    public async Task<ActionResult<ApiEnvelope<List<DashboardBatchLookupDto>>>> GetProductionBatches(CancellationToken cancellationToken)
    {
            var data = await _repository.GetProductionBatchesAsync(cancellationToken);
            return Ok(new ApiEnvelope<List<DashboardBatchLookupDto>>
            {
                Success = true,
                Message = "Production batches loaded successfully.",
                Data = data
            });
    }

    [HttpGet("sales")]
    public async Task<ActionResult<ApiEnvelope<SalesDashboardDto>>> GetSalesDashboard(CancellationToken cancellationToken)
    {
            var data = await _repository.GetSalesDashboardAsync(cancellationToken);
            return Ok(new ApiEnvelope<SalesDashboardDto>
            {
                Success = true,
                Message = "Sales dashboard loaded successfully.",
                Data = data
            });
    }
}

