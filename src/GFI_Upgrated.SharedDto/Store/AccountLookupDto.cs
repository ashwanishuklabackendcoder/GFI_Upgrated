namespace GFI_Upgrated.SharedDto.Store;

public class AccountLookupDto
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public int? StakeholderType { get; set; }
    public int? AccountGroupId { get; set; }
}
