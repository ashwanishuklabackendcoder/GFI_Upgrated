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

public sealed class BomRepository : IBomRepository
{
    private readonly string _connectionString;

    public BomRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<BomDto>> GetBomsAsync(BomListRequest request, CancellationToken cancellationToken = default)
    {
        var sortColumn = request.SortColumn ?? "BomId";
        var ambiguousColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "IsActive", "ItemId", "CreatedDate", "CreatedBy", "BomId", "UnitId"
        };
        if (ambiguousColumns.Contains(sortColumn))
        {
            sortColumn = "W_MasterBom." + sortColumn;
        }

        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@BomId", SqlDbType.BigInt) { Value = 0 },
            new SqlParameter("@BomName", SqlDbType.NVarChar, 500) { Value = request.SearchTerm ?? string.Empty },
            new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = request.ItemId ?? 0 },
            new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = request.PageNumber },
            new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = request.PageSize },
            new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = sortColumn },
            new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType ?? "DESC" },
            new SqlParameter("@ItemTypeId", SqlDbType.Int) { Value = request.ItemTypeId ?? 0 }
        };

        DataTable dt;
        try
        {
            dt = await ExecuteDataTableAsync("W_MasterBomList", parameters, cancellationToken);
        }
        catch
        {
            dt = new DataTable();
        }

        var items = new List<BomDto>();

        foreach (DataRow row in dt.Rows)
        {
            items.Add(new BomDto
            {
                BomId = row.SafeLong("BomId"),
                ItemId = row.SafeLong("ItemId"),
                ItemName = row.SafeString("ItemName"),
                BomName = row.SafeString("BomName"),
                Quantity = row.SafeDouble("Quantity"),
                UnitId = row.SafeLong("UnitId"),
                UnitName = row.SafeString("UnitName"),
                ExtraExpensesPerPiece = row.SafeDouble("ExtraExpensesPerPiece"),
                CreatedDate = row.SafeDateTime("CreatedDate"),
                CreatedBy = row.SafeString("CreatedBy"),
                IsActive = row.SafeBool("IsActive"),
                ItemTypeId = row.SafeLong("ItemTypeId", "ItemTypeID")
            });
        }

        if (!items.Any())
        {
            var fallbackSql = @"
                SELECT 
                    b.BomId, 
                    b.ItemId, 
                    COALESCE(i.ItemName, '') AS ItemName,
                    b.BomName, 
                    b.Quantity, 
                    b.UnitId, 
                    COALESCE(u.UnitName, '') AS UnitName,
                    b.ExtraExpensesPerPiece, 
                    b.CreatedDate, 
                    b.CreatedBy, 
                    b.IsActive, 
                    b.ItemTypeId
                FROM dbo.W_MasterBom b
                LEFT JOIN dbo.W_MasterUnit u ON b.UnitId = u.UnitId
                LEFT JOIN dbo.W_MasterItem i ON b.ItemId = i.ItemID
                WHERE (@ItemId = 0 OR b.ItemId = @ItemId)
                  AND (@ItemTypeId = 0 OR b.ItemTypeId = @ItemTypeId)
                  AND (@SearchTerm = '' OR b.BomName LIKE '%' + @SearchTerm + '%')
                ORDER BY b.BomId DESC";

            var fallbackParams = new[]
            {
                new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = request.ItemId ?? 0 },
                new SqlParameter("@ItemTypeId", SqlDbType.Int) { Value = request.ItemTypeId ?? 0 },
                new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 500) { Value = request.SearchTerm ?? string.Empty }
            };

            var fallbackDt = await ExecuteQueryTextDataTableAsync(fallbackSql, fallbackParams, cancellationToken);
            foreach (DataRow row in fallbackDt.Rows)
            {
                items.Add(new BomDto
                {
                    BomId = row.SafeLong("BomId"),
                    ItemId = row.SafeLong("ItemId"),
                    ItemName = row.SafeString("ItemName"),
                    BomName = row.SafeString("BomName"),
                    Quantity = row.SafeDouble("Quantity"),
                    UnitId = row.SafeLong("UnitId"),
                    UnitName = row.SafeString("UnitName"),
                    ExtraExpensesPerPiece = row.SafeDouble("ExtraExpensesPerPiece"),
                    CreatedDate = row.SafeDateTime("CreatedDate"),
                    CreatedBy = row.SafeString("CreatedBy"),
                    IsActive = row.SafeBool("IsActive"),
                    ItemTypeId = row.SafeLong("ItemTypeId", "ItemTypeID")
                });
            }
        }

        return new PagedResult<BomDto>
        {
            CurrentPage = request.PageNumber,
            TotalRecord = items.Count,
            Items = items
        };
    }

    public async Task<BomDto?> GetBomByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@BomId", SqlDbType.BigInt) { Value = id }
        };

        var dt = await ExecuteDataTableAsync("W_MasterBomSelectAll", parameters, cancellationToken);
        if (dt.Rows.Count == 0) return null;

        var row = dt.Rows[0];
        return new BomDto
        {
            BomId = row.SafeLong("BomId"),
            ItemId = row.SafeLong("ItemId"),
            ItemName = row.SafeString("ItemName"),
            BomName = row.SafeString("BomName"),
            Quantity = row.SafeDouble("Quantity"),
            UnitId = row.SafeLong("UnitId"),
            UnitName = row.SafeString("UnitName"),
            ExtraExpensesPerPiece = row.SafeDouble("ExtraExpensesPerPiece"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy"),
            IsActive = row.SafeBool("IsActive"),
            ItemTypeId = row.SafeLong("ItemTypeId", "ItemTypeID"),
            TentativeExpiryDays = row.SafeInt("TentativeExpiryDays")
        };
    }

    public async Task<IReadOnlyList<BomItemDto>> GetBomItemsAsync(long bomId, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@BomId", SqlDbType.BigInt) { Value = bomId }
        };

        var dt = await ExecuteDataTableAsync("W_MasterBomItemsByBom", parameters, cancellationToken);
        var list = new List<BomItemDto>();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new BomItemDto
            {
                BomItemsId = row.SafeLong("BomItemsId"),
                BomId = row.SafeLong("BomId"),
                ItemID = row.SafeLong("ItemID"),
                ItemName = row.SafeString("ItemName"),
                Quantity = row.SafeDouble("Quantity"),
                UnitId = row.SafeLong("UnitId"),
                UnitName = row.SafeString("UnitName"),
                CreatedDate = row.SafeDateTime("CreatedDate"),
                CreatedBy = row.SafeString("CreatedBy")
            });
        }

        return list;
    }

    private static bool _tableAltered = false;
    private async Task EnsureQuantityColumnsAreFloatAsync(CancellationToken cancellationToken)
    {
        if (_tableAltered) return;
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // 1. Alter Tables
            var sqlTables = @"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'W_MasterBom' AND COLUMN_NAME = 'Quantity' AND DATA_TYPE IN ('int', 'bigint', 'smallint', 'tinyint'))
                BEGIN
                    ALTER TABLE dbo.W_MasterBom ALTER COLUMN Quantity FLOAT NOT NULL;
                END
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'W_MasterBomItems' AND COLUMN_NAME = 'Quantity' AND DATA_TYPE IN ('int', 'bigint', 'smallint', 'tinyint'))
                BEGIN
                    ALTER TABLE dbo.W_MasterBomItems ALTER COLUMN Quantity FLOAT NOT NULL;
                END
            ";
            await using (var cmdTables = new SqlCommand(sqlTables, connection))
            {
                await cmdTables.ExecuteNonQueryAsync(cancellationToken);
            }

            // 2. Fix Stored Procedures: W_MasterBomModify & W_MasterBomItemsModify
            string[] spNames = { "W_MasterBomModify", "W_MasterBomItemsModify" };
            foreach (var sp in spNames)
            {
                var checkSp = $"SELECT OBJECT_DEFINITION(OBJECT_ID('dbo.{sp}'))";
                await using var cmdSp = new SqlCommand(checkSp, connection);
                var defObj = await cmdSp.ExecuteScalarAsync(cancellationToken);
                if (defObj != null && defObj != DBNull.Value)
                {
                    var def = defObj.ToString();
                    if (!string.IsNullOrEmpty(def) && (def.Contains("@Quantity INT", StringComparison.OrdinalIgnoreCase) || def.Contains("@Quantity BIGINT", StringComparison.OrdinalIgnoreCase)))
                    {
                        var updatedDef = def
                            .Replace("@Quantity INT", "@Quantity FLOAT", StringComparison.OrdinalIgnoreCase)
                            .Replace("@Quantity BIGINT", "@Quantity FLOAT", StringComparison.OrdinalIgnoreCase);

                        var idx = updatedDef.IndexOf("CREATE PROCEDURE", StringComparison.OrdinalIgnoreCase);
                        if (idx >= 0)
                        {
                            updatedDef = updatedDef.Remove(idx, 16).Insert(idx, "ALTER PROCEDURE");
                        }

                        await using var alterCmd = new SqlCommand(updatedDef, connection);
                        await alterCmd.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
            }

            _tableAltered = true;
        }
        catch
        {
            // Ignore if permissions or already float
        }
    }

    public async Task<int> SaveBomAsync(SaveBomRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureQuantityColumnsAreFloatAsync(cancellationToken);

        var parameters = new[]
        {
            new SqlParameter("@BomId", SqlDbType.BigInt) { Value = request.BomId },
            new SqlParameter("@BomName", SqlDbType.NVarChar, 500) { Value = request.BomName },
            new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = request.ItemId },
            new SqlParameter("@Quantity", SqlDbType.Float) { Value = request.Quantity },
            new SqlParameter("@UnitId", SqlDbType.Int) { Value = (int)request.UnitId },
            new SqlParameter("@ExtraExpensesPerPiece", SqlDbType.Float) { Value = request.ExtraExpensesPerPiece },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = request.CreatedBy },
            new SqlParameter("@ItemTypeId", SqlDbType.Int) { Value = (int)request.ItemTypeId },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterBomModify", parameters, cancellationToken);
        var bomId = Convert.ToInt32(parameters[^1].Value ?? 0);

        if (bomId > 0 && request.Items.Any())
        {
            foreach (var item in request.Items)
            {
                var itemParams = new[]
                {
                    new SqlParameter("@BomItemsId", SqlDbType.BigInt) { Value = item.BomItemsId },
                    new SqlParameter("@BomId", SqlDbType.BigInt) { Value = bomId },
                    new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = item.ItemID },
                    new SqlParameter("@Quantity", SqlDbType.Float) { Value = item.Quantity },
                    new SqlParameter("@UnitId", SqlDbType.Int) { Value = (int)item.UnitId },
                    new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.Now },
                    new SqlParameter("@CreatedBy", SqlDbType.VarChar, 200) { Value = request.CreatedBy },
                    new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
                };
                await ExecuteNonQueryAsync("W_MasterBomItemsModify", itemParams, cancellationToken);
            }
        }

        return bomId;
    }

    public async Task<bool> DeleteBomAsync(long id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = id.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 },
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 200) { Value = deletedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        try {
            await ExecuteNonQueryAsync("W_MasterBomOperation", parameters, cancellationToken);
            return Convert.ToInt32(parameters[^1].Value ?? 0) == 1;
        } catch {
             return false;
        }
    }

    public async Task<bool> ToggleBomStatusAsync(long id, bool isActive, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = id.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = isActive ? (short)2 : (short)3 },
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 200) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        try {
            await ExecuteNonQueryAsync("W_MasterBomOperation", parameters, cancellationToken);
            var result = Convert.ToInt32(parameters[^1].Value ?? 0);
            return result == (isActive ? 2 : 3);
        } catch {
            return false;
        }
    }

    public async Task<IReadOnlyList<RawMaterialDto>> GetItemsForBomLookupAsync(int? itemTypeId = null, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@ItemID", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@ItemName", SqlDbType.NVarChar, 500) { Value = string.Empty },
            new SqlParameter("@ItemCatID", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@ItemTypeID", SqlDbType.Int) { Value = itemTypeId ?? 0 },
            new SqlParameter("@StatusID", SqlDbType.Int) { Value = 0 }, // 0 includes all statuses
            new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = 1 },
            new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = 10000 },
            new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = "ItemName" },
            new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = "ASC" }
        };

        var dt = await ExecuteDataTableAsync("W_MasterItemList", parameters, cancellationToken);
        var list = new List<RawMaterialDto>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new RawMaterialDto {
                ItemId = row.SafeLong("ItemId", "ItemID"),
                ItemName = row.SafeString("ItemName"),
                ItemCode = row.SafeString("ItemCode"),
                ItemTypeId = row.SafeLong("ItemTypeID", "ItemTypeId"),
                TentativeExpiryDays = row.SafeInt("TentativeExpiryDays"),
                PurchaseUnit = row.SafeLong("PurchaseUnit", "PurchaseUnitId")
            });
        }
        return list;
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

    private async Task<DataTable> ExecuteQueryTextDataTableAsync(string sqlText, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(sqlText, connection)
        {
            CommandType = CommandType.Text
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
