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
    public DateTime? TransactionDate { get; set; }
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

public class SalesDashboardDto
{
    public List<SalesSummaryDto> TotalSales { get; set; } = new();
    public List<SalesPerYearDto> SalesPerYear { get; set; } = new();
    public List<SalesPerCustomerGroupDto> SalesPerCustomerGroup { get; set; } = new();
    public List<SalesPerCustomerDto> SalesPerCustomer { get; set; } = new();
}
