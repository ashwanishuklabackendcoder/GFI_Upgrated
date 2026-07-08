using System.ComponentModel.DataAnnotations;

namespace GFI_Upgrated.SharedDto.AdminSecurity;

public sealed class LoginRequest
{
    public long LoginId { get; set; }
    public string LoginName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public string? ComputerName { get; set; }
}

public sealed class LoginResultDto
{
    public int Status { get; set; }
    public long LoginId { get; set; }
    public string LoginName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
    public long RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public string? DashboardPath { get; set; }
    public long LanguageId { get; set; }
    public string? CultureName { get; set; }
    public string Token { get; set; } = string.Empty;
    public IReadOnlyList<MenuDto> Menus { get; set; } = Array.Empty<MenuDto>();
}

public sealed class RoleDto
{
    public long RoleId { get; set; }
    public long SchoolId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; }
    public long ModuleId { get; set; }
    public string? ModuleName { get; set; }
}

public sealed class SaveRoleRequest
{
    public long RoleId { get; set; }
    public long SchoolId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; }
    public long ModuleId { get; set; }
    public string CreatedBy { get; set; } = "System";
    public string UpdatedBy { get; set; } = "System";
}

public sealed class UserDto
{
    public long LoginId { get; set; }
    public long SchoolId { get; set; }
    public long StaffId { get; set; }
    public string LoginName { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? ForgotEmail { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? LoginType { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    public string? ComputerName { get; set; }
    public long RoleId { get; set; }
}

public sealed class SaveUserRequest
{
    public long LoginId { get; set; }
    public long StaffId { get; set; }
    public string LoginName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ForgotEmail { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "System";
    public string UpdatedBy { get; set; } = "System";
    public string? IpAddress { get; set; }
    public long RoleId { get; set; }
}

public sealed class MenuDto
{
    public long LinkId { get; set; }
    public long ModuleId { get; set; }
    public string PageHeading { get; set; } = string.Empty;
    public long ParentId { get; set; }
    public string? PagePath { get; set; }
    public string? ActualName { get; set; }
    public bool IsView { get; set; }
    public bool IsInsert { get; set; }
    public bool IsUpdate { get; set; }
    public bool IsDelete { get; set; }
    public bool IsExport { get; set; }
    public bool IsPrint { get; set; }
    public bool IsApprove { get; set; }
    public int LevelNo { get; set; }
    public int SequenceNo { get; set; }
    public bool IsDashboard { get; set; }
    public bool ShowInMenu { get; set; }
    public bool IsApp { get; set; }
    public string? ModuleName { get; set; }
    public string? DisplayName { get; set; }
    public string? IconClass { get; set; }
    public List<MenuDto> SubMenus { get; set; } = new();
}

public sealed class SaveMenuRequest
{
    public long LinkId { get; set; }
    public long ModuleId { get; set; }
    public string PageHeading { get; set; } = string.Empty;
    public long ParentId { get; set; }
    public string? PagePath { get; set; }
    public string? ActualName { get; set; }
    public bool IsView { get; set; }
    public bool IsInsert { get; set; }
    public bool IsUpdate { get; set; }
    public bool IsDelete { get; set; }
    public bool IsExport { get; set; }
    public bool IsPrint { get; set; }
    public bool IsApprove { get; set; }
    public int LevelNo { get; set; }
    public int SequenceNo { get; set; }
    public bool IsDashboard { get; set; }
    public bool ShowInMenu { get; set; }
    public bool IsApp { get; set; }
    public string? DisplayName { get; set; }
    public string? IconClass { get; set; }
    public string CreatedBy { get; set; } = "System";
    public string UpdatedBy { get; set; } = "System";
}

public sealed class RolePermissionDto
{
    public long RoleFormID { get; set; }
    public long RoleID { get; set; }
    public long LinkID { get; set; }
    public bool ViewPer { get; set; }
    public bool InsertPer { get; set; }
    public bool UpdatePer { get; set; }
    public bool DeletePer { get; set; }
    public bool ExportPer { get; set; }
    public bool PrintPer { get; set; }
    public bool ApprovePer { get; set; }
    public string? DisplayName { get; set; }
    public string? IconClass { get; set; }
    public string? PageHeading { get; set; }
    public string? PagePath { get; set; }
    public long ParentID { get; set; }
    public int SequenceNo { get; set; }
    public long ModuleID { get; set; }
}

public sealed class SaveRolePermissionsRequest
{
    public long RoleID { get; set; }
    public long ModuleID { get; set; }
    public string? DashboardPage { get; set; }
    public List<RolePermissionDto> Permissions { get; set; } = new();
}

public sealed class UserRoleAssignmentDto
{
    public long UserRoleId { get; set; }
    public long RoleId { get; set; }
    public long LoginId { get; set; }
    public bool IsDefault { get; set; }
    public string? RoleName { get; set; }
    public string? LoginName { get; set; }
}

public sealed class SaveUserRolesRequest
{
    public long LoginId { get; set; }
    public List<UserRoleAssignmentDto> Roles { get; set; } = new();
}

public sealed class ModuleDto
{
    public long ModuleId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
}

public sealed class StaffLookupDto
{
    public long StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Status { get; set; }
    public bool IsActive { get; set; }
}

public sealed class LookupItemDto
{
    public long Value { get; set; }
    public string Text { get; set; } = string.Empty;
}

public sealed class DropDownMasterDto
{
    public long Id { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedDate { get; set; }
    public int ValuesCount { get; set; }
}

public sealed class SaveDropDownMasterRequest
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "System";
    public string UpdatedBy { get; set; } = "System";
}

public sealed class DropDownValueDto
{
    public long Id { get; set; }
    public long DropDownMasterId { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedDate { get; set; }
}

public sealed class SaveDropDownValueRequest : IValidatableObject
{
    public long Id { get; set; }
    public long DropDownMasterId { get; set; }

    [Required]
    public string DisplayText { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "System";
    public string UpdatedBy { get; set; } = "System";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DropDownMasterId <= 0)
        {
            yield return new ValidationResult("Drop down master is required.", new[] { nameof(DropDownMasterId) });
        }

        if (string.IsNullOrWhiteSpace(DisplayText))
        {
            yield return new ValidationResult("Display text is required.", new[] { nameof(DisplayText) });
        }
    }
}

public sealed class StaffDto
{
    public long StaffId { get; set; }
    public int Status { get; set; }
    public string? StaffSalutation { get; set; }
    public string? StaffSalutationName { get; set; }
    public string StaffFirstName { get; set; } = string.Empty;
    public string StaffLastName { get; set; } = string.Empty;
    public string StaffName => string.Join(" ", new[] { StaffFirstName, StaffLastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
    public string? StaffNumber { get; set; }
    public string? NIN { get; set; }
    public DateTime? DOB { get; set; }
    public long Gender { get; set; }
    public string? GenderName { get; set; }
    public string? MobileNo { get; set; }
    public string? EmailIDPersonal { get; set; }
    public string EmailIDOfficial { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public bool HasLogin { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public sealed class SaveStaffRequest : IValidatableObject
{
    public long StaffId { get; set; }
    public int Status { get; set; }

    [Required]
    public string? StaffSalutation { get; set; }

    [Required]
    public string StaffFirstName { get; set; } = string.Empty;

    [Required]
    public string StaffLastName { get; set; } = string.Empty;

    public string? StaffNumber { get; set; }

    public string? NIN { get; set; }

    [Required]
    public DateTime? DOB { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "Gender is required.")]
    public long Gender { get; set; }

    public string? MobileNo { get; set; }

    [EmailAddress]
    public string? EmailIDPersonal { get; set; }

    [Required]
    [EmailAddress]
    public string EmailIDOfficial { get; set; } = string.Empty;

    public string? Photo { get; set; }
    public bool HasLogin { get; set; }
    public string CreatedBy { get; set; } = "System";
    public string? UpdatedBy { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DOB is not null && DOB.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult("DOB cannot be a future date.", new[] { nameof(DOB) });
        }

        if (string.IsNullOrWhiteSpace(StaffFirstName))
        {
            yield return new ValidationResult("First Name is required.", new[] { nameof(StaffFirstName) });
        }

        if (string.IsNullOrWhiteSpace(StaffLastName))
        {
            yield return new ValidationResult("Last Name is required.", new[] { nameof(StaffLastName) });
        }

        if (string.IsNullOrWhiteSpace(EmailIDOfficial))
        {
            yield return new ValidationResult("Official email is required.", new[] { nameof(EmailIDOfficial) });
        }

        if (Gender <= 0)
        {
            yield return new ValidationResult("Please select gender.", new[] { nameof(Gender) });
        }

        if (string.IsNullOrWhiteSpace(MobileNo))
        {
            yield return new ValidationResult("Mobile number is required.", new[] { nameof(MobileNo) });
        }

        if (string.IsNullOrWhiteSpace(StaffSalutation))
        {
            yield return new ValidationResult("Salutation is required.", new[] { nameof(StaffSalutation) });
        }
    }
}
