using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class PreProcessingRepository : IPreProcessingRepository
{
    private readonly string _connectionString;

    public PreProcessingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<PreProcessingDto>> GetPreProcessingListAsync(PreProcessingListRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@PreProcessingId", SqlDbType.Int) { Value = (object?)request.PreProcessingId ?? 0 },
            new("@CreatedBy", SqlDbType.NVarChar, 200) { Value = (object?)request.CreatedBy ?? DBNull.Value },
            new("@BomId", SqlDbType.Int) { Value = (object?)request.BomId ?? 0 },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "PreProcessingId" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_PreProcessingList", parameters, cancellationToken);
            
            return new PagedResult<PreProcessingDto>
            {
                CurrentPage = Convert.ToInt32(parameters[3].Value ?? request.CurrentPage),
                TotalRecord = Convert.ToInt32(parameters[5].Value ?? 0),
                Items = table.AsEnumerable().Select(MapPreProcessing).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetPreProcessingListAsync (Legacy SP: W_PreProcessingList): {ex.Message}", ex);
        }
    }

    public async Task<PreProcessingDto?> GetPreProcessingByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@PreProcessingId", SqlDbType.Int) { Value = (int)id }
        };

        try
        {
            // Note: W_PreProcessingList can also fetch by ID efficiently
            var table = await ExecuteDataTableAsync("W_PreProcessingList", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault();
            return row == null ? null : MapPreProcessing(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetPreProcessingByIdAsync (Legacy SP: W_PreProcessingList): {ex.Message}", ex);
        }
    }

    public async Task<int> SavePreProcessingAsync(SavePreProcessingRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@PreProcessingId", SqlDbType.BigInt) { Value = request.PreProcessingId },
            new SqlParameter("@BomId", SqlDbType.BigInt) { Value = request.BomId },
            new SqlParameter("@BomQty", SqlDbType.Int) { Value = request.BomQty },
            new SqlParameter("@ProcessingDate", SqlDbType.Date) { Value = (object?)request.ProcessingDate ?? DateTime.Today },
            new SqlParameter("@QuantityMade", SqlDbType.Float) { Value = request.QuantityMade },
            new SqlParameter("@UnitMade", SqlDbType.BigInt) { Value = request.UnitMade },
            new SqlParameter("@BatchNumberMade", SqlDbType.NVarChar, 200) { Value = (object?)request.BatchNumberMade ?? DBNull.Value },
            new SqlParameter("@ExpiryDate", SqlDbType.DateTime) { Value = (object?)request.ExpiryDate ?? DateTime.Today.AddYears(1) },
            new SqlParameter("@ProcessEmployees", SqlDbType.NVarChar, 200) { Value = (object?)request.ProcessEmployees ?? DBNull.Value },
            new SqlParameter("@Remarks", SqlDbType.NVarChar, 2000) { Value = (object?)request.Remarks ?? DBNull.Value },
            new SqlParameter("@DocumentUpload", SqlDbType.NVarChar, 1000) { Value = (object?)request.DocumentUpload ?? string.Empty },
            new SqlParameter("@WarehouseId", SqlDbType.BigInt) { Value = request.WarehouseId },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_PreProcessingModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeletePreProcessingAsync(long id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = id.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 = Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 200) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_PreProcessingOperation", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<IReadOnlyList<PreProcessingItemDto>> GetPreProcessingItemsAsync(long preProcessingId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@UsedForId", SqlDbType.Int) { Value = (int)preProcessingId },
            new("@UsedFor", SqlDbType.Int) { Value = 2 } // 2 = Pre-Processing
        };

        try
        {
            var table = await ExecuteDataTableAsync("Inv_ItemStockUsedById", parameters, cancellationToken);
            return table.AsEnumerable().Select(MapPreProcessingItem).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetPreProcessingItemsAsync (Legacy SP: Inv_ItemStockUsedById): {ex.Message}", ex);
        }
    }

    public async Task<int> SavePreProcessingItemAsync(SavePreProcessingItemRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemStockUsedID", SqlDbType.BigInt) { Value = request.ItemStockUsedID },
            new SqlParameter("@ItemStockByBatchId", SqlDbType.BigInt) { Value = request.ItemStockByBatchId },
            new SqlParameter("@UsedFor", SqlDbType.Int) { Value = request.UsedFor },
            new SqlParameter("@UsedForId", SqlDbType.BigInt) { Value = request.UsedForId },
            new SqlParameter("@Quantity", SqlDbType.Float) { Value = request.Quantity },
            new SqlParameter("@Description", SqlDbType.NVarChar, 1000) { Value = (object?)request.Description ?? DBNull.Value },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Inv_ItemStockUsedModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeletePreProcessingItemAsync(long itemStockUsedId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(
                "DELETE FROM dbo.Inv_ItemStockUsed WHERE ItemStockUsedID = @ItemStockUsedID",
                connection)
            {
                CommandType = CommandType.Text
            };
            command.Parameters.Add(new SqlParameter("@ItemStockUsedID", SqlDbType.BigInt) { Value = itemStockUsedId });
            await connection.OpenAsync(cancellationToken);
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting consumption item (ItemStockUsedID={itemStockUsedId}): {ex.Message}", ex);
        }
    }

    public async Task<int> FinalizeStockUpdateAsync(long preProcessingId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@UsedFor", SqlDbType.Int) { Value = 2 }, // 2 = Pre-Processing
            new SqlParameter("@UsedForId", SqlDbType.BigInt) { Value = preProcessingId },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = updatedBy },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Inv_ItemStockPreProcessingAndProductModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<IReadOnlyList<BomItemDetailDto>> GetBomItemsAsync(long bomId, CancellationToken cancellationToken = default)
    {
        if (bomId <= 0)
        {
            return Array.Empty<BomItemDetailDto>();
        }

        var parameters = new List<SqlParameter>
        {
            new("@BomId", SqlDbType.BigInt) { Value = bomId },
            new("@CurrentPage", SqlDbType.Int) { Value = 1 },
            new("@RecordPerPage", SqlDbType.Int) { Value = 1000 },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterBomItemsList", parameters, cancellationToken);
            return table.AsEnumerable().Select(r => new BomItemDetailDto
            {
                BomId = r.SafeLong("BomId"),
                BomName = r.SafeString("BomName"),
                ItemID = r.SafeLong("ItemID"),
                ItemName = r.SafeString("ItemName"),
                Quantity = r.SafeDouble("Quantity"),
                UnitId = r.SafeLong("UnitId")
            }).ToList();
        }
        catch (Exception ex)
        {
             throw new Exception($"Error in GetBomItemsAsync (Legacy SP: W_MasterBomItemsList): {ex.Message}", ex);
        }
    }

    public async Task<IReadOnlyList<BomLookupDto>> GetBomsLookupAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(
                "SELECT BomId, BomName, ItemTypeId FROM dbo.W_MasterBom WHERE IsActive = 1", 
                connection)
            {
                CommandType = CommandType.Text
            };
            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var table = new DataTable();
            table.Load(reader);

            return table.AsEnumerable().Select(r => new BomLookupDto
            {
                BomId = r.SafeLong("BomId"),
                BomName = r.SafeString("BomName"),
                ItemTypeId = r.SafeLong("ItemTypeId", "ItemTypeID")
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetBomsLookupAsync (Custom SQL Query): {ex.Message}", ex);
        }
    }

    public async Task<IReadOnlyList<WarehouseLookupDto>> GetWarehousesLookupAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var table = await ExecuteDataTableAsync("W_MasterWarehouseSelectAll", new List<SqlParameter>(), cancellationToken);
            return table.AsEnumerable().Select(r => new WarehouseLookupDto
            {
                WarehouseId = r.SafeLong("WarehouseId"),
                WarehouseName = r.SafeString("WarehouseName")
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetWarehousesLookupAsync (Legacy SP: W_MasterWarehouseSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<IReadOnlyList<AvailableBatchDto>> GetAvailableBatchesAsync(long itemId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemID", SqlDbType.Int) { Value = (int)itemId },
            new("@CurrentPage", SqlDbType.Int) { Value = 1 },
            new("@RecordPerPage", SqlDbType.Int) { Value = 500 },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        try
        {
            // Using Inv_ItemStockByBatchList which is similar to SelectAll but supports filtering
            var table = await ExecuteDataTableAsync("Inv_ItemStockByBatchList", parameters, cancellationToken);
            return table.AsEnumerable()
                .Where(r => r.SafeDouble("FinalQuantityLeft") > 0)
                .Select(r => new AvailableBatchDto
                {
                    ItemStockByBatchId = r.SafeLong("ItemStockByBatchId"),
                    BatchNo = r.SafeString("BatchNo"),
                    FinalQuantityLeft = r.SafeDouble("FinalQuantityLeft"),
                    ExpiryDate = r.SafeString("ExpiryDate"),
                    WarehouseName = r.SafeString("WarehouseName")
                }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetAvailableBatchesAsync (Legacy SP: Inv_ItemStockByBatchList): {ex.Message}", ex);
        }
    }

    private PreProcessingDto MapPreProcessing(DataRow row) => new()
    {
        PreProcessingId = row.SafeLong("PreProcessingId"),
        BomId = row.SafeLong("BomId"),
        BomName = row.SafeString("BomName"),
        BomQty = row.SafeInt("BomQty"),
        ProcessingDate = row.SafeString("ProcessingDate"), // SP returns as string formatted 106
        QuantityMade = row.SafeDouble("QuantityMade"),
        UnitMade = row.SafeLong("UnitMade"),
        UnitName = row.SafeString("UnitName"),
        BatchNumberMade = row.SafeString("BatchNumberMade"),
        ExpiryDate = row.SafeString("ExpiryDate"),
        ProcessEmployees = row.SafeString("ProcessEmployees"),
        Remarks = row.SafeString("Remarks"),
        DocumentUpload = row.SafeString("DocumentUpload"),
        WarehouseId = row.SafeLong("WarehouseId"),
        WarehouseName = row.SafeString("WarehouseName"),
        ItemName = row.SafeString("ItemName"),
        IsComplete = row.SafeInt("IsComplete"),
        CreatedBy = row.SafeString("CreatedBy"),
        CreatedDate = row.SafeDateTime("CreatedDate") ?? DateTime.MinValue
    };

    private PreProcessingItemDto MapPreProcessingItem(DataRow row) => new()
    {
        ItemStockUsedID = row.SafeLong("ItemStockUsedID"),
        UsedForId = row.SafeLong("UsedForId"),
        ItemStockByBatchId = row.SafeLong("ItemStockByBatchId"),
        ItemID = row.SafeLong("ItemID"),
        ItemName = row.SafeString("ItemName"),
        BatchNo = row.SafeString("BatchNo"),
        Quantity = row.SafeDouble("Quantity"),
        Description = row.SafeString("Description")
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

    private async Task<int> ExecuteNonQueryAsync(string storedProcedure, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
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
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
