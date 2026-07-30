const fs = require('fs');

function cleanFile(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');

    // 1. Replace double await
    content = content.replace(/await await ResetBatchForm\(\);/g, 'await ResetBatchForm();');

    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Cleaned double await in ${filePath}`);
}

cleanFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor');
cleanFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor');
cleanFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor');
