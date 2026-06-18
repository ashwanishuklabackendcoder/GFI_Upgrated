using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using GFI_Upgrated.SharedDto.Common;
using System.Linq;

namespace GFI_Upgrated.Data.Store;

public sealed class SemiFinishedProductRepository : ISemiFinishedProductRepository
{
    private readonly string _connectionString;

    public SemiFinishedProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<SemiFinishedProductDto>> GetSemiFinishedProductsAsync(SemiFinishedProductListRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@ItemID", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@ItemName", SqlDbType.NVarChar, 500) { Value = request.SearchTerm ?? string.Empty },
            new SqlParameter("@ItemCatID", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@ItemTypeID", SqlDbType.Int) { Value = 2 }, // Filter for Pre-processed / Semi-Finished
            new SqlParameter("@StatusID", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = request.PageNumber, Direction = ParameterDirection.InputOutput },
            new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = request.PageSize },
            new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = request.SortColumn ?? "ItemID" },
            new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType ?? "DESC" }
        };

        var dt = await ExecuteDataTableAsync("W_MasterItemList", parameters, cancellationToken);
        
        // Fetch lookups for mapping names
        var categories = await GetCategoriesLookupAsync(cancellationToken);
        var types = await GetItemTypesLookupAsync(cancellationToken);
        var units = await GetUnitsLookupAsync(cancellationToken);

        var items = new List<SemiFinishedProductDto>();

        foreach (DataRow row in dt.Rows)
        {
            var itemCatId = row.SafeLong("ItemCatId", "ItemCatID");
            var itemTypeId = row.SafeLong("ItemTypeId", "ItemTypeID");
            var unitId = row.SafeLong("PurchaseUnit", "UnitId", "UnitID");

            items.Add(new SemiFinishedProductDto
            {
                ItemId = row.SafeLong("ItemId", "ItemID"),
                ItemName = row.SafeString("ItemName"),
                ItemCode = row.SafeString("ItemCode"),
                ShortName = row.SafeString("ShortName"),
                ItemCategoryName = categories.GetValueOrDefault(itemCatId, ""),
                ItemTypeName = types.GetValueOrDefault(itemTypeId, ""),
                UnitName = units.GetValueOrDefault(unitId, ""),
                ItemCatId = itemCatId,
                ItemTypeId = itemTypeId,
                PurchaseUnit = unitId,
                StatusId = row.SafeInt("StatusId", "StatusID"),
                IsActive = row.SafeBool("IsActive"),
                Description = row.SafeString("Description"),
                StorageDetails = row.SafeString("StorageDetails"),
                Tags = row.SafeString("Tags"),
                TentativeExpiryDays = row.SafeInt("TentativeExpiryDays"),
                BrandId = row.SafeLong("BrandId", "BrandID")
            });
        }

        return new PagedResult<SemiFinishedProductDto>
        {
            CurrentPage = Convert.ToInt32(parameters.Find(p => p.ParameterName == "@CurrentPage")?.Value ?? request.PageNumber),
            TotalRecord = Convert.ToInt32(parameters.Find(p => p.ParameterName == "@TotalRecord")?.Value ?? 0),
            Items = items
        };
    }

    public async Task<SemiFinishedProductDto?> GetSemiFinishedProductByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemID", SqlDbType.Int) { Value = id }
        };

        var dt = await ExecuteDataTableAsync("W_MasterItemList", parameters, cancellationToken);
        if (dt.Rows.Count == 0) return null;

        var row = dt.Rows[0];
        var itemCatId = row.SafeLong("ItemCatId", "ItemCatID");
        var itemTypeId = row.SafeLong("ItemTypeId", "ItemTypeID");
        var unitId = row.SafeLong("PurchaseUnit", "UnitId", "UnitID");

        return new SemiFinishedProductDto
        {
            ItemId = row.SafeLong("ItemId", "ItemID"),
            ItemName = row.SafeString("ItemName"),
            ItemCode = row.SafeString("ItemCode"),
            ShortName = row.SafeString("ShortName"),
            ItemCategoryName = await GetCategoryNameAsync(itemCatId, cancellationToken),
            ItemTypeName = await GetItemTypeNameAsync(itemTypeId, cancellationToken),
            UnitName = await GetUnitNameAsync(unitId, cancellationToken),
            ItemCatId = itemCatId,
            ItemTypeId = itemTypeId,
            PurchaseUnit = unitId,
            StatusId = row.SafeInt("StatusId", "StatusID"),
            IsActive = row.SafeBool("IsActive"),
            Description = row.SafeString("Description"),
            StorageDetails = row.SafeString("StorageDetails"),
            Tags = row.SafeString("Tags"),
            TentativeExpiryDays = row.SafeInt("TentativeExpiryDays"),
            BrandId = row.SafeLong("BrandId", "BrandID")
        };
    }

    public async Task<int> SaveSemiFinishedProductAsync(SaveSemiFinishedProductRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = request.IsActive },
            new SqlParameter("@IsEditable", SqlDbType.Bit) { Value = true },
            new SqlParameter("@ItemID", SqlDbType.BigInt) { Value = request.ItemId },
            new SqlParameter("@ItemCatID", SqlDbType.BigInt) { Value = request.ItemCatId },
            new SqlParameter("@ItemTypeID", SqlDbType.BigInt) { Value = 2 }, // Filter for Pre-processed / Semi-Finished (2)
            new SqlParameter("@StatusID", SqlDbType.BigInt) { Value = request.StatusId },
            new SqlParameter("@ItemName", SqlDbType.NVarChar, 1000) { Value = request.ItemName },
            new SqlParameter("@Description", SqlDbType.NVarChar, 2000) { Value = (object?)request.Description ?? DBNull.Value },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = (object?)request.CreatedBy ?? "System" },
            new SqlParameter("@Tags", SqlDbType.NVarChar, 1000) { Value = (object?)request.Tags ?? DBNull.Value },
            new SqlParameter("@ItemCode", SqlDbType.NVarChar, 100) { Value = request.ItemCode },
            new SqlParameter("@ShortName", SqlDbType.NVarChar, 100) { Value = request.ShortName },
            new SqlParameter("@TentativeExpiryDays", SqlDbType.Int) { Value = request.TentativeExpiryDays },
            new SqlParameter("@PurchaseUnit", SqlDbType.BigInt) { Value = request.PurchaseUnit > 0 ? request.PurchaseUnit : DBNull.Value },
            new SqlParameter("@BrandId", SqlDbType.BigInt) { Value = request.BrandId > 0 ? request.BrandId : DBNull.Value },
            new SqlParameter("@StorageDetails", SqlDbType.NVarChar, 2000) { Value = (object?)request.StorageDetails ?? DBNull.Value },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterItemModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<bool> DeleteSemiFinishedProductAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 500) { Value = id.ToString() },
            new SqlParameter("@OprType", SqlDbType.Int) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 200) { Value = deletedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        try
        {
            await ExecuteNonQueryAsync("W_MasterItemOperation", parameters, cancellationToken);
            var err = Convert.ToInt32(parameters[^1].Value ?? 0);
            return err == 1;
        }
        catch
        {
            return false;
        }
    }

    public async Task<SemiFinishedProductDetailDto?> GetSemiFinishedProductDetailAsync(long itemId, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemId", SqlDbType.Int) { Value = itemId }
        };

        var dt = await ExecuteDataTableAsync("W_MasterItemDetailsList", parameters, cancellationToken);
        if (dt.Rows.Count == 0) return null;

        var row = dt.Rows[0];
        return new SemiFinishedProductDetailDto
        {
            ItemDetailId = row.SafeLong("ItemDetailId"),
            ItemId = row.SafeLong("ItemId"),
            CriticalLevelAlert = row.SafeBool("CriticalLevelAlert"),
            CriticalLevelQuantity = row.SafeInt("CriticalLevelQuantity"),
            ReorderLevelQuantity = row.SafeInt("ReorderLevelQuantity"),
            ReorderLevelDays = row.SafeInt("ReorderLevelDays"),
            MaximumQuantity = row.SafeInt("MaximumQuantity"),
            OpeningQuantity = row.SafeDouble("OpeningQuantity"),
            CurrentQuantity = row.SafeDouble("CurrentQuantity")
        };
    }

    public async Task<int> SaveSemiFinishedProductDetailAsync(SemiFinishedProductDetailDto request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemDetailId", SqlDbType.BigInt) { Value = request.ItemDetailId },
            new SqlParameter("@ItemId", SqlDbType.Int) { Value = request.ItemId },
            new SqlParameter("@CriticalLevelAlert", SqlDbType.Bit) { Value = request.CriticalLevelAlert },
            new SqlParameter("@CriticalLevelQuantity", SqlDbType.Int) { Value = request.CriticalLevelQuantity },
            new SqlParameter("@ReorderLevelQuantity", SqlDbType.Int) { Value = request.ReorderLevelQuantity },
            new SqlParameter("@ReorderLevelDays", SqlDbType.Int) { Value = request.ReorderLevelDays },
            new SqlParameter("@MaximumQuantity", SqlDbType.Int) { Value = request.MaximumQuantity },
            new SqlParameter("@OpeningQuantity", SqlDbType.Float) { Value = request.OpeningQuantity },
            new SqlParameter("@CurrentQuantity", SqlDbType.Float) { Value = request.CurrentQuantity },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterItemDetailsModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<IReadOnlyList<SemiFinishedProductVendorDto>> GetSemiFinishedProductVendorsAsync(long itemId, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemId", SqlDbType.Int) { Value = itemId }
        };

        var dt = await ExecuteDataTableAsync("W_MasterItemVendorsList", parameters, cancellationToken);
        var list = new List<SemiFinishedProductVendorDto>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SemiFinishedProductVendorDto
            {
                ItemVendorId = row.SafeLong("ItemVendorId"),
                ItemId = row.SafeLong("ItemId"),
                VendorAccountId = row.SafeInt("VendorAccountId"),
                VendorName = row.SafeString("AccountName"),
                StatusId = row.SafeInt("StatusId"),
                PurchasePrice = row.SafeDouble("PurchasePrice"),
                PurchaseUnit = (decimal)row.SafeDouble("PurchaseUnit"),
                StartDate = row.SafeDateTime("StartDate"),
                EndDate = row.SafeDateTime("EndDate"),
                Remarks = row.SafeString("Remarks"),
                BrandId = row.SafeLong("BrandId"),
                CurrencyId = row.SafeLong("CurrencyID"),
                PurchaseQuantity = row.SafeDouble("PurchaseQuantity")
            });
        }

        return list;
    }

    public async Task<int> SaveSemiFinishedProductVendorAsync(SemiFinishedProductVendorDto request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemVendorId", SqlDbType.BigInt) { Value = request.ItemVendorId },
            new SqlParameter("@ItemId", SqlDbType.Int) { Value = request.ItemId },
            new SqlParameter("@VendorAccountId", SqlDbType.Int) { Value = request.VendorAccountId },
            new SqlParameter("@StatusId", SqlDbType.Int) { Value = request.StatusId },
            new SqlParameter("@PurchasePrice", SqlDbType.Float) { Value = request.PurchasePrice },
            new SqlParameter("@PurchaseUnit", SqlDbType.Decimal) { Value = request.PurchaseUnit > 0 ? request.PurchaseUnit : DBNull.Value },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = request.CreatedBy },
            new SqlParameter("@StartDate", SqlDbType.Date) { Value = (object?)request.StartDate ?? DBNull.Value },
            new SqlParameter("@EndDate", SqlDbType.Date) { Value = (object?)request.EndDate ?? DBNull.Value },
            new SqlParameter("@Remarks", SqlDbType.NVarChar, 500) { Value = (object?)request.Remarks ?? DBNull.Value },
            new SqlParameter("@BrandId", SqlDbType.BigInt) { Value = request.BrandId > 0 ? request.BrandId : DBNull.Value },
            new SqlParameter("@CurrencyID", SqlDbType.BigInt) { Value = request.CurrencyId },
            new SqlParameter("@PurchaseQuantity", SqlDbType.Float) { Value = request.PurchaseQuantity },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterItemVendorsModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<bool> DeleteSemiFinishedProductVendorAsync(long vendorId, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = vendorId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = "System" },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterItemVendorsOperation", parameters, cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<SemiFinishedProductBatchDto>> GetSemiFinishedProductBatchesAsync(long itemId, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemID", SqlDbType.Int) { Value = itemId }
        };

        var dt = await ExecuteDataTableAsync("Inv_ItemStockByBatchList", parameters, cancellationToken);
        var list = new List<SemiFinishedProductBatchDto>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new SemiFinishedProductBatchDto
            {
                ItemStockByBatchId = row.SafeLong("ItemStockByBatchId"),
                ItemId = row.SafeLong("ItemId"),
                WarehouseId = row.SafeInt("WarehouseId"),
                WarehouseName = row.SafeString("WarehouseName"),
                Quantity = row.SafeDouble("Quantity"),
                UnitId = row.SafeInt("UnitId"),
                UnitName = row.SafeString("UnitName"),
                BatchNo = row.SafeString("BatchNo"),
                ExpiryDate = row.SafeDateTime("ExpiryDate"),
                CreatedBy = row.SafeString("CreatedBy")
            });
        }

        return list;
    }

    public async Task<int> SaveSemiFinishedProductBatchAsync(SemiFinishedProductBatchDto request, string? deletedBatchIds = null, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemStockByBatchId", SqlDbType.BigInt) { Value = request.ItemStockByBatchId },
            new SqlParameter("@PurchaseID", SqlDbType.BigInt) { Value = request.PurchaseID },
            new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = request.ItemId },
            new SqlParameter("@WarehouseId", SqlDbType.BigInt) { Value = request.WarehouseId },
            new SqlParameter("@Quantity", SqlDbType.Float) { Value = request.Quantity },
            new SqlParameter("@UnitId", SqlDbType.BigInt) { Value = request.UnitId },
            new SqlParameter("@BatchNo", SqlDbType.NVarChar, 100) { Value = request.BatchNo },
            new SqlParameter("@ExpiryDate", SqlDbType.DateTime) { Value = (object?)request.ExpiryDate ?? DBNull.Value },
            new SqlParameter("@StockById", SqlDbType.Int) { Value = 1 }, // Default to Master Item stock
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = request.CreatedBy },
            new SqlParameter("@DeletedBatchIds", SqlDbType.NVarChar, -1) { Value = (object?)deletedBatchIds ?? DBNull.Value },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("Inv_ItemStockByBatchModifyFromMaster", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<IReadOnlyList<AccountLookupDto>> GetAccountsLookupAsync(CancellationToken cancellationToken = default)
    {
        var dt = await ExecuteDataTableAsync("W_MasterItemVendorsSelectAll", new SqlParameter[] { }, cancellationToken);
        var list = new List<AccountLookupDto>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new AccountLookupDto
            {
                AccountId = row.SafeInt("AccountId"),
                AccountName = row.SafeString("AccountName")
            });
        }
        return list;
    }

    public async Task<IReadOnlyList<CurrencyLookupDto>> GetCurrenciesLookupAsync(CancellationToken cancellationToken = default)
    {
        var dt = await ExecuteDataTableAsync("W_MasterCurrencySelectAll", new SqlParameter[] { }, cancellationToken);
        var list = new List<CurrencyLookupDto>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new CurrencyLookupDto
            {
                CurrencyId = row.SafeLong("CurrencyID"),
                CurrencyName = row.SafeString("CurrencyName")
            });
        }
        return list;
    }

    #region Lookups

    private async Task<Dictionary<long, string>> GetCategoriesLookupAsync(CancellationToken cancellationToken)
    {
        var dt = await ExecuteDataTableAsync("W_MasterItemCategorySelectAll", new[] { new SqlParameter("@ItemCatId", 0) }, cancellationToken);
        return dt.AsEnumerable().ToDictionary(r => r.SafeLong("ItemCatId", "ItemCatID"), r => r.SafeString("ItemCatName"));
    }

    private async Task<Dictionary<long, string>> GetItemTypesLookupAsync(CancellationToken cancellationToken)
    {
        var dt = await ExecuteDataTableAsync("W_MasterItemTypeSelectAll", new[] { new SqlParameter("@ItemTypeId", 0) }, cancellationToken);
        return dt.AsEnumerable().ToDictionary(r => r.SafeLong("ItemTypeId", "ItemTypeID"), r => r.SafeString("ItemTypeName"));
    }

    private async Task<Dictionary<long, string>> GetUnitsLookupAsync(CancellationToken cancellationToken)
    {
        var dt = await ExecuteDataTableAsync("W_MasterUnitSelectAll", new[] { new SqlParameter("@UnitId", 0) }, cancellationToken);
        return dt.AsEnumerable().ToDictionary(r => r.SafeLong("UnitId", "UnitID"), r => r.SafeString("UnitName"));
    }

    private async Task<string> GetCategoryNameAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0) return "";
        var dt = await ExecuteDataTableAsync("W_MasterItemCategorySelectAll", new[] { new SqlParameter("@ItemCatId", id) }, cancellationToken);
        return dt.Rows.Count > 0 ? dt.Rows[0].SafeString("ItemCatName") : "";
    }

    private async Task<string> GetItemTypeNameAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0) return "";
        var dt = await ExecuteDataTableAsync("W_MasterItemTypeSelectAll", new[] { new SqlParameter("@ItemTypeId", id) }, cancellationToken);
        return dt.Rows.Count > 0 ? dt.Rows[0].SafeString("ItemTypeName") : "";
    }

    private async Task<string> GetUnitNameAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0) return "";
        var dt = await ExecuteDataTableAsync("W_MasterUnitSelectAll", new[] { new SqlParameter("@UnitId", id) }, cancellationToken);
        return dt.Rows.Count > 0 ? dt.Rows[0].SafeString("UnitName") : "";
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

    #endregion
}
