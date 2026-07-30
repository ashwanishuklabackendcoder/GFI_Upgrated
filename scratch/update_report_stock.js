const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\ReportItemStock.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Replace Unit Autocomplete with MasterItemType MudSelect
content = content.replace(
    /<MudItem xs="12" md="3">\s*<MudAutocomplete Clearable="true"    MaxItems="1000" T="UnitDto" Label="Unit"[\s\S]*?<\/MudItem>/,
    `<MudItem xs="12" md="3">
            <MudSelect T="int?" Label="Item Type" @bind-Value="_selectedItemTypeId" Variant="Variant.Outlined" Clearable="true">
                <MudSelectItem T="int?" Value="@((int)MasterItemType.RawMaterial)">Raw Material</MudSelectItem>
                <MudSelectItem T="int?" Value="@((int)MasterItemType.SemiFinished)">Semi-Finished Product</MudSelectItem>
                <MudSelectItem T="int?" Value="@((int)MasterItemType.FinishedProduct)">Finished Product</MudSelectItem>
            </MudSelect>
        </MudItem>`
);

// 2. Add Stock Value column header
content = content.replace(
    /<MudTh Style="text-align: right;">Final Stock<\/MudTh>/,
    `<MudTh Style="text-align: right;">Final Stock</MudTh>\n            <MudTh Style="text-align: right;">Stock Value</MudTh>`
);

// 3. Add Stock Value cell and format quantities in RowTemplate
content = content.replace(
    /<MudTd DataLabel="Opening Qty" Style="text-align: right;">@context.OpeningQuantity.ToString\("N2"\)<\/MudTd>\s*<MudTd DataLabel="Purchased Qty" Style="text-align: right;">@context.PurchasedQuantity.ToString\("N2"\)<\/MudTd>\s*<MudTd DataLabel="Issued Qty" Style="text-align: right;">@context.IssuedQuantity.ToString\("N2"\)<\/MudTd>\s*<MudTd DataLabel="Removed Qty" Style="text-align: right;">@context.RemovedQuantity.ToString\("N2"\)<\/MudTd>\s*<MudTd DataLabel="Final Stock" Style="text-align: right; font-weight: bold;">@context.FinalStock.ToString\("N2"\)<\/MudTd>/,
    `<MudTd DataLabel="Opening Qty" Style="text-align: right;">@FormatQty(context.OpeningQuantity, context.UnitId)</MudTd>
            <MudTd DataLabel="Purchased Qty" Style="text-align: right;">@FormatQty(context.PurchasedQuantity, context.UnitId)</MudTd>
            <MudTd DataLabel="Issued Qty" Style="text-align: right;">@FormatQty(context.IssuedQuantity, context.UnitId)</MudTd>
            <MudTd DataLabel="Removed Qty" Style="text-align: right;">@FormatQty(context.RemovedQuantity, context.UnitId)</MudTd>
            <MudTd DataLabel="Final Stock" Style="text-align: right; font-weight: bold;">@FormatQty(context.FinalStock, context.UnitId)</MudTd>
            <MudTd DataLabel="Stock Value" Style="text-align: right; font-weight: bold;">@FormatAmount(context.TotalValue) SRD</MudTd>`
);

// 4. Update the filter label in printable text
content = content.replace(
    /Unit: @\(_units\.FirstOrDefault\(u => u\.UnitId == _selectedUnitId\)\?\.UnitName \?\? "All Units"\)/,
    `Item Type: @(_selectedItemTypeId == 3 ? "Raw Material" : _selectedItemTypeId == 2 ? "Semi-Finished" : _selectedItemTypeId == 1 ? "Finished Product" : "All Types")`
);

// 5. Replace _selectedUnitId property with _selectedItemTypeId
content = content.replace(
    /private long\? _selectedUnitId;/,
    `private int? _selectedItemTypeId;`
);

// 6. Update LoadServerDataAsync request
content = content.replace(
    /UnitId = _selectedUnitId,/,
    `ItemTypeId = _selectedItemTypeId,`
);

// 7. Update GetAllDataAsync request
content = content.replace(
    /UnitId = _selectedUnitId,/,
    `ItemTypeId = _selectedItemTypeId,`
);

// 8. Add helper format methods
content = content.replace(
    /private MudTable<ItemStockReportDto>\? _table;/,
    `private string FormatQty(double qty, long unitId)
    {
        var unitName = _units.FirstOrDefault(u => u.UnitId == unitId)?.UnitName ?? "";
        string formattedVal = (qty % 1 == 0) ? qty.ToString("0") : qty.ToString("0.##");
        return string.IsNullOrWhiteSpace(unitName) ? formattedVal : \`\${formattedVal} \${unitName}\`;
    }

    private string FormatAmount(double val)
    {
        return (val % 1 == 0) ? val.ToString("0") : val.ToString("0.##");
    }

    private MudTable<ItemStockReportDto>? _table;`
);

// 9. Update ExportToCopyAsync string building
content = content.replace(
    /sb\.AppendLine\("StockID\\tItem Name\\tWarehouse\\tOpening Qty\\tPurchased Qty\\tIssued Qty\\tRemoved Qty\\tFinal Stock"\);\s*foreach\s*\(var\s+item\s+in\s+data\)\s*\{\s*sb\.AppendLine\(\$"{item\.StockID}\\t{item\.ItemName}\\t{item\.WarehouseName}\\t{item\.OpeningQuantity}\\t{item\.PurchasedQuantity}\\t{item\.IssuedQuantity}\\t{item\.RemovedQuantity}\\t{item\.FinalStock}"\);\s*\}/,
    `sb.AppendLine("StockID\\tItem Name\\tWarehouse\\tOpening Qty\\tPurchased Qty\\tIssued Qty\\tRemoved Qty\\tFinal Stock\\tStock Value");
        foreach (var item in data)
        {
            sb.AppendLine($"\${item.StockID}\\t\${item.ItemName}\\t\${item.WarehouseName}\\t\${FormatQty(item.OpeningQuantity, item.UnitId)}\\t\${FormatQty(item.PurchasedQuantity, item.UnitId)}\\t\${FormatQty(item.IssuedQuantity, item.UnitId)}\\t\${FormatQty(item.RemovedQuantity, item.UnitId)}\\t\${FormatQty(item.FinalStock, item.UnitId)}\\t\${FormatAmount(item.TotalValue)} SRD");
        }`
);

// 10. Update ExportToCsvAsync string building
content = content.replace(
    /sb\.AppendLine\("StockID,Item Name,Warehouse,Opening Qty,Purchased Qty,Issued Qty,Removed Qty,Final Stock"\);\s*foreach\s*\(var\s+item\s+in\s+data\)\s*\{\s*var\s+name\s*=\s*\(item\.ItemName\s*\?\?\s*""\)\.Replace\(\\\"\\\\\"\\\",\s*\\\"\\\\\"\\\\\"\\\"\);\s*var\s+whName\s*=\s*\(item\.WarehouseName\s*\?\?\s*""\)\.Replace\(\\\"\\\\\"\\\",\s*\\\"\\\\\"\\\\\"\\\"\);\s*sb\.AppendLine\(\$"{item\.StockID},\\\\"\$name\\\\",\\\\"\$whName\\\\",{item\.OpeningQuantity},{item\.PurchasedQuantity},{item\.IssuedQuantity},{item\.RemovedQuantity},{item\.FinalStock}"\);\s*\}/,
    `sb.AppendLine("StockID,Item Name,Warehouse,Opening Qty,Purchased Qty,Issued Qty,Removed Qty,Final Stock,Stock Value");
        foreach (var item in data)
        {
            var name = (item.ItemName ?? "").Replace("\\"", "\\"\\"");
            var whName = (item.WarehouseName ?? "").Replace("\\"", "\\"\\"");
            sb.AppendLine($"\${item.StockID},\\"\${name}\\",\\"\${whName}\\",\${FormatQty(item.OpeningQuantity, item.UnitId)},\${FormatQty(item.PurchasedQuantity, item.UnitId)},\${FormatQty(item.IssuedQuantity, item.UnitId)},\${FormatQty(item.RemovedQuantity, item.UnitId)},\${FormatQty(item.FinalStock, item.UnitId)},\${FormatAmount(item.TotalValue)} SRD");
        }`
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated ReportItemStock.razor!");
