using System.Data;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Store;
using Microsoft.Data.SqlClient;

namespace GFI_Upgrated.Data.Store;

public sealed class ReportRepository : IReportRepository
{
    private readonly string _connectionString;

    public ReportRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<PagedResult<ItemStockReportDto>> GetItemStockReportAsync(ItemStockReportRequest request, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@StockID", SqlDbType.BigInt) { Value = request.StockID ?? 0 },
            new("@CreatedBy", SqlDbType.VarChar, 200) { Value = request.CreatedBy ?? string.Empty },
            new("@CurrentPage", SqlDbType.Int) { Value = request.CurrentPage, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = request.RecordPerPage },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 20) { Value = request.SortType ?? "DESC" },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = request.SortColumn ?? "StockID" },
            new("@ItemID", SqlDbType.BigInt) { Value = request.ItemID ?? 0 },
            new("@WarehouseID", SqlDbType.BigInt) { Value = request.WarehouseID ?? 0 },
            new("@UnitId", SqlDbType.BigInt) { Value = request.UnitId ?? 0 }
        };

        try
        {
            var table = await ExecuteDataTableAsync("W_ItemStockList", parameters, cancellationToken);
            
            return new PagedResult<ItemStockReportDto>
            {
                CurrentPage = Convert.ToInt32(parameters[2].Value ?? request.CurrentPage),
                TotalRecord = Convert.ToInt32(parameters[4].Value ?? 0),
                Items = table.AsEnumerable().Select(MapItemStockReport).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemStockReportAsync: {ex.Message}", ex);
        }
    }

    public async Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByBatchNoAsync(string batchNo, int page, int size, string sortType, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@BatchNo", SqlDbType.NVarChar, 50) { Value = batchNo },
            new("@CurrentPage", SqlDbType.Int) { Value = page, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = size },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortType }
        };

        try
        {
            var table = await ExecuteDataTableAsync("BatchNumberList", parameters, cancellationToken);
            
            return new PagedResult<BatchWiseItemDto>
            {
                CurrentPage = Convert.ToInt32(parameters[1].Value ?? page),
                TotalRecord = Convert.ToInt32(parameters[3].Value ?? 0),
                Items = table.AsEnumerable().Select(MapBatchWiseItem).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetBatchWiseItemsByBatchNoAsync: {ex.Message}", ex);
        }
    }

    public async Task<PagedResult<BatchWiseItemDto>> GetBatchWiseItemsByItemAsync(long itemId, int page, int size, string sortType, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemID", SqlDbType.Int) { Value = (int)itemId },
            new("@CurrentPage", SqlDbType.Int) { Value = page, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = size },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortType }
        };

        try
        {
            var table = await ExecuteDataTableAsync("ItemBatchNumberList", parameters, cancellationToken);
            
            return new PagedResult<BatchWiseItemDto>
            {
                CurrentPage = Convert.ToInt32(parameters[1].Value ?? page),
                TotalRecord = Convert.ToInt32(parameters[3].Value ?? 0),
                Items = table.AsEnumerable().Select(MapBatchWiseItem).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetBatchWiseItemsByItemAsync: {ex.Message}", ex);
        }
    }

    public async Task<PagedResult<ItemStockByBatchReportDto>> GetItemStockByBatchReportAsync(long? itemStockByBatchId, long? stockById, long? itemId, int page, int size, string sortCol, string sortOrd, CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@ItemStockByBatchId", SqlDbType.Int) { Value = (int)(itemStockByBatchId ?? 0) },
            new("@StockById", SqlDbType.Int) { Value = (int)(stockById ?? 0) },
            new("@ItemID", SqlDbType.Int) { Value = (int)(itemId ?? 0) },
            new("@CurrentPage", SqlDbType.Int) { Value = page, Direction = ParameterDirection.InputOutput },
            new("@RecordPerPage", SqlDbType.Int) { Value = size },
            new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.InputOutput },
            new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortOrd },
            new("@SortColumn", SqlDbType.VarChar, 20) { Value = sortCol }
        };

        try
        {
            var table = await ExecuteDataTableAsync("Inv_ItemStockByBatchList", parameters, cancellationToken);
            
            return new PagedResult<ItemStockByBatchReportDto>
            {
                CurrentPage = Convert.ToInt32(parameters[3].Value ?? page),
                TotalRecord = Convert.ToInt32(parameters[5].Value ?? 0),
                Items = table.AsEnumerable().Select(MapItemStockByBatchReport).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in GetItemStockByBatchReportAsync: {ex.Message}", ex);
        }
    }

    private ItemStockReportDto MapItemStockReport(DataRow row) => new()
    {
        StockID = row.SafeLong("StockID"),
        ItemID = row.SafeLong("ItemID"),
        ItemName = row.SafeString("ItemName"),
        ItemCode = row.SafeString("ItemCode"),
        WarehouseName = row.SafeString("WarehouseName"),
        OpeningQuantity = row.SafeDouble("OpeningQuantity"),
        PurchasedQuantity = row.SafeDouble("PurchasedQuantity"),
        IssuedQuantity = row.SafeDouble("IssuedQuantity"),
        RemovedQuantity = row.SafeDouble("RemovedQuantity"),
        FinalStock = row.SafeDouble("FinalStock")
    };

    private BatchWiseItemDto MapBatchWiseItem(DataRow row) => new()
    {
        Id = row.SafeLong("Id"),
        BatchNo = row.SafeString("BatchNo"),
        ProcessingDate = row.SafeString("ProcessingDate"),
        ExpiryDate = row.SafeString("ExpiryDate"),
        ItemName = row.SafeString("ItemName"),
        AccountName = row.SafeString("AccountName")
    };

    private ItemStockByBatchReportDto MapItemStockByBatchReport(DataRow row) => new()
    {
        ItemStockByBatchId = row.SafeLong("ItemStockByBatchId"),
        StockById = row.SafeInt("StockById"),
        IdFrom = row.SafeLong("IdFrom"),
        ItemId = row.SafeLong("ItemId"),
        Quantity = row.SafeDouble("Quantity"),
        Unit = row.SafeInt("Unit"),
        BatchNo = row.SafeString("BatchNo"),
        ExpiryDate = row.SafeString("ExpiryDate"),
        WarehouseId = row.SafeLong("WarehouseId"),
        FinalQuantityLeft = row.SafeDouble("FinalQuantityLeft"),
        ItemName = row.SafeString("ItemName"),
        UnitName = row.SafeString("UnitName"),
        WarehouseName = row.SafeString("WarehouseName")
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
}
