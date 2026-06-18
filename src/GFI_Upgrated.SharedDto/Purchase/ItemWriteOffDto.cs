using System;

namespace GFI_Upgrated.SharedDto.Purchase
{
    public class ItemWriteOffDto
    {
        public long ItemWriteOffID { get; set; }
        public long ItemStockByBatchId { get; set; }
        public double Quantity { get; set; }
        public string? SellingPrice { get; set; }
        public string? PurchasePrice { get; set; }
        public long? ReasonFor { get; set; }
        public DateTime? RemovalDate { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        // Joined fields for UI
        public string? ItemName { get; set; }
        public string? UnitName { get; set; }
        public string? ReasonName { get; set; }
        public string? BatchNo { get; set; }
    }
}
