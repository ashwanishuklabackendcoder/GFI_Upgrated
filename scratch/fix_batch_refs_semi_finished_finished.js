const fs = require('fs');

function fixSemiFinished() {
    const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor';
    let content = fs.readFileSync(filePath, 'utf8');

    // 1. Declare the private refs correctly for SemiFinishedProductBatchDto
    content = content.replace(
        'private SemiFinishedProductBatchDto _batchModel = new();',
        `private SemiFinishedProductBatchDto _batchModel = new();
    private MudAutocomplete<WarehouseLookupDto> _warehouseAutocomplete;
    private MudAutocomplete<UnitDto> _unitAutocomplete;`
    );

    // 2. Update EditBatch
    const targetEdit = `    private void EditBatch(SemiFinishedProductBatchDto batch)
    {
        _batchModel = new SemiFinishedProductBatchDto
        {
            ItemStockByBatchId = batch.ItemStockByBatchId,
            ItemId = batch.ItemId,
            WarehouseId = batch.WarehouseId,
            Quantity = batch.Quantity,
            UnitId = batch.UnitId,
            BatchNo = batch.BatchNo,
            Amount = batch.Amount,
            ExpiryDate = batch.ExpiryDate
        };
    }`;
    const replacementEdit = `    private void EditBatch(SemiFinishedProductBatchDto batch)
    {
        _batchModel = new SemiFinishedProductBatchDto
        {
            ItemStockByBatchId = batch.ItemStockByBatchId,
            ItemId = batch.ItemId,
            WarehouseId = batch.WarehouseId,
            Quantity = batch.Quantity,
            UnitId = batch.UnitId,
            BatchNo = batch.BatchNo,
            Amount = batch.Amount,
            ExpiryDate = batch.ExpiryDate
        };
        _warehouseAutocomplete?.SelectOption(_warehouses.FirstOrDefault(x => x.WarehouseId == batch.WarehouseId));
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == batch.UnitId));
    }`;
    content = content.replace(targetEdit, replacementEdit);

    // 3. Update ResetBatchForm
    const targetReset = `    private void ResetBatchForm()
    {
        _batchModel = new SemiFinishedProductBatchDto
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0)
        };
    }`;
    const replacementReset = `    private void ResetBatchForm()
    {
        _batchModel = new SemiFinishedProductBatchDto
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0)
        };
        _warehouseAutocomplete?.Clear();
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId));
    }`;
    content = content.replace(targetReset, replacementReset);

    fs.writeFileSync(filePath, content, 'utf8');
    console.log("Updated SemiFinishedProductEdit.razor successfully!");
}

function fixFinished() {
    const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor';
    let content = fs.readFileSync(filePath, 'utf8');

    // Restore original file state first to get rid of unitAutocomplete references if any
    content = content.replace(/_unitAutocomplete\?\..*;/g, '');
    content = content.replace('private MudAutocomplete<UnitDto> _unitAutocomplete;', '');

    // Add ref only for warehouse
    content = content.replace(
        'private FinishedProductBatchDto _batchModel = new();',
        `private FinishedProductBatchDto _batchModel = new();
    private MudAutocomplete<WarehouseLookupDto> _warehouseAutocomplete;`
    );

    // Update EditBatch for Finished
    const targetEdit = `    private void EditBatch(FinishedProductBatchDto batch)
    {
        _batchModel = new FinishedProductBatchDto
        {
            ItemStockByBatchId = batch.ItemStockByBatchId,
            ItemId = batch.ItemId,
            WarehouseId = batch.WarehouseId,
            Quantity = batch.Quantity,
            BatchNo = batch.BatchNo,
            ExpiryDate = batch.ExpiryDate
        };
    }`;
    const replacementEdit = `    private void EditBatch(FinishedProductBatchDto batch)
    {
        _batchModel = new FinishedProductBatchDto
        {
            ItemStockByBatchId = batch.ItemStockByBatchId,
            ItemId = batch.ItemId,
            WarehouseId = batch.WarehouseId,
            Quantity = batch.Quantity,
            BatchNo = batch.BatchNo,
            ExpiryDate = batch.ExpiryDate
        };
        _warehouseAutocomplete?.SelectOption(_warehouses.FirstOrDefault(x => x.WarehouseId == batch.WarehouseId));
    }`;
    content = content.replace(targetEdit, replacementEdit);

    // Update ResetBatchForm for Finished
    const targetReset = `    private void ResetBatchForm()
    {
        _batchModel = new FinishedProductBatchDto();
    }`;
    const replacementReset = `    private void ResetBatchForm()
    {
        _batchModel = new FinishedProductBatchDto();
        _warehouseAutocomplete?.Clear();
    }`;
    content = content.replace(targetReset, replacementReset);

    fs.writeFileSync(filePath, content, 'utf8');
    console.log("Updated FinishedProductEdit.razor successfully!");
}

fixSemiFinished();
fixFinished();
