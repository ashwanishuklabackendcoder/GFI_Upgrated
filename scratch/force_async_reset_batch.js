const fs = require('fs');

function applyFix(filePath, batchType) {
    let content = fs.readFileSync(filePath, 'utf8');

    // 1. Force the method signature to be private async Task
    content = content.replace(
        `private void ResetBatchForm()`,
        `private async Task ResetBatchForm()`
    );

    // 2. Insert Task.Delay(50) before the reset validation calls
    if (batchType === "FinishedProductBatchDto") {
        content = content.replace(
            `        _warehouseAutocomplete?.Clear();
        _warehouseAutocomplete?.ResetValidation();`,
            `        _warehouseAutocomplete?.Clear();
        await Task.Delay(50);
        _warehouseAutocomplete?.ResetValidation();`
        );
    } else {
        content = content.replace(
            `        _warehouseAutocomplete?.Clear();
        _warehouseAutocomplete?.ResetValidation();
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId));
        _unitAutocomplete?.ResetValidation();`,
            `        _warehouseAutocomplete?.Clear();
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId));
        await Task.Delay(50);
        _warehouseAutocomplete?.ResetValidation();
        _unitAutocomplete?.ResetValidation();`
        );
    }

    // 3. Make sure all invocations of ResetBatchForm inside async methods use await
    // In SaveBatch:
    content = content.replace(
        /([ \t]*)ResetBatchForm\(\);/g,
        '$1await ResetBatchForm();'
    );

    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Successfully forced async ResetBatchForm in ${filePath}`);
}

applyFix('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor', 'RawMaterialBatchDto');
applyFix('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor', 'SemiFinishedProductBatchDto');
applyFix('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor', 'FinishedProductBatchDto');
