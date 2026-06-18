using GFI_Upgrated.Data.Account;
using GFI_Upgrated.SharedDto.Account;
using GFI_Upgrated.SharedDto.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GFI_Upgrated.Data.Account
{
    public sealed class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Currency

        public async Task<PagedResult<CurrencyDto>> GetCurrenciesAsync(long? currencyId, string? symbol, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new[]
            {
                new SqlParameter("@CurrencyID", (object?)currencyId ?? DBNull.Value),
                new SqlParameter("@CurrencySymbol", (object?)symbol ?? DBNull.Value),
                new SqlParameter("@CurrentPage", page),
                new SqlParameter("@RecordPerPage", pageSize),
                new SqlParameter("@SortColumn", sortCol),
                new SqlParameter("@SortOrd", sortOrd),
                new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            var table = await ExecuteStoredProcedureAsync("A_MasterCurrencyList", parameters);
            var list = new List<CurrencyDto>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(new CurrencyDto
                {
                    CurrencyID = Convert.ToInt64(row["CurrencyID"]),
                    CurrencySymbol = row["CurrencySymbol"]?.ToString() ?? "",
                    CurrencyString = row["CurrencyString"]?.ToString() ?? "",
                    CurrencySubString = row["CurrencySubString"]?.ToString() ?? "",
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }

            return new PagedResult<CurrencyDto>
            {
                Items = list,
                TotalRecord = Convert.ToInt32(parameters[^1].Value ?? 0),
                CurrentPage = page
            };
        }

        public async Task<long> SaveCurrencyAsync(CurrencyDto currency)
        {
            var parameters = new[]
            {
                new SqlParameter("@CurrencyID", currency.CurrencyID),
                new SqlParameter("@CurrencySymbol", currency.CurrencySymbol),
                new SqlParameter("@CurrencyString", currency.CurrencyString),
                new SqlParameter("@CurrencySubString", currency.CurrencySubString),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_MasterCurrencyModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<bool> DeleteCurrencyAsync(string ids)
        {
            var parameters = new[]
            {
                new SqlParameter("@IDs", ids),
                new SqlParameter("@Operation", "DELETE"),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_MasterCurrencyOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) > 0;
        }

        #endregion

        #region Account Group

        public async Task<PagedResult<AccountGroupDto>> GetAccountGroupsAsync(long? groupId, string? groupName, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new[]
            {
                new SqlParameter("@AccountGroupID", (object?)groupId ?? DBNull.Value),
                new SqlParameter("@AccountGroupName", (object?)groupName ?? DBNull.Value),
                new SqlParameter("@CurrentPage", page),
                new SqlParameter("@RecordPerPage", pageSize),
                new SqlParameter("@SortColumn", sortCol),
                new SqlParameter("@SortOrd", sortOrd),
                new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            var table = await ExecuteStoredProcedureAsync("A_AccountGroupMasterList", parameters);
            var list = new List<AccountGroupDto>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(new AccountGroupDto
                {
                    AccountGroupID = Convert.ToInt64(row["AccountGroupID"]),
                    AccountGroupName = row["AccountGroupName"]?.ToString() ?? "",
                    IsMainAccountGroup = Convert.ToBoolean(row["IsMainAccountGroup"]),
                    MainAccountGroupID = row["MainAccountGroupID"] == DBNull.Value ? null : Convert.ToInt64(row["MainAccountGroupID"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    IsEditable = Convert.ToBoolean(row["IsEditable"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    CreatedBy = row["CreatedBy"]?.ToString()
                });
            }

            return new PagedResult<AccountGroupDto>
            {
                Items = list,
                TotalRecord = Convert.ToInt32(parameters[^1].Value ?? 0),
                CurrentPage = page
            };
        }

        public async Task<IReadOnlyList<AccountGroupLookupDto>> GetMainAccountGroupsLookupAsync()
        {
            var table = await ExecuteStoredProcedureAsync("A_AccountGroupMasterSelectMain", Array.Empty<SqlParameter>());
            var list = new List<AccountGroupLookupDto>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(new AccountGroupLookupDto
                {
                    AccountGroupID = Convert.ToInt64(row["AccountGroupID"]),
                    AccountGroupName = row["AccountGroupName"]?.ToString() ?? ""
                });
            }

            return list;
        }

        public async Task<long> SaveAccountGroupAsync(AccountGroupDto group)
        {
            var parameters = new[]
            {
                new SqlParameter("@AccountGroupID", group.AccountGroupID),
                new SqlParameter("@AccountGroupName", group.AccountGroupName),
                new SqlParameter("@IsMainAccountGroup", group.IsMainAccountGroup),
                new SqlParameter("@MainAccountGroupID", (object?)group.MainAccountGroupID ?? DBNull.Value),
                new SqlParameter("@IsEditable", group.IsEditable),
                new SqlParameter("@CreatedDate", DateTime.UtcNow),
                new SqlParameter("@CreatedBy", (object?)group.CreatedBy ?? DBNull.Value),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_AccountGroupMasterModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<bool> DeleteAccountGroupAsync(string ids)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", ids),
                new SqlParameter("@OprType", 1), // 1 for Delete
                new SqlParameter("@UpdatedBy", "S"),
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_AccountGroupMasterOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) == 1; // 1 = Success
        }

        public async Task<IReadOnlyList<AccountGroupLookupDto>> GetAccountGroupsLookupAsync()
        {
            var table = await ExecuteStoredProcedureAsync("A_AccountGroupMasterSelectAll", Array.Empty<SqlParameter>());
            var list = new List<AccountGroupLookupDto>();

            foreach (DataRow row in table.Rows)
            {
                list.Add(new AccountGroupLookupDto
                {
                    AccountGroupID = Convert.ToInt64(row["AccountGroupID"]),
                    AccountGroupName = row["AccountGroupName"]?.ToString() ?? ""
                });
            }

            return list;
        }

        #endregion

        #region Account Master

        public async Task<PagedResult<AccountMasterDto>> GetAccountsAsync(long? accountId, string? accountName, long? groupId, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new[]
            {
                new SqlParameter("@AccountID", (object?)accountId ?? DBNull.Value),
                new SqlParameter("@AccountName", (object?)accountName ?? DBNull.Value),
                new SqlParameter("@AccountGroupID", (object?)groupId ?? DBNull.Value),
                new SqlParameter("@CurrentPage", page),
                new SqlParameter("@RecordPerPage", pageSize),
                new SqlParameter("@SortColumn", sortCol),
                new SqlParameter("@SortOrd", sortOrd),
                new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            var table = await ExecuteStoredProcedureAsync("A_MasterAccountsList", parameters);
            var list = new List<AccountMasterDto>();

            foreach (DataRow row in table.Rows)
            {
                var cols = row.Table.Columns;
                list.Add(new AccountMasterDto
                {
                    AccountID = cols.Contains("AccountID") && row["AccountID"] != DBNull.Value ? Convert.ToInt64(row["AccountID"]) : 0,
                    AccountName = cols.Contains("AccountName") ? row["AccountName"]?.ToString() ?? "" : "",
                    AccountGroupID = cols.Contains("AccountGroupID") && row["AccountGroupID"] != DBNull.Value ? Convert.ToInt64(row["AccountGroupID"]) : 0,
                    AccountGroupName = cols.Contains("AccountGroupName") ? row["AccountGroupName"]?.ToString() : null,
                    Address = cols.Contains("Address") ? row["Address"]?.ToString() : null,
                    ContactNo = cols.Contains("PhoneNo") && row["PhoneNo"] != DBNull.Value ? row["PhoneNo"]?.ToString() : (cols.Contains("ContactNo") ? row["ContactNo"]?.ToString() : null),
                    MobileNo = cols.Contains("MobileNo") ? row["MobileNo"]?.ToString() : null,
                    EmailID = cols.Contains("EmailID") ? row["EmailID"]?.ToString() : null,
                    OpeningBalance = cols.Contains("OpeningBalance") && row["OpeningBalance"] != DBNull.Value ? Convert.ToDouble(row["OpeningBalance"]) : null,
                    OpeningBalanceDate = cols.Contains("OpeningBalanceDate") && row["OpeningBalanceDate"] != DBNull.Value ? Convert.ToDateTime(row["OpeningBalanceDate"]) : null,
                    StakeholderGroup = cols.Contains("StakeholderGroup") ? row["StakeholderGroup"]?.ToString() : null,
                    StakeholderType = cols.Contains("StakeholderType") ? row["StakeholderType"]?.ToString() : null,
                    CurrencyID = cols.Contains("CurrencyID") && row["CurrencyID"] != DBNull.Value ? Convert.ToInt64(row["CurrencyID"]) : null,
                    CurrencySymbol = cols.Contains("CurrencySymbol") ? row["CurrencySymbol"]?.ToString() : null,
                    CreatedDate = cols.Contains("CreatedDate") && row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue,
                    CreatedBy = cols.Contains("CreatedBy") ? row["CreatedBy"]?.ToString() : null
                });
            }

            return new PagedResult<AccountMasterDto>
            {
                Items = list,
                TotalRecord = Convert.ToInt32(parameters[^1].Value ?? 0),
                CurrentPage = page
            };
        }

        public async Task<long> SaveAccountAsync(AccountMasterDto account)
        {
            var parameters = new[]
            {
                new SqlParameter("@AccountId", account.AccountID),
                new SqlParameter("@AccountName", account.AccountName),
                new SqlParameter("@OpeningBalance", (object?)account.OpeningBalance ?? 0),
                new SqlParameter("@AccountGroupId", account.AccountGroupID),
                new SqlParameter("@IsActive", true),
                new SqlParameter("@IsMultiCurrency", false),
                new SqlParameter("@OpeningBalanceType", DBNull.Value),
                new SqlParameter("@IsEditable", true),
                new SqlParameter("@CreatedDate", DateTime.UtcNow),
                new SqlParameter("@CreatedBy", (object?)account.CreatedBy ?? DBNull.Value),
                new SqlParameter("@ZipCode", DBNull.Value),
                new SqlParameter("@Address", (object?)account.Address ?? DBNull.Value),
                new SqlParameter("@AddressCity", DBNull.Value),
                new SqlParameter("@MasterCountry", DBNull.Value),
                new SqlParameter("@ContactPerson", DBNull.Value),
                new SqlParameter("@PhoneNo", (object?)account.ContactNo ?? DBNull.Value),
                new SqlParameter("@MobileNo", (object?)account.MobileNo ?? DBNull.Value),
                new SqlParameter("@EmailID", (object?)account.EmailID ?? DBNull.Value),
                new SqlParameter("@StakeholderGroup", (object?)account.StakeholderGroup ?? DBNull.Value),
                new SqlParameter("@StakeholderType", (object?)account.StakeholderType ?? DBNull.Value),
                new SqlParameter("@CurrencyID", (object?)account.CurrencyID ?? DBNull.Value),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_MasterAccountsModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<bool> DeleteAccountAsync(string ids)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", ids),
                new SqlParameter("@OprType", 1), // 1 for Delete
                new SqlParameter("@UpdatedBy", "S"),
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_MasterAccountsOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) == 1;
        }

        #endregion

        #region Customer Order

        public async Task<PagedResult<CustomerOrderDto>> GetCustomerOrdersAsync(long? orderId, string? orderNo, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new[]
            {
                new SqlParameter("@OrderID", (object?)orderId ?? DBNull.Value),
                new SqlParameter("@OrderNo", (object?)orderNo ?? DBNull.Value),
                new SqlParameter("@CurrentPage", page),
                new SqlParameter("@RecordPerPage", pageSize),
                new SqlParameter("@SortColumn", sortCol),
                new SqlParameter("@SortOrd", sortOrd),
                new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            var table = await ExecuteStoredProcedureAsync("W_OrderCustomerList", parameters);
            var list = new List<CustomerOrderDto>();

            foreach (DataRow row in table.Rows)
            {
                var cols = row.Table.Columns;
                list.Add(new CustomerOrderDto
                {
                    OrderID = cols.Contains("OrderID") && row["OrderID"] != DBNull.Value ? Convert.ToInt64(row["OrderID"]) : 0,
                    CustomerID = cols.Contains("CustomerID") && row["CustomerID"] != DBNull.Value ? Convert.ToInt64(row["CustomerID"]) : 0,
                    CustomerName = cols.Contains("AccountName") ? row["AccountName"]?.ToString() : null,
                    OrderNo = cols.Contains("OrderNo") ? row["OrderNo"]?.ToString() : null,
                    OrderDate = cols.Contains("OrderDate") && row["OrderDate"] != DBNull.Value ? Convert.ToDateTime(row["OrderDate"]) : null,
                    SentDate = cols.Contains("SentDate") && row["SentDate"] != DBNull.Value ? Convert.ToDateTime(row["SentDate"]) : null,
                    Remarks = cols.Contains("Remarks") ? row["Remarks"]?.ToString() : null,
                    DocumentUpload = cols.Contains("DocumentUpload") ? row["DocumentUpload"]?.ToString() : (cols.Contains("UploadedFile") ? row["UploadedFile"]?.ToString() : null),
                    CreatedDate = cols.Contains("CreatedDate") && row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : null,
                    CreatedBy = cols.Contains("CreatedBy") ? row["CreatedBy"]?.ToString() : null
                });
            }

            return new PagedResult<CustomerOrderDto>
            {
                Items = list,
                TotalRecord = Convert.ToInt32(parameters[^1].Value ?? 0),
                CurrentPage = page
            };
        }

        public async Task<CustomerOrderDto?> GetCustomerOrderByIdAsync(long orderId)
        {
            var headerParams = new[]
            {
                new SqlParameter("@OrderID", orderId)
            };

            var headerTable = await ExecuteStoredProcedureAsync("W_OrderCustomerList", headerParams);
            if (headerTable.Rows.Count == 0) return null;

            DataRow row = headerTable.Rows[0];
            var cols = row.Table.Columns;

            var order = new CustomerOrderDto
            {
                OrderID = cols.Contains("OrderID") && row["OrderID"] != DBNull.Value ? Convert.ToInt64(row["OrderID"]) : 0,
                CustomerID = cols.Contains("CustomerID") && row["CustomerID"] != DBNull.Value ? Convert.ToInt64(row["CustomerID"]) : 0,
                CustomerName = cols.Contains("AccountName") ? row["AccountName"]?.ToString() : null,
                OrderNo = cols.Contains("OrderNo") ? row["OrderNo"]?.ToString() : null,
                OrderDate = cols.Contains("OrderDate") && row["OrderDate"] != DBNull.Value ? Convert.ToDateTime(row["OrderDate"]) : null,
                SentDate = cols.Contains("SentDate") && row["SentDate"] != DBNull.Value ? Convert.ToDateTime(row["SentDate"]) : null,
                Remarks = cols.Contains("Remarks") ? row["Remarks"]?.ToString() : null,
                DocumentUpload = cols.Contains("DocumentUpload") ? row["DocumentUpload"]?.ToString() : (cols.Contains("UploadedFile") ? row["UploadedFile"]?.ToString() : null),
                CreatedDate = cols.Contains("CreatedDate") && row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : null,
                CreatedBy = cols.Contains("CreatedBy") ? row["CreatedBy"]?.ToString() : null
            };

            var itemParams = new[]
            {
                new SqlParameter("@OrderID", orderId)
            };

            var itemTable = await ExecuteStoredProcedureAsync("W_OrderCustomerSelectItemByID", itemParams);
            foreach (DataRow itemRow in itemTable.Rows)
            {
                var itemCols = itemRow.Table.Columns;
                order.Items.Add(new CustomerOrderItemDto
                {
                    OrderItemId = itemCols.Contains("CustomerOrderDetailsID") && itemRow["CustomerOrderDetailsID"] != DBNull.Value ? Convert.ToInt64(itemRow["CustomerOrderDetailsID"]) : 0,
                    OrderId = itemCols.Contains("OrderID") && itemRow["OrderID"] != DBNull.Value ? Convert.ToInt64(itemRow["OrderID"]) : 0,
                    ItemId = itemCols.Contains("ItemID") && itemRow["ItemID"] != DBNull.Value ? Convert.ToInt64(itemRow["ItemID"]) : 0,
                    ItemName = itemCols.Contains("ItemName") ? itemRow["ItemName"]?.ToString() : null,
                    ItemCode = itemCols.Contains("ItemCode") ? itemRow["ItemCode"]?.ToString() : null,
                    Qty = itemCols.Contains("Qty") && itemRow["Qty"] != DBNull.Value ? Convert.ToInt32(itemRow["Qty"]) : 0,
                    Remarks = itemCols.Contains("Remarks") ? itemRow["Remarks"]?.ToString() : null,
                    IsComplete = itemCols.Contains("IsComplete") && itemRow["IsComplete"] != DBNull.Value ? Convert.ToBoolean(itemRow["IsComplete"]) : null
                });
            }

            return order;
        }

        public async Task<long> SaveCustomerOrderAsync(CustomerOrderDto order)
        {
            var parameters = new[]
            {
                new SqlParameter("@OrderID", order.OrderID),
                new SqlParameter("@OrderNo", order.OrderNo ?? ""),
                new SqlParameter("@CreatedDate", DateTime.UtcNow),
                new SqlParameter("@CreatedBy", order.CreatedBy ?? "System"),
                new SqlParameter("@OrderDate", order.OrderDate ?? DateTime.UtcNow),
                new SqlParameter("@SentDate", (object?)order.SentDate ?? DBNull.Value),
                new SqlParameter("@Remarks", order.Remarks ?? ""),
                new SqlParameter("@CustomerID", order.CustomerID),
                new SqlParameter("@UploadedFile", order.DocumentUpload ?? ""),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_OrderCustomerModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<long> SaveCustomerOrderItemAsync(CustomerOrderItemDto item)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", item.OrderItemId),
                new SqlParameter("@OrderID", item.OrderId),
                new SqlParameter("@ItemID", item.ItemId),
                new SqlParameter("@Qty", item.Qty),
                new SqlParameter("@Remarks", item.Remarks ?? ""),
                new SqlParameter("@IsComplete", (object?)item.IsComplete ?? DBNull.Value),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@DeletedBatchIds", (object?)item.DeletedOrderItemIds ?? DBNull.Value)
            };

            await ExecuteNonQueryAsync("W_OrderCustomerDetailsModify", parameters);
            return Convert.ToInt64(parameters[^2].Value ?? 0);
        }

        public async Task<bool> DeleteCustomerOrderAsync(string ids)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", ids),
                new SqlParameter("@OprType", 1), // 1 for Delete
                new SqlParameter("@UpdatedBy", "System"),
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_OrderCustomerOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) >= 0;
        }

        public async Task<bool> DeleteCustomerOrderItemAsync(string ids)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", ids),
                new SqlParameter("@OprType", 1), // 1 for Delete
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_MasterCustomerOrderItemsOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) >= 0;
        }

        public async Task<IReadOnlyList<CustomerOrderDto>> GetCustomerOrdersByCustomerIdAsync(long customerId)
        {
            var parameters = new[]
            {
                new SqlParameter("@CustomerID", customerId)
            };

            var table = await ExecuteStoredProcedureAsync("W_OrderCustomerSelectByID", parameters);
            var list = new List<CustomerOrderDto>();

            foreach (DataRow row in table.Rows)
            {
                var cols = row.Table.Columns;
                list.Add(new CustomerOrderDto
                {
                    OrderID = cols.Contains("OrderID") && row["OrderID"] != DBNull.Value ? Convert.ToInt64(row["OrderID"]) : 0,
                    CustomerID = cols.Contains("CustomerID") && row["CustomerID"] != DBNull.Value ? Convert.ToInt64(row["CustomerID"]) : 0,
                    OrderNo = cols.Contains("OrderNo") ? row["OrderNo"]?.ToString() : null,
                    OrderDate = cols.Contains("OrderDate") && row["OrderDate"] != DBNull.Value ? Convert.ToDateTime(row["OrderDate"]) : null,
                    SentDate = cols.Contains("SentDate") && row["SentDate"] != DBNull.Value ? Convert.ToDateTime(row["SentDate"]) : null,
                    Remarks = cols.Contains("Remarks") ? row["Remarks"]?.ToString() : null
                });
            }

            return list;
        }

        #endregion

        #region Invoice

        public async Task<PagedResult<InvoiceDto>> GetInvoicesAsync(long? invoiceId, string? invoiceNumber, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new[]
            {
                new SqlParameter("@InvoiceID", (object?)invoiceId ?? DBNull.Value),
                new SqlParameter("@InvoiceNumber", (object?)invoiceNumber ?? DBNull.Value),
                new SqlParameter("@CurrentPage", page),
                new SqlParameter("@RecordPerPage", pageSize),
                new SqlParameter("@SortColumn", sortCol),
                new SqlParameter("@SortOrd", sortOrd),
                new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            var table = await ExecuteStoredProcedureAsync("A_InvoicesList", parameters);
            var list = new List<InvoiceDto>();

            foreach (DataRow row in table.Rows)
            {
                var cols = row.Table.Columns;
                list.Add(new InvoiceDto
                {
                    InvoiceID = cols.Contains("InvoiceID") && row["InvoiceID"] != DBNull.Value ? Convert.ToInt64(row["InvoiceID"]) : 0,
                    AccountID = cols.Contains("AccountID") && row["AccountID"] != DBNull.Value ? Convert.ToInt64(row["AccountID"]) : 0,
                    InvoiceNumber = cols.Contains("InvoiceNumber") ? row["InvoiceNumber"]?.ToString() : null,
                    DueDate = cols.Contains("DueDate") && row["DueDate"] != DBNull.Value ? Convert.ToDateTime(row["DueDate"]) : null,
                    InvoiceDate = cols.Contains("InvoiceDate") && row["InvoiceDate"] != DBNull.Value ? Convert.ToDateTime(row["InvoiceDate"]) : null,
                    InvoiceStatus = cols.Contains("InvoiceStatus") ? row["InvoiceStatus"]?.ToString() : null,
                    IsPaid = cols.Contains("IsPaid") && row["IsPaid"] != DBNull.Value ? Convert.ToBoolean(row["IsPaid"]) : null,
                    AccountName = cols.Contains("AccountName") ? row["AccountName"]?.ToString() : null,
                    CurrencySymbol = cols.Contains("CurrencySymbol") ? row["CurrencySymbol"]?.ToString() : null,
                    TotalAmount = cols.Contains("TotalAmount") && row["TotalAmount"] != DBNull.Value ? Convert.ToDouble(row["TotalAmount"]) : 0
                });
            }

            return new PagedResult<InvoiceDto>
            {
                Items = list,
                TotalRecord = Convert.ToInt32(parameters[^1].Value ?? 0),
                CurrentPage = page
            };
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(long invoiceId)
        {
            var headerParams = new[]
            {
                new SqlParameter("@InvoiceID", invoiceId)
            };

            var headerTable = await ExecuteStoredProcedureAsync("A_InvoicesList", headerParams);
            if (headerTable.Rows.Count == 0) return null;

            DataRow row = headerTable.Rows[0];
            var cols = row.Table.Columns;

            var invoice = new InvoiceDto
            {
                InvoiceID = cols.Contains("InvoiceID") && row["InvoiceID"] != DBNull.Value ? Convert.ToInt64(row["InvoiceID"]) : 0,
                AccountID = cols.Contains("AccountID") && row["AccountID"] != DBNull.Value ? Convert.ToInt64(row["AccountID"]) : 0,
                InvoiceNumber = cols.Contains("InvoiceNumber") ? row["InvoiceNumber"]?.ToString() : null,
                DueDate = cols.Contains("DueDate") && row["DueDate"] != DBNull.Value ? Convert.ToDateTime(row["DueDate"]) : null,
                InvoiceDate = cols.Contains("InvoiceDate") && row["InvoiceDate"] != DBNull.Value ? Convert.ToDateTime(row["InvoiceDate"]) : null,
                InvoiceStatus = cols.Contains("InvoiceStatus") ? row["InvoiceStatus"]?.ToString() : null,
                IsPaid = cols.Contains("IsPaid") && row["IsPaid"] != DBNull.Value ? Convert.ToBoolean(row["IsPaid"]) : null,
                AccountName = cols.Contains("AccountName") ? row["AccountName"]?.ToString() : null,
                CurrencySymbol = cols.Contains("CurrencySymbol") ? row["CurrencySymbol"]?.ToString() : null,
                Remarks = cols.Contains("Remarks") ? row["Remarks"]?.ToString() : null,
                CurrencyID = cols.Contains("CurrencyID") && row["CurrencyID"] != DBNull.Value ? Convert.ToInt64(row["CurrencyID"]) : null,
                CurrencyConversion = cols.Contains("CurrencyConversion") && row["CurrencyConversion"] != DBNull.Value ? Convert.ToDouble(row["CurrencyConversion"]) : null,
                CreatedDate = cols.Contains("CreatedDate") && row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : null,
                CreatedBy = cols.Contains("CreatedBy") ? row["CreatedBy"]?.ToString() : null
            };

            var itemParams = new[]
            {
                new SqlParameter("@InvoiceID", invoiceId)
            };

            var itemTable = await ExecuteStoredProcedureAsync("A_InvoicesItemByInvoicesID", itemParams);
            foreach (DataRow itemRow in itemTable.Rows)
            {
                var itemCols = itemRow.Table.Columns;
                invoice.Items.Add(new InvoiceItemDto
                {
                    InvoiceChildID = itemCols.Contains("InvoiceChildID") && itemRow["InvoiceChildID"] != DBNull.Value ? Convert.ToInt64(itemRow["InvoiceChildID"]) : 0,
                    InvoiceID = itemCols.Contains("InvoiceID") && itemRow["InvoiceID"] != DBNull.Value ? Convert.ToInt64(itemRow["InvoiceID"]) : 0,
                    OrderID = itemCols.Contains("OrderID") && itemRow["OrderID"] != DBNull.Value ? Convert.ToInt64(itemRow["OrderID"]) : 0,
                    ItemId = itemCols.Contains("ItemId") && itemRow["ItemId"] != DBNull.Value ? Convert.ToInt64(itemRow["ItemId"]) : 0,
                    ItemName = itemCols.Contains("ItemName") ? itemRow["ItemName"]?.ToString() : null,
                    OrderNo = itemCols.Contains("OrderNo") ? itemRow["OrderNo"]?.ToString() : null,
                    PrintHeading = itemCols.Contains("PrintHeading") ? itemRow["PrintHeading"]?.ToString() : null,
                    Amount = itemCols.Contains("Amount") && itemRow["Amount"] != DBNull.Value ? Convert.ToDouble(itemRow["Amount"]) : 0,
                    Quantity = itemCols.Contains("Quantity") && itemRow["Quantity"] != DBNull.Value ? Convert.ToInt32(itemRow["Quantity"]) : 0,
                    UnitPrice = itemCols.Contains("UnitPrice") && itemRow["UnitPrice"] != DBNull.Value ? Convert.ToDouble(itemRow["UnitPrice"]) : 0,
                    Description = itemCols.Contains("Description") ? itemRow["Description"]?.ToString() : null,
                    BatchNumber = itemCols.Contains("BatchNumber") ? itemRow["BatchNumber"]?.ToString() : null
                });
            }

            return invoice;
        }

        public async Task<long> SaveInvoiceAsync(InvoiceDto invoice)
        {
            var parameters = new[]
            {
                new SqlParameter("@InvoiceID", invoice.InvoiceID),
                new SqlParameter("@AccountID", invoice.AccountID),
                new SqlParameter("@InvoiceNumber", invoice.InvoiceNumber ?? ""),
                new SqlParameter("@DueDate", (object?)invoice.DueDate ?? DBNull.Value),
                new SqlParameter("@InvoiceDate", (object?)invoice.InvoiceDate ?? DBNull.Value),
                new SqlParameter("@InvoiceStatus", invoice.InvoiceStatus ?? ""),
                new SqlParameter("@CreatedDate", DateTime.UtcNow),
                new SqlParameter("@CreatedBy", invoice.CreatedBy ?? "System"),
                new SqlParameter("@CurrencyID", (object?)invoice.CurrencyID ?? DBNull.Value),
                new SqlParameter("@CurrencyConversion", (object?)invoice.CurrencyConversion ?? 1.0),
                new SqlParameter("@Remarks", invoice.Remarks ?? ""),
                new SqlParameter("@IsPaid", invoice.IsPaid ?? false),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_InvoicesModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<long> SaveInvoiceItemAsync(InvoiceItemDto item)
        {
            var parameters = new[]
            {
                new SqlParameter("@InvoiceChildID", item.InvoiceChildID),
                new SqlParameter("@InvoiceID", item.InvoiceID),
                new SqlParameter("@OrderID", item.OrderID),
                new SqlParameter("@ItemId", item.ItemId),
                new SqlParameter("@Quantity", item.Quantity),
                new SqlParameter("@Description", item.Description ?? ""),
                new SqlParameter("@Amount", item.Amount),
                new SqlParameter("@UnitPrice", item.UnitPrice),
                new SqlParameter("@BatchNumber", item.BatchNumber ?? ""),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_InvoicesChildModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<bool> DeleteInvoiceAsync(string ids)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", ids),
                new SqlParameter("@OprType", 1), // 1 for Delete
                new SqlParameter("@UpdatedBy", "System"),
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_InvoiceOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) >= 0;
        }

        public async Task<bool> DeleteInvoiceItemAsync(string ids)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", ids),
                new SqlParameter("@OprType", 1), // 1 for Delete
                new SqlParameter("@UpdatedBy", "System"),
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("A_InvoiceChildOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) >= 0;
        }

        #endregion

        #region Customer Order Return

        public async Task<PagedResult<CustomerOrderReturnDto>> GetCustomerOrderReturnsAsync(long? orderReturnId, long? orderId, int page, int pageSize, string sortCol, string sortOrd)
        {
            var parameters = new[]
            {
                new SqlParameter("@OrderReturnID", (object?)orderReturnId ?? DBNull.Value),
                new SqlParameter("@OrderID", (object?)orderId ?? DBNull.Value),
                new SqlParameter("@CurrentPage", page),
                new SqlParameter("@RecordPerPage", pageSize),
                new SqlParameter("@SortColumn", sortCol),
                new SqlParameter("@SortOrd", sortOrd),
                new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            var table = await ExecuteStoredProcedureAsync("W_OrderCustomerReturnList", parameters);
            var list = new List<CustomerOrderReturnDto>();

            foreach (DataRow row in table.Rows)
            {
                var cols = row.Table.Columns;
                list.Add(new CustomerOrderReturnDto
                {
                    OrderReturnID = cols.Contains("OrderReturnID") && row["OrderReturnID"] != DBNull.Value ? Convert.ToInt64(row["OrderReturnID"]) : 0,
                    OrderID = cols.Contains("OrderID") && row["OrderID"] != DBNull.Value ? Convert.ToInt64(row["OrderID"]) : 0,
                    OrderDetailsID = cols.Contains("OrderDetailsID") && row["OrderDetailsID"] != DBNull.Value ? Convert.ToInt64(row["OrderDetailsID"]) : 0,
                    ItemStockByBatchId = cols.Contains("ItemStockByBatchId") && row["ItemStockByBatchId"] != DBNull.Value ? Convert.ToInt64(row["ItemStockByBatchId"]) : 0,
                    ItemStockUsedID = cols.Contains("ItemStockUsedID") && row["ItemStockUsedID"] != DBNull.Value ? Convert.ToInt64(row["ItemStockUsedID"]) : 0,
                    ReturnDate = cols.Contains("ReturnDate") && row["ReturnDate"] != DBNull.Value ? Convert.ToDateTime(row["ReturnDate"]) : null,
                    Quantity = cols.Contains("Quantity") && row["Quantity"] != DBNull.Value ? Convert.ToDouble(row["Quantity"]) : 0,
                    TotalReturnedQty = cols.Contains("TotalReturnedQty") && row["TotalReturnedQty"] != DBNull.Value ? Convert.ToDouble(row["TotalReturnedQty"]) : 0,
                    CreatedDate = cols.Contains("CreatedDate") && row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : null,
                    CreatedBy = cols.Contains("CreatedBy") ? row["CreatedBy"]?.ToString() : null,
                    ReturnReason = cols.Contains("ReturnReason") ? row["ReturnReason"]?.ToString() : null,
                    AccountName = cols.Contains("AccountName") ? row["AccountName"]?.ToString() : null,
                    BatchNo = cols.Contains("BatchNo") ? row["BatchNo"]?.ToString() : null,
                    ItemName = cols.Contains("ItemName") ? row["ItemName"]?.ToString() : null,
                    OrderedQty = cols.Contains("OrderedQty") && row["OrderedQty"] != DBNull.Value ? Convert.ToDouble(row["OrderedQty"]) : 0
                });
            }

            return new PagedResult<CustomerOrderReturnDto>
            {
                Items = list,
                TotalRecord = Convert.ToInt32(parameters[^1].Value ?? 0),
                CurrentPage = page
            };
        }

        public async Task<long> SaveCustomerOrderReturnAsync(CustomerOrderReturnDto returnDto)
        {
            var parameters = new[]
            {
                new SqlParameter("@OrderReturnID", returnDto.OrderReturnID),
                new SqlParameter("@OrderID", returnDto.OrderID),
                new SqlParameter("@OrderDetailsID", returnDto.OrderDetailsID),
                new SqlParameter("@ItemStockByBatchId", returnDto.ItemStockByBatchId),
                new SqlParameter("@ReturnDate", returnDto.ReturnDate ?? DateTime.UtcNow),
                new SqlParameter("@Quantity", returnDto.Quantity),
                new SqlParameter("@CreatedDate", DateTime.UtcNow),
                new SqlParameter("@CreatedBy", returnDto.CreatedBy ?? "System"),
                new SqlParameter("@ReturnReason", returnDto.ReturnReason ?? ""),
                new SqlParameter("@ItemStockUsedID", returnDto.ItemStockUsedID),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_OrderCustomerReturnModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        public async Task<bool> DeleteCustomerOrderReturnAsync(string ids)
        {
            var parameters = new[]
            {
                new SqlParameter("@ID", ids),
                new SqlParameter("@OprType", 1), // 1 for Delete
                new SqlParameter("@Iserror", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("W_CustomerOrderReturnOperation", parameters);
            return Convert.ToInt32(parameters[^1].Value ?? 0) >= 0;
        }

        public async Task<IReadOnlyList<ItemStockByBatchForBOMDto>> GetItemStockByItemIDBatchForBOMListAsync(long itemId)
        {
            var parameters = new[]
            {
                new SqlParameter("@ItemID", itemId)
            };

            var dt = await ExecuteStoredProcedureAsync("Inv_ItemStockByBatchForBOMList", parameters);
            var list = new List<ItemStockByBatchForBOMDto>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ItemStockByBatchForBOMDto
                {
                    ItemStockByBatchID = Convert.ToInt64(row["ItemStockByBatchID"]),
                    BatchNo = row["BatchNo"]?.ToString(),
                    FinalQuantityLeft = Convert.ToDouble(row["FinalQuantityLeft"] != DBNull.Value ? row["FinalQuantityLeft"] : 0.0),
                    ExpiryDateBOM = row["ExpiryDateBOM"]?.ToString()
                });
            }
            return list;
        }

        public async Task<IReadOnlyList<ItemStockUsedForBOMDto>> GetItemStockUsedForBOMByOrderDetailIdAsync(long orderDetailsId)
        {
            var parameters = new[]
            {
                new SqlParameter("@OrderDetailsId", orderDetailsId)
            };

            var dt = await ExecuteStoredProcedureAsync("Inv_ItemStockUsedForBOMByOrderDetailId", parameters);
            var list = new List<ItemStockUsedForBOMDto>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ItemStockUsedForBOMDto
                {
                    ItemStockUsedID = Convert.ToInt64(row["ItemStockUsedID"]),
                    ItemStockByBatchID = Convert.ToInt64(row["ItemStockByBatchID"]),
                    BatchNo = row.Table.Columns.Contains("BatchNo") ? row["BatchNo"]?.ToString() : null,
                    Quantity = Convert.ToDouble(row["Quantity"] != DBNull.Value ? row["Quantity"] : 0.0),
                    ReturnQty = Convert.ToDouble(row["ReturnQty"] != DBNull.Value ? row["ReturnQty"] : 0.0),
                    Description = row.Table.Columns.Contains("Description") ? row["Description"]?.ToString() : null
                });
            }
            return list;
        }

        public async Task<long> SaveItemStockUsedForBOMAsync(ItemStockUsedForBOMDto dto)
        {
            var parameters = new[]
            {
                new SqlParameter("@ItemStockUsedID", dto.ItemStockUsedID),
                new SqlParameter("@ItemStockByBatchId", dto.ItemStockByBatchID),
                new SqlParameter("@OrderCustomerID", dto.OrderCustomerID),
                new SqlParameter("@OrderCustomerDetailsID", dto.OrderCustomerDetailsID),
                new SqlParameter("@Quantity", dto.Quantity),
                new SqlParameter("@UsedFor", dto.UsedFor),
                new SqlParameter("@Description", (object?)dto.Description ?? DBNull.Value),
                new SqlParameter("@CreatedBy", (object?)dto.CreatedBy ?? DBNull.Value),
                new SqlParameter("@CreatedDate", DateTime.UtcNow),
                new SqlParameter("@ReturnVal", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await ExecuteNonQueryAsync("Inv_ItemStockUsedForBOMModify", parameters);
            return Convert.ToInt64(parameters[^1].Value ?? 0);
        }

        #endregion

        #region Helpers

        private async Task<DataTable> ExecuteStoredProcedureAsync(string storedProcedure, IEnumerable<SqlParameter> parameters)
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

            var table = new DataTable();
            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();
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
