const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Admin\\Pages\\StaffEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Replace field declaration and add the lazy EditContext property
content = content.replace(
    'private EditContext? _editContext;',
    'private EditContext _editContext => _lazyEditContext ??= new EditContext(_editor);\n    private EditContext? _lazyEditContext;'
);

// 2. Remove manual instantiations of _editContext in OnInitialized and OnInitializedAsync
content = content.replace(
    '_editContext = new EditContext(_editor);',
    ''
);
// Replace other instances of it as well
content = content.replace(
    '_editContext = new EditContext(_editor);',
    ''
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated EditContext instantiation in StaffEdit.razor!");
