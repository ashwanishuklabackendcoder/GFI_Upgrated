const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Admin\\Pages\\StaffEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Change EditForm declaration
content = content.replace(
    '<EditForm Model="_editor" OnValidSubmit="SaveAsync">',
    '<EditForm EditContext="_editContext" OnValidSubmit="SaveAsync">'
);

// 2. Change Salutation ValueChanged
content = content.replace(
    'ValueChanged="@((LookupItemDto val) => { _editor.StaffSalutation = val?.Value.ToString() ?? string.Empty; })"',
    'ValueChanged="@(async (LookupItemDto val) => await OnSalutationChanged(val))"'
);

// 3. Change Gender ValueChanged
content = content.replace(
    'ValueChanged="@((LookupItemDto val) => { _editor.Gender = long.TryParse(val?.Value.ToString(), out var g) ? g : 0; })"',
    'ValueChanged="@(async (LookupItemDto val) => await OnGenderChanged(val))"'
);

// 4. Inject private field and event handlers in @code block
const codeToInject = `    private EditContext? _editContext;

    private async Task OnSalutationChanged(LookupItemDto val)
    {
        _editor.StaffSalutation = val?.Value.ToString() ?? string.Empty;
        if (_editContext != null)
        {
            _editContext.NotifyFieldChanged(_editContext.Field(nameof(_editor.StaffSalutation)));
        }
        await Task.CompletedTask;
    }

    private async Task OnGenderChanged(LookupItemDto val)
    {
        _editor.Gender = val?.Value ?? 0;
        if (_editContext != null)
        {
            _editContext.NotifyFieldChanged(_editContext.Field(nameof(_editor.Gender)));
        }
        await Task.CompletedTask;
    }

    protected override void OnInitialized()`;

content = content.replace('protected override void OnInitialized()', codeToInject);

// 5. Initialize/Recreate EditContext inside OnInitialized and after loading existing staff
content = content.replace(
    `    protected override void OnInitialized()
    {
        if (!SessionState.IsLoggedIn)
        {
            Navigation.NavigateTo("/login");
        }
    }`,
    `    protected override void OnInitialized()
    {
        if (!SessionState.IsLoggedIn)
        {
            Navigation.NavigateTo("/login");
        }
        _editContext = new EditContext(_editor);
    }`
);

content = content.replace(
    `                _editor.Photo = selected.Photo;
                _editor.HasLogin = selected.HasLogin;
                _selectedPhotoName = selected.Photo;
            }`,
    `                _editor.Photo = selected.Photo;
                _editor.HasLogin = selected.HasLogin;
                _selectedPhotoName = selected.Photo;
                _editContext = new EditContext(_editor);
            }`
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated StaffEdit.razor validation behavior!");
