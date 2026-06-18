using System;

namespace GFI_Upgrated.SharedDto.AdminSecurity
{
    public class UserActivityLogDto
    {
        public long LogId { get; set; }
        public string? UserName { get; set; }
        public string? LoginName { get; set; }
        public DateTime? DT { get; set; }
        public string? EventName { get; set; }
        public string? EventModule { get; set; }
        public string? RefKey { get; set; }
        public string? Remarks { get; set; }
        public string? Url { get; set; }
        public long? LoginId { get; set; }
    }
}
