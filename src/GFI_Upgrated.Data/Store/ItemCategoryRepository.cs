using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class ItemCategoryRepository : IItemCategoryRepository
{
    private readonly string _connectionString;

    public ItemCategoryRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<ItemCategoryDto>> GetItemCategoriesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemCatId", SqlDbType.Int) { Value = 0 },
            new("@ItemCatName", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "ItemCatId" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterItemCategoryList", parameters, cancellationToken);
            
            return new PagedResult<ItemCategoryDto>
            {
                CurrentPage = request.CurrentPage,
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapItemCategory).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemCategoriesAsync (Legacy SP: W_MasterItemCategoryList): {ex.Message}", ex);
        }
    }

    public async Task<ItemCategoryDto?> GetItemCategoryByIdAsync(long itemCatId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemCatId", SqlDbType.BigInt) { Value = itemCatId }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterItemCategorySelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault();
            return row == null ? null : MapItemCategory(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemCategoryByIdAsync (Legacy SP: W_MasterItemCategorySelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveItemCategoryAsync(SaveItemCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ItemCatId", SqlDbType.BigInt) { Value = request.ItemCatId },
            new SqlParameter("@ItemCatName", SqlDbType.NVarChar, 200) { Value = request.ItemCatName.Trim() },
            new SqlParameter("@IsMainCategory", SqlDbType.Bit) { Value = request.IsMainCategory },
            new SqlParameter("@MainCategoryId", SqlDbType.Int) { Value = request.MainCategoryId.HasValue ? (int)request.MainCategoryId.Value : DBNull.Value },
            new SqlParameter("@IsEditable", SqlDbType.Bit) { Value = true },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterItemCategoryModify", parameters, cancellationToken);
        var resultId = Convert.ToInt32(parameters[^1].Value ?? 0);

        // If it's an update and result is successful, ensure IsActive is updated via the Operation SP
        // because W_MasterItemCategoryModify update statement ignores IsActive.
        if (request.ItemCatId > 0 && resultId > 0)
        {
            var opParams = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = request.ItemCatId.ToString() },
                new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = request.IsActive ? (short)2 : (short)3 },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = request.UpdatedBy },
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
            };
            await ExecuteNonQueryAsync("W_MasterItemCategoryOperation", opParams, cancellationToken);
        }

        return resultId;
    }

    public async Task<int> DeleteItemCategoryAsync(long itemCatId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = itemCatId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_MasterItemCategoryOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        // W_MasterItemCategoryOperation: 1 = Success, 0 = In Use
        return result == 1 ? 1 : (result == 0 ? -1 : 0);
    }

    public async Task<IReadOnlyList<ParentCategoryLookupDto>> GetParentCategoriesLookupAsync(CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("W_MasterItemCategorySelectMain", new List<SqlParameter>(), cancellationToken);
        return table.AsEnumerable().Select(r => new ParentCategoryLookupDto
        {
            ItemCatId = r.SafeLong("ItemCatId", "ItemCatID"),
            ItemCatName = r.SafeString("ItemCatName")
        }).ToList();
    }

    private ItemCategoryDto MapItemCategory(DataRow row) => new()
    {
        ItemCatId = row.SafeLong("ItemCatId", "ItemCatID"),
        ItemCatName = row.SafeString("ItemCatName"),
        IsMainCategory = row.SafeBool("IsMainCategory"),
        MainCategoryId = row.Table.Columns.Contains("MainCategoryID") && row["MainCategoryID"] != DBNull.Value ? Convert.ToInt64(row["MainCategoryID"]) : null,
        ParentItemCatName = row.SafeString("ParentItemCatName"),
        IsActive = row.SafeBool("IsActive"),
        IsEditable = row.SafeBool("IsEditable"),
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
