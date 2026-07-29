const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');
content = content.replace('SearchFunc="@SearchUnits"', 'SearchFunc="@SearchBaseUnits"');
fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated SearchFunc to SearchBaseUnits in FinishedProductEdit.razor!");
