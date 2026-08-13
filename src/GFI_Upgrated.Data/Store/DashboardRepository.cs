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

        // 1. Critical Stock Level (value = 1)
        var criticalParams = new[] { new SqlParameter("@value", SqlDbType.Int) { Value = 1 } };
        var criticalTable = await ExecuteDataTableAsync("ItemStockAlert", criticalParams, cancellationToken);
        foreach (DataRow row in criticalTable.Rows)
        {
            result.CriticalStockItems.Add(new CriticalStockItemDto
            {
                ItemName = row.SafeString("ItemName"),
                FinalStock = Convert.ToDecimal(row.SafeDouble("FinalStock")),
                CriticalLevelQuantity = Convert.ToDecimal(row.SafeDouble("CriticalLevelQuantity"))
            });
        }

        // 2. Reorder Stock Level (value = 2)
        var reorderParams = new[] { new SqlParameter("@value", SqlDbType.Int) { Value = 2 } };
        var reorderTable = await ExecuteDataTableAsync("ItemStockAlert", reorderParams, cancellationToken);
        foreach (DataRow row in reorderTable.Rows)
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
            SELECT b.ItemStockByBatchId AS ProductionId, b.ItemId, u.CreatedDate AS FilledDate, i.ItemName, u.Quantity
            FROM Inv_ItemStockUsed u
            JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
            JOIN W_MasterItem i ON b.ItemId = i.ItemID
            JOIN W_PreProcessing pre ON u.UsedForId = pre.PreProcessingId
            WHERE u.UsedFor = 2 AND pre.BatchNumberMade = @BatchNo

            UNION ALL

            SELECT b.ItemStockByBatchId AS ProductionId, b.ItemId, u.CreatedDate AS FilledDate, i.ItemName, u.Quantity
            FROM Inv_ItemStockUsed u
            JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
            JOIN W_MasterItem i ON b.ItemID = i.ItemID
            JOIN W_Production prod ON u.UsedForId = prod.ProductionId
            WHERE u.UsedFor = 3 AND prod.BatchNo = @BatchNo";

        const string soldQuery = @"
            SELECT c.InvoiceChildID AS ProductionId, c.ItemId, m.InvoiceDate AS FilledDate, i.ItemName, c.Quantity
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN W_MasterItem i ON c.ItemId = i.ItemID
            WHERE c.BatchNumber = @BatchNo";

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
                            Quantity = Convert.ToDecimal(reader["Quantity"])
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
                            Quantity = Convert.ToDecimal(reader["Quantity"])
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
        const string query = "SELECT DISTINCT ItemStockByBatchId AS ProductionId, BatchNo FROM Inv_ItemStockByBatch WHERE ISNULL(BatchNo, '') <> '' ORDER BY BatchNo ASC";
        
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
                ProductionId = Convert.ToInt64(reader["ProductionId"]),
                BatchNo = reader["BatchNo"]?.ToString() ?? string.Empty
            });
        }

        return list;
    }


    public async Task<SalesDashboardDto> GetSalesDashboardAsync(CancellationToken cancellationToken = default)
    {
        var result = new SalesDashboardDto();

        // 1. Total Sales Query
        const string totalSalesQuery = @"
            SELECT TOP 50 i.ItemName, m.InvoiceDate AS TransactionDate, c.Quantity, c.Amount AS TotalAmount
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN W_MasterItem i ON c.ItemId = i.ItemID
            ORDER BY m.InvoiceDate DESC";

        // 2. Sales Per Year Query
        const string salesPerYearQuery = @"
            SELECT YEAR(m.InvoiceDate) AS [Year], SUM(c.Amount) AS SalesAmount, SUM(c.Quantity) AS QuantitySold
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            GROUP BY YEAR(m.InvoiceDate)
            ORDER BY YEAR(m.InvoiceDate) DESC";

        // 3. Sales Per Customer Group Query
        const string salesPerGroupQuery = @"
            SELECT g.AccountGroupName AS CustomerGroup, SUM(c.Amount) AS TotalAmount, SUM(c.Quantity) AS QuantitySold
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN A_MasterAccounts a ON m.AccountID = a.AccountID
            INNER JOIN A_AccountGroupMaster g ON a.AccountGroupID = g.AccountGroupID
            GROUP BY g.AccountGroupName
            ORDER BY TotalAmount DESC";

        // 4. Sales Per Customer Query
        const string salesPerCustomerQuery = @"
            SELECT a.AccountName AS CustomerName, SUM(c.Amount) AS TotalAmount, SUM(c.Quantity) AS QuantitySold
            FROM A_InvoiceChild c
            INNER JOIN A_InvoiceMaster m ON c.InvoiceID = m.InvoiceID
            INNER JOIN A_MasterAccounts a ON m.AccountID = a.AccountID
            GROUP BY a.AccountName
            ORDER BY TotalAmount DESC";

        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            // Fetch Total Sales
            using (var cmd = new SqlCommand(totalSalesQuery, connection))
            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.TotalSales.Add(new SalesSummaryDto
                    {
                        ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                        TransactionDate = reader["TransactionDate"] != DBNull.Value ? Convert.ToDateTime(reader["TransactionDate"]) : null,
                        Quantity = Convert.ToDecimal(reader["Quantity"]),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                    });
                }
            }

            // Fetch Sales Per Year
            using (var cmd = new SqlCommand(salesPerYearQuery, connection))
            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.SalesPerYear.Add(new SalesPerYearDto
                    {
                        Year = Convert.ToInt32(reader["Year"]),
                        SalesAmount = Convert.ToDecimal(reader["SalesAmount"]),
                        QuantitySold = Convert.ToDecimal(reader["QuantitySold"])
                    });
                }
            }

            // Fetch Sales Per Customer Group
            using (var cmd = new SqlCommand(salesPerGroupQuery, connection))
            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.SalesPerCustomerGroup.Add(new SalesPerCustomerGroupDto
                    {
                        CustomerGroup = reader["CustomerGroup"]?.ToString() ?? string.Empty,
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        QuantitySold = Convert.ToDecimal(reader["QuantitySold"])
                    });
                }
            }

            // Fetch Sales Per Customer
            using (var cmd = new SqlCommand(salesPerCustomerQuery, connection))
            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    result.SalesPerCustomer.Add(new SalesPerCustomerDto
                    {
                        CustomerName = reader["CustomerName"]?.ToString() ?? string.Empty,
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        QuantitySold = Convert.ToDecimal(reader["QuantitySold"])
                    });
                }
            }
        }

        return result;
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
