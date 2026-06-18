using System;

namespace GFI_Upgrated.SharedDto.Account
{
    public sealed class AccountMasterDto
    {
        public long AccountID { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public long AccountGroupID { get; set; }
        public string? AccountGroupName { get; set; }
        public string? Address { get; set; }
        public string? ContactNo { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailID { get; set; }
        public double? OpeningBalance { get; set; }
        public DateTime? OpeningBalanceDate { get; set; }
        public string? StakeholderGroup { get; set; }
        public string? StakeholderType { get; set; }
        public long? CurrencyID { get; set; }
        public string? CurrencySymbol { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }
}
