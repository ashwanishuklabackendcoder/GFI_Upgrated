using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class StatusRepository : IStatusRepository
{
    private readonly string _connectionString;

    public StatusRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<StatusDto>> GetStatusesAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@StatusID", SqlDbType.Int) { Value = 0 },
            new("@StatusName", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "StatusId" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterStatusList", parameters, cancellationToken);
            
            return new PagedResult<StatusDto>
            {
                CurrentPage = request.CurrentPage,
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapStatus).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetStatusesAsync (Legacy SP: W_MasterStatusList): {ex.Message}", ex);
        }
    }

    public async Task<StatusDto?> GetStatusByIdAsync(long statusId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@StatusID", SqlDbType.Int) { Value = (int)statusId },
            new("@StatusName", SqlDbType.NVarChar, 200) { Value = string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = 1, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = 1 },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = "ASC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = "StatusId" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterStatusList", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault();
            return row == null ? null : MapStatus(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetStatusByIdAsync (Legacy SP: W_MasterStatusList): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveStatusAsync(SaveStatusRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@StatusID", SqlDbType.BigInt) { Value = request.StatusId },
            new SqlParameter("@StatusName", SqlDbType.NVarChar, 500) { Value = request.StatusName.Trim() },
            new SqlParameter("@StatusOf", SqlDbType.Int) { Value = request.StatusOf },
            new SqlParameter("@IsEditable", SqlDbType.Bit) { Value = true },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = request.IsActive },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterStatusModify", parameters, cancellationToken);
        var resultId = Convert.ToInt32(parameters[^1].Value ?? 0);

        // If it's an update and result is successful, ensure IsActive is updated via the Operation SP
        // because W_MasterStatusModify update statement ignores IsActive.
        if (request.StatusId > 0 && resultId > 0)
        {
            var opParams = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = request.StatusId.ToString() },
                new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = request.IsActive ? (short)2 : (short)3 },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = request.UpdatedBy },
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
            };
            await ExecuteNonQueryAsync("W_MasterStatusOperation", opParams, cancellationToken);
        }

        return resultId;
    }

    public async Task<int> DeleteStatusAsync(long statusId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = statusId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_MasterStatusOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        // W_MasterStatusOperation: 1 = Success, 0 = In Use
        return result == 1 ? 1 : (result == 0 ? -1 : 0);
    }

    private StatusDto MapStatus(DataRow row) => new()
    {
        StatusId = row.SafeLong("StatusID", "StatusId"),
        StatusName = row.SafeString("StatusName"),
        StatusOf = row.SafeInt("StatusOf"),
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
