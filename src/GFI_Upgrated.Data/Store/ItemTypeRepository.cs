using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class ItemTypeRepository : IItemTypeRepository
{
    private readonly string _connectionString;

    public ItemTypeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<ItemTypeDto>> GetItemTypesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemTypeId", SqlDbType.Int) { Value = 0 },
            new("@ItemTypeName", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "ItemTypeId" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterItemTypeList", parameters, cancellationToken);
            
            return new PagedResult<ItemTypeDto>
            {
                CurrentPage = request.CurrentPage,
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapItemType).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemTypesAsync (Legacy SP: W_MasterItemTypeList): {ex.Message}", ex);
        }
    }

    public async Task<ItemTypeDto?> GetItemTypeByIdAsync(long itemTypeId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemTypeId", SqlDbType.BigInt) { Value = itemTypeId }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterItemTypeSelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault();
            return row == null ? null : MapItemType(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemTypeByIdAsync (Legacy SP: W_MasterItemTypeSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveItemTypeAsync(SaveItemTypeRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemTypeId", SqlDbType.BigInt) { Value = request.ItemTypeId },
            new SqlParameter("@ItemTypeName", SqlDbType.NVarChar, 200) { Value = request.ItemTypeName.Trim() },
            new SqlParameter("@IsMainType", SqlDbType.Bit) { Value = request.IsMainType },
            new SqlParameter("@MainTypeId", SqlDbType.Int) { Value = request.MainTypeId.HasValue ? (int)request.MainTypeId.Value : DBNull.Value },
            new SqlParameter("@IsEditable", SqlDbType.Bit) { Value = true },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterItemTypeModify", parameters, cancellationToken);
        var resultId = Convert.ToInt32(parameters[^1].Value ?? 0);

        // Synchronize IsActive status via Operation SP for updates
        if (request.ItemTypeId > 0 && resultId > 0)
        {
            var opParams = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = request.ItemTypeId.ToString() },
                new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = request.IsActive ? (short)2 : (short)3 },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = request.UpdatedBy },
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
            };
            await ExecuteNonQueryAsync("W_MasterItemTypeOperation", opParams, cancellationToken);
        }

        return resultId;
    }

    public async Task<int> DeleteItemTypeAsync(long itemTypeId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = itemTypeId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_MasterItemTypeOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        // W_MasterItemTypeOperation: 0 = Success, 1 = Fail (In Use or Not Editable)
        return result == 0 ? 1 : -1;
    }

    public async Task<IReadOnlyList<ParentTypeLookupDto>> GetParentTypesLookupAsync(CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("W_MasterItemTypeSelectMain", new List<SqlParameter>(), cancellationToken);
        return table.AsEnumerable().Select(r => new ParentTypeLookupDto
        {
            ItemTypeId = r.SafeLong("ItemTypeId", "ItemTypeID"),
            ItemTypeName = r.SafeString("ItemTypeName")
        }).ToList();
    }

    private ItemTypeDto MapItemType(DataRow row) => new()
    {
        ItemTypeId = row.SafeLong("ItemTypeId", "ItemTypeID"),
        ItemTypeName = row.SafeString("ItemTypeName"),
        IsMainType = row.SafeBool("IsMainType"),
        IsEditable = row.SafeBool("IsEditable"),
        MainTypeId = row.Table.Columns.Contains("MainTypeId") && row["MainTypeId"] != DBNull.Value ? Convert.ToInt64(row["MainTypeId"]) : null,
        ParentItemTypeName = row.SafeString("ParentItemTypeName"),
        IsActive = row.SafeBool("IsActive"),
        CreatedBy = row.SafeString("CreatedBy"),
        CreatedDate = row.SafeDateTime("CreatedDate") ?? DateTime.MinValue
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
