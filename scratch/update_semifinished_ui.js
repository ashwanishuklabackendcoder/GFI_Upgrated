const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Add Amount field in layout form (using the unique match for batch unit autocomplete)
content = content.replace(
    /(\s*<MudItem xs="12" md="4">\s*<MudAutocomplete (?:Clearable="true" )?MaxItems="1000" T="UnitDto" Label="Unit" Value="@\(_units\.FirstOrDefault\(x => x\.UnitId == _batchModel\.UnitId\)\)"[\s\S]*?<\/MudItem>)/,
    `$1\n                        <MudItem xs="12" md="4">\n                            <MudNumericField @bind-Value="_batchModel.Amount" Label="Amount (SRD)" Variant="Variant.Outlined" Min="0" Adornment="Adornment.Start" AdornmentText="SRD" Format="N2" />\n                        </MudItem>`
);

// 2. Add Amount header in table
content = content.replace(
    /<MudTh>Warehouse<\/MudTh>\s*<MudTh>Expiry<\/MudTh>/,
    `<MudTh>Warehouse</MudTh>\n                        <MudTh>Expiry</MudTh>\n                        <MudTh>Amount (SRD)</MudTh>`
);

// 3. Add Amount cell in table row template
content = content.replace(
    /<MudTd DataLabel="Warehouse">@context.WarehouseName<\/MudTd>\s*<MudTd DataLabel="Expiry">@context.ExpiryDate\?\.ToShortDateString\(\)<\/MudTd>/,
    `<MudTd DataLabel="Warehouse">@context.WarehouseName</MudTd>\n                        <MudTd DataLabel="Expiry">@context.ExpiryDate?.ToShortDateString()</MudTd>\n                        <MudTd DataLabel="Amount">@context.Amount.ToString("N2") SRD</MudTd>`
);

// 4. Update EditBatch method (matching up to the closing brace of the method)
content = content.replace(
    /private void EditBatch\(SemiFinishedProductBatchDto batch\)[\s\S]*?};\s*}/,
    `private void EditBatch(SemiFinishedProductBatchDto batch)
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
    }`
);

// 5. Update ResetBatchForm method (matching up to the closing brace of the method)
content = content.replace(
    /private void ResetBatchForm\(\)[\s\S]*?};\s*}/,
    `private void ResetBatchForm()
    {
        _batchModel = new SemiFinishedProductBatchDto
        {
            UnitId = _model.PurchaseUnit > 0 ? (int)_model.PurchaseUnit : (int)(_units.FirstOrDefault()?.UnitId ?? 0),
            Amount = 0
        };
    }`
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated SemiFinishedProductEdit.razor!");
