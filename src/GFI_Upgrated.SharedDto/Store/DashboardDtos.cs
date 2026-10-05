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

public class StockDashboardDto
{
    public List<CriticalStockItemDto> CriticalStockItems { get; set; } = new();
    public List<ReorderStockItemDto> ReorderStockItems { get; set; } = new();
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
