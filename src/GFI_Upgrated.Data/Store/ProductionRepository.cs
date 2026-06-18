using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class ProductionRepository : IProductionRepository
{
    private readonly string _connectionString;

    public ProductionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<ProductionDto>> GetProductionListAsync(ProductionListRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ProductionId", SqlDbType.BigInt) { Value = (object?)request.ProductionId ?? 0 },
            new("@Remarks", SqlDbType.VarChar, 200) { Value = (object?)request.Remarks ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "ProductionId" },
            new("@BomId", SqlDbType.BigInt) { Value = (object?)request.BomId ?? 0 },
            new("@SkuId", SqlDbType.BigInt) { Value = (object?)request.SkuId ?? 0 }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_ProductionList", parameters, cancellationToken);
            
            return new PagedResult<ProductionDto>
            {
                CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapProduction).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetProductionListAsync (Legacy SP: W_ProductionList): {ex.Message}", ex);
        }
    }

    public async Task<ProductionDto?> GetProductionByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ProductionId", SqlDbType.BigInt) { Value = id }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_ProductionSelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault();
            return row == null ? null : MapProduction(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetProductionByIdAsync (Legacy SP: W_ProductionSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveProductionAsync(SaveProductionRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ProductionId", SqlDbType.BigInt) { Value = request.ProductionId },
            new SqlParameter("@BomId", SqlDbType.BigInt) { Value = request.BomId },
            new SqlParameter("@BomQty", SqlDbType.BigInt) { Value = request.BomQty },
            new SqlParameter("@CookingDate", SqlDbType.DateTime) { Value = (object?)request.CookingDate ?? DBNull.Value },
            new SqlParameter("@ExpiryDate", SqlDbType.DateTime) { Value = (object?)request.ExpiryDate ?? DBNull.Value },
            new SqlParameter("@FilledDate", SqlDbType.DateTime) { Value = (object?)request.FilledDate ?? DBNull.Value },
            new SqlParameter("@PackedCountry", SqlDbType.VarChar, 200) { Value = (object?)request.PackedCountry ?? DBNull.Value },
            new SqlParameter("@Colli", SqlDbType.VarChar, 100) { Value = (object?)request.Colli ?? DBNull.Value },
            new SqlParameter("@PalletNumber", SqlDbType.VarChar, 50) { Value = (object?)request.PalletNumber ?? DBNull.Value },
            new SqlParameter("@ProcessEmployees", SqlDbType.VarChar, 200) { Value = (object?)request.ProcessEmployees ?? DBNull.Value },
            new SqlParameter("@KettleId", SqlDbType.VarChar, 200) { Value = (object?)request.KettleId ?? DBNull.Value },
            new SqlParameter("@KettleRun", SqlDbType.Int) { Value = request.KettleRun },
            new SqlParameter("@BatchNo", SqlDbType.NVarChar, 50) { Value = (object?)request.BatchNo ?? DBNull.Value },
            new SqlParameter("@SkuId", SqlDbType.BigInt) { Value = request.SkuId == 0 ? DBNull.Value : request.SkuId },
            new SqlParameter("@DocumentUpload", SqlDbType.VarChar, 200) { Value = (object?)request.DocumentUpload ?? string.Empty },
            new SqlParameter("@FillingBottles", SqlDbType.Int) { Value = request.FillingBottles },
            new SqlParameter("@FillingPerBottleUnit", SqlDbType.BigInt) { Value = request.FillingPerBottleUnit == 0 ? DBNull.Value : request.FillingPerBottleUnit },
            new SqlParameter("@ExtraBottles", SqlDbType.Float) { Value = request.ExtraBottles },
            new SqlParameter("@WarehouseId", SqlDbType.BigInt) { Value = request.WarehouseId == 0 ? DBNull.Value : request.WarehouseId },
            new SqlParameter("@Remarks", SqlDbType.NVarChar, 1000) { Value = (object?)request.Remarks ?? DBNull.Value },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_ProductionModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteProductionAsync(long id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 500) { Value = id.ToString() },
            new SqlParameter("@OprType", SqlDbType.Int) { Value = 1 }, // 1 = Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 200) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_ProductionOperation", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<IReadOnlyList<PreProcessingItemDto>> GetProductionItemsAsync(long productionId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@UsedForId", SqlDbType.Int) { Value = (int)productionId },
            new("@UsedFor", SqlDbType.Int) { Value = 3 } // 3 = Production
        };

        try
        {
            var table = await ExecuteDataTableAsync("Inv_ItemStockUsedById", parameters, cancellationToken);
            return table.AsEnumerable().Select(MapProductionItem).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetProductionItemsAsync (Legacy SP: Inv_ItemStockUsedById): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveProductionItemAsync(SavePreProcessingItemRequest request, CancellationToken cancellationToken = default)
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

    public async Task<int> DeleteProductionItemAsync(long itemStockUsedId, CancellationToken cancellationToken = default)
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
            throw new Exception($"Error deleting production item (ItemStockUsedID={itemStockUsedId}): {ex.Message}", ex);
        }
    }

    public async Task<int> FinalizeStockUpdateAsync(long productionId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@UsedFor", SqlDbType.Int) { Value = 3 }, // 3 = Production
            new SqlParameter("@UsedForId", SqlDbType.BigInt) { Value = productionId },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = updatedBy },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Inv_ItemStockPreProcessingAndProductModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<IReadOnlyList<CountryLookupDto>> GetCountriesLookupAsync(CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@CountryID", SqlDbType.BigInt) { Value = 0 }
        };

        try
        {
            var table = await ExecuteDataTableAsync("Z_CountryMasterSelectAll", parameters, cancellationToken);
            return table.AsEnumerable().Select(r => new CountryLookupDto
            {
                CountryId = r.SafeLong("CountryID", "CountryId"),
                CountryName = r.SafeString("CountryName")
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetCountriesLookupAsync (Legacy SP: Z_CountryMasterSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<IReadOnlyList<SkuLookupDto>> GetSkusLookupAsync(CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@SkuID", SqlDbType.BigInt) { Value = 0 }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterSkusSelectAll", parameters, cancellationToken);
            return table.AsEnumerable().Select(r => new SkuLookupDto
            {
                SkuId = r.SafeLong("SkuId", "SkuID"),
                SkuName = r.SafeString("SkuName")
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetSkusLookupAsync (Legacy SP: W_MasterSkusSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<IReadOnlyList<KettleLookupDto>> GetKettlesLookupAsync(CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@KettleId", SqlDbType.BigInt) { Value = 0 }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterKettleSelectAll", parameters, cancellationToken);
            return table.AsEnumerable().Select(r => new KettleLookupDto
            {
                KettleId = r.SafeLong("KettleId", "KettleID"),
                KettleNumber = r.SafeString("KettleNumber")
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetKettlesLookupAsync (Legacy SP: W_MasterKettleSelectAll): {ex.Message}", ex);
        }
    }

    private ProductionDto MapProduction(DataRow row) => new()
    {
        ProductionId = row.SafeLong("ProductionId", "ProductionID"),
        BomId = row.SafeLong("BomId", "BomID"),
        BomName = row.SafeString("BomName"),
        BomQty = row.SafeLong("BomQty"),
        CookingDate = row.SafeString("CookingDate"),
        ExpiryDate = row.SafeString("ExpiryDate"),
        FilledDate = row.SafeString("FilledDate"),
        PackedCountryName = row.SafeString("PackedCountryName"),
        PackedCountry = row.SafeString("PackedCountry"),
        Colli = row.SafeString("Colli"),
        PalletNumber = row.SafeString("PalletNumber"),
        ProcessEmployees = row.SafeString("ProcessEmployees"),
        KettleId = row.SafeString("KettleId", "KettleID"),
        KettleNumber = row.SafeString("KettleNumber"),
        KettleRun = row.SafeInt("KettleRun"),
        BatchNo = row.SafeString("BatchNo"),
        SkuId = row.SafeLong("SkuId", "SkuID"),
        SkuName = row.SafeString("SkuName"),
        DocumentUpload = row.SafeString("DocumentUpload"),
        FillingBottles = row.SafeInt("FillingBottles"),
        FillingPerBottleUnit = row.SafeLong("FillingPerBottleUnit"),
        FillingUnitName = row.SafeString("FillingUnitName"),
        ExtraBottles = row.SafeDouble("ExtraBottles"),
        WarehouseId = row.SafeLong("WarehouseId", "WarehouseID"),
        WarehouseName = row.SafeString("WarehouseName"),
        Remarks = row.SafeString("Remarks"),
        IsComplete = row.SafeInt("IsComplete"),
        CreatedBy = row.SafeString("CreatedBy"),
        CreatedDate = row.SafeDateTime("CreatedDate") ?? DateTime.MinValue
    };

    private PreProcessingItemDto MapProductionItem(DataRow row) => new()
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
