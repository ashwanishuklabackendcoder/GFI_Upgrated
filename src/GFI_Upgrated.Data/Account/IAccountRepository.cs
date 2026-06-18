using GFI_Upgrated.SharedDto.Account;
using GFI_Upgrated.SharedDto.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GFI_Upgrated.Data.Account
{
    public interface IAccountRepository
    {
        #region Currency
        Task<PagedResult<CurrencyDto>> GetCurrenciesAsync(long? currencyId, string? symbol, int page, int pageSize, string sortCol, string sortOrd);
        Task<long> SaveCurrencyAsync(CurrencyDto currency);
        Task<bool> DeleteCurrencyAsync(string ids);
        #endregion

        #region Account Group
        Task<PagedResult<AccountGroupDto>> GetAccountGroupsAsync(long? groupId, string? groupName, int page, int pageSize, string sortCol, string sortOrd);
        Task<IReadOnlyList<AccountGroupLookupDto>> GetMainAccountGroupsLookupAsync();
        Task<long> SaveAccountGroupAsync(AccountGroupDto group);
        Task<bool> DeleteAccountGroupAsync(string ids);
        Task<IReadOnlyList<AccountGroupLookupDto>> GetAccountGroupsLookupAsync();
        #endregion

        #region Account Master
        Task<PagedResult<AccountMasterDto>> GetAccountsAsync(long? accountId, string? accountName, long? groupId, int page, int pageSize, string sortCol, string sortOrd);
        Task<long> SaveAccountAsync(AccountMasterDto account);
        Task<bool> DeleteAccountAsync(string ids);
        #endregion

        #region Customer Order
        Task<PagedResult<CustomerOrderDto>> GetCustomerOrdersAsync(long? orderId, string? orderNo, int page, int pageSize, string sortCol, string sortOrd);
        Task<CustomerOrderDto?> GetCustomerOrderByIdAsync(long orderId);
        Task<long> SaveCustomerOrderAsync(CustomerOrderDto order);
        Task<long> SaveCustomerOrderItemAsync(CustomerOrderItemDto item);
        Task<bool> DeleteCustomerOrderAsync(string ids);
        Task<bool> DeleteCustomerOrderItemAsync(string ids);
        Task<IReadOnlyList<CustomerOrderDto>> GetCustomerOrdersByCustomerIdAsync(long customerId);
        #endregion

        #region Invoice
        Task<PagedResult<InvoiceDto>> GetInvoicesAsync(long? invoiceId, string? invoiceNumber, int page, int pageSize, string sortCol, string sortOrd);
        Task<InvoiceDto?> GetInvoiceByIdAsync(long invoiceId);
        Task<long> SaveInvoiceAsync(InvoiceDto invoice);
        Task<long> SaveInvoiceItemAsync(InvoiceItemDto item);
        Task<bool> DeleteInvoiceAsync(string ids);
        Task<bool> DeleteInvoiceItemAsync(string ids);
        #endregion

        #region Customer Order Return
        Task<PagedResult<CustomerOrderReturnDto>> GetCustomerOrderReturnsAsync(long? orderReturnId, long? orderId, int page, int pageSize, string sortCol, string sortOrd);
        Task<long> SaveCustomerOrderReturnAsync(CustomerOrderReturnDto returnDto);
        Task<bool> DeleteCustomerOrderReturnAsync(string ids);
        Task<IReadOnlyList<ItemStockByBatchForBOMDto>> GetItemStockByItemIDBatchForBOMListAsync(long itemId);
        Task<IReadOnlyList<ItemStockUsedForBOMDto>> GetItemStockUsedForBOMByOrderDetailIdAsync(long orderDetailsId);
        Task<long> SaveItemStockUsedForBOMAsync(ItemStockUsedForBOMDto dto);
        #endregion
    }
}
