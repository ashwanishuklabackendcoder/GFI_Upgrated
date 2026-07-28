const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\ReportItemStock.razor';

let content = fs.readFileSync(filePath, 'utf8');
let lines = content.split(/\r?\n/);

for (let i = 0; i < lines.length; i++) {
    // 1. Replace Unit Autocomplete with MudSelect
    if (lines[i].includes('T="UnitDto"') && lines[i].includes('Label="Unit"')) {
        lines[i] = `            <MudSelect T="int?" Label="Item Type" @bind-Value="_selectedItemTypeId" Variant="Variant.Outlined" Clearable="true">
                <MudSelectItem T="int?" Value="@((int)MasterItemType.RawMaterial)">Raw Material</MudSelectItem>
                <MudSelectItem T="int?" Value="@((int)MasterItemType.SemiFinished)">Semi-Finished Product</MudSelectItem>
                <MudSelectItem T="int?" Value="@((int)MasterItemType.FinishedProduct)">Finished Product</MudSelectItem>
            </MudSelect>`;
    }
    
    // 2. Add Stock Value column header
    if (lines[i].includes('<MudTh Style="text-align: right;">Final Stock</MudTh>')) {
        lines[i] = `            <MudTh Style="text-align: right;">Final Stock</MudTh>
            <MudTh Style="text-align: right;">Stock Value</MudTh>`;
    }

    // 3. Replace RowTemplate cells
    if (lines[i].includes('DataLabel="Opening Qty"') && lines[i].includes('OpeningQuantity.ToString')) {
        lines[i] = `            <MudTd DataLabel="Opening Qty" Style="text-align: right;">@FormatQty(context.OpeningQuantity, context.UnitId)</MudTd>`;
    }
    if (lines[i].includes('DataLabel="Purchased Qty"') && lines[i].includes('PurchasedQuantity.ToString')) {
        lines[i] = `            <MudTd DataLabel="Purchased Qty" Style="text-align: right;">@FormatQty(context.PurchasedQuantity, context.UnitId)</MudTd>`;
    }
    if (lines[i].includes('DataLabel="Issued Qty"') && lines[i].includes('IssuedQuantity.ToString')) {
        lines[i] = `            <MudTd DataLabel="Issued Qty" Style="text-align: right;">@FormatQty(context.IssuedQuantity, context.UnitId)</MudTd>`;
    }
    if (lines[i].includes('DataLabel="Removed Qty"') && lines[i].includes('RemovedQuantity.ToString')) {
        lines[i] = `            <MudTd DataLabel="Removed Qty" Style="text-align: right;">@FormatQty(context.RemovedQuantity, context.UnitId)</MudTd>`;
    }
    if (lines[i].includes('DataLabel="Final Stock"') && lines[i].includes('FinalStock.ToString')) {
        lines[i] = `            <MudTd DataLabel="Final Stock" Style="text-align: right; font-weight: bold;">@FormatQty(context.FinalStock, context.UnitId)</MudTd>
            <MudTd DataLabel="Stock Value" Style="text-align: right; font-weight: bold;">@FormatAmount(context.TotalValue) SRD</MudTd>`;
    }

    // 4. Update the filter label in printable text
    if (lines[i].includes('Unit: @(_units.FirstOrDefault(u => u.UnitId == _selectedUnitId)')) {
        lines[i] = `            Item Type: @(_selectedItemTypeId == 3 ? "Raw Material" : _selectedItemTypeId == 2 ? "Semi-Finished" : _selectedItemTypeId == 1 ? "Finished Product" : "All Types")`;
    }

    // 5. Replace _selectedUnitId property with _selectedItemTypeId
    if (lines[i].includes('private long? _selectedUnitId;')) {
        lines[i] = `    private int? _selectedItemTypeId;`;
    }

    // 6. Update requests
    if (lines[i].includes('UnitId = _selectedUnitId,')) {
        lines[i] = `                ItemTypeId = _selectedItemTypeId,`;
    }

    // 7. Inject helper methods
    if (lines[i].includes('private MudTable<ItemStockReportDto>? _table;')) {
        lines[i] = `    private string FormatQty(double qty, long unitId)
    {
        var unitName = _units.FirstOrDefault(u => u.UnitId == unitId)?.UnitName ?? "";
        string formattedVal = (qty % 1 == 0) ? qty.ToString("0") : qty.ToString("0.##");
        return string.IsNullOrWhiteSpace(unitName) ? formattedVal : $"{formattedVal} {unitName}";
    }

    private string FormatAmount(double val)
    {
        return (val % 1 == 0) ? val.ToString("0") : val.ToString("0.##");
    }

    private MudTable<ItemStockReportDto>? _table;`;
    }
}

let newContent = lines.join('\n');

// 8. Replace Copy to Clipboard logic
const oldExportCopy = `sb.AppendLine("StockID\\tItem Name\\tWarehouse\\tOpening Qty\\tPurchased Qty\\tIssued Qty\\tRemoved Qty\\tFinal Stock");
        foreach (var item in data)
        {
            sb.AppendLine($"\\{item.StockID}\\t\\{item.ItemName}\\t\\{item.WarehouseName}\\t\\{item.OpeningQuantity}\\t\\{item.PurchasedQuantity}\\t\\{item.IssuedQuantity}\\t\\{item.RemovedQuantity}\\t\\{item.FinalStock}");
        }`;
// Since backslashes/quotes can vary, we will replace by finding:
newContent = newContent.replace(
    /sb\.AppendLine\("StockID\\tItem Name\\tWarehouse\\tOpening Qty\\tPurchased Qty\\tIssued Qty\\tRemoved Qty\\tFinal Stock"\);\s*foreach\s*\(var\s+item\s+in\s+data\)\s*\{\s*sb\.AppendLine\(\$"{item\.StockID}\\t{item\.ItemName}\\t{item\.WarehouseName}\\t{item\.OpeningQuantity}\\t{item\.PurchasedQuantity}\\t{item\.IssuedQuantity}\\t{item\.RemovedQuantity}\\t{item\.FinalStock}"\);\s*\}/,
    `sb.AppendLine("StockID\\tItem Name\\tWarehouse\\tOpening Qty\\tPurchased Qty\\tIssued Qty\\tRemoved Qty\\tFinal Stock\\tStock Value");
        foreach (var item in data)
        {
            sb.AppendLine($"\${item.StockID}\\t\${item.ItemName}\\t\${item.WarehouseName}\\t\${FormatQty(item.OpeningQuantity, item.UnitId)}\\t\${FormatQty(item.PurchasedQuantity, item.UnitId)}\\t\${FormatQty(item.IssuedQuantity, item.UnitId)}\\t\${FormatQty(item.RemovedQuantity, item.UnitId)}\\t\${FormatQty(item.FinalStock, item.UnitId)}\\t\${FormatAmount(item.TotalValue)} SRD");
        }`
);

// 9. Replace CSV Export logic
newContent = newContent.replace(
    /sb\.AppendLine\("StockID,Item Name,Warehouse,Opening Qty,Purchased Qty,Issued Qty,Removed Qty,Final Stock"\);\s*foreach\s*\(var\s+item\s+in\s+data\)\s*\{\s*var\s+name\s*=\s*\(item\.ItemName\s*\?\?\s*""\)\.Replace\(\\\"\\\\\"\\\",\s*\\\"\\\\\"\\\\\"\\\"\);\s*var\s+whName\s*=\s*\(item\.WarehouseName\s*\?\?\s*""\)\.Replace\(\\\"\\\\\"\\\",\s*\\\"\\\\\"\\\\\"\\\"\);\s*sb\.AppendLine\(\$"{item\.StockID},\\\\"\$name\\\\",\\\\"\$whName\\\\",{item\.OpeningQuantity},{item\.PurchasedQuantity},{item\.IssuedQuantity},{item\.RemovedQuantity},{item\.FinalStock}"\);\s*\}/,
    `sb.AppendLine("StockID,Item Name,Warehouse,Opening Qty,Purchased Qty,Issued Qty,Removed Qty,Final Stock,Stock Value");
        foreach (var item in data)
        {
            var name = (item.ItemName ?? "").Replace("\\"", "\\"\\"");
            var whName = (item.WarehouseName ?? "").Replace("\\"", "\\"\\"");
            sb.AppendLine($"\${item.StockID},\\"\${name}\\",\\"\${whName}\\",\${FormatQty(item.OpeningQuantity, item.UnitId)},\${FormatQty(item.PurchasedQuantity, item.UnitId)},\${FormatQty(item.IssuedQuantity, item.UnitId)},\${FormatQty(item.RemovedQuantity, item.UnitId)},\${FormatQty(item.FinalStock, item.UnitId)},\${FormatAmount(item.TotalValue)} SRD");
        }`
);

fs.writeFileSync(filePath, newContent, 'utf8');
console.log("Successfully updated ReportItemStock.razor line-by-line!");
