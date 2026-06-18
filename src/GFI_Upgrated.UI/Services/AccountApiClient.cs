using System.Net.Http.Json;
using System.Text.Json;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Account;
using GFI_Upgrated.UI.State;

namespace GFI_Upgrated.UI.Services;

public sealed class AccountApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly AppSessionState _sessionState;

    public AccountApiClient(HttpClient httpClient, AppSessionState sessionState)
    {
        _httpClient = httpClient;
        _sessionState = sessionState;
    }

    #region Currency

    public async Task<PagedResult<CurrencyDto>> GetCurrenciesAsync(long? currencyId = null, string? symbol = null, int page = 1, int pageSize = 10, string sortCol = "CurrencyID", string sortOrd = "DESC", CancellationToken cancellationToken = default)
    {
        var query = $"api/account/currencies?page={page}&pageSize={pageSize}&sortCol={sortCol}&sortOrd={sortOrd}";
        if (currencyId.HasValue) query += $"&currencyId={currencyId}";
        if (!string.IsNullOrEmpty(symbol)) query += $"&symbol={Uri.EscapeDataString(symbol)}";
        
        return await GetEnvelopeAsync<PagedResult<CurrencyDto>>(query, cancellationToken) ?? new();
    }

    public async Task<long> SaveCurrencyAsync(CurrencyDto currency, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<CurrencyDto, long>("api/account/currencies", currency, cancellationToken);

    public async Task<bool> DeleteCurrenciesAsync(string ids, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/account/currencies?ids={ids}", cancellationToken);

    #endregion

    #region Account Group

    public async Task<PagedResult<AccountGroupDto>> GetAccountGroupsAsync(long? groupId = null, string? groupName = null, int page = 1, int pageSize = 10, string sortCol = "AccountGroupID", string sortOrd = "DESC", CancellationToken cancellationToken = default)
    {
        var query = $"api/account/groups?page={page}&pageSize={pageSize}&sortCol={sortCol}&sortOrd={sortOrd}";
        if (groupId.HasValue) query += $"&groupId={groupId}";
        if (!string.IsNullOrEmpty(groupName)) query += $"&groupName={Uri.EscapeDataString(groupName)}";
        
        return await GetEnvelopeAsync<PagedResult<AccountGroupDto>>(query, cancellationToken) ?? new();
    }

    public async Task<IReadOnlyList<AccountGroupLookupDto>> GetMainAccountGroupsLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<AccountGroupLookupDto>>("api/account/groups/main-lookup", cancellationToken) ?? Array.Empty<AccountGroupLookupDto>();

    public async Task<IReadOnlyList<AccountGroupLookupDto>> GetAccountGroupsLookupAsync(CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<AccountGroupLookupDto>>("api/account/groups/lookup", cancellationToken) ?? Array.Empty<AccountGroupLookupDto>();

    public async Task<long> SaveAccountGroupAsync(AccountGroupDto group, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<AccountGroupDto, long>("api/account/groups", group, cancellationToken);

    public async Task<bool> DeleteAccountGroupsAsync(string ids, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/account/groups?ids={ids}", cancellationToken);

    #endregion

    #region Account Master

    public async Task<PagedResult<AccountMasterDto>> GetAccountsAsync(long? accountId = null, string? accountName = null, long? groupId = null, int page = 1, int pageSize = 10, string sortCol = "AccountID", string sortOrd = "DESC", CancellationToken cancellationToken = default)
    {
        var query = $"api/account/accounts?page={page}&pageSize={pageSize}&sortCol={sortCol}&sortOrd={sortOrd}";
        if (accountId.HasValue) query += $"&accountId={accountId}";
        if (!string.IsNullOrEmpty(accountName)) query += $"&accountName={Uri.EscapeDataString(accountName)}";
        if (groupId.HasValue) query += $"&groupId={groupId}";
        
        return await GetEnvelopeAsync<PagedResult<AccountMasterDto>>(query, cancellationToken) ?? new();
    }

    public async Task<long> SaveAccountAsync(AccountMasterDto account, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<AccountMasterDto, long>("api/account/accounts", account, cancellationToken);

    public async Task<bool> DeleteAccountsAsync(string ids, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/account/accounts?ids={ids}", cancellationToken);

    #endregion

    #region Customer Order

    public async Task<PagedResult<CustomerOrderDto>> GetCustomerOrdersAsync(long? orderId = null, string? orderNo = null, int page = 1, int pageSize = 10, string sortCol = "OrderID", string sortOrd = "DESC", CancellationToken cancellationToken = default)
    {
        var query = $"api/account/orders?page={page}&pageSize={pageSize}&sortCol={sortCol}&sortOrd={sortOrd}";
        if (orderId.HasValue) query += $"&orderId={orderId}";
        if (!string.IsNullOrEmpty(orderNo)) query += $"&orderNo={Uri.EscapeDataString(orderNo)}";
        
        return await GetEnvelopeAsync<PagedResult<CustomerOrderDto>>(query, cancellationToken) ?? new();
    }

    public async Task<CustomerOrderDto?> GetCustomerOrderByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<CustomerOrderDto>($"api/account/orders/{id}", cancellationToken);

    public async Task<long> SaveCustomerOrderAsync(CustomerOrderDto order, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<CustomerOrderDto, long>("api/account/orders", order, cancellationToken);

    public async Task<long> SaveCustomerOrderItemAsync(CustomerOrderItemDto item, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<CustomerOrderItemDto, long>("api/account/orders/item", item, cancellationToken);

    public async Task<bool> DeleteCustomerOrdersAsync(string ids, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/account/orders?ids={ids}", cancellationToken);

    public async Task<IReadOnlyList<CustomerOrderDto>> GetCustomerOrdersByCustomerIdAsync(long customerId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<CustomerOrderDto>>($"api/account/orders/customer/{customerId}", cancellationToken) ?? Array.Empty<CustomerOrderDto>();

    public async Task<bool> DeleteCustomerOrderItemAsync(string ids, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/account/orders/item?ids={ids}", cancellationToken);

    #endregion

    #region Invoice

    public async Task<PagedResult<InvoiceDto>> GetInvoicesAsync(long? invoiceId = null, string? invoiceNumber = null, int page = 1, int pageSize = 10, string sortCol = "InvoiceID", string sortOrd = "DESC", CancellationToken cancellationToken = default)
    {
        var query = $"api/account/invoices?page={page}&pageSize={pageSize}&sortCol={sortCol}&sortOrd={sortOrd}";
        if (invoiceId.HasValue) query += $"&invoiceId={invoiceId}";
        if (!string.IsNullOrEmpty(invoiceNumber)) query += $"&invoiceNumber={Uri.EscapeDataString(invoiceNumber)}";
        
        return await GetEnvelopeAsync<PagedResult<InvoiceDto>>(query, cancellationToken) ?? new();
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(long id, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<InvoiceDto>($"api/account/invoices/{id}", cancellationToken);

    public async Task<long> SaveInvoiceAsync(InvoiceDto invoice, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<InvoiceDto, long>("api/account/invoices", invoice, cancellationToken);

    public async Task<long> SaveInvoiceItemAsync(InvoiceItemDto item, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<InvoiceItemDto, long>("api/account/invoices/item", item, cancellationToken);

    public async Task<bool> DeleteInvoicesAsync(string ids, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/account/invoices?ids={ids}", cancellationToken);
    public async Task<bool> DeleteInvoiceItemAsync(string ids, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/account/invoices/item?ids={ids}", cancellationToken);

    #endregion

    #region Customer Order Return

    public async Task<PagedResult<CustomerOrderReturnDto>> GetCustomerOrderReturnsAsync(long? orderReturnId = null, long? orderId = null, int page = 1, int pageSize = 10, string sortCol = "OrderReturnID", string sortOrd = "DESC", CancellationToken cancellationToken = default)
    {
        var query = $"api/account/order-returns?page={page}&pageSize={pageSize}&sortCol={sortCol}&sortOrd={sortOrd}";
        if (orderReturnId.HasValue) query += $"&orderReturnId={orderReturnId}";
        if (orderId.HasValue) query += $"&orderId={orderId}";
        
        return await GetEnvelopeAsync<PagedResult<CustomerOrderReturnDto>>(query, cancellationToken) ?? new();
    }

    public async Task<long> SaveCustomerOrderReturnAsync(CustomerOrderReturnDto returnDto, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<CustomerOrderReturnDto, long>("api/account/order-returns", returnDto, cancellationToken);

    public async Task<bool> DeleteCustomerOrderReturnsAsync(string ids, CancellationToken cancellationToken = default)
        => await DeleteEnvelopeAsync<bool>($"api/account/order-returns?ids={ids}", cancellationToken);

    public async Task<IReadOnlyList<ItemStockByBatchForBOMDto>> GetItemStockByItemIDBatchForBOMListAsync(long itemId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<ItemStockByBatchForBOMDto>>($"api/account/order-returns/item-stock?itemId={itemId}", cancellationToken) ?? Array.Empty<ItemStockByBatchForBOMDto>();

    public async Task<IReadOnlyList<ItemStockUsedForBOMDto>> GetItemStockUsedForBOMByOrderDetailIdAsync(long orderDetailsId, CancellationToken cancellationToken = default)
        => await GetEnvelopeAsync<IReadOnlyList<ItemStockUsedForBOMDto>>($"api/account/order-returns/item-stock-used?orderDetailsId={orderDetailsId}", cancellationToken) ?? Array.Empty<ItemStockUsedForBOMDto>();

    public async Task<long> SaveItemStockUsedForBOMAsync(ItemStockUsedForBOMDto dto, CancellationToken cancellationToken = default)
        => await PostEnvelopeAsync<ItemStockUsedForBOMDto, long>("api/account/order-returns/fulfill", dto, cancellationToken);

    #endregion

    #region Helpers

    private void AddAuthHeader()
    {
        if (_sessionState.CurrentUser?.Token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionState.CurrentUser.Token);
        }
    }

    private async Task<TResponse> GetEnvelopeAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with {(int)response.StatusCode} ({response.ReasonPhrase}): {payload}");
        }

        var envelope = string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);

        if (envelope != null && !envelope.Success && !string.IsNullOrWhiteSpace(envelope.Message))
        {
            throw new InvalidOperationException(envelope.Message);
        }

        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    private async Task<TResponse> PostEnvelopeAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with {(int)response.StatusCode} ({response.ReasonPhrase}): {payload}");
        }

        var envelope = string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);

        if (envelope != null && !envelope.Success && !string.IsNullOrWhiteSpace(envelope.Message))
        {
            throw new InvalidOperationException(envelope.Message);
        }

        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    private async Task<TResponse> DeleteEnvelopeAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        AddAuthHeader();
        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with {(int)response.StatusCode} ({response.ReasonPhrase}): {payload}");
        }

        var envelope = string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonSerializer.Deserialize<ApiEnvelope<TResponse>>(payload, JsonOptions);

        if (envelope != null && !envelope.Success && !string.IsNullOrWhiteSpace(envelope.Message))
        {
            throw new InvalidOperationException(envelope.Message);
        }

        return envelope is { Success: true, Data: not null } ? envelope.Data : default!;
    }

    #endregion
}
