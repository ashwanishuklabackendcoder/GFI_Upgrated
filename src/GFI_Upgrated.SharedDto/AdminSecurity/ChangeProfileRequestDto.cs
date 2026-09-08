using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.AdminSecurity
{
    public class ChangeProfileRequestDto
    {
        [Required(ErrorMessage = "Login Name is required")]
        public string LoginName { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = string.Empty;
    }
}
