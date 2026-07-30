const fs = require('fs');

function injectFunctions(filePath, isLookupDto, itemProp, dtoType) {
    let content = fs.readFileSync(filePath, 'utf8');
    let lines = content.split('\n');
    let updated = false;

    for (let i = 0; i < lines.length; i++) {
        if (lines[i].includes('SearchUnits(string value, CancellationToken token)')) {
            // Find the closing brace of the SearchUnits function
            // SearchUnits function body has a return line, then closing brace. Usually it's within 5 lines.
            for (let j = 1; j <= 6; j++) {
                if (lines[i + j] && lines[i + j].trim() === '}') {
                    // Insert the new functions after this closing brace!
                    let newCode = '';
                    if (filePath.includes('RawMaterialEdit.razor') || filePath.includes('SemiFinishedProductEdit.razor')) {
                        newCode = `
    private async Task<IEnumerable<UnitDto>> SearchBaseUnits(string value, CancellationToken token)
    {
        var baseUnits = _units.Where(x => x.BaseUnit == null);
        if (string.IsNullOrEmpty(value)) return baseUnits;
        return baseUnits.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IEnumerable<UnitDto>> SearchBatchUnits(string value, CancellationToken token)
    {
        var purchaseUnit = _model.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;
                    } else if (filePath.includes('FinishedProductEdit.razor')) {
                        newCode = `
    private async Task<IEnumerable<UnitDto>> SearchBaseUnits(string value, CancellationToken token)
    {
        var baseUnits = _units.Where(x => x.BaseUnit == null);
        if (string.IsNullOrEmpty(value)) return baseUnits;
        return baseUnits.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;
                    } else if (filePath.includes('BomEdit.razor')) {
                        newCode = `
    private async Task<IEnumerable<UnitDto>> SearchBaseUnits(string value, CancellationToken token)
    {
        var baseUnits = _units.Where(x => x.BaseUnit == null);
        if (string.IsNullOrEmpty(value)) return baseUnits;
        return baseUnits.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IEnumerable<UnitDto>> SearchUnitsForProduct(long itemId, string value, CancellationToken token)
    {
        var item = _allComponents.FirstOrDefault(x => x.ItemId == itemId);
        if (item == null) return _units;
        var purchaseUnit = item.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;
                    } else {
                        // Purchase pages
                        newCode = `
    private async Task<IEnumerable<${dtoType}>> SearchUnitsForProduct(long itemId, string value, CancellationToken token)
    {
        var item = _allItems.FirstOrDefault(x => x.ItemId == itemId);
        if (item == null) return _units;
        var purchaseUnit = item.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;
                    }
                    lines[i + j] = lines[i + j] + '\n' + newCode;
                    console.log(`Injected unit functions into ${filePath} at line ${i + j + 1}`);
                    updated = true;
                    break;
                }
            }
            if (updated) break;
        }
    }

    if (updated) {
        fs.writeFileSync(filePath, lines.join('\n'), 'utf8');
    }
}

injectFunctions('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor', false, '', 'UnitDto');
injectFunctions('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor', false, '', 'UnitDto');
injectFunctions('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor', false, '', 'UnitDto');
injectFunctions('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\BomEdit.razor', false, '', 'UnitDto');
injectFunctions('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseEdit.razor', true, 'ItemID', 'UnitLookupDto');
injectFunctions('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseOrderEdit.razor', false, 'ItemID', 'UnitDto');
injectFunctions('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseRequestEdit.razor', false, 'ItemID', 'UnitDto');
