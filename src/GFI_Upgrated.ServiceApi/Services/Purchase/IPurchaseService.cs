using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Purchase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Purchase
{
    public interface IPurchaseService
    {
        // Purchase Request
        Task<PagedResult<PurchaseRequestDto>> GetPurchaseRequestsAsync(long? requestId, string? requestNumber, long? requestedBy, int page, int pageSize, string sortCol, string sortOrd);
        Task<List<PurchaseRequestItemDto>> GetPurchaseRequestItemsAsync(long requestId);
        Task<long> SavePurchaseRequestAsync(PurchaseRequestDto request);
        Task<bool> DeletePurchaseRequestAsync(string ids, string deletedBy);

        // Purchase Order
        Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(long? orderId, string? voucherNumber, long? accountId, int page, int pageSize, string sortCol, string sortOrd);
        Task<List<PurchaseOrderItemDto>> GetPurchaseOrderItemsAsync(long orderId);
        Task<long> SavePurchaseOrderAsync(PurchaseOrderDto order);
        Task<bool> DeletePurchaseOrderAsync(string ids, string deletedBy);

        // Purchase (GRN)
        Task<PagedResult<PurchaseDto>> GetPurchasesAsync(long? purchaseId, string? voucherNumber, string? invoiceNo, long? accountId, int page, int pageSize, string sortCol, string sortOrd);
        Task<List<PurchaseItemDto>> GetPurchaseItemsAsync(long purchaseId);
        Task<long> SavePurchaseAsync(PurchaseDto purchase);
        Task<bool> DeletePurchaseAsync(string ids, string deletedBy);

        // Purchase Return
        Task<PagedResult<PurchaseReturnDto>> GetPurchaseReturnsAsync(long? returnId, long? itemId, int page, int pageSize, string sortCol, string sortOrd);
        Task<long> SavePurchaseReturnAsync(PurchaseReturnDto @return);
        Task<bool> DeletePurchaseReturnAsync(string ids, string deletedBy);
        Task<List<PurchaseReturnItemLookupDto>> GetPurchaseReturnItemsLookupAsync(long accountId);
        Task<List<PurchaseReturnBatchLookupDto>> GetPurchaseReturnBatchesLookupAsync(long itemId, long brandId);

        // Item Write Off
        Task<PagedResult<ItemWriteOffDto>> GetItemWriteOffsAsync(long? writeOffId, long? itemId, int page, int pageSize, string sortCol, string sortOrd);
        Task<long> SaveItemWriteOffAsync(ItemWriteOffDto dto);
        Task<bool> DeleteItemWriteOffAsync(string ids, string deletedBy);
        Task<List<WriteOffBatchLookupDto>> GetWriteOffBatchesLookupAsync();
    }
}
