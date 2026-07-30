const fs = require('fs');

function fixBomFile() {
    const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\BomEdit.razor';
    let content = fs.readFileSync(filePath, 'utf8');
    let lines = content.split('\n');
    let updated = false;

    for (let i = 0; i < lines.length; i++) {
        // 1. Yield Unit search function replacement
        if (lines[i].includes('Label="Yield Unit"')) {
            for (let j = 1; j <= 5; j++) {
                if (lines[i + j] && lines[i + j].includes('SearchFunc="@SearchUnits"')) {
                    lines[i + j] = lines[i + j].replace('SearchFunc="@SearchUnits"', 'SearchFunc="@SearchBaseUnits"');
                    console.log(`Updated Yield Unit in BomEdit.razor at line ${i + j + 1}`);
                    updated = true;
                    break;
                }
            }
        }
        // 2. Component Unit search function replacement
        if (lines[i].includes('Value="@(_units.FirstOrDefault(x => x.UnitId == itemContext.UnitId))"')) {
            for (let j = 1; j <= 5; j++) {
                if (lines[i + j] && lines[i + j].includes('SearchFunc="@SearchUnits"')) {
                    lines[i + j] = lines[i + j].replace('SearchFunc="@SearchUnits"', 'SearchFunc="@((string val, CancellationToken token) => SearchUnitsForProduct(itemContext.ItemID, val, token))"');
                    console.log(`Updated Component Unit in BomEdit.razor at line ${i + j + 1}`);
                    updated = true;
                    break;
                }
            }
        }
    }

    if (updated) {
        fs.writeFileSync(filePath, lines.join('\n'), 'utf8');
    }
}

fixBomFile();
