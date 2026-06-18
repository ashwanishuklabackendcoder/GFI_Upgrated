using GFI_Upgrated.ServiceApi.Infrastructure;
using GFI_Upgrated.ServiceApi.Services;
using GFI_Upgrated.ServiceApi.Services.Purchase;
using GFI_Upgrated.SharedDto.Common;
using GFI_Upgrated.SharedDto.Purchase;
using GFI_Upgrated.SharedDto.AdminSecurity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GFI_Upgrated.ServiceApi.Controllers.Purchase
{
    [Authorize]
    [ApiController]
    [Route("api/purchase")]
    public sealed class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _service;
        private readonly IAdminSecurityService _securityService;

        public PurchaseController(IPurchaseService service, IAdminSecurityService securityService)
        {
            _service = service;
            _securityService = securityService;
        }

        #region Purchase Request

        [HttpGet("requests")]
        [RequirePermission("Purchase", "_PurchaseRequest", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<PurchaseRequestDto>>>> GetPurchaseRequests(
            [FromQuery] long? requestId, [FromQuery] string? requestNumber, [FromQuery] long? requestedBy,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "PurchaseRequestMasterID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetPurchaseRequestsAsync(requestId, requestNumber, requestedBy, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<PurchaseRequestDto>> { Success = true, Data = result });
        }

        [HttpGet("requests/{id:long}/items")]
        [RequirePermission("Purchase", "_PurchaseRequest", "view")]
        public async Task<ActionResult<ApiEnvelope<List<PurchaseRequestItemDto>>>> GetPurchaseRequestItems(long id)
        {
            var result = await _service.GetPurchaseRequestItemsAsync(id);
            return Ok(new ApiEnvelope<List<PurchaseRequestItemDto>> { Success = true, Data = result });
        }

        [HttpPost("requests")]
        [RequirePermission("Purchase", "_PurchaseRequest", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SavePurchaseRequest([FromBody] PurchaseRequestDto request)
        {
            var id = await _service.SavePurchaseRequestAsync(request);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Purchase Request saved." : "Failed to save." });
        }

        [HttpDelete("requests")]
        [RequirePermission("Purchase", "_PurchaseRequest", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeletePurchaseRequests([FromQuery] string ids, [FromQuery] string deletedBy)
        {
            var success = await _service.DeletePurchaseRequestAsync(ids, deletedBy);
            return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
        }

        #endregion

        #region Purchase Order

        [HttpGet("orders")]
        [RequirePermission("Purchase", "_PurchaseOrder", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<PurchaseOrderDto>>>> GetPurchaseOrders(
            [FromQuery] long? orderId, [FromQuery] string? voucherNumber, [FromQuery] long? accountId,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "PurchaseOrderID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetPurchaseOrdersAsync(orderId, voucherNumber, accountId, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<PurchaseOrderDto>> { Success = true, Data = result });
        }

        [HttpGet("orders/{id:long}/items")]
        [RequirePermission("Purchase", "_PurchaseOrder", "view")]
        public async Task<ActionResult<ApiEnvelope<List<PurchaseOrderItemDto>>>> GetPurchaseOrderItems(long id)
        {
            var result = await _service.GetPurchaseOrderItemsAsync(id);
            return Ok(new ApiEnvelope<List<PurchaseOrderItemDto>> { Success = true, Data = result });
        }

        [HttpPost("orders")]
        [RequirePermission("Purchase", "_PurchaseOrder", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SavePurchaseOrder([FromBody] PurchaseOrderDto order)
        {
            var id = await _service.SavePurchaseOrderAsync(order);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Purchase Order saved." : "Failed to save." });
        }

        [HttpDelete("orders")]
        [RequirePermission("Purchase", "_PurchaseOrder", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeletePurchaseOrders([FromQuery] string ids, [FromQuery] string deletedBy)
        {
            var success = await _service.DeletePurchaseOrderAsync(ids, deletedBy);
            return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
        }

        #endregion

        #region Purchase (GRN)

        [HttpGet("grn")]
        [RequirePermission("Purchase", "_Purchase", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<PurchaseDto>>>> GetPurchases(
            [FromQuery] long? purchaseId, [FromQuery] string? voucherNumber, [FromQuery] string? invoiceNo, [FromQuery] long? accountId,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "PurchaseID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetPurchasesAsync(purchaseId, voucherNumber, invoiceNo, accountId, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<PurchaseDto>> { Success = true, Data = result });
        }

        [HttpGet("grn/{id:long}/items")]
        [RequirePermission("Purchase", "_Purchase", "view")]
        public async Task<ActionResult<ApiEnvelope<List<PurchaseItemDto>>>> GetPurchaseItems(long id)
        {
            var result = await _service.GetPurchaseItemsAsync(id);
            return Ok(new ApiEnvelope<List<PurchaseItemDto>> { Success = true, Data = result });
        }

        [HttpPost("grn")]
        [RequirePermission("Purchase", "_Purchase", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SavePurchase([FromBody] PurchaseDto purchase)
        {
            var id = await _service.SavePurchaseAsync(purchase);
            return Ok(new ApiEnvelope<long> { Success = id != 0, Data = id, Message = id > 0 ? "Purchase recorded." : (id == -1 ? "Voucher Number already exists." : "Failed to record.") });
        }

        [HttpDelete("grn")]
        [RequirePermission("Purchase", "_Purchase", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeletePurchases([FromQuery] string ids, [FromQuery] string deletedBy)
        {
            var success = await _service.DeletePurchaseAsync(ids, deletedBy);
            return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
        }

        #endregion

        #region Purchase Return

        [HttpGet("returns")]
        [RequirePermission("Purchase", "_PurchaseReturn", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<PurchaseReturnDto>>>> GetPurchaseReturns(
            [FromQuery] long? returnId, [FromQuery] long? itemId,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "PurchaseReturnID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetPurchaseReturnsAsync(returnId, itemId, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<PurchaseReturnDto>> { Success = true, Data = result });
        }

        [HttpPost("returns")]
        [RequirePermission("Purchase", "_PurchaseReturn", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SavePurchaseReturn([FromBody] PurchaseReturnDto @return)
        {
            var id = await _service.SavePurchaseReturnAsync(@return);
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Return recorded." : "Failed to record." });
        }

        [HttpDelete("returns")]
        [RequirePermission("Purchase", "_PurchaseReturn", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeletePurchaseReturns([FromQuery] string ids, [FromQuery] string deletedBy)
        {
            var success = await _service.DeletePurchaseReturnAsync(ids, deletedBy);
            return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
        }

        [HttpGet("returns/items-lookup")]
        [RequirePermission("Purchase", "_PurchaseReturn", "view")]
        public async Task<ActionResult<ApiEnvelope<List<PurchaseReturnItemLookupDto>>>> GetPurchaseReturnItemsLookup([FromQuery] long accountId)
        {
            var result = await _service.GetPurchaseReturnItemsLookupAsync(accountId);
            return Ok(new ApiEnvelope<List<PurchaseReturnItemLookupDto>> { Success = true, Data = result });
        }

        [HttpGet("returns/batches-lookup")]
        [RequirePermission("Purchase", "_PurchaseReturn", "view")]
        public async Task<ActionResult<ApiEnvelope<List<PurchaseReturnBatchLookupDto>>>> GetPurchaseReturnBatchesLookup([FromQuery] long itemId, [FromQuery] long brandId)
        {
            var result = await _service.GetPurchaseReturnBatchesLookupAsync(itemId, brandId);
            return Ok(new ApiEnvelope<List<PurchaseReturnBatchLookupDto>> { Success = true, Data = result });
        }

        #endregion

        #region Item Write Off

        [HttpGet("write-offs")]
        [RequirePermission("Purchase", "_ItemWriteOff", "view")]
        public async Task<ActionResult<ApiEnvelope<PagedResult<ItemWriteOffDto>>>> GetItemWriteOffs(
            [FromQuery] long? writeOffId, [FromQuery] long? itemId,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string sortCol = "ItemWriteOffID", [FromQuery] string sortOrd = "DESC")
        {
            var result = await _service.GetItemWriteOffsAsync(writeOffId, itemId, page, pageSize, sortCol, sortOrd);
            return Ok(new ApiEnvelope<PagedResult<ItemWriteOffDto>> { Success = true, Data = result });
        }

        [HttpPost("write-offs")]
        [RequirePermission("Purchase", "_ItemWriteOff", "insert")]
        public async Task<ActionResult<ApiEnvelope<long>>> SaveItemWriteOff([FromBody] ItemWriteOffDto dto)
        {
            var isEdit = dto.ItemWriteOffID > 0;
            var id = await _service.SaveItemWriteOffAsync(dto);
            if (id > 0)
            {
                await _securityService.InsertUserActivityLogAsync(new UserActivityLogDto
                {
                    UserName = dto.CreatedBy ?? "System",
                    LoginName = dto.CreatedBy ?? "System",
                    DT = DateTime.Now,
                    EventName = isEdit ? "UPDATE" : "INSERT",
                    EventModule = "Purchase",
                    RefKey = id.ToString(),
                    Remarks = isEdit ? $"Updated item write off ID {dto.ItemWriteOffID}." : $"Created item write off for batch ID {dto.ItemStockByBatchId} with quantity {dto.Quantity}.",
                    Url = "/admin/item-write-off"
                });
            }
            return Ok(new ApiEnvelope<long> { Success = id > 0, Data = id, Message = id > 0 ? "Item write off recorded successfully." : "Failed to record write off." });
        }

        [HttpDelete("write-offs")]
        [RequirePermission("Purchase", "_ItemWriteOff", "delete")]
        public async Task<ActionResult<ApiEnvelope<bool>>> DeleteItemWriteOffs([FromQuery] string ids, [FromQuery] string deletedBy)
        {
            var success = await _service.DeleteItemWriteOffAsync(ids, deletedBy);
            if (success)
            {
                await _securityService.InsertUserActivityLogAsync(new UserActivityLogDto
                {
                    UserName = deletedBy,
                    LoginName = deletedBy,
                    DT = DateTime.Now,
                    EventName = "DELETE",
                    EventModule = "Purchase",
                    RefKey = ids,
                    Remarks = $"Deleted item write off IDs: {ids}.",
                    Url = "/admin/item-write-off"
                });
            }
            return Ok(new ApiEnvelope<bool> { Success = success, Data = success });
        }

        [HttpGet("write-offs/batches-lookup")]
        [RequirePermission("Purchase", "_ItemWriteOff", "view")]
        public async Task<ActionResult<ApiEnvelope<List<WriteOffBatchLookupDto>>>> GetWriteOffBatchesLookup()
        {
            var result = await _service.GetWriteOffBatchesLookupAsync();
            return Ok(new ApiEnvelope<List<WriteOffBatchLookupDto>> { Success = true, Data = result });
        }

        #endregion
    }
}
