using Microsoft.AspNetCore.Mvc;

namespace GFI_Upgrated.ServiceApi.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
public class AdminController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { message = "Admin module is ready" });
    }
}
