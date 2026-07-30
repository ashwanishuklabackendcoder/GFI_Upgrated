const fs = require('fs');

function fixFile(filePath, isFinished = false) {
    let content = fs.readFileSync(filePath, 'utf8');

    if (isFinished) {
        const target = `    private void ResetBatchForm()
    {
        _batchModel = new FinishedProductBatchDto();
        _warehouseAutocomplete?.Clear();
    }`;
        const replacement = `    private void ResetBatchForm()
    {
        _batchModel = new FinishedProductBatchDto();
        _warehouseAutocomplete?.Clear();
        _warehouseAutocomplete?.ResetValidation();
    }`;
        content = content.replace(target, replacement);
    } else {
        // Semi-finished or Raw material
        const isSemi = filePath.includes("SemiFinishedProductEdit");
        const batchType = isSemi ? "SemiFinishedProductBatchDto" : "RawMaterialBatchDto";
        
        const target = `    private void ResetBatchForm()
    {
        _batchModel = new ${batchType}
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0)
        };
        _warehouseAutocomplete?.Clear();
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId));
    }`;
        const replacement = `    private void ResetBatchForm()
    {
        _batchModel = new ${batchType}
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0)
        };
        _warehouseAutocomplete?.Clear();
        _warehouseAutocomplete?.ResetValidation();
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId));
        _unitAutocomplete?.ResetValidation();
    }`;
        content = content.replace(target, replacement);
    }

    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Updated ResetBatchForm in ${filePath}`);
}

fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor', false);
fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor', false);
fixFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor', true);
