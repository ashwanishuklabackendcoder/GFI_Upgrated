using System;

namespace GFI_Upgrated.SharedDto.Account
{
    public sealed class CurrencyDto
    {
        public long CurrencyID { get; set; }
        public string CurrencySymbol { get; set; } = string.Empty;
        public string CurrencyString { get; set; } = string.Empty;
        public string CurrencySubString { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
