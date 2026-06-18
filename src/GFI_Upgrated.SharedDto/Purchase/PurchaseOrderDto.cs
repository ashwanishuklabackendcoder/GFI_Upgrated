using System;
using System.Collections.Generic;

namespace GFI_Upgrated.SharedDto.Purchase
{
    public class PurchaseOrderDto
    {
        public long PurchaseOrderID { get; set; }
        public string? VoucherNumber { get; set; }
        public long AccountID { get; set; }
        public long? PurchaseRequestID { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? TaxType { get; set; }
        public string? TaxName1 { get; set; }
        public double? TaxAmount1 { get; set; }
        public string? TaxName2 { get; set; }
        public double? TaxAmount2 { get; set; }
        public string? TaxName3 { get; set; }
        public double? TaxAmount3 { get; set; }
        public double? DiscountPercent { get; set; }
        public double? DiscountAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public long? CheckedBy { get; set; }
        public long? ConfirmedBy { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string? DocumentUpload { get; set; }
        public string? Remarks { get; set; }

        // Joined fields for UI
        public string? VendorName { get; set; }
        public string? PRNumber { get; set; }
        public string? CheckedByName { get; set; }
        public string? ConfirmedByName { get; set; }
        public double TotalAmount { get; set; }

        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class PurchaseOrderItemDto
    {
        public long PurchaseOrderChildID { get; set; }
        public long PurchaseOrderID { get; set; }
        public long ItemID { get; set; }
        public long? PreferredBrand { get; set; }
        public string? Description { get; set; }
        public double Quantity { get; set; }
        public long? UnitId { get; set; }
        public double Price { get; set; }
        public bool? ItemReceivedCheck { get; set; }
        public long? ItemReceivedCheckBy { get; set; }
        public long? ItemRecievedBy { get; set; }
        public DateTime? ItemReceivedDate { get; set; }
        public string? ItemReceiveRemarks { get; set; }

        // Joined fields for UI
        public string? ItemName { get; set; }
        public string? UnitName { get; set; }
        public double Amount => Quantity * Price;
    }
}
