using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class ReportRepository : IReportRepository
{
    private readonly string _connectionString;

    public ReportRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<ItemStockReportDto>> GetItemStockReportAsync(ItemStockReportRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@StockID", SqlDbType.BigInt) { Value = request.StockID ?? 0 },
            new("@CreatedBy", SqlDbType.VarChar, 200) { Value = request.CreatedBy ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "StockID" },
            new("@ItemID", SqlDbType.BigInt) { Value = request.ItemID ?? 0 },
            new("@WarehouseID", SqlDbType.BigInt) { Value = request.WarehouseID ?? 0 },
            new("@ItemTypeId", SqlDbType.Int) { Value = request.ItemTypeId ?? 0 },
            new("@FromDate", SqlDbType.Date) { Value = (object?)request.FromDate ?? DBNull.Value },
            new("@ToDate", SqlDbType.Date) { Value = (object?)request.ToDate ?? DBNull.Value }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_ItemStockList", parameters, cancellationToken);
            
            return new PagedResult<ItemStockReportDto>
            {
                CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapItemStockReport).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemStockReportAsync: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<ItemStockTraceabilityDto>> GetItemStockTraceabilityAsync(long itemId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemId", SqlDbType.BigInt) { Value = itemId }
        };

        try
        {
            var table = await ExecuteDataTableAsync("Rpt_ItemStockTraceability", parameters, cancellationToken);
            return table.AsEnumerable().Select(MapItemStockTraceability).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemStockTraceabilityAsync: {ex.Message}", ex);
        }
    }

    public async Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByBatchNoAsync(string batchNo, int page, int size, string sortType, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@BatchNo", SqlDbType.NVarChar, 50) { Value = batchNo },
            new("@CurrentPage", SqlDbType.Int) { Value = page, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = size },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortType }
        };

        try
        {
            var table = await ExecuteDataTableAsync("BatchNumberList", parameters, cancellationToken);
            
            return new PagedResult<BatchWiseItemDto>
            {
                CurrentPage = parameters[1].Value == DBNull.Value ? page : Convert.ToInt32(parameters[1].Value),
                TotalRecord = parameters[3].Value == DBNull.Value ? 0 : Convert.ToInt32(parameters[3].Value),
                Items = table.AsEnumerable().Select(MapBatchWiseItem).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetBatchWiseItemsByBatchNoAsync: {ex.Message}", ex);
        }
    }

    public async Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByItemAsync(long itemId, int page, int size, string sortType, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemID", SqlDbType.Int) { Value = (int)itemId },
            new("@CurrentPage", SqlDbType.Int) { Value = page, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = size },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortType }
        };

        try
        {
            var table = await ExecuteDataTableAsync("ItemBatchNumberList", parameters, cancellationToken);
            
            return new PagedResult<BatchWiseItemDto>
            {
                CurrentPage = parameters[1].Value == DBNull.Value ? page : Convert.ToInt32(parameters[1].Value),
                TotalRecord = parameters[3].Value == DBNull.Value ? 0 : Convert.ToInt32(parameters[3].Value),
                Items = table.AsEnumerable().Select(MapBatchWiseItem).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetBatchWiseItemsByItemAsync: {ex.Message}", ex);
        }
    }

    public async Task<PagedResult<ItemStockByBatchReportDto>> GetItemStockByBatchReportAsync(long? itemStockByBatchId, long? stockById, long? itemId, int page, int size, string sortCol, string sortOrd, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemStockByBatchId", SqlDbType.Int) { Value = (int)(itemStockByBatchId ?? 0) },
            new("@StockById", SqlDbType.Int) { Value = (int)(stockById ?? 0) },
            new("@ItemID", SqlDbType.Int) { Value = (int)(itemId ?? 0) },
            new("@CurrentPage", SqlDbType.Int) { Value = page, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = size },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortOrd },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = sortCol }
        };

        try
        {
            var table = await ExecuteDataTableAsync("Inv_ItemStockByBatchList", parameters, cancellationToken);
            
            return new PagedResult<ItemStockByBatchReportDto>
            {
                CurrentPage = Convert.ToInt32(parameters[3].Value ?? page),
                TotalRecord = Convert.ToInt32(parameters[5].Value ?? 0),
                Items = table.AsEnumerable().Select(MapItemStockByBatchReport).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemStockByBatchReportAsync: {ex.Message}", ex);
        }
    }

    private ItemStockReportDto MapItemStockReport(DataRow row) => new()
    {
        StockID = row.SafeLong("StockID"),
        ItemID = row.SafeLong("ItemID"),
        ItemName = row.SafeString("ItemName"),
        ItemCode = row.SafeString("ItemCode"),
        WarehouseName = row.SafeString("WarehouseName"),
        OpeningQuantity = row.SafeDouble("OpeningQuantity"),
        PurchasedQuantity = row.SafeDouble("PurchasedQuantity"),
        ProducedQuantity = row.SafeDouble("ProducedQuantity"),
        IssuedQuantity = row.SafeDouble("IssuedQuantity"),
        RemovedQuantity = row.SafeDouble("RemovedQuantity"),
        FinalStock = row.SafeDouble("FinalStock"),
        UnitId = row.SafeLong("UnitId"),
        TotalValue = row.SafeDouble("TotalValue")
    };

    public async Task<IEnumerable<ItemStockTraceabilityDto>> GetBatchTraceabilityAsync(string batchNo, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@BatchNo", SqlDbType.NVarChar, 50) { Value = batchNo }
        };

        var table = await ExecuteDataTableAsync("Rpt_BatchTraceability", parameters, cancellationToken);
        return table.AsEnumerable().Select(MapItemStockTraceability).ToList();
    }

    private ItemStockTraceabilityDto MapItemStockTraceability(DataRow row) => new()
    {
        TransactionDate = row.IsNull("TransactionDate") ? null : row.Field<DateTime?>("TransactionDate"),
        BatchNo = row.SafeString("BatchNo"),
        TransactionType = row.SafeString("TransactionType"),
        Reference = row.SafeString("Reference"),
        InQty = row.SafeDouble("InQty"),
        OutQty = row.SafeDouble("OutQty"),
        TotalValue = row.SafeDouble("TotalValue"),
        UnitName = row.SafeString("UnitName"),
        ItemName = row.SafeString("ItemName"),
        RefUsedFor = row.SafeInt("RefUsedFor"),
        RefUsedForId = row.SafeLong("RefUsedForId")
    };

    public async Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsPagedAsync(string? batchNo, long? itemId, long? itemTypeId, bool inStockOnly, int page, int size, string sortType, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        size = Math.Max(1, size);
        var sortOrd = string.Equals(sortType, "ASC", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";

        var whereConditions = new List<string>();
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrWhiteSpace(batchNo))
        {
            whereConditions.Add("t5.BatchNo LIKE @BatchNo");
            parameters.Add(new SqlParameter("@BatchNo", SqlDbType.NVarChar, 50) { Value = $"%{batchNo.Trim()}%" });
        }

        if (itemId.HasValue && itemId.Value > 0)
        {
            whereConditions.Add("t5.ItemId = @ItemId");
            parameters.Add(new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = itemId.Value });
        }

        if (itemTypeId.HasValue && itemTypeId.Value > 0)
        {
            whereConditions.Add("t3.ItemTypeId = @ItemTypeId");
            parameters.Add(new SqlParameter("@ItemTypeId", SqlDbType.BigInt) { Value = itemTypeId.Value });
        }

        if (inStockOnly)
        {
            whereConditions.Add("t5.FinalQuantityLeft > 0");
        }

        whereConditions.Add(@"(
            t5.StockById NOT IN (2, 4)
            OR (t5.StockById = 2 AND EXISTS (SELECT 1 FROM dbo.W_PreProcessing pp WHERE pp.PreProcessingId = t5.IdFrom AND pp.IsComplete = 1))
            OR (t5.StockById = 4 AND EXISTS (
                SELECT 1 FROM dbo.W_Production p 
                WHERE p.ProductionId = t5.IdFrom 
                AND NOT EXISTS (
                    SELECT 1 FROM dbo.W_MasterBomItems AS Bom 
                    WHERE Bom.BomId = p.BomId 
                    AND NOT EXISTS (
                        SELECT 1 FROM dbo.inv_itemstockused AS StockUsed 
                        WHERE StockUsed.UsedFor = 3 AND StockUsed.UsedForId = p.ProductionId
                    )
                )
            ))
        )");

        string whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

        string countSql = $@"
            SELECT COUNT(1)
            FROM Inv_ItemStockByBatch t5
            LEFT JOIN W_MasterItem t3 ON t5.ItemId = t3.ItemID
            {whereClause}";

        int totalRecords = 0;
        var items = new List<BatchWiseItemDto>();

        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            using (var countCmd = new SqlCommand(countSql, connection))
            {
                foreach (var p in parameters) countCmd.Parameters.Add((SqlParameter)((ICloneable)p).Clone());
                var countRes = await countCmd.ExecuteScalarAsync(cancellationToken);
                totalRecords = countRes != null && countRes != DBNull.Value ? Convert.ToInt32(countRes) : 0;
            }

            int offset = (page - 1) * size;
            string dataSql = $@"
                SELECT 
                    t5.ItemStockByBatchId AS Id,
                    t5.BatchNo,
                    t5.ExpiryDate,
                    CASE
                        WHEN t5.StockById = 1 THEN t2.GoodsRecievedDate
                        WHEN t5.StockById = 2 OR t5.StockById = 4 THEN prod.CookingDate
                        WHEN t5.StockById = 3 THEN stock.OpeningStockDate
                    END AS ProcessingDate,
                    t3.ItemName,
                    t3.ItemTypeId,
                    it.ItemTypeName,
                    CASE
                        WHEN t5.StockById = 1 THEN t4.AccountName
                        WHEN t5.StockById = 2 OR t5.StockById = 4 THEN 'Production'
                        WHEN t5.StockById = 3 THEN 'Opening Stock'
                        ELSE 'Manual Entry'
                    END AS AccountName,
                    t5.FinalQuantityLeft AS AvailableQty,
                    mu.UnitName
                FROM Inv_ItemStockByBatch t5
                LEFT JOIN W_MasterItem t3 ON t5.ItemId = t3.ItemID
                LEFT JOIN W_MasterItemType it ON t3.ItemTypeId = it.ItemTypeId
                LEFT JOIN W_MasterUnit mu ON t5.Unit = mu.UnitId
                LEFT JOIN W_PurchaseChild t1 ON t5.IdFrom = t1.PurchaseItemID AND t5.StockById = 1
                LEFT JOIN W_PurchaseMaster t2 ON t1.PurchaseID = t2.PurchaseID
                LEFT JOIN A_MasterAccounts t4 ON t4.AccountId = t2.AccountID
                LEFT JOIN W_Production prod ON t5.IdFrom = prod.ProductionId AND (t5.StockById = 2 OR t5.StockById = 4)
                LEFT JOIN W_ItemStock stock ON t5.IdFrom = stock.StockID AND t5.StockById = 3
                {whereClause}
                ORDER BY t5.ItemStockByBatchId {sortOrd}
                OFFSET {offset} ROWS FETCH NEXT {size} ROWS ONLY";

            using (var dataCmd = new SqlCommand(dataSql, connection))
            {
                foreach (var p in parameters) dataCmd.Parameters.Add((SqlParameter)((ICloneable)p).Clone());
                await using (var reader = await dataCmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        items.Add(new BatchWiseItemDto
                        {
                            Id = Convert.ToInt64(reader["Id"]),
                            BatchNo = reader["BatchNo"]?.ToString(),
                            ProcessingDate = reader["ProcessingDate"] != DBNull.Value ? Convert.ToDateTime(reader["ProcessingDate"]).ToString("yyyy-MM-dd") : null,
                            ExpiryDate = reader["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpiryDate"]).ToString("yyyy-MM-dd") : null,
                            ItemName = reader["ItemName"]?.ToString(),
                            AccountName = reader["AccountName"]?.ToString(),
                            AvailableQty = reader["AvailableQty"] != DBNull.Value ? Convert.ToDouble(reader["AvailableQty"]) : 0,
                            UnitName = reader["UnitName"]?.ToString(),
                            ItemTypeId = reader["ItemTypeId"] != DBNull.Value ? Convert.ToInt64(reader["ItemTypeId"]) : 0,
                            ItemTypeName = reader["ItemTypeName"]?.ToString()
                        });
                    }
                }
            }
        }

        return new PagedResult<BatchWiseItemDto>
        {
            CurrentPage = page,
            TotalRecord = totalRecords,
            Items = items
        };
    }

    private BatchWiseItemDto MapBatchWiseItem(DataRow row) => new()
    {
        Id = row.SafeLong("Id"),
        BatchNo = row.SafeString("BatchNo"),
        ProcessingDate = row.IsNull("ProcessingDate") || row["ProcessingDate"] == DBNull.Value ? null : Convert.ToDateTime(row["ProcessingDate"]).ToString("yyyy-MM-dd"),
        ExpiryDate = row.IsNull("ExpiryDate") || row["ExpiryDate"] == DBNull.Value ? null : Convert.ToDateTime(row["ExpiryDate"]).ToString("yyyy-MM-dd"),
        ItemName = row.SafeString("ItemName"),
        AccountName = row.SafeString("AccountName"),
        AvailableQty = row.SafeDouble("AvailableQty"),
        UnitName = row.SafeString("UnitName"),
        ItemTypeId = row.Table.Columns.Contains("ItemTypeId") ? row.SafeLong("ItemTypeId") : 0,
        ItemTypeName = row.Table.Columns.Contains("ItemTypeName") ? row.SafeString("ItemTypeName") : string.Empty
    };


    private ItemStockByBatchReportDto MapItemStockByBatchReport(DataRow row) => new()
    {
        ItemStockByBatchId = row.SafeLong("ItemStockByBatchId"),
        StockById = row.SafeInt("StockById"),
        IdFrom = row.SafeLong("IdFrom"),
        ItemId = row.SafeLong("ItemId"),
        Quantity = row.SafeDouble("Quantity"),
        Unit = row.SafeInt("Unit"),
        BatchNo = row.SafeString("BatchNo"),
        ExpiryDate = row.IsNull("ExpiryDate") || row["ExpiryDate"] == DBNull.Value ? null : Convert.ToDateTime(row["ExpiryDate"]).ToString("yyyy-MM-dd"),
        WarehouseId = row.SafeLong("WarehouseId"),
        FinalQuantityLeft = row.SafeDouble("FinalQuantityLeft"),
        ItemName = row.SafeString("ItemName"),
        UnitName = row.SafeString("UnitName"),
        WarehouseName = row.SafeString("WarehouseName"),
        IssuedStock = row.SafeDouble("IssuedStock"),
        StockValue = row.SafeDouble("StockValue")
    };

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
