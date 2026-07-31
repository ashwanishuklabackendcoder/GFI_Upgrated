const fs = require('fs');

const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Admin\\Pages\\Staff.razor';
let content = fs.readFileSync(filePath, 'utf8');

// 1. Add services
content = content.replace(
    `@inject IJSRuntime JS`,
    `@inject IJSRuntime JS
@inject IDialogService DialogService
@inject ISnackbar Snackbar`
);

// 2. Modify Action column template
content = content.replace(
    `<MudTd DataLabel="Action">
                <MudStack Row="true" Spacing="1">
                    <MudIconButton Icon="@Icons.Material.Filled.Edit" Color="Color.Primary" Size="Size.Small" OnClick="@(() => EditStaffAsync(context.StaffId))" />
                    <MudIconButton Icon="@Icons.Material.Filled.Delete" Color="Color.Error" Size="Size.Small" OnClick="@(() => DeleteStaffAsync(context.StaffId))" />
                </MudStack>
            </MudTd>`,
    `<MudTd DataLabel="Action">
                <MudStack Row="true" Spacing="1">
                    <MudIconButton Icon="@Icons.Material.Filled.Email" Color="Color.Info" Size="Size.Small" OnClick="@(() => OpenSendMailDialogAsync(context))" Title="Send Mail" />
                    <MudIconButton Icon="@Icons.Material.Filled.Key" Color="Color.Success" Size="Size.Small" OnClick="@(() => ResendLoginCredentialsAsync(context))" Title="Resend Login Credentials" />
                    <MudIconButton Icon="@Icons.Material.Filled.History" Color="Color.Warning" Size="Size.Small" OnClick="@(() => OpenMailHistoryDialogAsync(context))" Title="View Mail Status" />
                    <MudIconButton Icon="@Icons.Material.Filled.Edit" Color="Color.Primary" Size="Size.Small" OnClick="@(() => EditStaffAsync(context.StaffId))" />
                    <MudIconButton Icon="@Icons.Material.Filled.Delete" Color="Color.Error" Size="Size.Small" OnClick="@(() => DeleteStaffAsync(context.StaffId))" />
                </MudStack>
            </MudTd>`
);

// 3. Add helper methods inside @code block before the final closing brace
const helperCode = `
    private async Task OpenSendMailDialogAsync(StaffDto staff)
    {
        var parameters = new DialogParameters<SendMailDialog>
        {
            { x => x.StaffId, staff.StaffId },
            { x => x.StaffName, staff.StaffName },
            { x => x.InitialEmail, !string.IsNullOrWhiteSpace(staff.EmailIDOfficial) ? staff.EmailIDOfficial : staff.EmailIDPersonal ?? "" }
        };
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = await DialogService.ShowAsync<SendMailDialog>("Send Email to Staff", parameters, options);
        await dialog.Result;
    }

    private async Task OpenMailHistoryDialogAsync(StaffDto staff)
    {
        var parameters = new DialogParameters<MailHistoryDialog>
        {
            { x => x.StaffId, staff.StaffId },
            { x => x.StaffName, staff.StaffName }
        };
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await DialogService.ShowAsync<MailHistoryDialog>("Mail Send History", parameters, options);
        await dialog.Result;
    }

    private async Task ResendLoginCredentialsAsync(StaffDto staff)
    {
        var email = !string.IsNullOrWhiteSpace(staff.EmailIDOfficial) ? staff.EmailIDOfficial : staff.EmailIDPersonal;
        if (string.IsNullOrWhiteSpace(email))
        {
            Snackbar.Add("This staff member does not have a registered email address.", Severity.Warning);
            return;
        }

        var confirmed = await JS.InvokeAsync<bool>("confirm", $"Resend login credentials to {staff.StaffName} at {email}?");
        if (!confirmed) return;

        try
        {
            var success = await ApiClient.ResendStaffLoginAsync(staff.StaffId);
            if (success)
            {
                Snackbar.Add($"Login credentials resent successfully to {email}.", Severity.Success);
            }
            else
            {
                Snackbar.Add("No active user login account exists for this staff member.", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }
`;

// Insert helperCode before the last closing brace
const lastBraceIndex = content.lastIndexOf('}');
content = content.substring(0, lastBraceIndex) + helperCode + content.substring(lastBraceIndex);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated Staff.razor with standard C# string interpolation!");
