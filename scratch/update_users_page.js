const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Admin\\Pages\\Users.razor';

let content = fs.readFileSync(filePath, 'utf8');

// Use a regular expression that is immune to Windows CRLF vs Unix LF differences
const regex = /try\s*\{\s*_editor\.CreatedBy\s*=\s*SessionState\.CurrentUserAuditName\s*\?\?\s*"System";\s*_editor\.UpdatedBy\s*=\s*SessionState\.CurrentUserAuditName\s*\?\?\s*"System";\s*await\s*ApiClient\.SaveUserAsync\(_editor\);\s*Snackbar\.Add\("User saved successfully\.",\s*Severity\.Success\);\s*await\s*LoadAsync\(\);\s*NewUser\(\);\s*\}\s*catch\s*\(Exception\s*ex\)\s*\{\s*Snackbar\.Add\(\$"Error\s*saving\s*user:\s*\{ex\.Message\}",\s*Severity\.Error\);\s*\}/;

const replacementStr = `try
        {
            _editor.CreatedBy = SessionState.CurrentUserAuditName ?? "System";
            _editor.UpdatedBy = SessionState.CurrentUserAuditName ?? "System";
            var res = await ApiClient.SaveUserAsync(_editor);
            if (res == -1)
            {
                Snackbar.Add("This username is already registered. Please try with another username.", Severity.Error);
            }
            else if (res == -2)
            {
                Snackbar.Add("This staff member is already assigned to a login account.", Severity.Error);
            }
            else if (res > 0)
            {
                Snackbar.Add("User saved successfully.", Severity.Success);
                await LoadAsync();
                NewUser();
            }
            else
            {
                Snackbar.Add("Error saving user.", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error saving user: {ex.Message}", Severity.Error);
        }`;

content = content.replace(regex, replacementStr);
fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated Users.razor with regex line endings check!");
