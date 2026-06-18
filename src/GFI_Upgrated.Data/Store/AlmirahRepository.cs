using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class AlmirahRepository : IAlmirahRepository
{
    private readonly string _connectionString;

    public AlmirahRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<AlmirahDto>> GetAlmirahsAsync(PagedRequest request, string? searchText, long? warehouseId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@AlmirahShelfID", SqlDbType.Int) { Value = 0 },
            new("@AlmirahShelfName", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@WarehouseID", SqlDbType.BigInt) { Value = warehouseId ?? 0 },
            new("@ParentId", SqlDbType.BigInt) { Value = 0 },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "AlmirahShelfID" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterAlmirahShelfList", parameters, cancellationToken);
            
            return new PagedResult<AlmirahDto>
            {
                CurrentPage = request.CurrentPage,
                TotalRecord = Convert.ToInt32(parameters[6].Value ?? 0),
                Items = table.AsEnumerable().Select(MapAlmirah).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetAlmirahsAsync (Legacy SP: W_MasterAlmirahShelfList): {ex.Message}", ex);
        }
    }

    public async Task<AlmirahDto?> GetAlmirahByIdAsync(long almirahId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@AlmirahShelfID", SqlDbType.BigInt) { Value = almirahId }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterAlmirahShelfSelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault();
            return row == null ? null : MapAlmirah(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetAlmirahByIdAsync (Legacy SP: W_MasterAlmirahShelfSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveAlmirahAsync(SaveAlmirahRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@AlmirahShelfID", SqlDbType.BigInt) { Value = request.AlmirahShelfID },
            new SqlParameter("@AlmirahShelfName", SqlDbType.NVarChar, 200) { Value = request.AlmirahShelfName.Trim() },
            new SqlParameter("@NoShelf", SqlDbType.Int) { Value = request.NoShelf },
            new SqlParameter("@ParentId", SqlDbType.Int) { Value = (object?)request.ParentId ?? DBNull.Value },
            new SqlParameter("@WarehouseID", SqlDbType.BigInt) { Value = request.WarehouseID },
            new SqlParameter("@AlmirahLocation", SqlDbType.NVarChar, 200) { Value = (object?)request.AlmirahLocation ?? DBNull.Value },
            new SqlParameter("@Description", SqlDbType.VarChar, 1000) { Value = (object?)request.Description ?? DBNull.Value },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterAlmirahShelfModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteAlmirahAsync(long almirahId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = almirahId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_MasterAlmirahShelfOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        // W_MasterAlmirahShelfOperation: 1 = Success
        return result == 1 ? 1 : 0;
    }

    public async Task<IReadOnlyList<AlmirahDto>> GetShelvesByAlmirahIdAsync(long almirahId, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@AlmirahShelfID", SqlDbType.Int) { Value = (int)almirahId }
        };

        var table = await ExecuteDataTableAsync("W_AlmirahShelfList", parameters, cancellationToken);
        return table.AsEnumerable().Select(MapAlmirah).ToList();
    }

    private AlmirahDto MapAlmirah(DataRow row) => new()
    {
        AlmirahShelfID = row.SafeLong("AlmirahShelfID", "AlmirahShelfId"),
        AlmirahShelfName = row.SafeString("AlmirahShelfName"),
        NoShelf = row.Table.Columns.Contains("NoShelf") && row["NoShelf"] != DBNull.Value ? Convert.ToInt32(row["NoShelf"]) : null,
        WarehouseID = row.SafeLong("WarehouseID", "WarehouseId"),
        WarehouseName = row.SafeString("WarehouseName"),
        AlmirahLocation = row.SafeString("AlmirahLocation"),
        Description = row.SafeString("Description"),
        ParentId = row.Table.Columns.Contains("ParentId") && row["ParentId"] != DBNull.Value ? Convert.ToInt64(row["ParentId"]) : null,
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
