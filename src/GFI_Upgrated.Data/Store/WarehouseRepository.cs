using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class WarehouseRepository : IWarehouseRepository
{
    private readonly string _connectionString;

    public WarehouseRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<WarehouseDto>> GetWarehousesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@WarehouseId", SqlDbType.Int) { Value = 0 },
            new("@WarehouseName", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "WarehouseId" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterWarehouseList", parameters, cancellationToken);
            
            return new PagedResult<WarehouseDto>
            {
                CurrentPage = request.CurrentPage,
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapWarehouse).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetWarehousesAsync (Legacy SP: W_MasterWarehouseList): {ex.Message}", ex);
        }
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(long warehouseId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@WarehouseId", SqlDbType.BigInt) { Value = warehouseId }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterWarehouseSelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault();
            return row == null ? null : MapWarehouse(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetWarehouseByIdAsync (Legacy SP: W_MasterWarehouseSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveWarehouseAsync(SaveWarehouseRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@WarehouseId", SqlDbType.BigInt) { Value = request.WarehouseId },
            new SqlParameter("@WarehouseName", SqlDbType.NVarChar, 300) { Value = request.WarehouseName.Trim() },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@Description", SqlDbType.NVarChar, 1000) { Value = (object?)request.Description ?? DBNull.Value },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterWarehouseModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteWarehouseAsync(long warehouseId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = warehouseId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_MasterWarehouseOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        // W_MasterWarehouseOperation: 1 = Success, 0 = In Use
        return result == 1 ? 1 : (result == 0 ? -1 : 0);
    }

    private WarehouseDto MapWarehouse(DataRow row) => new()
    {
        WarehouseId = row.SafeLong("WarehouseId", "WarehouseID"),
        WarehouseName = row.SafeString("WarehouseName"),
        Description = row.SafeString("Description"),
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
