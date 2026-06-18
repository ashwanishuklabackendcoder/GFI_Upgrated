using System;

namespace GFI_Upgrated.SharedDto.Purchase
{
    public class WriteOffBatchLookupDto
    {
        public long ItemStockByBatchId { get; set; }
        public string? ItemName { get; set; }
        public string? BatchNo { get; set; }
        public string? ExpiryDate { get; set; }
        public double FinalQuantityLeft { get; set; }
        public string? UnitName { get; set; }
    }
}
