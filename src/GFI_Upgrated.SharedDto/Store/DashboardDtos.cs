using System;
using System.Collections.Generic;

namespace GFI_Upgrated.SharedDto.Store;

public class CriticalStockItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public decimal FinalStock { get; set; }
    public decimal CriticalLevelQuantity { get; set; }
}

public class ReorderStockItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public decimal FinalStock { get; set; }
    public decimal ReorderLevelQuantity { get; set; }
}

public class ItemStockUsageOverviewDto
{
    public long ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal CurrentStockValue { get; set; }

    // This Quarter
    public decimal ThisQuarterStockUsed { get; set; }
    public decimal ThisQuarterAvgMonthlyUsed { get; set; }
    public decimal ThisQuarterStockValueUsed { get; set; }

    // Last Quarter
    public decimal LastQuarterStockUsed { get; set; }
    public decimal LastQuarterAvgMonthlyUsed { get; set; }
    public decimal LastQuarterStockValueUsed { get; set; }

    // Last 12 Months
    public decimal Last12MonthsStockUsed { get; set; }
    public decimal Last12MonthsAvgMonthlyUsed { get; set; }
    public decimal Last12MonthsStockValueUsed { get; set; }
}

public class StockUsageSummaryMetricsDto
{
    public decimal TotalCurrentStockValue { get; set; }
    public decimal ThisQuarterTotalUsedQty { get; set; }
    public decimal ThisQuarterTotalUsedValue { get; set; }
    public decimal LastQuarterTotalUsedQty { get; set; }
    public decimal LastQuarterTotalUsedValue { get; set; }
    public decimal Last12MonthsTotalUsedQty { get; set; }
    public decimal Last12MonthsTotalUsedValue { get; set; }
    public string ThisQuarterLabel { get; set; } = string.Empty;
    public string LastQuarterLabel { get; set; } = string.Empty;
    public string Last12MonthsLabel { get; set; } = string.Empty;
}

public class StockDashboardDto
{
    public List<CriticalStockItemDto> CriticalStockItems { get; set; } = new();
    public List<ReorderStockItemDto> ReorderStockItems { get; set; } = new();
    public StockUsageSummaryMetricsDto UsageMetrics { get; set; } = new();
    public List<ItemStockUsageOverviewDto> ItemUsageOverviews { get; set; } = new();
}

public class ProductionDashboardItemDto
{
    public long ProductionId { get; set; }
    public long ItemId { get; set; }
    public DateTime? FilledDate { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UsedBatchNo { get; set; } = string.Empty;
}

public class ProductionDashboardDto
{
    public List<ProductionDashboardItemDto> TotalSoldItems { get; set; } = new();
    public List<ProductionDashboardItemDto> TotalUsedItems { get; set; } = new();
}

public class DashboardBatchLookupDto
{
    public long ProductionId { get; set; }
    public string BatchNo { get; set; } = string.Empty;
}

public class SalesSummaryDto
{
    public string ItemName { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Quantity { get; set; }
    public decimal TotalAmount { get; set; }
}

public class SalesPerYearDto
{
    public int Year { get; set; }
    public decimal SalesAmount { get; set; }
    public decimal QuantitySold { get; set; }
}

public class SalesPerCustomerGroupDto
{
    public string CustomerGroup { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal QuantitySold { get; set; }
}

public class SalesPerCustomerDto
{
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal QuantitySold { get; set; }
}

public class AnnualSalesPerformanceDto
{
    public int Year { get; set; }
    public decimal SrdBottles { get; set; }
    public decimal SrdLiters { get; set; }
    public decimal SrdValue { get; set; }
    public decimal UsdBottles { get; set; }
    public decimal UsdLiters { get; set; }
    public decimal UsdValue { get; set; }
    public decimal TotalSalesSrd { get; set; }
}

public class SalesPerTastePerYearDto
{
    public int Year { get; set; }
    public decimal TotalLiters { get; set; }
    public Dictionary<string, decimal> TasteLiters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public class SalesDashboardDto
{
    public List<SalesSummaryDto> TotalSales { get; set; } = new();
    public List<SalesPerYearDto> SalesPerYear { get; set; } = new();
    public List<AnnualSalesPerformanceDto> AnnualPerformance { get; set; } = new();
    public List<SalesPerTastePerYearDto> SalesPerTastePerYear { get; set; } = new();
    public List<string> AllTasteCodes { get; set; } = new();
    public List<SalesPerCustomerGroupDto> SalesPerCustomerGroup { get; set; } = new();
    public List<SalesPerCustomerDto> SalesPerCustomer { get; set; } = new();
}
