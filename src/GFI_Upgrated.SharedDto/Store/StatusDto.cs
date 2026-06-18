using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class StatusDto
{
    public long StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int StatusOf { get; set; }
    public bool IsActive { get; set; }
    public bool IsEditable { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveStatusRequest
{
    public long StatusId { get; set; }

    [Required(ErrorMessage = "Status Name is required.")]
    [StringLength(500, ErrorMessage = "Status Name cannot exceed 500 characters.")]
    public string StatusName { get; set; } = string.Empty;

    public int StatusOf { get; set; }
    public bool IsActive { get; set; } = true;
    public string UpdatedBy { get; set; } = "System";
}
