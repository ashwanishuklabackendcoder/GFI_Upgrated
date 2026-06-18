using System;
using System.Collections.Generic;

namespace GFI_Upgrated.SharedDto.Purchase
{
    public class PurchaseDto
    {
        public long PurchaseID { get; set; }
        public long AccountID { get; set; }
        public string? VoucherNumber { get; set; }
        public DateTime? GoodsRecievedDate { get; set; }
        public long? PurchaseOrderID { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public double? Taxes { get; set; }
        public double? Shipping { get; set; }
        public double? Discount { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? FileName { get; set; }
        public string? Narration { get; set; }

        // Joined fields for UI
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }

        public List<PurchaseItemDto> Items { get; set; } = new();
    }

    public class PurchaseItemDto
    {
        public long PurchaseItemID { get; set; }
        public long PurchaseID { get; set; }
        public long? BrandID { get; set; }
        public long ItemID { get; set; }
        public double Quantity { get; set; }
        public long? UnitId { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        // Joined fields for UI
        public string? ItemName { get; set; }
        public string? UnitName { get; set; }
        public string? BrandName { get; set; }
        
        // Batch details (for GRN receipt)
        public string? BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long? WarehouseId { get; set; }
    }
}
