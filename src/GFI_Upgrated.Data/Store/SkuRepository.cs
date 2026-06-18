using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class SkuRepository : ISkuRepository
{
    private readonly string _connectionString;

    public SkuRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<SkuDto>> GetSkusAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@SkuId", SqlDbType.Int) { Value = 0 },
            new("@SkuName", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "SkuID" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterSkusList", parameters, cancellationToken);
            
            return new PagedResult<SkuDto>
            {
                CurrentPage = request.CurrentPage,
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapSku).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetSkusAsync (Legacy SP: W_MasterSkusList): {ex.Message}", ex);
        }
    }

    public async Task<SkuDto?> GetSkuByIdAsync(long skuId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@SkuID", SqlDbType.BigInt) { Value = skuId }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterSkusSelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault(r => r.SafeLong("SkuId", "SkuID") == skuId);
            return row == null ? null : MapSku(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetSkuByIdAsync (Legacy SP: W_MasterSkusSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveSkuAsync(SaveSkuRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@SkuId", SqlDbType.BigInt) { Value = request.SkuId },
            new SqlParameter("@SkuName", SqlDbType.NVarChar, 500) { Value = request.SkuName.Trim() },
            new SqlParameter("@UnitId", SqlDbType.BigInt) { Value = request.UnitId },
            new SqlParameter("@Quantity", SqlDbType.Float) { Value = request.Quantity },
            new SqlParameter("@QuantityPerColli", SqlDbType.Float) { Value = request.QuantityPerColli },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = request.IsActive },
            new SqlParameter("@Remarks", SqlDbType.NVarChar, 1000) { Value = (object?)request.Remarks?.Trim() ?? DBNull.Value },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterSkuModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteSkuAsync(long skuId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = skuId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_MasterSkusOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        // W_MasterSkusOperation: 0 = Success (Deleted), 1 = Record In Use (Not deleted)
        return result == 0 ? 1 : (result == 1 ? -1 : 0);
    }

    public async Task<IReadOnlyList<UnitLookupDto>> GetUnitsLookupAsync(CancellationToken cancellationToken = default)
    {
        var table = await ExecuteDataTableAsync("W_MasterUnitSelectAll", new List<SqlParameter>(), cancellationToken);
        return table.AsEnumerable().Select(r => new UnitLookupDto
        {
            UnitId = r.SafeLong("UnitID", "UnitId"),
            UnitName = r.SafeString("UnitName", "Unit_Name")
        }).ToList();
    }

    private SkuDto MapSku(DataRow row) => new()
    {
        SkuId = row.SafeLong("SkuId", "SkuID"),
        SkuName = row.SafeString("SkuName"),
        Quantity = row.SafeDouble("Quantity"),
        UnitId = row.SafeLong("UnitId", "UnitID"),
        UnitName = row.SafeString("UnitName", "Unit_Name"),
        QuantityPerColli = row.SafeDouble("QuantityPerColli"),
        IsActive = row.SafeBool("IsActive"),
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
