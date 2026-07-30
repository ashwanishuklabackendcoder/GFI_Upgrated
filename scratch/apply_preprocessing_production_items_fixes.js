const fs = require('fs');

function fixFile(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');

    // 1. Filter dropdown to type 2 & 3 only
    content = content.replace(
        `_allRawMaterials = (await ApiClient.GetItemsForBomLookupAsync(null)).ToList();`,
        `_allRawMaterials = (await ApiClient.GetItemsForBomLookupAsync(null)).Where(x => x.ItemTypeId == 2 || x.ItemTypeId == 3).ToList();`
    );

    // 2. Format RecipeQty using "0.####"
    content = content.replace(
        `@context.RecipeQty.ToString("N4")`,
        `@context.RecipeQty.ToString("0.####")`
    );

    // 3. Format Qty Used input field using "0.####"
    content = content.replace(
        `Format="N4"`,
        `Format="0.####"`
    );

    // 4. Format header alert properties using "0.####"
    content = content.replace(
        `.ToString("N4")`,
        `.ToString("0.####")`
    );
    content = content.replace(
        `.ToString("N4")`,
        `.ToString("0.####")`
    );

    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Updated ${filePath} successfully!`);
}

fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\PreProcessingItems.razor');
fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\ProductionItems.razor');
