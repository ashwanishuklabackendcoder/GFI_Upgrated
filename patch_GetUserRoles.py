import os

file = r'd:\GFI_Upgrated\src\GFI_Upgrated.Data\AdminSecurity\AdminSecurityRepository.cs'
with open(file, 'r', encoding='utf-8') as f:
    content = f.read()

old = '''        return table.AsEnumerable().Select(row => new UserRoleAssignmentDto
        {
            UserRoleId = row.SafeLong("UserRoleID"),
            RoleId = row.SafeLong("RoleID"),
            LoginId = row.SafeLong("LoginID"),
            IsDefault = row.SafeBool("IsDefault"),
            RoleName = row.SafeString("RoleName"),
            LoginName = row.SafeString("LoginName")
        }).ToList();'''

new = '''        return table.AsEnumerable()
            .Where(row => row.SafeBool("IsAssigned"))
            .Select(row => new UserRoleAssignmentDto
        {
            UserRoleId = row.SafeLong("UserRoleID"),
            RoleId = row.SafeLong("RoleID"),
            LoginId = row.SafeLong("LoginID"),
            IsDefault = row.SafeBool("IsDefault"),
            RoleName = row.SafeString("RoleName"),
            LoginName = row.SafeString("LoginName")
        }).ToList();'''

content = content.replace(old, new)

with open(file, 'w', encoding='utf-8') as f:
    f.write(content)
