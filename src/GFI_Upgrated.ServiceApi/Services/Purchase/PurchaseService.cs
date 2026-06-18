using GFI_Upgrated.Data.Purchase;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Purchase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Services.Purchase
{
    public sealed class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _repository;

        public PurchaseService(IPurchaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<PurchaseRequestDto>> GetPurchaseRequestsAsync(long? requestId, string? requestNumber, long? requestedBy, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetPurchaseRequestsAsync(requestId, requestNumber, requestedBy, page, pageSize, sortCol, sortOrd);

        public async Task<List<PurchaseRequestItemDto>> GetPurchaseRequestItemsAsync(long requestId)
            => await _repository.GetPurchaseRequestItemsAsync(requestId);

        public async Task<long> SavePurchaseRequestAsync(PurchaseRequestDto request)
            => await _repository.SavePurchaseRequestAsync(request);

        public async Task<bool> DeletePurchaseRequestAsync(string ids, string deletedBy)
            => await _repository.DeletePurchaseRequestAsync(ids, deletedBy);

        public async Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(long? orderId, string? voucherNumber, long? accountId, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetPurchaseOrdersAsync(orderId, voucherNumber, accountId, page, pageSize, sortCol, sortOrd);

        public async Task<List<PurchaseOrderItemDto>> GetPurchaseOrderItemsAsync(long orderId)
            => await _repository.GetPurchaseOrderItemsAsync(orderId);

        public async Task<long> SavePurchaseOrderAsync(PurchaseOrderDto order)
            => await _repository.SavePurchaseOrderAsync(order);

        public async Task<bool> DeletePurchaseOrderAsync(string ids, string deletedBy)
            => await _repository.DeletePurchaseOrderAsync(ids, deletedBy);

        public async Task<PagedResult<PurchaseDto>> GetPurchasesAsync(long? purchaseId, string? voucherNumber, string? invoiceNo, long? accountId, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetPurchasesAsync(purchaseId, voucherNumber, invoiceNo, accountId, page, pageSize, sortCol, sortOrd);

        public async Task<List<PurchaseItemDto>> GetPurchaseItemsAsync(long purchaseId)
            => await _repository.GetPurchaseItemsAsync(purchaseId);

        public async Task<long> SavePurchaseAsync(PurchaseDto purchase)
            => await _repository.SavePurchaseAsync(purchase);

        public async Task<bool> DeletePurchaseAsync(string ids, string deletedBy)
            => await _repository.DeletePurchaseAsync(ids, deletedBy);

        public async Task<PagedResult<PurchaseReturnDto>> GetPurchaseReturnsAsync(long? returnId, long? itemId, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetPurchaseReturnsAsync(returnId, itemId, page, pageSize, sortCol, sortOrd);

        public async Task<long> SavePurchaseReturnAsync(PurchaseReturnDto @return)
            => await _repository.SavePurchaseReturnAsync(@return);

        public async Task<bool> DeletePurchaseReturnAsync(string ids, string deletedBy)
            => await _repository.DeletePurchaseReturnAsync(ids, deletedBy);

        public async Task<List<PurchaseReturnItemLookupDto>> GetPurchaseReturnItemsLookupAsync(long accountId)
            => await _repository.GetPurchaseReturnItemsLookupAsync(accountId);

        public async Task<List<PurchaseReturnBatchLookupDto>> GetPurchaseReturnBatchesLookupAsync(long itemId, long brandId)
            => await _repository.GetPurchaseReturnBatchesLookupAsync(itemId, brandId);

        public async Task<PagedResult<ItemWriteOffDto>> GetItemWriteOffsAsync(long? writeOffId, long? itemId, int page, int pageSize, string sortCol, string sortOrd)
            => await _repository.GetItemWriteOffsAsync(writeOffId, itemId, page, pageSize, sortCol, sortOrd);

        public async Task<long> SaveItemWriteOffAsync(ItemWriteOffDto dto)
            => await _repository.SaveItemWriteOffAsync(dto);

        public async Task<bool> DeleteItemWriteOffAsync(string ids, string deletedBy)
            => await _repository.DeleteItemWriteOffAsync(ids, deletedBy);

        public async Task<List<WriteOffBatchLookupDto>> GetWriteOffBatchesLookupAsync()
            => await _repository.GetWriteOffBatchesLookupAsync();
    }
}
