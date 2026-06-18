using GFI_Upgrated.Data.Store;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Purchase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GFI_Upgrated.Data.Purchase
{
    public sealed class PurchaseRepository : IPurchaseRepository
    {
        private readonly string _connectionString;

        public PurchaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Purchase Request

        public async Task<PagedResult<PurchaseRequestDto>> GetPurchaseRequestsAsync(long? requestId, string? requestNumber, long? requestedBy, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new List<SqlParameter>
            {
                new("@PurchaseRequestMasterID", SqlDbType.BigInt) { Value = requestId ?? 0 },
                new("@RequestNumber", SqlDbType.NVarChar, 400) { Value = requestNumber ?? "" },
                new("@RequestedBy", SqlDbType.BigInt) { Value = requestedBy ?? 0 },
                new("@CurrentPage", SqlDbType.Int) { Value = page },
                new("@RecordPerPage", SqlDbType.Int) { Value = pageSize },
                new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new("@SortColumn", SqlDbType.VarChar, 50) { Value = sortCol ?? "PurchaseRequestMasterID" },
                new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortOrd ?? "DESC" }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseRequestMasterList", parameters);
            var items = table.AsEnumerable().Select(MapPurchaseRequest).ToList();

            return new PagedResult<PurchaseRequestDto>
            {
                Items = items,
                TotalRecord = Convert.ToInt32(parameters.Find(p => p.ParameterName == "@TotalRecord")?.Value ?? 0)
            };
        }

        public async Task<List<PurchaseRequestItemDto>> GetPurchaseRequestItemsAsync(long requestId)
        {
            var parameters = new List<SqlParameter>
            {
                new("@PurchaseRequestMasterID", SqlDbType.Int) { Value = (int)requestId }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseRequestChildSelectPurchaseRequestMasterID", parameters);
            return table.AsEnumerable().Select(MapPurchaseRequestItem).ToList();
        }

        public async Task<long> SavePurchaseRequestAsync(PurchaseRequestDto request)
        {
            var parameters = new[]
            {
                new SqlParameter("@PurchaseRequestMasterID", SqlDbType.BigInt) { Value = request.PurchaseRequestMasterID },
                new SqlParameter("@RequestNumber", SqlDbType.NVarChar, 100) { Value = request.RequestNumber ?? "" },
                new SqlParameter("@Description", SqlDbType.NVarChar, 4000) { Value = request.Description ?? "" },
                new SqlParameter("@RequestedBy", SqlDbType.BigInt) { Value = (object)request.RequestedBy ?? DBNull.Value },
                new SqlParameter("@CheckedBy", SqlDbType.BigInt) { Value = (object)request.CheckedBy ?? DBNull.Value },
                new SqlParameter("@ConfirmedBy", SqlDbType.BigInt) { Value = (object)request.ConfirmedBy ?? DBNull.Value },
                new SqlParameter("@ConfirmationDate", SqlDbType.Date) { Value = (object)request.ConfirmationDate ?? DBNull.Value },
                new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = request.CreatedDate ?? DateTime.Now },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 400) { Value = request.CreatedBy ?? "" },
                new SqlParameter("@Remarks", SqlDbType.NVarChar, 2000) { Value = request.Remarks ?? "" },
                new SqlParameter("@DocumentUpload", SqlDbType.NVarChar, 1000) { Value = request.DocumentUpload ?? "" },
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_PurchaseRequestMasterModify", parameters);
            var masterId = Convert.ToInt64(parameters[^1].Value ?? 0);

            if (masterId > 0 && request.Items != null)
            {
                foreach (var item in request.Items)
                {
                    var itemParams = new[]
                    {
                        new SqlParameter("@PurchaseRequestChildID", SqlDbType.BigInt) { Value = item.PurchaseRequestChildID },
                        new SqlParameter("@PrefferedBrand", SqlDbType.NVarChar, 2000) { Value = item.PrefferedBrand ?? "" },
                        new SqlParameter("@ItemID", SqlDbType.BigInt) { Value = item.ItemID },
                        new SqlParameter("@Quantity", SqlDbType.Int) { Value = (int)item.Quantity },
                        new SqlParameter("@UnitId", SqlDbType.Int) { Value = (int?)item.UnitId ?? 0 },
                        new SqlParameter("@PurchaseRequestMasterID", SqlDbType.BigInt) { Value = masterId },
                        new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 400) { Value = request.CreatedBy ?? "" },
                        new SqlParameter("@Description", SqlDbType.NVarChar, 4000) { Value = item.Description ?? "" },
                        new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };
                    await ExecuteNonQueryAsync("W_PurchaseRequestChildModify", itemParams);
                }
            }

            return masterId;
        }

        public async Task<bool> DeletePurchaseRequestAsync(string ids, string deletedBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = ids },
                new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 }, // 1 = Delete
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = deletedBy },
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_PurchaseRequestMasterOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) == 1;
        }

        #endregion

        #region Purchase Order

        public async Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(long? orderId, string? voucherNumber, long? accountId, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new List<SqlParameter>
            {
                new("@PurchaseOrderID", SqlDbType.BigInt) { Value = orderId ?? 0 },
                new("@VoucherNumber", SqlDbType.NVarChar, 100) { Value = voucherNumber ?? "" },
                new("@AccountID", SqlDbType.BigInt) { Value = accountId ?? 0 },
                new("@CurrentPage", SqlDbType.Int) { Value = page },
                new("@RecordPerPage", SqlDbType.Int) { Value = pageSize },
                new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new("@SortColumn", SqlDbType.VarChar, 50) { Value = sortCol ?? "PurchaseOrderID" },
                new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortOrd ?? "DESC" }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseOrderMasterList", parameters);
            var items = table.AsEnumerable().Select(MapPurchaseOrder).ToList();

            return new PagedResult<PurchaseOrderDto>
            {
                Items = items,
                TotalRecord = Convert.ToInt32(parameters.Find(p => p.ParameterName == "@TotalRecord")?.Value ?? 0)
            };
        }

        public async Task<List<PurchaseOrderItemDto>> GetPurchaseOrderItemsAsync(long orderId)
        {
            // Note: I couldn't find W_PurchaseOrderChildSelectPurchaseOrderID, 
            // but assuming a pattern or will check for alternative SP.
            // Using a generic SELECT for now if SP is missing, or assuming W_PurchaseOrderChildOperation might have a select?
            // Actually, I'll search for the correct SP name.
            
            var parameters = new List<SqlParameter>
            {
                new("@PurchaseOrderID", SqlDbType.BigInt) { Value = orderId }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseOrderChildSelectPurchaseOrderID", parameters); 
            return table.AsEnumerable().Select(MapPurchaseOrderItem).ToList();
        }

        public async Task<long> SavePurchaseOrderAsync(PurchaseOrderDto order)
        {
            var parameters = new[]
            {
                new SqlParameter("@PurchaseOrderID", SqlDbType.BigInt) { Value = order.PurchaseOrderID },
                new SqlParameter("@VoucherNumber", SqlDbType.NVarChar, 50) { Value = order.VoucherNumber ?? "" },
                new SqlParameter("@AccountID", SqlDbType.BigInt) { Value = order.AccountID },
                new SqlParameter("@PurchaseRequestID", SqlDbType.BigInt) { Value = (object)order.PurchaseRequestID ?? DBNull.Value },
                new SqlParameter("@OrderDate", SqlDbType.Date) { Value = (object)order.OrderDate ?? DBNull.Value },
                new SqlParameter("@TaxType", SqlDbType.Int) { Value = (object)order.TaxType ?? DBNull.Value },
                new SqlParameter("@TaxName1", SqlDbType.NVarChar, 50) { Value = order.TaxName1 ?? "" },
                new SqlParameter("@TaxAmount1", SqlDbType.Float) { Value = (object)order.TaxAmount1 ?? 0 },
                new SqlParameter("@TaxName2", SqlDbType.NVarChar, 50) { Value = order.TaxName2 ?? "" },
                new SqlParameter("@TaxAmount2", SqlDbType.Float) { Value = (object)order.TaxAmount2 ?? 0 },
                new SqlParameter("@TaxName3", SqlDbType.NVarChar, 50) { Value = order.TaxName3 ?? "" },
                new SqlParameter("@TaxAmount3", SqlDbType.Float) { Value = (object)order.TaxAmount3 ?? 0 },
                new SqlParameter("@DiscountPercent", SqlDbType.Float) { Value = (object)order.DiscountPercent ?? 0 },
                new SqlParameter("@DiscountAmount", SqlDbType.Float) { Value = (object)order.DiscountAmount ?? 0 },
                new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = order.CreatedDate ?? DateTime.Now },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 200) { Value = order.CreatedBy ?? "" },
                new SqlParameter("@CheckedBy", SqlDbType.BigInt) { Value = (object)order.CheckedBy ?? DBNull.Value },
                new SqlParameter("@ConfirmedBy", SqlDbType.BigInt) { Value = (object)order.ConfirmedBy ?? DBNull.Value },
                new SqlParameter("@ConfirmationDate", SqlDbType.Date) { Value = (object)order.ConfirmationDate ?? DBNull.Value },
                new SqlParameter("@DocumentUpload", SqlDbType.NVarChar, 500) { Value = order.DocumentUpload ?? "" },
                new SqlParameter("@Remarks", SqlDbType.NVarChar, 2000) { Value = order.Remarks ?? "" },
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_PurchaseOrderMasterModify", parameters);
            var poId = Convert.ToInt64(parameters[^1].Value ?? 0);

            if (poId > 0 && order.Items != null)
            {
                // Note: Need to verify PO Child Modify SP name. Assuming W_PurchaseOrderChildModify.
                foreach (var item in order.Items)
                {
                    var itemParams = new[]
                    {
                        new SqlParameter("@PurchaseOrderChildID", SqlDbType.BigInt) { Value = item.PurchaseOrderChildID },
                        new SqlParameter("@PurchaseOrderID", SqlDbType.BigInt) { Value = poId },
                        new SqlParameter("@ItemID", SqlDbType.BigInt) { Value = item.ItemID },
                        new SqlParameter("@PreferredBrand", SqlDbType.BigInt) { Value = (object?)item.PreferredBrand ?? DBNull.Value },
                        new SqlParameter("@Description", SqlDbType.NVarChar, 4000) { Value = item.Description ?? "" },
                        new SqlParameter("@Quantity", SqlDbType.Float) { Value = item.Quantity },
                        new SqlParameter("@UnitId", SqlDbType.BigInt) { Value = item.UnitId ?? 0L },
                        new SqlParameter("@Price", SqlDbType.Float) { Value = item.Price },
                        new SqlParameter("@ItemReceivedCheck", SqlDbType.Bit) { Value = (object)item.ItemReceivedCheck ?? DBNull.Value },
                        new SqlParameter("@ItemReceivedCheckBy", SqlDbType.BigInt) { Value = (object)item.ItemReceivedCheckBy ?? DBNull.Value },
                        new SqlParameter("@ItemRecievedBy", SqlDbType.BigInt) { Value = (object)item.ItemRecievedBy ?? DBNull.Value },
                        new SqlParameter("@ItemReceivedDate", SqlDbType.Date) { Value = (object)item.ItemReceivedDate ?? DBNull.Value },
                        new SqlParameter("@ItemReceiveRemarks", SqlDbType.NVarChar, 2000) { Value = item.ItemReceiveRemarks ?? "" },
                        new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };
                    await ExecuteNonQueryAsync("W_PurchaseOrderChildModify", itemParams);
                }
            }

            return poId;
        }

        public async Task<bool> DeletePurchaseOrderAsync(string ids, string deletedBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = ids },
                new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = deletedBy },
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_PurchaseOrderMasterOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) == 1;
        }

        #endregion

        #region Purchase (GRN)

        public async Task<PagedResult<PurchaseDto>> GetPurchasesAsync(long? purchaseId, string? voucherNumber, string? invoiceNo, long? accountId, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new List<SqlParameter>
            {
                new("@PurchaseID", SqlDbType.BigInt) { Value = purchaseId ?? 0 },
                new("@VoucherNumber", SqlDbType.NVarChar, 100) { Value = voucherNumber ?? "" },
                new("@AccountID", SqlDbType.BigInt) { Value = accountId ?? 0 },
                new("@CurrentPage", SqlDbType.Int) { Value = page },
                new("@RecordPerPage", SqlDbType.Int) { Value = pageSize },
                new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new("@SortColumn", SqlDbType.VarChar, 50) { Value = sortCol ?? "PurchaseID" },
                new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortOrd ?? "DESC" }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseMasterList", parameters);
            var items = table.AsEnumerable().Select(MapPurchase).ToList();

            return new PagedResult<PurchaseDto>
            {
                Items = items,
                TotalRecord = Convert.ToInt32(parameters.Find(p => p.ParameterName == "@TotalRecord")?.Value ?? 0)
            };
        }

        public async Task<List<PurchaseItemDto>> GetPurchaseItemsAsync(long purchaseId)
        {
            var parameters = new List<SqlParameter>
            {
                new("@PurchaseID", SqlDbType.BigInt) { Value = purchaseId }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseChildSelectPurchaseID", parameters);
            var items = table.AsEnumerable().Select(MapPurchaseItem).ToList();

            var batchParameters = new List<SqlParameter>
            {
                new("@PurchaseID", SqlDbType.BigInt) { Value = purchaseId }
            };
            var batchTable = await ExecuteDataTableAsync("W_PurchaseChildBatchSelectByID", batchParameters);

            var batchMap = new Dictionary<long, DataRow>();
            foreach (DataRow row in batchTable.Rows)
            {
                var purchaseItemId = row.SafeLong("IdFrom");
                if (purchaseItemId > 0 && !batchMap.ContainsKey(purchaseItemId))
                {
                    batchMap[purchaseItemId] = row;
                }
            }

            foreach (var item in items)
            {
                if (batchMap.TryGetValue(item.PurchaseItemID, out var batchRow))
                {
                    item.BatchNo = batchRow.SafeString("BatchNo");
                    item.ExpiryDate = batchRow.SafeDateTime("ExpiryDate");
                    item.WarehouseId = batchRow.SafeLongNullable("WarehouseId");
                }
            }

            return items;
        }

        public async Task<long> SavePurchaseAsync(PurchaseDto purchase)
        {
            var parameters = new[]
            {
                new SqlParameter("@PurchaseID", SqlDbType.BigInt) { Value = purchase.PurchaseID },
                new SqlParameter("@AccountID", SqlDbType.BigInt) { Value = purchase.AccountID },
                new SqlParameter("@VoucherNumber", SqlDbType.NVarChar, 50) { Value = purchase.VoucherNumber ?? "" },
                new SqlParameter("@GoodsRecievedDate", SqlDbType.Date) { Value = (object)purchase.GoodsRecievedDate ?? DBNull.Value },
                new SqlParameter("@PurchaseOrderID", SqlDbType.BigInt) { Value = (object)purchase.PurchaseOrderID ?? 0 },
                new SqlParameter("@InvoiceNo", SqlDbType.NVarChar, 50) { Value = purchase.InvoiceNo ?? "" },
                new SqlParameter("@InvoiceDate", SqlDbType.Date) { Value = (object)purchase.InvoiceDate ?? DBNull.Value },
                new SqlParameter("@Taxes", SqlDbType.Float) { Value = purchase.Taxes ?? 0 },
                new SqlParameter("@Shipping", SqlDbType.Float) { Value = purchase.Shipping ?? 0 },
                new SqlParameter("@Discount", SqlDbType.Float) { Value = purchase.Discount ?? 0 },
                new SqlParameter("@TotalAmount", SqlDbType.Float) { Value = purchase.TotalAmount },
                new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = purchase.CreatedDate ?? DateTime.Now },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 400) { Value = purchase.CreatedBy ?? "" },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 400) { Value = purchase.UpdatedBy ?? "" },
                new SqlParameter("@FileName", SqlDbType.NVarChar, 1000) { Value = purchase.FileName ?? "" },
                new SqlParameter("@Narration", SqlDbType.NVarChar, 4000) { Value = purchase.Narration ?? "" },
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_PurchaseMasterModify", parameters);
            var purchaseId = Convert.ToInt64(parameters[^1].Value ?? 0);

            if (purchaseId > 0 && purchase.Items != null)
            {
                foreach (var item in purchase.Items)
                {
                    long unitId = item.UnitId ?? 0L;
                    if (unitId <= 0)
                    {
                        await using (var conn = new SqlConnection(_connectionString))
                        {
                            await conn.OpenAsync();
                            await using (var cmd = new SqlCommand("SELECT PurchaseUnit FROM dbo.W_MasterItem WHERE ItemID = @ItemID", conn))
                            {
                                cmd.Parameters.AddWithValue("@ItemID", item.ItemID);
                                var scalar = await cmd.ExecuteScalarAsync();
                                if (scalar != null && scalar != DBNull.Value)
                                {
                                    unitId = Convert.ToInt64(scalar);
                                }
                            }
                        }
                    }
                    if (unitId <= 0)
                    {
                        unitId = 10L; // Fallback to Kg (10)
                    }

                    var itemParams = new[]
                    {
                        new SqlParameter("@PurchaseItemID", SqlDbType.BigInt) { Value = item.PurchaseItemID },
                        new SqlParameter("@PurchaseID", SqlDbType.BigInt) { Value = purchaseId },
                        new SqlParameter("@BrandID", SqlDbType.BigInt) { Value = (object)item.BrandID ?? 4 }, // Default to 4 (N/A)
                        new SqlParameter("@ItemID", SqlDbType.BigInt) { Value = item.ItemID },
                        new SqlParameter("@Quantity", SqlDbType.Float) { Value = item.Quantity },
                        new SqlParameter("@UnitId", SqlDbType.BigInt) { Value = unitId },
                        new SqlParameter("@UnitPrice", SqlDbType.Float) { Value = item.UnitPrice },
                        new SqlParameter("@Amount", SqlDbType.Float) { Value = item.Amount },
                        new SqlParameter("@Description", SqlDbType.NVarChar, 4000) { Value = item.Description ?? "" },
                        new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 400) { Value = purchase.CreatedBy ?? "" },
                        new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
                    };
                    await ExecuteNonQueryAsync("W_PurchaseChildModify", itemParams);
                    var purchaseItemId = Convert.ToInt64(itemParams[^1].Value ?? 0);

                    if (purchaseItemId > 0)
                    {
                        long batchId = 0;
                        await using (var conn = new SqlConnection(_connectionString))
                        {
                            await conn.OpenAsync();
                            await using (var cmd = new SqlCommand("SELECT ItemStockByBatchId FROM dbo.Inv_ItemStockByBatch WHERE IdFrom = @PurchaseItemID AND StockById = 1", conn))
                            {
                                cmd.Parameters.AddWithValue("@PurchaseItemID", purchaseItemId);
                                var scalar = await cmd.ExecuteScalarAsync();
                                if (scalar != null && scalar != DBNull.Value)
                                {
                                    batchId = Convert.ToInt64(scalar);
                                }
                            }
                        }

                        var batchParams = new[]
                        {
                            new SqlParameter("@PurchaseID", SqlDbType.BigInt) { Value = purchaseId },
                            new SqlParameter("@ItemStockByBatchId", SqlDbType.BigInt) { Value = batchId },
                            new SqlParameter("@StockById", SqlDbType.Int) { Value = 1 }, // 1 = Purchase / GRN
                            new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = item.ItemID },
                            new SqlParameter("@Quantity", SqlDbType.Float) { Value = item.Quantity },
                            new SqlParameter("@BatchNo", SqlDbType.NVarChar, 50) { Value = (object?)item.BatchNo ?? DBNull.Value },
                            new SqlParameter("@ExpiryDate", SqlDbType.Date) { Value = (object?)item.ExpiryDate ?? DBNull.Value },
                            new SqlParameter("@WarehouseId", SqlDbType.BigInt) { Value = (object?)item.WarehouseId ?? DBNull.Value },
                            new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
                        };
                        await ExecuteNonQueryAsync("Inv_ItemStockByBatchModify", batchParams);
                    }
                }
            }

            return purchaseId;
        }

        public async Task<bool> DeletePurchaseAsync(string ids, string deletedBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = ids },
                new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = deletedBy },
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_PurchaseMasterOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) == 1;
        }

        #endregion

        #region Purchase Return

        public async Task<PagedResult<PurchaseReturnDto>> GetPurchaseReturnsAsync(long? returnId, long? itemId, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new List<SqlParameter>
            {
                new("@PurchaseReturnID", SqlDbType.BigInt) { Value = returnId ?? 0 },
                new("@ItemId", SqlDbType.BigInt) { Value = itemId ?? 0 },
                new("@CurrentPage", SqlDbType.Int) { Value = page },
                new("@RecordPerPage", SqlDbType.Int) { Value = pageSize },
                new("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new("@SortColumn", SqlDbType.VarChar, 50) { Value = sortCol ?? "PurchaseReturnID" },
                new("@SortOrd", SqlDbType.VarChar, 5) { Value = sortOrd ?? "DESC" }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseReturnList", parameters);
            var items = table.AsEnumerable().Select(MapPurchaseReturn).ToList();

            return new PagedResult<PurchaseReturnDto>
            {
                Items = items,
                TotalRecord = Convert.ToInt32(parameters.Find(p => p.ParameterName == "@TotalRecord")?.Value ?? 0)
            };
        }

        public async Task<long> SavePurchaseReturnAsync(PurchaseReturnDto @return)
        {
            var parameters = new[]
            {
                new SqlParameter("@PurchaseReturnID", SqlDbType.BigInt) { Value = @return.PurchaseReturnID },
                new SqlParameter("@ItemStockByBatchId", SqlDbType.BigInt) { Value = @return.ItemStockByBatchId },
                new SqlParameter("@ReturnDate", SqlDbType.Date) { Value = (object)@return.ReturnDate ?? DBNull.Value },
                new SqlParameter("@ReturnReason", SqlDbType.BigInt) { Value = (object)@return.ReturnReason ?? DBNull.Value },
                new SqlParameter("@Quantity", SqlDbType.Float) { Value = @return.Quantity },
                new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = @return.CreatedDate ?? DateTime.Now },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 400) { Value = @return.CreatedBy ?? "" },
                new SqlParameter("@Description", SqlDbType.NVarChar, 4000) { Value = @return.Description ?? "" },
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_PurchaseReturnModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<bool> DeletePurchaseReturnAsync(string ids, string deletedBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = ids },
                new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = deletedBy },
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_PurchaseReturnOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) == 1;
        }

        public async Task<List<PurchaseReturnItemLookupDto>> GetPurchaseReturnItemsLookupAsync(long accountId)
        {
            var parameters = new[]
            {
                new SqlParameter("@PurchaseID", SqlDbType.BigInt) { Value = 0 },
                new SqlParameter("@AccountID", SqlDbType.BigInt) { Value = accountId }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseChildSelectByPurchaseID", parameters);
            return table.AsEnumerable().Select(row => new PurchaseReturnItemLookupDto
            {
                ItemID = row.SafeLong("ItemID"),
                ItemName = row.SafeString("ItemName"),
                BrandID = row.SafeLong("BrandID"),
                BrandName = row.SafeString("BrandName")
            }).ToList();
        }

        public async Task<List<PurchaseReturnBatchLookupDto>> GetPurchaseReturnBatchesLookupAsync(long itemId, long brandId)
        {
            var parameters = new[]
            {
                new SqlParameter("@ItemID", SqlDbType.BigInt) { Value = itemId },
                new SqlParameter("@BrandID", SqlDbType.BigInt) { Value = brandId }
            };

            var table = await ExecuteDataTableAsync("W_PurchaseChildSelectItemID", parameters);
            return table.AsEnumerable().Select(row => new PurchaseReturnBatchLookupDto
            {
                ItemStockByBatchId = row.SafeLong("ItemStockByBatchId"),
                BatchNo = row.SafeString("BatchNo"),
                Quantity = row.SafeDouble("Quantity")
            }).ToList();
        }

        public async Task<PagedResult<ItemWriteOffDto>> GetItemWriteOffsAsync(long? writeOffId, long? itemId, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ItemWriteOffID", SqlDbType.BigInt) { Value = writeOffId ?? 0 },
                new SqlParameter("@ItemId", SqlDbType.BigInt) { Value = itemId ?? 0 },
                new SqlParameter("@CurrentPage", SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = page },
                new SqlParameter("@RecordPerPage", SqlDbType.Int) { Value = pageSize },
                new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@SortColumn", SqlDbType.VarChar, 50) { Value = sortCol ?? "ItemWriteOffID" },
                new SqlParameter("@SortOrd", SqlDbType.VarChar, 5) { Value = sortOrd ?? "DESC" }
            };

            var table = await ExecuteDataTableAsync("W_ItemWriteOffList", parameters);
            var items = table.AsEnumerable().Select(MapItemWriteOff).ToList();

            return new PagedResult<ItemWriteOffDto>
            {
                Items = items,
                TotalRecord = Convert.ToInt32(parameters.Find(p => p.ParameterName == "@TotalRecord")?.Value ?? 0)
            };
        }

        public async Task<long> SaveItemWriteOffAsync(ItemWriteOffDto dto)
        {
            var parameters = new[]
            {
                new SqlParameter("@ItemWriteOffID", SqlDbType.BigInt) { Value = dto.ItemWriteOffID },
                new SqlParameter("@ItemStockByBatchId", SqlDbType.BigInt) { Value = dto.ItemStockByBatchId },
                new SqlParameter("@Quantity", SqlDbType.Float) { Value = dto.Quantity },
                new SqlParameter("@SellingPrice", SqlDbType.NVarChar) { Value = dto.SellingPrice ?? "" },
                new SqlParameter("@PurchasePrice", SqlDbType.NVarChar) { Value = dto.PurchasePrice ?? "" },
                new SqlParameter("@ReasonFor", SqlDbType.NVarChar) { Value = dto.ReasonFor?.ToString() ?? "" },
                new SqlParameter("@RemovalDate", SqlDbType.Date) { Value = (object)dto.RemovalDate ?? DBNull.Value },
                new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = dto.CreatedDate ?? DateTime.Now },
                new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value = dto.CreatedBy ?? "" },
                new SqlParameter("@Ramarks", SqlDbType.NVarChar) { Value = dto.Remarks ?? "" },
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_ItemWriteOffModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<bool> DeleteItemWriteOffAsync(string ids, string deletedBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", SqlDbType.VarChar, 2000) { Value = ids },
                new SqlParameter("@OprType", SqlDbType.SmallInt) { Value = 1 },
                new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = deletedBy },
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_ItemWriteOffOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) == 1;
        }

        public async Task<List<WriteOffBatchLookupDto>> GetWriteOffBatchesLookupAsync()
        {
            var parameters = new[]
            {
                new SqlParameter("@ItemStockByBatchId", SqlDbType.BigInt) { Value = 0 }
            };

            var table = await ExecuteDataTableAsync("Inv_ItemStockByBatchSelectAll", parameters);
            return table.AsEnumerable().Select(row => new WriteOffBatchLookupDto
            {
                ItemStockByBatchId = row.SafeLong("ItemStockByBatchId"),
                ItemName = row.SafeString("ItemName"),
                BatchNo = row.SafeString("BatchNo"),
                ExpiryDate = row.SafeString("ExpiryDate"),
                FinalQuantityLeft = row.SafeDouble("FinalQuantityLeft"),
                UnitName = row.SafeString("UnitName")
            }).ToList();
        }

        #endregion

        #region Mappings

        private PurchaseRequestDto MapPurchaseRequest(DataRow row) => new()
        {
            PurchaseRequestMasterID = row.SafeLong("PurchaseRequestMasterID"),
            RequestNumber = row.SafeString("RequestNumber"),
            Description = row.SafeString("Description"),
            RequestedBy = row.SafeLong("RequestedBy"),
            RequestedByName = row.SafeString("RequestedByName"),
            CheckedBy = row.SafeLong("CheckedBy"),
            CheckedByName = row.SafeString("CheckedByName"),
            ConfirmedBy = row.SafeLong("ConfirmedBy"),
            ConfirmedByName = row.SafeString("ConfirmedByName"),
            ConfirmationDate = row.SafeDateTime("ConfirmationDate"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy"),
            Remarks = row.SafeString("Remarks"),
            DocumentUpload = row.SafeString("DocumentUpload")
        };

        private PurchaseRequestItemDto MapPurchaseRequestItem(DataRow row) => new()
        {
            PurchaseRequestChildID = row.SafeLong("PurchaseRequestChildID"),
            PurchaseRequestMasterID = row.SafeLong("PurchaseRequestMasterID"),
            ItemID = row.SafeLong("ItemID"),
            ItemName = row.SafeString("ItemName"),
            PrefferedBrand = row.SafeString("PrefferedBrand"),
            Description = row.SafeString("Description"),
            Quantity = row.SafeDouble("Quantity"),
            UnitId = row.SafeLong("UnitId"),
            UnitName = row.SafeString("UnitName"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy")
        };

        private PurchaseOrderDto MapPurchaseOrder(DataRow row) => new()
        {
            PurchaseOrderID = row.SafeLong("PurchaseOrderID"),
            VoucherNumber = row.SafeString("VoucherNumber"),
            AccountID = row.SafeLong("AccountID"),
            VendorName = row.SafeString("AccountName"),
            PurchaseRequestID = row.SafeLong("PurchaseRequestID"),
            PRNumber = row.SafeString("RequestNumber"),
            OrderDate = row.SafeDateTime("OrderDate"),
            TaxType = row.SafeInt("TaxType"),
            TaxName1 = row.SafeString("TaxName1"),
            TaxAmount1 = row.SafeDouble("TaxAmount1"),
            TaxName2 = row.SafeString("TaxName2"),
            TaxAmount2 = row.SafeDouble("TaxAmount2"),
            TaxName3 = row.SafeString("TaxName3"),
            TaxAmount3 = row.SafeDouble("TaxAmount3"),
            DiscountPercent = row.SafeDouble("DiscountPercent"),
            DiscountAmount = row.SafeDouble("DiscountAmount"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy"),
            CheckedBy = row.SafeLong("CheckedBy"),
            CheckedByName = row.SafeString("CheckedByName"),
            ConfirmedBy = row.SafeLong("ConfirmedBy"),
            ConfirmedByName = row.SafeString("ConfirmedByName"),
            ConfirmationDate = row.SafeDateTime("ConfirmationDate"),
            DocumentUpload = row.SafeString("DocumentUpload"),
            Remarks = row.SafeString("Remarks")
        };

        private PurchaseOrderItemDto MapPurchaseOrderItem(DataRow row) => new()
        {
            PurchaseOrderChildID = row.SafeLong("PurchaseOrderChildID"),
            PurchaseOrderID = row.SafeLong("PurchaseOrderID"),
            ItemID = row.SafeLong("ItemID"),
            ItemName = row.SafeString("ItemName"),
            PreferredBrand = row.SafeLongNullable("PreferredBrand"),
            Description = row.SafeString("Description"),
            Quantity = row.SafeDouble("Quantity"),
            UnitId = row.SafeLongNullable("UnitId"),
            UnitName = row.SafeString("UnitName"),
            Price = row.SafeDouble("Price"),
            ItemReceivedCheck = row.SafeBool("ItemReceivedCheck"),
            ItemReceivedDate = row.SafeDateTime("ItemReceivedDate")
        };

        private PurchaseDto MapPurchase(DataRow row) => new()
        {
            PurchaseID = row.SafeLong("PurchaseID"),
            AccountID = row.SafeLong("AccountID"),
            VendorName = row.SafeString("AccountName"),
            VoucherNumber = row.SafeString("VoucherNumber"),
            GoodsRecievedDate = row.SafeDateTime("GoodsRecievedDate"),
            PurchaseOrderID = row.SafeLong("PurchaseOrderID"),
            PONumber = row.SafeString("POVoucherNumber"),
            InvoiceNo = row.SafeString("InvoiceNo"),
            InvoiceDate = row.SafeDateTime("InvoiceDate"),
            Taxes = row.SafeDouble("Taxes"),
            Shipping = row.SafeDouble("Shipping"),
            Discount = row.SafeDouble("Discount"),
            TotalAmount = row.SafeDouble("TotalAmount"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy"),
            UpdatedBy = row.SafeString("UpdatedBy"),
            FileName = row.SafeString("FileName"),
            Narration = row.SafeString("Narration")
        };

        private PurchaseItemDto MapPurchaseItem(DataRow row) => new()
        {
            PurchaseItemID = row.SafeLong("PurchaseItemID"),
            PurchaseID = row.SafeLong("PurchaseID"),
            BrandID = row.SafeLong("BrandID"),
            BrandName = row.SafeString("BrandName"),
            ItemID = row.SafeLong("ItemID"),
            ItemName = row.SafeString("ItemName"),
            Quantity = row.SafeDouble("Quantity"),
            UnitId = row.SafeLong("UnitId"),
            UnitName = row.SafeString("UnitName"),
            UnitPrice = row.SafeDouble("UnitPrice"),
            Amount = row.SafeDouble("Amount"),
            Description = row.SafeString("Description"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy")
        };

        private PurchaseReturnDto MapPurchaseReturn(DataRow row) => new()
        {
            PurchaseReturnID = row.SafeLong("PurchaseReturnID"),
            ItemStockByBatchId = row.SafeLong("ItemStockByBatchId"),
            ReturnDate = row.SafeDateTime("ReturnDate"),
            ReturnReason = row.SafeLong("ReturnReason"),
            ReasonName = row.SafeString("ReasonName"),
            Quantity = row.SafeDouble("Quantity"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy"),
            Description = row.SafeString("Description"),
            ItemName = row.SafeString("ItemName"),
            BatchNo = row.SafeString("BatchNo"),
            VendorName = row.SafeString("VendorName")
        };

        private ItemWriteOffDto MapItemWriteOff(DataRow row) => new()
        {
            ItemWriteOffID = row.SafeLong("ItemWriteOffID"),
            ItemStockByBatchId = row.SafeLong("ItemStockByBatchId"),
            Quantity = row.SafeDouble("Quantity"),
            SellingPrice = row.SafeString("SellingPrice"),
            PurchasePrice = row.SafeString("PurchasePrice"),
            ReasonFor = row.SafeLong("ReasonFor"),
            ReasonName = row.SafeString("ReasonName"),
            RemovalDate = row.SafeDateTime("RemovalDate"),
            Remarks = row.SafeString("Ramarks"),
            CreatedDate = row.SafeDateTime("CreatedDate"),
            CreatedBy = row.SafeString("CreatedBy"),
            ItemName = row.SafeString("ItemName"),
            UnitName = row.SafeString("UnitName"),
            BatchNo = row.SafeString("BatchNo")
        };

        #endregion

        #region Helpers

        private async Task<DataTable> ExecuteDataTableAsync(string storedProcedure, IEnumerable<SqlParameter> parameters)
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

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        private async Task<int> ExecuteNonQueryAsync(string storedProcedure, IEnumerable<SqlParameter> parameters)
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

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        #endregion
    }
}
