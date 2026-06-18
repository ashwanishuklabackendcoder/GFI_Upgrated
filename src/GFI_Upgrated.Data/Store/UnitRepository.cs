using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class UnitRepository : IUnitRepository
{
    private readonly string _connectionString;

    public UnitRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<UnitDto>> GetUnitsAsync(PagedRequest request, string? searchText, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@UnitId", SqlDbType.BigInt) { Value = 0L },
            new("@UnitName", SqlDbType.NVarChar, 200) { Value = searchText ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "UnitId" }
        };

        try
        {
            var table = await Common.ResilientSqlExecutor.ExecuteDataTableAsync(_connectionString, "W_MasterUnitList", parameters, cancellationToken);
            
            return new PagedResult<UnitDto>
            {
                CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapUnit).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetUnitsAsync (Legacy SP: W_MasterUnitList): {ex.Message}", ex);
        }
    }

    public async Task<UnitDto?> GetUnitByIdAsync(long unitId, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@UnitId", SqlDbType.BigInt) { Value = unitId }
        };

        try
        {
            var table = await Common.ResilientSqlExecutor.ExecuteDataTableAsync(_connectionString, "W_MasterUnitSelectAll", parameters, cancellationToken);
            var row = table.AsEnumerable().FirstOrDefault(r => r.SafeLong("UnitId", "UnitID") == unitId);
            return row == null ? null : MapUnit(row);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetUnitByIdAsync (Legacy SP: W_MasterUnitSelectAll): {ex.Message}", ex);
        }
    }

    public async Task<int> SaveUnitAsync(SaveUnitRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@UnitId", SqlDbType.BigInt) { Value = request.UnitId },
            new SqlParameter("@UnitName", SqlDbType.NVarChar, 200) { Value = request.UnitName.Trim() },
            new SqlParameter("@ConversionValue", SqlDbType.Float) { Value = (object?)request.ConversionValue ?? DBNull.Value },
            new SqlParameter("@BaseUnit", SqlDbType.BigInt) { Value = (object?)request.BaseUnit ?? DBNull.Value },
            new SqlParameter("@IsActive", SqlDbType.Bit) { Value = request.IsActive },
            new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = request.UpdatedBy },
            new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        await Common.ResilientSqlExecutor.ExecuteNonQueryAsync(_connectionString, "W_MasterUnitModify", parameters, cancellationToken);
        return Convert.ToInt32(parameters[^1].Value ?? 0);
    }

    public async Task<int> DeleteUnitAsync(long unitId, string updatedBy, CancellationToken cancellationToken = default)
    {
        var parameters = new[]
        {
            new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = unitId.ToString() },
            new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 for Delete
            new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = updatedBy },
            new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.InputOutput }
        };

        await Common.ResilientSqlExecutor.ExecuteNonQueryAsync(_connectionString, "W_MasterUnitOperation", parameters, cancellationToken);
        var result = Convert.ToInt32(parameters[^1].Value ?? 0);
        
        return result == 1 ? 1 : (result == 0 ? -1 : 0);
    }

    public async Task<IReadOnlyList<BaseUnitLookupDto>> GetBaseUnitsLookupAsync(CancellationToken cancellationToken = default)
    {
        var table = await Common.ResilientSqlExecutor.ExecuteDataTableAsync(_connectionString, "W_MasterBaseUnitSelectAll", new List<SqlParameter>(), cancellationToken);
        return table.AsEnumerable().Select(r => new BaseUnitLookupDto
        {
            UnitId = r.SafeLong("UnitId", "UnitID"),
            UnitName = r.SafeString("UnitName")
        }).ToList();
    }

    private UnitDto MapUnit(DataRow row) => new()
    {
        UnitId = row.SafeLong("UnitId", "UnitID"),
        UnitName = row.SafeString("UnitName"),
        ConversionValue = row.Table.Columns.Contains("ConversionValue") && row["ConversionValue"] != DBNull.Value ? Convert.ToDouble(row["ConversionValue"]) : null,
        BaseUnit = row.Table.Columns.Contains("BaseUnit") && row["BaseUnit"] != DBNull.Value ? Convert.ToInt64(row["BaseUnit"]) : null,
        BaseName = row.SafeString("BaseName"),
        IsActive = row.SafeBool("IsActive"),
        CreatedBy = row.SafeString("CreatedBy"),
        CreatedDate = row.SafeDateTime("CreatedDate") ?? DateTime.MinValue
    };
}
