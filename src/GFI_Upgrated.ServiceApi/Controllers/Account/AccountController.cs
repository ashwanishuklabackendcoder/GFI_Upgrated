using GFI_Upgrated.ServiceApi.Infrastructure;
using GFI_Upgrated.ServiceApi.Services.Account;
using GFI_Upgrated.SharedDto.Account;
using GFI_Upgrated.SharedDto.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Controllers.Account
{
    [Authorize]
    [ApiController]
    [Route("api/account")]
    public sealed class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        #region Currency

        [HttpGet("currencies")]
        [RequirePermission("Accounts", "_Currency", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<CurrencyDto>>>> GetCurrencies(
            [FromQuery] long? currencyId, [FromQuery] string? symbol,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "CurrencyID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetCurrenciesAsync(currencyId, symbol, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<CurrencyDto>> { Success = true, Data = result });
        }

        [HttpPost("currencies")]
        [RequirePermission("Accounts", "_Currency", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveCurrency([FromBody] CurrencyDto currency)
        {
            var id = await _service.SaveCurrencyAsync(currency);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Currency saved." : "Failed to save." });
        }

        [HttpDelete("currencies")]
        [RequirePermission("Accounts", "_Currency", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteCurrencies([FromQuery] string ids)
        {
            try
            {
                var success = await _service.DeleteCurrencyAsync(ids);
                return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Ok(new ApiEnvelope<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "This currency cannot be deleted because it is currently linked to accounts, transactions, or other records."
                });
            }
        }

        #endregion

        #region Account Group

        [HttpGet("groups")]
        [RequirePermission("Accounts", "_AccountGroup", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<AccountGroupDto>>>> GetAccountGroups(
            [FromQuery] long? groupId, [FromQuery] string? groupName,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "AccountGroupID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetAccountGroupsAsync(groupId, groupName, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<AccountGroupDto>> { Success = true, Data = result });
        }

        [HttpGet("groups/main-lookup")]
        [RequirePermission("Accounts", "_AccountGroup", "view")]
        public async Task<ActionResult<ApiEnvelope<IReadOnlyList<AccountGroupLookupDto>>>> GetMainAccountGroupsLookup()
        {
            var result = await _service.GetMainAccountGroupsLookupAsync();
            return Ok(new ApiEnvelope<IReadOnlyList<AccountGroupLookupDto>> { Success = true, Data = result });
        }

        [HttpGet("groups/lookup")]
        [RequirePermission("Accounts", "_AccountGroup", "view")]
        public async Task<ActionResult<ApiEnvelope<IReadOnlyList<AccountGroupLookupDto>>>> GetAccountGroupsLookup()
        {
            var result = await _service.GetAccountGroupsLookupAsync();
            return Ok(new ApiEnvelope<IReadOnlyList<AccountGroupLookupDto>> { Success = true, Data = result });
        }

        [HttpPost("groups")]
        [RequirePermission("Accounts", "_AccountGroup", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveAccountGroup([FromBody] AccountGroupDto group)
        {
            var id = await _service.SaveAccountGroupAsync(group);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Account Group saved." : "Failed to save." });
        }

        [HttpDelete("groups")]
        [RequirePermission("Accounts", "_AccountGroup", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteAccountGroups([FromQuery] string ids)
        {
            try
            {
                var success = await _service.DeleteAccountGroupAsync(ids);
                return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Ok(new ApiEnvelope<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "This account group cannot be deleted because it is currently linked to accounts, subgroups, or other records."
                });
            }
        }

        #endregion

        #region Account Master

        [HttpGet("accounts")]
        [RequirePermission("Accounts", "_AccountMaster", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<AccountMasterDto>>>> GetAccounts(
            [FromQuery] long? accountId, [FromQuery] string? accountName, [FromQuery] long? groupId,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "AccountID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetAccountsAsync(accountId, accountName, groupId, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<AccountMasterDto>> { Success = true, Data = result });
        }

        [HttpPost("accounts")]
        [RequirePermission("Accounts", "_AccountMaster", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveAccount([FromBody] AccountMasterDto account)
        {
            var id = await _service.SaveAccountAsync(account);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Account saved." : "Failed to save." });
        }

        [HttpDelete("accounts")]
        [RequirePermission("Accounts", "_AccountMaster", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteAccounts([FromQuery] string ids)
        {
            try
            {
                var success = await _service.DeleteAccountAsync(ids);
                return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Ok(new ApiEnvelope<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "This account cannot be deleted because it is currently linked to orders, invoices, or other records."
                });
            }
        }

        #endregion

        #region Customer Order

        [HttpGet("orders")]
        [RequirePermission("Accounts", "_CustomerOrder", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<CustomerOrderDto>>>> GetCustomerOrders(
            [FromQuery] long? orderId, [FromQuery] string? orderNo,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "OrderID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetCustomerOrdersAsync(orderId, orderNo, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<CustomerOrderDto>> { Success = true, Data = result });
        }

        [HttpGet("orders/customer/{customerId:long}")]
        [RequirePermission("Accounts", "_CustomerOrder", "view")]
        public async Task<ActionResult<ApiEnvelope<IReadOnlyList<CustomerOrderDto>>>> GetCustomerOrdersByCustomerId(long customerId)
        {
            var result = await _service.GetCustomerOrdersByCustomerIdAsync(customerId);
            return Ok(new ApiEnvelope<IReadOnlyList<CustomerOrderDto>> { Success = true, Data = result });
        }

        [HttpGet("orders/{id:long}")]
        [RequirePermission("Accounts", "_CustomerOrder", "view")]
        public async Task<ActionResult<ApiEnvelope<CustomerOrderDto>>> GetCustomerOrderById(long id)
        {
            var result = await _service.GetCustomerOrderByIdAsync(id);
            if (result == null)
            {
                return Ok(new ApiEnvelope<CustomerOrderDto> { Success = false, Message = "Order not found." });
            }
            return Ok(new ApiEnvelope<CustomerOrderDto> { Success = true, Data = result });
        }

        [HttpPost("orders")]
        [RequirePermission("Accounts", "_CustomerOrder", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveCustomerOrder([FromBody] CustomerOrderDto order)
        {
            var id = await _service.SaveCustomerOrderAsync(order);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Order saved." : "Failed to save." });
        }

        [HttpPost("orders/item")]
        [RequirePermission("Accounts", "_CustomerOrder", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveCustomerOrderItem([FromBody] CustomerOrderItemDto item)
        {
            var id = await _service.SaveCustomerOrderItemAsync(item);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Order item saved." : "Failed to save." });
        }

        [HttpDelete("orders")]
        [RequirePermission("Accounts", "_CustomerOrder", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteCustomerOrders([FromQuery] string ids)
        {
            try
            {
                var success = await _service.DeleteCustomerOrderAsync(ids);
                return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Ok(new ApiEnvelope<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "This order cannot be deleted because it is currently linked to other records (e.g. invoices or manufacturing components)."
                });
            }
        }

        [HttpDelete("orders/item")]
        [RequirePermission("Accounts", "_CustomerOrder", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteCustomerOrderItem([FromQuery] string ids)
        {
            var success = await _service.DeleteCustomerOrderItemAsync(ids);
            return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
        }

        #endregion

        #region Invoice

        [HttpGet("invoices")]
        [RequirePermission("Accounts", "_Invoice", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<InvoiceDto>>>> GetInvoices(
            [FromQuery] long? invoiceId, [FromQuery] string? invoiceNumber,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "InvoiceID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetInvoicesAsync(invoiceId, invoiceNumber, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<InvoiceDto>> { Success = true, Data = result });
        }

        [HttpGet("invoices/{id:long}")]
        [RequirePermission("Accounts", "_Invoice", "view")]
        public async Task<ActionResult<ApiEnvelope<InvoiceDto>>> GetInvoiceById(long id)
        {
            var result = await _service.GetInvoiceByIdAsync(id);
            if (result == null)
            {
                return Ok(new ApiEnvelope<InvoiceDto> { Success = false, Message = "Invoice not found." });
            }
            return Ok(new ApiEnvelope<InvoiceDto> { Success = true, Data = result });
        }

        [HttpPost("invoices")]
        [RequirePermission("Accounts", "_Invoice", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveInvoice([FromBody] InvoiceDto invoice)
        {
            var id = await _service.SaveInvoiceAsync(invoice);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Invoice saved." : "Failed to save." });
        }

        [HttpPost("invoices/item")]
        [RequirePermission("Accounts", "_Invoice", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveInvoiceItem([FromBody] InvoiceItemDto item)
        {
            var id = await _service.SaveInvoiceItemAsync(item);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Invoice item saved." : "Failed to save." });
        }

        [HttpDelete("invoices")]
        [RequirePermission("Accounts", "_Invoice", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteInvoices([FromQuery] string ids)
        {
            try
            {
                var success = await _service.DeleteInvoiceAsync(ids);
                return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return Ok(new ApiEnvelope<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "This invoice cannot be deleted because it is currently linked to receipts or other financial records."
                });
            }
        }

        [HttpDelete("invoices/item")]
        [RequirePermission("Accounts", "_Invoice", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteInvoiceItem([FromQuery] string ids)
        {
            var success = await _service.DeleteInvoiceItemAsync(ids);
            return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
        }

        #endregion

        #region Customer Order Return

        [HttpGet("order-returns")]
        [RequirePermission("Accounts", "_OrderReturn", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<CustomerOrderReturnDto>>>> GetCustomerOrderReturns(
            [FromQuery] long? orderReturnId, [FromQuery] long? orderId,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "OrderReturnID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetCustomerOrderReturnsAsync(orderReturnId, orderId, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<CustomerOrderReturnDto>> { Success = true, Data = result });
        }

        [HttpPost("order-returns")]
        [RequirePermission("Accounts", "_OrderReturn", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveCustomerOrderReturn([FromBody] CustomerOrderReturnDto returnDto)
        {
            var id = await _service.SaveCustomerOrderReturnAsync(returnDto);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Order return saved successfully." : "Failed to save." });
        }

        [HttpDelete("order-returns")]
        [RequirePermission("Accounts", "_OrderReturn", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteCustomerOrderReturns([FromQuery] string ids)
        {
            var success = await _service.DeleteCustomerOrderReturnAsync(ids);
            return Ok(new ApiEnvelope<bool> { Success = success, Data = success, Message = success ? "Order return deleted." : "Failed to delete." });
        }

        [HttpGet("order-returns/item-stock")]
        [RequirePermission("Accounts", "_OrderReturn", "view")]
        public async Task<ActionResult<ApiEnvelope<IReadOnlyList<ItemStockByBatchForBOMDto>>>> GetItemStockByItemIDBatchForBOMList([FromQuery] long itemId)
        {
            var result = await _service.GetItemStockByItemIDBatchForBOMListAsync(itemId);
            return Ok(new ApiEnvelope<IReadOnlyList<ItemStockByBatchForBOMDto>> { Success = true, Data = result });
        }

        [HttpGet("order-returns/item-stock-used")]
        [RequirePermission("Accounts", "_OrderReturn", "view")]
        public async Task<ActionResult<ApiEnvelope<IReadOnlyList<ItemStockUsedForBOMDto>>>> GetItemStockUsedForBOMByOrderDetailId([FromQuery] long orderDetailsId)
        {
            var result = await _service.GetItemStockUsedForBOMByOrderDetailIdAsync(orderDetailsId);
            return Ok(new ApiEnvelope<IReadOnlyList<ItemStockUsedForBOMDto>> { Success = true, Data = result });
        }

        [HttpPost("order-returns/fulfill")]
        [RequirePermission("Accounts", "_CustomerOrder", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveItemStockUsedForBOM([FromBody] ItemStockUsedForBOMDto dto)
        {
            var id = await _service.SaveItemStockUsedForBOMAsync(dto);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Item dispatched successfully." : "Failed to dispatch item." });
        }

        #endregion
    }
}
