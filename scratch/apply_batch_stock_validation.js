const fs = require('fs');

function applyValidation(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');

    // 1. Add alert on item change when no batches found
    const targetItemChange = `row.AvailableBatches = (await ApiClient.GetAvailableBatchesAsync(item.ItemId)).ToList();`;
    const replacementItemChange = `row.AvailableBatches = (await ApiClient.GetAvailableBatchesAsync(item.ItemId)).ToList();
            if (!row.AvailableBatches.Any())
            {
                Snackbar.Add($"No stock found for ingredient '{item.ItemName}'. Please select another ingredient.", Severity.Warning);
            }`;
    content = content.replace(targetItemChange, replacementItemChange);

    // 2. Add validation on Save (SaveConsumptionAsync)
    const targetSaveValidation = `            if (row.ItemStockByBatchId == 0)
            {
                Snackbar.Add(\`Please select a lot/batch number for '\${row.ItemName}'.\`, Severity.Warning);
                return false;
            }`;
    const replacementSaveValidation = `            if (!row.AvailableBatches.Any())
            {
                Snackbar.Add(\`No stock found for ingredient '\${row.ItemName}'. Please select another ingredient or remove this row.\`, Severity.Warning);
                return false;
            }
            if (row.ItemStockByBatchId == 0)
            {
                Snackbar.Add(\`Please select a lot/batch number for '\${row.ItemName}'.\`, Severity.Warning);
                return false;
            }`;
    content = content.replace(targetSaveValidation, replacementSaveValidation);

    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Successfully added batch validation in ${filePath}`);
}

applyValidation('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\PreProcessingItems.razor');
applyValidation('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\ProductionItems.razor');
