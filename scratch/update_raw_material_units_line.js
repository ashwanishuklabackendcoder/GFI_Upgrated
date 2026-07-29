const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');
let lines = content.split('\n');

for (let i = 0; i < lines.length; i++) {
    if (lines[i].includes('Label="Base Unit"') && lines[i].includes('SearchFunc="@SearchUnits"')) {
        lines[i] = lines[i].replace('SearchFunc="@SearchUnits"', 'SearchFunc="@SearchBaseUnits"');
        console.log(`Updated line ${i + 1}: ${lines[i].trim()}`);
    }
}

fs.writeFileSync(filePath, lines.join('\n'), 'utf8');
