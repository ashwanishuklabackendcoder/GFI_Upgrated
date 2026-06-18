using System;

namespace GFI_Upgrated.SharedDto.AdminSecurity
{
    public class LoginLogDto
    {
        public long LoginLogID { get; set; }
        public string? LoginName { get; set; }
        public string? UserName { get; set; }
        public string? MasterName { get; set; }
        public string? LoginType { get; set; }
        public string? IpAddress { get; set; }
        public DateTime? LoginDateTime { get; set; }
        public DateTime? LogoutDateTime { get; set; }
        public string? BrowserName { get; set; }
        public string? OperatingSystem { get; set; }
        public string? ComputerName { get; set; }
    }
}
