using GFI_Upgrated.Data.Account;
using GFI_Upgrated.SharedDto.Account;
using GFI_Upgrated.SharedDto.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Account
{
    public sealed class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        #region Currency
        public async Task<PagedResult<CurrencyDto>> GetCurrenciesAsync(long? currencyId, string? symbol, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetCurrenciesAsync(currencyId, symbol, page, pageSize, sortCol, sortOrd);

        public async Task<long> SaveCurrencyAsync(CurrencyDto currency)
            => await _repository.SaveCurrencyAsync(currency);

        public async Task<bool> DeleteCurrencyAsync(string ids)
            => await _repository.DeleteCurrencyAsync(ids);
        #endregion

        #region Account Group
        public async Task<PagedResult<AccountGroupDto>> GetAccountGroupsAsync(long? groupId, string? groupName, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetAccountGroupsAsync(groupId, groupName, page, pageSize, sortCol, sortOrd);

        public async Task<IReadOnlyList<AccountGroupLookupDto>> GetMainAccountGroupsLookupAsync()
            => await _repository.GetMainAccountGroupsLookupAsync();

        public async Task<long> SaveAccountGroupAsync(AccountGroupDto group)
            => await _repository.SaveAccountGroupAsync(group);

        public async Task<bool> DeleteAccountGroupAsync(string ids)
            => await _repository.DeleteAccountGroupAsync(ids);

        public async Task<IReadOnlyList<AccountGroupLookupDto>> GetAccountGroupsLookupAsync()
            => await _repository.GetAccountGroupsLookupAsync();
        #endregion

        #region Account Master
        public async Task<PagedResult<AccountMasterDto>> GetAccountsAsync(long? accountId, string? accountName, long? groupId, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetAccountsAsync(accountId, accountName, groupId, page, pageSize, sortCol, sortOrd);

        public async Task<long> SaveAccountAsync(AccountMasterDto account)
            => await _repository.SaveAccountAsync(account);

        public async Task<bool> DeleteAccountAsync(string ids)
            => await _repository.DeleteAccountAsync(ids);
        #endregion

        #region Customer Order
        public async Task<PagedResult<CustomerOrderDto>> GetCustomerOrdersAsync(long? orderId, string? orderNo, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetCustomerOrdersAsync(orderId, orderNo, page, pageSize, sortCol, sortOrd);

        public async Task<CustomerOrderDto?> GetCustomerOrderByIdAsync(long orderId)
            => await _repository.GetCustomerOrderByIdAsync(orderId);

        public async Task<long> SaveCustomerOrderAsync(CustomerOrderDto order)
            => await _repository.SaveCustomerOrderAsync(order);

        public async Task<long> SaveCustomerOrderItemAsync(CustomerOrderItemDto item)
            => await _repository.SaveCustomerOrderItemAsync(item);

        public async Task<bool> DeleteCustomerOrderAsync(string ids)
            => await _repository.DeleteCustomerOrderAsync(ids);

        public async Task<bool> DeleteCustomerOrderItemAsync(string ids)
            => await _repository.DeleteCustomerOrderItemAsync(ids);

        public async Task<IReadOnlyList<CustomerOrderDto>> GetCustomerOrdersByCustomerIdAsync(long customerId)
            => await _repository.GetCustomerOrdersByCustomerIdAsync(customerId);
        #endregion

        #region Invoice
        public async Task<PagedResult<InvoiceDto>> GetInvoicesAsync(long? invoiceId, string? invoiceNumber, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetInvoicesAsync(invoiceId, invoiceNumber, page, pageSize, sortCol, sortOrd);

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(long invoiceId)
            => await _repository.GetInvoiceByIdAsync(invoiceId);

        public async Task<long> SaveInvoiceAsync(InvoiceDto invoice)
            => await _repository.SaveInvoiceAsync(invoice);

        public async Task<long> SaveInvoiceItemAsync(InvoiceItemDto item)
            => await _repository.SaveInvoiceItemAsync(item);

        public async Task<bool> DeleteInvoiceAsync(string ids)
            => await _repository.DeleteInvoiceAsync(ids);

        public async Task<bool> DeleteInvoiceItemAsync(string ids)
            => await _repository.DeleteInvoiceItemAsync(ids);
        #endregion

        #region Customer Order Return
        public async Task<PagedResult<CustomerOrderReturnDto>> GetCustomerOrderReturnsAsync(long? orderReturnId, long? orderId, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetCustomerOrderReturnsAsync(orderReturnId, orderId, page, pageSize, sortCol, sortOrd);

        public async Task<long> SaveCustomerOrderReturnAsync(CustomerOrderReturnDto returnDto)
            => await _repository.SaveCustomerOrderReturnAsync(returnDto);

        public async Task<bool> DeleteCustomerOrderReturnAsync(string ids)
            => await _repository.DeleteCustomerOrderReturnAsync(ids);

        public async Task<IReadOnlyList<ItemStockByBatchForBOMDto>> GetItemStockByItemIDBatchForBOMListAsync(long itemId)
            => await _repository.GetItemStockByItemIDBatchForBOMListAsync(itemId);

        public async Task<IReadOnlyList<ItemStockUsedForBOMDto>> GetItemStockUsedForBOMByOrderDetailIdAsync(long orderDetailsId)
            => await _repository.GetItemStockUsedForBOMByOrderDetailIdAsync(orderDetailsId);

        public async Task<long> SaveItemStockUsedForBOMAsync(ItemStockUsedForBOMDto dto)
            => await _repository.SaveItemStockUsedForBOMAsync(dto);
        #endregion
    }
}
