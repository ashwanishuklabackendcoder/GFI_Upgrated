const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Admin\\Pages\\StaffEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Add For attribute to Salutation MudAutocomplete
content = content.replace(
    'ToStringFunc="@(x => x?.Text ?? "")"',
    'ToStringFunc="@(x => x?.Text ?? "")"\n                                 For="@(() => _editor.StaffSalutation)"'
);

// 2. Add For attribute to Gender MudAutocomplete
content = content.replace(
    'ToStringFunc="@(x => x?.Text ?? "")"', // wait, there is a second occurrence of ToStringFunc="@(x => x?.Text ?? "")" for Gender!
    // Since replace only replaces the first occurrence, let's do a targeted replace for gender.
    // Let's find: Value="@(_genders.FirstOrDefault(x => x.Value.ToString() == _editor.Gender.ToString()))" and replace within that block.
    'ToStringFunc="@(x => x?.Text ?? "")"' // This would match the first one. Let's write a cleaner replace logic.
);

// Let's do it precisely using split/replace
content = fs.readFileSync(filePath, 'utf8');
content = content.replace(
    /Value="@\(_salutations\.FirstOrDefault\(x => x\.Value\.ToString\(\) == _editor\.StaffSalutation\)\)"\s*ValueChanged="@\(\(LookupItemDto val\) => \{ _editor\.StaffSalutation = val\?\.Value\.ToString\(\) \?\? string\.Empty; \}\)"\s*SearchFunc="@SearchSalutations"\s*ToStringFunc="@\(x => x\?\.Text \?\? ""\)"/,
    `Value="@(_salutations.FirstOrDefault(x => x.Value.ToString() == _editor.StaffSalutation))"
                                 ValueChanged="@((LookupItemDto val) => { _editor.StaffSalutation = val?.Value.ToString() ?? string.Empty; })"
                                 SearchFunc="@SearchSalutations"
                                 ToStringFunc="@(x => x?.Text ?? "")"
                                 For="@(() => _editor.StaffSalutation)"`
);

content = content.replace(
    /Value="@\(_genders\.FirstOrDefault\(x => x\.Value\.ToString\(\) == _editor\.Gender\.ToString\(\)\)\)"\s*ValueChanged="@\(\(LookupItemDto val\) => \{ _editor\.Gender = long\.TryParse\(val\?\.Value\.ToString\(\), out var g\) \? g : 0; \}\)"\s*SearchFunc="@SearchGenders"\s*ToStringFunc="@\(x => x\?\.Text \?\? ""\)"/,
    `Value="@(_genders.FirstOrDefault(x => x.Value.ToString() == _editor.Gender.ToString()))"
                                 ValueChanged="@((LookupItemDto val) => { _editor.Gender = long.TryParse(val?.Value.ToString(), out var g) ? g : 0; })"
                                 SearchFunc="@SearchGenders"
                                 ToStringFunc="@(x => x?.Text ?? "")"
                                 For="@(() => _editor.Gender)"`
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated StaffEdit.razor!");
