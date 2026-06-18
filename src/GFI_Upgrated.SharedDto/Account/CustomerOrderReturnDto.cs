using System;

namespace GFI_Upgrated.SharedDto.Account
{
    public class CustomerOrderReturnDto
    {
        public long OrderReturnID { get; set; }
        public long OrderID { get; set; }
        public long OrderDetailsID { get; set; }
        public long ItemStockByBatchId { get; set; }
        public long ItemStockUsedID { get; set; }
        public DateTime? ReturnDate { get; set; }
        public double Quantity { get; set; }
        public double TotalReturnedQty { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ReturnReason { get; set; }

        // Joined UI fields
        public string? AccountName { get; set; }
        public string? BatchNo { get; set; }
        public string? ItemName { get; set; }
        public double OrderedQty { get; set; }
    }

    public class ItemStockByBatchForBOMDto
    {
        public long ItemStockByBatchID { get; set; }
        public string? BatchNo { get; set; }
        public double FinalQuantityLeft { get; set; }
        public string? ExpiryDateBOM { get; set; }
    }

    public class ItemStockUsedForBOMDto
    {
        public long ItemStockUsedID { get; set; }
        public long ItemStockByBatchID { get; set; }
        public long OrderCustomerID { get; set; }
        public long OrderCustomerDetailsID { get; set; }
        public int UsedFor { get; set; }
        public string? BatchNo { get; set; }
        public double Quantity { get; set; }
        public double ReturnQty { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
