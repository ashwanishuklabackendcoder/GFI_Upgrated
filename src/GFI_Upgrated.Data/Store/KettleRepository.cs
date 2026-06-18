using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class KettleRepository : IKettleRepository
{
    private readonly string _connectionString;

    public KettleRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<KettleDto>> GetKettlesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@KettleId", SqlDbType.Int) { Value = 0 },
            new("@KettleNumber", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "KettleId" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterKettleList", parameters, cancellationToken);
            
            return new PagedResult<KettleDto>
            {
                CurrentPage = request.CurrentPage,
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapKettle).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetKettlesAsync (Legacy SP: W_MasterKettleList): {ex.Message}", ex);
        }
    }

    public async Task<KettleDto?> GetKettleByIdAsync(long kettleId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@KettleId", SqlDbType.BigInt) { Value = kettleId }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterKettleSelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault();
            return row == null ? null : MapKettle(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetKettleByIdAsync (Legacy SP: W_MasterKettleSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveKettleAsync(SaveKettleRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@KettleId", SqlDbType.BigInt) { Value = request.KettleId },
            new SqlParameter("@KettleNumber", SqlDbType.NVarChar, 500) { Value = request.KettleNumber.Trim() },
            new SqlParameter("@Remarks", SqlDbType.NVarChar, 1000) { Value = (object?)request.Remarks ?? DBNull.Value },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterKettleModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteKettleAsync(long kettleId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = kettleId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_MasterKettleOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        // W_MasterKettleOperation: 0 = Success, 1 = In Use
        return result == 0 ? 1 : (result == 1 ? -1 : 0);
    }

    private KettleDto MapKettle(DataRow row) => new()
    {
        KettleId = row.SafeLong("KettleId", "KettleID"),
        KettleNumber = row.SafeString("KettleNumber"),
        Remarks = row.SafeString("Remarks"),
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
