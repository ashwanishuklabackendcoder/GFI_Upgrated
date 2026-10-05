using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;
using GFI_Upgrated.Data.Common;

namespace GFI_Upgrated.Data.Store;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly string _connectionString;

    public DashboardRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<StockDashboardDto> GetStockDashboardAsync(CancellationToken cancellationToken = default)
    {
        var result = new StockDashboardDto();

        var criticalTask = ExecuteDataTableAsync("ItemStockAlert", new[] { new SqlParameter("@value", SqlDbType.Int) { Value = 1 } }, cancellationToken);
        var reorderTask = ExecuteDataTableAsync("ItemStockAlert", new[] { new SqlParameter("@value", SqlDbType.Int) { Value = 2 } }, cancellationToken);

        await Task.WhenAll(criticalTask, reorderTask);

        foreach (DataRow row in criticalTask.Result.Rows)
        {
            result.CriticalStockItems.Add(new CriticalStockItemDto
            {
                ItemName = row.SafeString("ItemName"),
                FinalStock = Convert.ToDecimal(row.SafeDouble("FinalStock")),
                CriticalLevelQuantity = Convert.ToDecimal(row.SafeDouble("CriticalLevelQuantity"))
            });
        }

        foreach (DataRow row in reorderTask.Result.Rows)
        {
            result.ReorderStockItems.Add(new ReorderStockItemDto
            {
                ItemName = row.SafeString("ItemName"),
                FinalStock = Convert.ToDecimal(row.SafeDouble("FinalStock")),
                ReorderLevelQuantity = Convert.ToDecimal(row.SafeDouble("ReorderLevelQuantity"))
            });
        }

        return result;
    }

    public async Task<ProductionDashboardDto> GetProductionDashboardAsync(string batchNo, CancellationToken cancellationToken = default)
    {
        var result = new ProductionDashboardDto();
        if (string.IsNullOrWhiteSpace(batchNo)) return result;

        const string usedQuery = @"
            SELECT b.ItemStockByBatchId AS ProductionId, b.ItemId, u.CreatedDate AS FilledDate, i.ItemName, u.Quantity, b.BatchNo AS UsedBatchNo
            FROM Inv_ItemStockUsed u
            JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
            JOIN W_MasterItem i ON b.ItemId = i.ItemID
            JOIN W_PreProcessing pre ON u.UsedForId = pre.PreProcessingId
            WHERE u.UsedFor = 2 AND pre.BatchNumberMade = @BatchNo

            UNION ALL

            SELECT b.ItemStockByBatchId AS ProductionId, b.ItemId, u.CreatedDate AS FilledDate, i.ItemName, u.Quantity, b.BatchNo AS UsedBatchNo
            FROM Inv_ItemStockUsed u
            JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
            JOIN W_MasterItem i ON b.ItemID = i.ItemID
            JOIN W_Production prod ON u.UsedForId = prod.ProductionId
            WHERE u.UsedFor = 3 AND prod.BatchNo = @BatchNo";

        const string soldQuery = @"
            SELECT c.InvoiceChildID AS ProductionId, c.ItemId, m.InvoiceDate AS FilledDate, i.ItemName, c.Quantity, '' AS UsedBatchNo
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN W_MasterItem i ON c.ItemId = i.ItemID
            WHERE c.BatchNumber = @BatchNo AND m.InvoiceStatus = 'Submitted'";

        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            // Fetch Total Used
            using (var cmd = new SqlCommand(usedQuery, connection))
            {
                cmd.Parameters.AddWithValue("@BatchNo", batchNo);
                await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        result.TotalUsedItems.Add(new ProductionDashboardItemDto
                        {
                            ProductionId = Convert.ToInt64(reader["ProductionId"]),
                            ItemId = Convert.ToInt64(reader["ItemId"]),
                            FilledDate = reader["FilledDate"] != DBNull.Value ? Convert.ToDateTime(reader["FilledDate"]) : null,
                            ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                            Quantity = Convert.ToDecimal(reader["Quantity"]),
                            UsedBatchNo = reader["UsedBatchNo"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }

            // Fetch Total Sold
            using (var cmd = new SqlCommand(soldQuery, connection))
            {
                cmd.Parameters.AddWithValue("@BatchNo", batchNo);
                await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        result.TotalSoldItems.Add(new ProductionDashboardItemDto
                        {
                            ProductionId = Convert.ToInt64(reader["ProductionId"]),
                            ItemId = Convert.ToInt64(reader["ItemId"]),
                            FilledDate = reader["FilledDate"] != DBNull.Value ? Convert.ToDateTime(reader["FilledDate"]) : null,
                            ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                            Quantity = Convert.ToDecimal(reader["Quantity"]),
                            UsedBatchNo = reader["UsedBatchNo"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }
        }

        return result;
    }

    public async Task<List<DashboardBatchLookupDto>> GetProductionBatchesAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<DashboardBatchLookupDto>();
        const string query = @"
            SELECT DISTINCT BatchNo FROM (
                SELECT BatchNo FROM Inv_ItemStockByBatch WHERE ISNULL(BatchNo, '') <> ''
                UNION
                SELECT BatchNo FROM Inv_ItemStockByBatchForBOM WHERE ISNULL(BatchNo, '') <> ''
            ) t ORDER BY BatchNo ASC";
        
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection)
        {
            CommandType = CommandType.Text
        };

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new DashboardBatchLookupDto
            {
                BatchNo = reader["BatchNo"]?.ToString() ?? string.Empty
            });
        }

        return list;
    }


    public async Task<SalesDashboardDto> GetSalesDashboardAsync(CancellationToken cancellationToken = default)
    {
        var result = new SalesDashboardDto();

        const string totalSalesQuery = @"
            SELECT YEAR(m.InvoiceDate) AS [Year], i.ItemName, SUM(c.Quantity) AS Quantity, SUM(c.Amount) AS TotalAmount
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN W_MasterItem i ON c.ItemId = i.ItemID
            WHERE m.InvoiceStatus = 'Submitted'
            GROUP BY YEAR(m.InvoiceDate), i.ItemName
            ORDER BY YEAR(m.InvoiceDate) DESC, TotalAmount DESC";

        const string salesPerYearQuery = @"
            SELECT YEAR(m.InvoiceDate) AS [Year], SUM(c.Amount) AS SalesAmount, SUM(c.Quantity) AS QuantitySold
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            WHERE m.InvoiceStatus = 'Submitted'
            GROUP BY YEAR(m.InvoiceDate)
            ORDER BY YEAR(m.InvoiceDate) DESC";

        const string salesPerGroupQuery = @"
            SELECT g.AccountGroupName AS CustomerGroup, SUM(c.Amount) AS TotalAmount, SUM(c.Quantity) AS QuantitySold
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN A_MasterAccounts a ON m.AccountID = a.AccountID
            INNER JOIN A_AccountGroupMaster g ON a.AccountGroupID = g.AccountGroupID
            WHERE m.InvoiceStatus = 'Submitted'
            GROUP BY g.AccountGroupName
            ORDER BY TotalAmount DESC";

        const string salesPerCustomerQuery = @"
            SELECT a.AccountName AS CustomerName, SUM(c.Amount) AS TotalAmount, SUM(c.Quantity) AS QuantitySold
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN A_MasterAccounts a ON m.AccountID = a.AccountID
            WHERE m.InvoiceStatus = 'Submitted'
            GROUP BY a.AccountName
            ORDER BY TotalAmount DESC";

        const string detailedSalesQuery = @"
            SELECT 
                YEAR(m.InvoiceDate) AS [Year],
                mc.CurrencySymbol,
                ISNULL(m.CurrencyConversion, 1.0) AS CurrencyConversion,
                i.ItemName,
                i.ItemCode,
                i.ShortName,
                SUM(c.Quantity) AS TotalQuantity,
                SUM(c.Amount) AS TotalAmount
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN W_MasterItem i ON c.ItemId = i.ItemID
            LEFT JOIN A_MasterCurrency mc ON m.CurrencyID = mc.CurrencyID
            WHERE m.InvoiceStatus = 'Submitted'
            GROUP BY 
                YEAR(m.InvoiceDate),
                mc.CurrencySymbol,
                ISNULL(m.CurrencyConversion, 1.0),
                i.ItemName,
                i.ItemCode,
                i.ShortName
            ORDER BY YEAR(m.InvoiceDate) DESC";

        var totalSalesTask = ExecuteTotalSalesAsync(totalSalesQuery, cancellationToken);
        var salesPerYearTask = ExecuteSalesPerYearAsync(salesPerYearQuery, cancellationToken);
        var salesPerGroupTask = ExecuteSalesPerGroupAsync(salesPerGroupQuery, cancellationToken);
        var salesPerCustomerTask = ExecuteSalesPerCustomerAsync(salesPerCustomerQuery, cancellationToken);
        var detailedSalesTask = ExecuteDetailedSalesAsync(detailedSalesQuery, cancellationToken);

        await Task.WhenAll(totalSalesTask, salesPerYearTask, salesPerGroupTask, salesPerCustomerTask, detailedSalesTask);

        result.TotalSales = totalSalesTask.Result;
        result.SalesPerYear = salesPerYearTask.Result;
        result.SalesPerCustomerGroup = salesPerGroupTask.Result;
        result.SalesPerCustomer = salesPerCustomerTask.Result;
        
        var salesRows = detailedSalesTask.Result;

        var yearGroups = salesRows.GroupBy(r => r.Year).OrderByDescending(g => g.Key);
        foreach (var g in yearGroups)
        {
            var srdItems = g.Where(r => !r.IsUsd).ToList();
            var usdItems = g.Where(r => r.IsUsd).ToList();

            result.AnnualPerformance.Add(new AnnualSalesPerformanceDto
            {
                Year = g.Key,
                SrdBottles = srdItems.Sum(r => r.Quantity),
                SrdLiters = srdItems.Sum(r => r.Liters),
                SrdValue = srdItems.Sum(r => r.Amount),
                UsdBottles = usdItems.Sum(r => r.Quantity),
                UsdLiters = usdItems.Sum(r => r.Liters),
                UsdValue = usdItems.Sum(r => r.Amount),
                TotalSalesSrd = g.Sum(r => r.SrdAmount)
            });
        }

        var allTastes = salesRows.Select(r => r.TasteCode).Where(t => t != "OTHER").Distinct().OrderBy(t => t).ToList();
        if (salesRows.Any(r => r.TasteCode == "OTHER")) allTastes.Add("OTHER");
        result.AllTasteCodes = allTastes;

        foreach (var g in yearGroups)
        {
            var tasteMap = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in allTastes) tasteMap[t] = 0m;

            foreach (var r in g)
            {
                if (tasteMap.ContainsKey(r.TasteCode))
                {
                    tasteMap[r.TasteCode] += r.Liters;
                }
                else
                {
                    tasteMap[r.TasteCode] = r.Liters;
                }
            }

            result.SalesPerTastePerYear.Add(new SalesPerTastePerYearDto
            {
                Year = g.Key,
                TotalLiters = g.Sum(r => r.Liters),
                TasteLiters = tasteMap
            });
        }

        return result;
    }

    private async Task<List<SalesSummaryDto>> ExecuteTotalSalesAsync(string query, CancellationToken cancellationToken)
    {
        var list = new List<SalesSummaryDto>();
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(query, connection);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new SalesSummaryDto
            {
                ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                Year = Convert.ToInt32(reader["Year"]),
                Quantity = Convert.ToDecimal(reader["Quantity"]),
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
            });
        }
        return list;
    }

    private async Task<List<SalesPerYearDto>> ExecuteSalesPerYearAsync(string query, CancellationToken cancellationToken)
    {
        var list = new List<SalesPerYearDto>();
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(query, connection);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new SalesPerYearDto
            {
                Year = Convert.ToInt32(reader["Year"]),
                SalesAmount = Convert.ToDecimal(reader["SalesAmount"]),
                QuantitySold = Convert.ToDecimal(reader["QuantitySold"])
            });
        }
        return list;
    }

    private async Task<List<SalesPerCustomerGroupDto>> ExecuteSalesPerGroupAsync(string query, CancellationToken cancellationToken)
    {
        var list = new List<SalesPerCustomerGroupDto>();
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(query, connection);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new SalesPerCustomerGroupDto
            {
                CustomerGroup = reader["CustomerGroup"]?.ToString() ?? string.Empty,
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                QuantitySold = Convert.ToDecimal(reader["QuantitySold"])
            });
        }
        return list;
    }

    private async Task<List<SalesPerCustomerDto>> ExecuteSalesPerCustomerAsync(string query, CancellationToken cancellationToken)
    {
        var list = new List<SalesPerCustomerDto>();
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(query, connection);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(new SalesPerCustomerDto
            {
                CustomerName = reader["CustomerName"]?.ToString() ?? string.Empty,
                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                QuantitySold = Convert.ToDecimal(reader["QuantitySold"])
            });
        }
        return list;
    }

    private async Task<List<(int Year, decimal Quantity, decimal Amount, bool IsUsd, decimal Liters, string TasteCode, decimal SrdAmount)>> ExecuteDetailedSalesAsync(string query, CancellationToken cancellationToken)
    {
        var salesRows = new List<(int Year, decimal Quantity, decimal Amount, bool IsUsd, decimal Liters, string TasteCode, decimal SrdAmount)>();
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var cmd = new SqlCommand(query, connection);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            int yr = Convert.ToInt32(reader["Year"]);
            decimal qty = Convert.ToDecimal(reader["TotalQuantity"]);
            decimal amt = Convert.ToDecimal(reader["TotalAmount"]);
            string symbol = reader["CurrencySymbol"]?.ToString() ?? string.Empty;
            double convRate = reader["CurrencyConversion"] != DBNull.Value ? Convert.ToDouble(reader["CurrencyConversion"]) : 1.0;
            if (convRate <= 0) convRate = 1.0;

            string itemName = reader["ItemName"]?.ToString() ?? string.Empty;
            string itemCode = reader["ItemCode"]?.ToString() ?? string.Empty;
            string shortName = reader["ShortName"]?.ToString() ?? string.Empty;

            bool isUsd = symbol.Contains("USD", StringComparison.OrdinalIgnoreCase) || symbol.Contains("$", StringComparison.OrdinalIgnoreCase);
            decimal volPerUnit = ParseItemVolumeLiters(shortName, itemCode, itemName);
            decimal liters = qty * volPerUnit;
            string taste = ParseTasteCode(shortName, itemCode, itemName);

            decimal srdAmount = isUsd ? (decimal)((double)amt * convRate) : amt;

            salesRows.Add((yr, qty, amt, isUsd, liters, taste, srdAmount));
        }
        return salesRows;
    }

    private static decimal ParseItemVolumeLiters(string? shortName, string? itemCode, string? itemName)
    {
        var text = $"{shortName} {itemCode} {itemName}".ToLowerInvariant();

        if (text.Contains("350ml") || text.Contains("350_ml")) return 0.35m;
        if (text.Contains("250ml") || text.Contains("250_ml")) return 0.25m;
        if (text.Contains("500ml") || text.Contains("500_ml")) return 0.50m;
        if (text.Contains("750ml") || text.Contains("750_ml")) return 0.75m;
        if (text.Contains("1000ml") || text.Contains("1000_ml") || text.Contains("1ltr") || text.Contains("1_ltr")) return 1.0m;
        if (text.Contains("2ltr") || text.Contains("2_ltr")) return 2.0m;
        if (text.Contains("4ltr") || text.Contains("4_ltr")) return 4.0m;
        if (text.Contains("5ltr") || text.Contains("5_ltr")) return 5.0m;
        if (text.Contains("10ltr") || text.Contains("10_ltr")) return 10.0m;
        if (text.Contains("20ltr") || text.Contains("20_ltr")) return 20.0m;
        if (text.Contains("200ltr") || text.Contains("200_ltr")) return 200.0m;

        var match = System.Text.RegularExpressions.Regex.Match(text, @"(\d+(?:\.\d+)?)\s*(ml|ltr|liter|l)\b");
        if (match.Success && decimal.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var num))
        {
            var unit = match.Groups[2].Value;
            if (unit == "ml") return num / 1000m;
            return num;
        }

        return 1.0m;
    }

    private static string ParseTasteCode(string? shortName, string? itemCode, string? itemName)
    {
        var str = !string.IsNullOrWhiteSpace(shortName) ? shortName : (!string.IsNullOrWhiteSpace(itemCode) ? itemCode : itemName);
        if (string.IsNullOrWhiteSpace(str)) return "OTHER";

        var parts = str.Trim().Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0)
        {
            var code = parts[0].ToUpperInvariant();
            if (code.Length >= 2 && code.Length <= 5) return code;
        }
        return "OTHER";
    }

    private async Task<DataTable> ExecuteDataTableAsync(string storedProcedure, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(storedProcedure, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var table = new DataTable();
        table.Load(reader);
        return table;
    }
}
