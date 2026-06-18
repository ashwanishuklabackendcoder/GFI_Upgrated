using System;

namespace GFI_Upgrated.SharedDto.Purchase
{
    public class PurchaseReturnDto
    {
        public long PurchaseReturnID { get; set; }
        public long ItemStockByBatchId { get; set; }
        public DateTime? ReturnDate { get; set; }
        public long? ReturnReason { get; set; }
        public double Quantity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Description { get; set; }

        // Joined fields for UI
        public string? ItemName { get; set; }
        public string? BatchNo { get; set; }
        public string? ReasonName { get; set; }
        public string? VendorName { get; set; }
    }

    public class PurchaseReturnItemLookupDto
    {
        public long ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public long BrandID { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }

    public class PurchaseReturnBatchLookupDto
    {
        public long ItemStockByBatchId { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public double Quantity { get; set; }
    }
}
