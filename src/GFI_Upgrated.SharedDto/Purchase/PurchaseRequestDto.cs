using System;
using System.Collections.Generic;

namespace GFI_Upgrated.SharedDto.Purchase
{
    public class PurchaseRequestDto
    {
        public long PurchaseRequestMasterID { get; set; }
        public string? RequestNumber { get; set; }
        public string? Description { get; set; }
        public long? RequestedBy { get; set; }
        public long? CheckedBy { get; set; }
        public long? ConfirmedBy { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Remarks { get; set; }
        public string? DocumentUpload { get; set; }
        
        // Joined fields for UI
        public string? RequestedByName { get; set; }
        public string? CheckedByName { get; set; }
        public string? ConfirmedByName { get; set; }

        public List<PurchaseRequestItemDto> Items { get; set; } = new();
    }

    public class PurchaseRequestItemDto
    {
        public long PurchaseRequestChildID { get; set; }
        public long PurchaseRequestMasterID { get; set; }
        public long ItemID { get; set; }
        public string? PrefferedBrand { get; set; }
        public string? Description { get; set; }
        public double Quantity { get; set; }
        public long? UnitId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        // Joined fields for UI
        public string? ItemName { get; set; }
        public string? UnitName { get; set; }
    }
}
