using System;
using System.Collections.Generic;

namespace GFI_Upgrated.SharedDto.Account
{
    public class CustomerOrderDto
    {
        public long OrderID { get; set; }
        public long CustomerID { get; set; }
        public string? OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Remarks { get; set; }
        public string? DocumentUpload { get; set; }

        // Joined UI fields
        public string? CustomerName { get; set; }
        public int TotalItems { get; set; }

        public List<CustomerOrderItemDto> Items { get; set; } = new();
    }

    public class CustomerOrderItemDto
    {
        public long OrderItemId { get; set; } // Maps to CustomerOrderDetailsID
        public long OrderId { get; set; }
        public long ItemId { get; set; }
        public int Qty { get; set; }
        public string? Remarks { get; set; }
        public bool? IsComplete { get; set; }

        // Joined UI fields
        public string? ItemName { get; set; }
        public string? ItemCode { get; set; }

        // Helper field for batch save/delete tracking
        public string? DeletedOrderItemIds { get; set; }
    }
}
