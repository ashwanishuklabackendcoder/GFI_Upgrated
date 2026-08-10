namespace GFI_Upgrated.SharedDto.Account
{
    public class ItemStockUsedDto
    {
        public long ItemStockUsedID { get; set; }
        public long ItemStockByBatchId { get; set; }
        public int UsedFor { get; set; }
        public long UsedForId { get; set; }
        public double Quantity { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public long? UnitId { get; set; }
    }
}
