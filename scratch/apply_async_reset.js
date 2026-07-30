const fs = require('fs');

function applyAsyncReset(filePath, isFinished = false) {
    let content = fs.readFileSync(filePath, 'utf8');

    if (isFinished) {
        // FinishedProductEdit
        content = content.replace(
            `    private void ResetBatchForm()
    {
        _batchModel = new FinishedProductBatchDto();
        _warehouseAutocomplete?.Clear();
        _warehouseAutocomplete?.ResetValidation();
    }`,
            `    private async Task ResetBatchForm()
    {
        _batchModel = new FinishedProductBatchDto();
        _warehouseAutocomplete?.Clear();
        await Task.Delay(50);
        _warehouseAutocomplete?.ResetValidation();
    }`
        );
        content = content.replace(
            `            ResetBatchForm();`,
            `            await ResetBatchForm();`
        );
    } else {
        // RawMaterialEdit or SemiFinishedProductEdit
        const isSemi = filePath.includes("SemiFinishedProductEdit");
        const batchType = isSemi ? "SemiFinishedProductBatchDto" : "RawMaterialBatchDto";

        content = content.replace(
            `    private void ResetBatchForm()
    {
        _batchModel = new ${batchType}
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0)
        };
        _warehouseAutocomplete?.Clear();
        _warehouseAutocomplete?.ResetValidation();
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId));
        _unitAutocomplete?.ResetValidation();
    }`,
            `    private async Task ResetBatchForm()
    {
        _batchModel = new ${batchType}
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0)
        };
        _warehouseAutocomplete?.Clear();
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId));
        await Task.Delay(50);
        _warehouseAutocomplete?.ResetValidation();
        _unitAutocomplete?.ResetValidation();
    }`
        );
        content = content.replace(
            `            ResetBatchForm();`,
            `            await ResetBatchForm();`
        );
    }

    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Successfully updated async ResetBatchForm in ${filePath}`);
}

applyAsyncReset('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor', false);
applyAsyncReset('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor', false);
applyAsyncReset('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor', true);
