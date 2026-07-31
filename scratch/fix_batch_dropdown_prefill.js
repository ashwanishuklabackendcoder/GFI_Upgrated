const fs = require('fs');

function fixRepo(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');
    content = content.replace(
        'UnitId = row.SafeInt("UnitId"),',
        'UnitId = row.Table.Columns.Contains("Unit") ? row.SafeInt("Unit") : row.SafeInt("UnitId"),'
    );
    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Updated repo file: ${filePath}`);
}

function fixUI(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');

    // Add ref attributes
    content = content.replace(
        'Label="Warehouse" Value="@(_warehouses.FirstOrDefault(x => x.WarehouseId == _batchModel.WarehouseId))"',
        '@ref="_warehouseAutocomplete" Label="Warehouse" Value="@(_warehouses.FirstOrDefault(x => x.WarehouseId == _batchModel.WarehouseId))"'
    );
    content = content.replace(
        'Label="Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId))"',
        '@ref="_unitAutocomplete" Label="Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId))"'
    );

    // Add private variables
    content = content.replace(
        'private RawMaterialBatchDto _batchModel = new();',
        `private RawMaterialBatchDto _batchModel = new();
    private MudAutocomplete<WarehouseLookupDto> _warehouseAutocomplete;
    private MudAutocomplete<UnitDto> _unitAutocomplete;`
    );

    // Update EditBatch
    const targetEdit = `    private void EditBatch(RawMaterialBatchDto batch)
    {
        _batchModel = new RawMaterialBatchDto
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
    const replacementEdit = `    private void EditBatch(RawMaterialBatchDto batch)
    {
        _batchModel = new RawMaterialBatchDto
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

    // Update ResetBatchForm
    const targetReset = `    private void ResetBatchForm()
    {
        _batchModel = new RawMaterialBatchDto
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0)
        };
    }`;
    const replacementReset = `    private void ResetBatchForm()
    {
        _batchModel = new RawMaterialBatchDto
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0)
        };
        _warehouseAutocomplete?.Clear();
        _unitAutocomplete?.SelectOption(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId));
    }`;
    content = content.replace(targetReset, replacementReset);

    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Updated UI file: ${filePath}`);
}

// Fix Repo files
fixRepo('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.Data\\Store\\RawMaterialRepository.cs');
fixRepo('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.Data\\Store\\SemiFinishedProductRepository.cs');
fixRepo('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.Data\\Store\\FinishedProductRepository.cs');

// Fix UI files
fixUI('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor');
fixUI('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor');
fixUI('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor');
