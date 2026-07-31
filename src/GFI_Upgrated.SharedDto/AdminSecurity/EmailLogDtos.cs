using System;

namespace GFI_Upgrated.SharedDto.AdminSecurity;

public class SendStaffEmailRequest
{
    public long StaffId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class EmailLogDto
{
    public long EmailLogID { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public long? StaffId { get; set; }
}
