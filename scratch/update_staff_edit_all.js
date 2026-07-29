const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Admin\\Pages\\StaffEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Fix the DOB DatePicker label
content = content.replace(
    'Label="@(LocalizationState.Text( (DD/MM/YYYY)"Admin.Staff.Edit.Dob", "D.O.B") + " (DD/MM/YYYY)")"',
    'Label="@(LocalizationState.Text("Admin.Staff.Edit.Dob", "D.O.B") + " (DD/MM/YYYY)")"'
);

// 2. Change SaveAsync returned id logic to show duplicate error
content = content.replace(
    /var id = await ApiClient\.SaveStaffAsync\(_editor\);\s*if \(id > 0\)\s*\{\s*Navigation\.NavigateTo\("\/admin\/staff"\);\s*return;\s*\}/,
    `var id = await ApiClient.SaveStaffAsync(_editor);
            if (id > 0)
            {
                Navigation.NavigateTo("/admin/staff");
                return;
            }
            else if (id == -1)
            {
                _message = "This name is already registered. Please try with another name.";
            }`
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully ran all StaffEdit.razor updates via JS!");
