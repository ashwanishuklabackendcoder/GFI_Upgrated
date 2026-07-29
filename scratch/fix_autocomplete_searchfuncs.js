const fs = require('fs');

function fixFile(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');
    let lines = content.split('\n');
    let updated = false;

    for (let i = 0; i < lines.length; i++) {
        let line = lines[i];
        if (line.includes('Label="Base Unit"') && line.includes('SearchFunc="@SearchUnits"')) {
            lines[i] = line.replace('SearchFunc="@SearchUnits"', 'SearchFunc="@SearchBaseUnits"');
            console.log(`Updated Base Unit line in ${filePath}: ${lines[i].trim()}`);
            updated = true;
        }
        else if (line.includes('Label="Unit"') && line.includes('SearchFunc="@SearchUnits"')) {
            lines[i] = line.replace('SearchFunc="@SearchUnits"', 'SearchFunc="@SearchBatchUnits"');
            console.log(`Updated Batch Unit line in ${filePath}: ${lines[i].trim()}`);
            updated = true;
        }
        else if (line.includes('Label="Yield Unit"') && line.includes('SearchFunc="@SearchUnits"')) {
            lines[i] = line.replace('SearchFunc="@SearchUnits"', 'SearchFunc="@SearchBaseUnits"');
            console.log(`Updated Yield Unit line in ${filePath}: ${lines[i].trim()}`);
            updated = true;
        }
    }

    if (updated) {
        fs.writeFileSync(filePath, lines.join('\n'), 'utf8');
    }
}

fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor');
fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor');
fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor');
fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\BomEdit.razor');
