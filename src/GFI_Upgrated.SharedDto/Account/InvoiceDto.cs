using System;
using System.Collections.Generic;

namespace GFI_Upgrated.SharedDto.Account
{
    public class InvoiceDto
    {
        public long InvoiceID { get; set; }
        public long AccountID { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? InvoiceStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Remarks { get; set; }
        public long? CurrencyID { get; set; }
        public double? CurrencyConversion { get; set; }

        // Joined UI fields
        public string? AccountName { get; set; }
        public string? CurrencySymbol { get; set; }
        public double TotalAmount { get; set; }

        public List<InvoiceItemDto> Items { get; set; } = new();
    }

    public class InvoiceItemDto
    {
        public long InvoiceChildID { get; set; }
        public long InvoiceID { get; set; }
        public long OrderID { get; set; }
        public long ItemId { get; set; }
        public string? PrintHeading { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string? Description { get; set; }
        public string? BatchNumber { get; set; }

        // Joined UI fields
        public string? ItemName { get; set; }
        public string? OrderNo { get; set; }
    }
}
