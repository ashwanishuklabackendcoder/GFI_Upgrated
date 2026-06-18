using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.Store;

public sealed class KettleDto
{
    public long KettleId { get; set; }
    public string KettleNumber { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public sealed class SaveKettleRequest
{
    public long KettleId { get; set; }

    [Required(ErrorMessage = "Kettle Number is required.")]
    [StringLength(500, ErrorMessage = "Kettle Number cannot exceed 500 characters.")]
    public string KettleNumber { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Remarks cannot exceed 1000 characters.")]
    public string? Remarks { get; set; }

    public string UpdatedBy { get; set; } = "System";
}
