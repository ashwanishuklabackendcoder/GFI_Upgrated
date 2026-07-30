const fs = require('fs');

// Helper to replace in file
function updateFile(filePath, searchFuncReplace, itemProp, dtoType) {
    let content = fs.readFileSync(filePath, 'utf8');
    
    // Replace SearchFunc
    content = content.replace(
        'SearchFunc="@SearchUnits"',
        `SearchFunc="@((string val, CancellationToken token) => SearchUnitsForProduct(itemContext.${itemProp}, val, token))"`
    );
    
    // Add SearchUnitsForProduct method
    const searchUnitsOld = `    private async Task<IEnumerable<${dtoType}>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;
    
    const searchUnitsNew = `    private async Task<IEnumerable<${dtoType}>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task<IEnumerable<${dtoType}>> SearchUnitsForProduct(long itemId, string value, CancellationToken token)
    {
        var item = _allItems.FirstOrDefault(x => x.ItemId == itemId);
        if (item == null) return _units;
        var purchaseUnit = item.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;
    
    content = content.replace(searchUnitsOld, searchUnitsNew);
    fs.writeFileSync(filePath, content, 'utf8');
    console.log(`Updated units filtering in ${filePath}`);
}

updateFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseEdit.razor', '', 'ItemID', 'UnitLookupDto');
updateFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseOrderEdit.razor', '', 'ItemId', 'UnitDto');
updateFile('D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseRequestEdit.razor', '', 'ItemId', 'UnitDto');
