const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Admin\\Pages\\StaffEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// Revert previous autocompletes to remove inline For attributes and add MudValidationMessage below them
content = content.replace(
    /For="@\(\(\) => _editor\.StaffSalutation\)"\s*ResetValueOnEmptyText="true"/,
    `ResetValueOnEmptyText="true"`
);

content = content.replace(
    /For="@\(\(\) => _editor\.Gender\)"\s*ResetValueOnEmptyText="true"/,
    `ResetValueOnEmptyText="true"`
);

// Add the MudValidationMessage tags
content = content.replace(
    /SearchFunc="@SearchSalutations"\s*ToStringFunc="@\(x => x\?\.Text \?\? ""\)"\s*ResetValueOnEmptyText="true" CoerceText="true" CoerceValue="true" \/>/,
    `SearchFunc="@SearchSalutations"
                                 ToStringFunc="@(x => x?.Text ?? "")"
                                 ResetValueOnEmptyText="true" CoerceText="true" CoerceValue="true" />
                <MudValidationMessage For="@(() => _editor.StaffSalutation)" />`
);

content = content.replace(
    /SearchFunc="@SearchGenders"\s*ToStringFunc="@\(x => x\?\.Text \?\? ""\)"\s*ResetValueOnEmptyText="true" CoerceText="true" CoerceValue="true" \/>/,
    `SearchFunc="@SearchGenders"
                                 ToStringFunc="@(x => x?.Text ?? "")"
                                 ResetValueOnEmptyText="true" CoerceText="true" CoerceValue="true" />
                <MudValidationMessage For="@(() => _editor.Gender)" />`
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated StaffEdit.razor with MudValidationMessage elements!");
