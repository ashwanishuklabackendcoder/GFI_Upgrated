using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class BrandRepository : IBrandRepository
{
    private readonly string _connectionString;

    public BrandRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<BrandDto>> GetBrandsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@BrandID", SqlDbType.Int) { Value = 0 },
            new("@BrandName", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "BrandID" }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterBrandsList", parameters, cancellationToken);
            
            return new PagedResult<BrandDto>
            {
                CurrentPage = request.CurrentPage,
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapBrand).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetBrandsAsync (Legacy SP: W_MasterBrandsList): {ex.Message}", ex);
        }
    }

    public async Task<BrandDto?> GetBrandByIdAsync(long brandId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@BrandID", SqlDbType.Int) { Value = (int)brandId }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_MasterBrandsSelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault(r => r.SafeLong("BrandID") == brandId);
            return row == null ? null : MapBrand(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetBrandByIdAsync (Legacy SP: W_MasterBrandsSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveBrandAsync(SaveBrandRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@BrandID", SqlDbType.BigInt) { Value = request.BrandId },
            new SqlParameter("@BrandName", SqlDbType.NVarChar, 500) { Value = request.BrandName.Trim() },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = request.IsActive },
            new SqlParameter("@Remarks", SqlDbType.NVarChar, 1000) { Value = (object?)request.Remarks?.Trim() ?? DBNull.Value },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await ExecuteNonQueryAsync("W_MasterBrandsModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteBrandAsync(long brandId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = brandId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 1) { Value = updatedBy.Length > 0 ? updatedBy[..1] : "S" },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await ExecuteNonQueryAsync("W_MasterBrandsOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        // W_MasterBrandsOperation: 1 = Success (Deleted), 0 = Record In Use (Not deleted)
        return result == 1 ? 1 : (result == 0 ? -1 : 0);
    }

    private BrandDto MapBrand(DataRow row) => new()
    {
        BrandId = row.SafeLong("BrandID", "BrandId"),
        BrandName = row.SafeString("BrandName", "BrandName"),
        IsActive = row.SafeBool("IsActive", "IsActive"),
        Remarks = row.SafeString("Remarks", "Remarks"),
        CreatedBy = row.SafeString("CreatedBy", "CreatedBy"),
        CreatedDate = row.SafeDateTime("CreatedDate", "CreatedDate") ?? DateTime.MinValue
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


