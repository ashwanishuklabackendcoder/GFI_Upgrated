using System;

namespace GFI_Upgrated.SharedDto.Store
{
    public class ItemStockTraceabilityDto
    {
        public DateTime? TransactionDate { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public double InQty { get; set; }
        public double OutQty { get; set; }
        public double TotalValue { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public int RefUsedFor { get; set; }
        public long RefUsedForId { get; set; }
    }
}
