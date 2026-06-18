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
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@BomId", SqlDbType.BigInt) { Value = 0 },
            new SqlParameter("@BomName", SqlDbType.NVarChar, 500) { Value = request.SearchTerm ?? string.Empty },
            new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = 0 },
            new SqlParameter("@CurrentPage", SqlDbType.Int) { Value = request.PageNumber },
            new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = request.PageSize },
            new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = request.SortColumn ?? "BomId" },
            new SqlParameter("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType ?? "DESC" },
            new SqlParameter("@ItemTypeId", SqlDbType.Int) { Value = request.ItemTypeId ?? 0 }
        };

        var dt = await ExecuteDataTableAsync("W_MasterBomList", parameters, cancellationToken);
        var items = new List<BomDto>();

        foreach (DataRow row in dt.Rows)
        {
            items.Add(new BomDto
            {
                BomId = row.SafeLong("BomId"),
                ItemId = row.SafeLong("ItemId"),
                ItemName = row.SafeString("ItemName"),
                BomName = row.SafeString("BomName"),
                Quantity = row.SafeInt("Quantity"),
                UnitId = row.SafeLong("UnitId"),
                UnitName = row.SafeString("UnitName"),
                ExtraExpensesPerPiece = row.SafeDouble("ExtraExpensesPerPiece"),
                CreatedDate = row.SafeDateTime("CreatedDate"),
                CreatedBy = row.SafeString("CreatedBy"),
                IsActive = row.SafeBool("IsActive"),
                ItemTypeId = row.SafeLong("ItemTypeId", "ItemTypeID")
            });
        }

        return new PagedResult<BomDto>
        {
            CurrentPage = request.PageNumber,
            TotalRecord = Convert.ToInt32(parameters.Find(p => p.ParameterName == "@TotalRecord")?.Value ?? 0),
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
            Quantity = row.SafeInt("Quantity"),
            UnitId = row.SafeLong("UnitId"),
            UnitName = row.SafeString("UnitName"),
            ExtraExpensesPerPiece = row.SafeDouble("ExtraExpensesPerPiece"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy"),
            IsActive = row.SafeBool("IsActive"),
            ItemTypeId = row.SafeLong("ItemTypeId", "ItemTypeID")
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

    public async Task<int> SaveBomAsync(SaveBomRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@BomId", SqlDbType.BigInt) { Value = request.BomId },
            new SqlParameter("@BomName", SqlDbType.NVarChar, 500) { Value = request.BomName },
            new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = request.ItemId },
            new SqlParameter("@Quantity", SqlDbType.Int) { Value = request.Quantity },
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
                    new SqlParameter("@Quantity", SqlDbType.Int) { Value = (int)item.Quantity },
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

    public async Task<IReadOnlyList<RawMaterialDto>> GetItemsForBomLookupAsync(int? itemTypeId = null, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new SqlParameter("@ItemID", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@ItemName", SqlDbType.NVarChar, 500) { Value = string.Empty },
            new SqlParameter("@ItemCatID", SqlDbType.Int) { Value = 0 },
            new SqlParameter("@ItemTypeID", SqlDbType.Int) { Value = itemTypeId ?? 0 },
            new SqlParameter("@StatusID", SqlDbType.Int) { Value = 1 }, // Only active
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
                ItemTypeId = row.SafeLong("ItemTypeID", "ItemTypeId")
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
}
