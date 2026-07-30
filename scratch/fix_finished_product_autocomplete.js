const fs = require('fs');

function fixFinishedProductFile() {
    const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor';
    let content = fs.readFileSync(filePath, 'utf8');
    let lines = content.split('\n');
    let updated = false;

    for (let i = 0; i < lines.length; i++) {
        if (lines[i].includes('Label="Base Unit"')) {
            for (let j = 1; j <= 5; j++) {
                if (lines[i + j] && lines[i + j].includes('SearchFunc="@SearchUnits"')) {
                    lines[i + j] = lines[i + j].replace('SearchFunc="@SearchUnits"', 'SearchFunc="@SearchBaseUnits"');
                    console.log(`Updated Base Unit SearchFunc in FinishedProductEdit.razor at line ${i + j + 1}`);
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

fixFinishedProductFile();
