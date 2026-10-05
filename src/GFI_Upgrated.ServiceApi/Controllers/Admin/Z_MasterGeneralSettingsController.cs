using GFI_Upgrated.Data.AdminSecurity;
using GFI_Upgrated.SharedDto.AdminSecurity;
using GFI_Upgrated.SharedDto.Common;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

namespace GFI_Upgrated.ServiceApi.Controllers.Admin;

[ApiController]
public sealed class Z_MasterGeneralSettingsController : ControllerBase
{
    private readonly IAdminSecurityRepository _repository;
    private readonly IWebHostEnvironment _environment;

    public Z_MasterGeneralSettingsController(IAdminSecurityRepository repository, IWebHostEnvironment environment)
    {
        _repository = repository;
        _environment = environment;
    }

    /// <summary>
    /// Legacy endpoint for front-end JS and OAuth setting loading
    /// </summary>
    [HttpGet("api/Z_MasterGeneralSettings/GetGeneralSettingList")]
    public async Task<IActionResult> GetGeneralSettingList(CancellationToken cancellationToken)
    {
        try
        {
            var dict = await _repository.GetGeneralSettingsDictionaryAsync(cancellationToken);
            var list = dict.Select(kvp => new GeneralSettingItemDto
            {
                ConfigKey = kvp.Key,
                ConfigValue = kvp.Value
            }).ToList();

            var jsonMessage = JsonSerializer.Serialize(list);
            return Ok(new
            {
                errorcode = 0,
                message = jsonMessage
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching GeneralSettingList");
            return StatusCode(500, new { errorcode = 1, message = ex.Message });
        }
    }

    /// <summary>
    /// Strongly typed GET endpoint for Admin General Settings
    /// </summary>
    [HttpGet("api/admin/general-settings")]
    public async Task<ActionResult<ApiEnvelope<GeneralSettingsDto>>> GetGeneralSettings(CancellationToken cancellationToken)
    {
        try
        {
            var settings = await _repository.GetGeneralSettingsAsync(cancellationToken);
            return Ok(new ApiEnvelope<GeneralSettingsDto>
            {
                Success = true,
                Message = "General settings loaded successfully.",
                Data = settings
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving general settings");
            return StatusCode(500, new ApiEnvelope<GeneralSettingsDto>
            {
                Success = false,
                Message = $"Error retrieving general settings: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Strongly typed POST endpoint to update Admin General Settings
    /// </summary>
    [HttpPost("api/admin/general-settings")]
    public async Task<ActionResult<ApiEnvelope<bool>>> SaveGeneralSettings([FromBody] UpdateGeneralSettingsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _repository.SaveGeneralSettingsAsync(request, cancellationToken);
            return Ok(new ApiEnvelope<bool>
            {
                Success = success,
                Message = success ? "General settings saved successfully." : "Failed to save settings.",
                Data = success
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving general settings");
            return StatusCode(500, new ApiEnvelope<bool>
            {
                Success = false,
                Message = $"Error saving settings: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Image upload endpoint for logos (Company Logo, Topbar Logo, Login Page Logo)
    /// </summary>
    [HttpPost("api/admin/general-settings/upload-logo")]
    public async Task<ActionResult<ApiEnvelope<string>>> UploadLogo(IFormFile file, [FromQuery] string logoType, CancellationToken cancellationToken)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiEnvelope<string>
                {
                    Success = false,
                    Message = "No image file provided."
                });
            }

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new ApiEnvelope<string>
                {
                    Success = false,
                    Message = "Invalid file type. Only image files (PNG, JPG, SVG, WEBP) are allowed."
                });
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "branding");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var cleanType = string.IsNullOrWhiteSpace(logoType) ? "logo" : logoType.ToLowerInvariant();
            var fileName = $"{cleanType}_{DateTime.UtcNow.Ticks}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var relativeUrl = $"/uploads/branding/{fileName}";
            return Ok(new ApiEnvelope<string>
            {
                Success = true,
                Message = "Logo uploaded successfully.",
                Data = relativeUrl
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error uploading logo");
            return StatusCode(500, new ApiEnvelope<string>
            {
                Success = false,
                Message = $"Error uploading logo: {ex.Message}"
            });
        }
    }
}
