using System;

namespace GFI_Upgrated.SharedDto.Account
{
    public sealed class AccountGroupDto
    {
        public long AccountGroupID { get; set; }
        public string AccountGroupName { get; set; } = string.Empty;
        public bool IsMainAccountGroup { get; set; }
        public long? MainAccountGroupID { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEditable { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }
    }
}
