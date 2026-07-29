const fs = require('fs');

function normalizeNewlines(str) {
    return str.replace(/\r?\n/g, '\r\n');
}

// 1. RawMaterialEdit.razor
function updateRawMaterial() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/Label="Base Unit"([^>]*?)SearchFunc="@SearchUnits"/, 'Label="Base Unit"$1SearchFunc="@SearchBaseUnits"');
    content = content.replace(/Label="Unit"([^>]*?)SearchFunc="@SearchUnits"/, 'Label="Unit"$1SearchFunc="@SearchBatchUnits"');
    
    const regex = /private async Task<IEnumerable<UnitDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)[^}]*?\}\s*return _units\.Where\(x => \(x\.UnitName \?\? ""\)\.Contains\(value, StringComparison\.OrdinalIgnoreCase\)\);\s*\}/i;
    
    const replacement = `private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || _units.Any(x => (x.UnitName ?? "").Equals(value, StringComparison.OrdinalIgnoreCase))) return _units;
        return _units.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }

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

    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated RawMaterialEdit.razor");
}

// 2. SemiFinishedProductEdit.razor
function updateSemiFinished() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/Label="Base Unit"([^>]*?)SearchFunc="@SearchUnits"/, 'Label="Base Unit"$1SearchFunc="@SearchBaseUnits"');
    content = content.replace(/Label="Unit"([^>]*?)SearchFunc="@SearchUnits"/, 'Label="Unit"$1SearchFunc="@SearchBatchUnits"');
    
    const regex = /private async Task<IEnumerable<UnitDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)[^}]*?\}\s*return _units\.Where\(x => \(x\.UnitName \?\? ""\)\.Contains\(value, StringComparison\.OrdinalIgnoreCase\)\);\s*\}/i;
    
    const replacement = `private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || _units.Any(x => (x.UnitName ?? "").Equals(value, StringComparison.OrdinalIgnoreCase))) return _units;
        return _units.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }

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

    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated SemiFinishedProductEdit.razor");
}

// 3. FinishedProductEdit.razor
function updateFinished() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/Label="Base Unit"([^>]*?)SearchFunc="@SearchUnits"/s, 'Label="Base Unit"$1SearchFunc="@SearchBaseUnits"');
    
    const regex = /private async Task<IEnumerable<UnitDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)\) return _units;\s*return _units\.Where\(x => x\.UnitName\?\.Contains\(value, StringComparison\.OrdinalIgnoreCase\) == true\);\s*\}/i;
    
    const replacement = `private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task<IEnumerable<UnitDto>> SearchBaseUnits(string value, CancellationToken token)
    {
        var baseUnits = _units.Where(x => x.BaseUnit == null);
        if (string.IsNullOrEmpty(value)) return baseUnits;
        return baseUnits.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;

    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated FinishedProductEdit.razor");
}

// 4. PurchaseEdit.razor
function updatePurchaseEdit() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/T="UnitLookupDto"([^>]*?)SearchFunc="@SearchUnits"/s, 'T="UnitLookupDto"$1SearchFunc="@((string val, CancellationToken token) => SearchUnitsForProduct(itemContext.ItemID, val, token))"');
    
    const regex = /private async Task<IEnumerable<UnitLookupDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)\) return _units;\s*return _units\.Where\(x => x\.UnitName\?\.Contains\(value, StringComparison\.OrdinalIgnoreCase\) == true\);\s*\}/i;
    
    const replacement = `private async Task<IEnumerable<UnitLookupDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task<IEnumerable<UnitLookupDto>> SearchUnitsForProduct(long itemId, string value, CancellationToken token)
    {
        var item = _allItems.FirstOrDefault(x => x.ItemId == itemId);
        if (item == null) return _units;
        var purchaseUnit = item.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;

    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated PurchaseEdit.razor");
}

// 5. PurchaseOrderEdit.razor
function updatePurchaseOrder() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseOrderEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/T="UnitDto"([^>]*?)SearchFunc="@SearchUnits"/s, 'T="UnitDto"$1SearchFunc="@((string val, CancellationToken token) => SearchUnitsForProduct(itemContext.ItemID, val, token))"');
    
    const regex = /private async Task<IEnumerable<UnitDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)\) return _units;\s*return _units\.Where\(x => x\.UnitName\?\.Contains\(value, StringComparison\.OrdinalIgnoreCase\) == true\);\s*\}/i;
    
    const replacement = `private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task<IEnumerable<UnitDto>> SearchUnitsForProduct(long itemId, string value, CancellationToken token)
    {
        var item = _allItems.FirstOrDefault(x => x.ItemId == itemId);
        if (item == null) return _units;
        var purchaseUnit = item.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;

    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated PurchaseOrderEdit.razor");
}

// 7. PurchaseRequestEdit.razor
function updatePurchaseRequest() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Purchase\\Pages\\PurchaseRequestEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/T="UnitDto"([^>]*?)SearchFunc="@SearchUnits"/s, 'T="UnitDto"$1SearchFunc="@((string val, CancellationToken token) => SearchUnitsForProduct(itemContext.ItemID, val, token))"');
    
    const regex = /private async Task<IEnumerable<UnitDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)\) return _units;\s*return _units\.Where\(x => x\.UnitName\?\.Contains\(value, StringComparison\.OrdinalIgnoreCase\) == true\);\s*\}/i;
    
    const replacement = `private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task<IEnumerable<UnitDto>> SearchUnitsForProduct(long itemId, string value, CancellationToken token)
    {
        var item = _allItems.FirstOrDefault(x => x.ItemId == itemId);
        if (item == null) return _units;
        var purchaseUnit = item.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;

    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated PurchaseRequestEdit.razor");
}

// 8. PreProcessingEdit.razor
function updatePreProcessing() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\PreProcessingEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/T="BomLookupDto"([^>]*?)ValueChanged="@\(\(BomLookupDto val\) => \{ _editor\.BomId = val\?\.BomId \?\? 0; \}\)"/s, 'T="BomLookupDto"$1ValueChanged="@(async (BomLookupDto val) => await OnBomChanged(val))"');
    
    content = content.replace('_boms = (await bomsTask).ToList();', `_boms = (await bomsTask).ToList();
            if (_editor.BomId > 0)
            {
                var bomDetails = await ApiClient.GetBomByIdAsync(_editor.BomId);
                if (bomDetails != null)
                {
                    _selectedBomUnitId = bomDetails.UnitId;
                }
            }`);
            
    const regex = /private async Task<IEnumerable<UnitLookupDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)\) return _units;\s*return _units\.Where\(x => x\.UnitName\?\.Contains\(value, StringComparison\.OrdinalIgnoreCase\) == true\);\s*\}/i;

    const replacement = `private long? _selectedBomUnitId;

    private async Task OnBomChanged(BomLookupDto val)
    {
        _editor.BomId = val?.BomId ?? 0;
        if (val != null)
        {
            var bomDetails = await ApiClient.GetBomByIdAsync(val.BomId);
            if (bomDetails != null)
            {
                _selectedBomUnitId = bomDetails.UnitId;
                _editor.UnitMade = bomDetails.UnitId;
            }
        }
        else
        {
            _selectedBomUnitId = null;
            _editor.UnitMade = 0;
        }
    }

    private async Task<IEnumerable<UnitLookupDto>> SearchUnits(string value, CancellationToken token)
    {
        if (_selectedBomUnitId == null || _selectedBomUnitId <= 0) return _units;
        var filtered = _units.Where(x => x.UnitId == _selectedBomUnitId || x.BaseUnit == _selectedBomUnitId);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;
    
    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated PreProcessingEdit.razor");
}

// 9. ProductionEdit.razor
function updateProduction() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\ProductionEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/T="SkuLookupDto"([^>]*?)ValueChanged="@\(\(SkuLookupDto val\) => \{ _editor\.SkuId = val\?\.SkuId \?\? 0; \}\)"/s, 'T="SkuLookupDto"$1ValueChanged="@(async (SkuLookupDto val) => await OnSkuChanged(val))"');
    
    content = content.replace('_editor = await ApiClient.GetProductionByIdAsync(Id);', `_editor = await ApiClient.GetProductionByIdAsync(Id);
            if (_editor != null && _editor.SkuId > 0)
            {
                var skuDetails = await ApiClient.GetSkuByIdAsync(_editor.SkuId);
                if (skuDetails != null)
                {
                    _selectedSkuUnitId = skuDetails.UnitId;
                }
            }`);
            
    content = content.replace('_editor = new SaveProductionRequest();', `_editor = new SaveProductionRequest();
            if (_editor.SkuId > 0)
            {
                var skuDetails = await ApiClient.GetSkuByIdAsync(_editor.SkuId);
                if (skuDetails != null)
                {
                    _selectedSkuUnitId = skuDetails.UnitId;
                }
            }`);
            
    const regex = /private async Task<IEnumerable<UnitLookupDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)[^}]*?\}\s*return _units\.Where\(x => \(x\.UnitName \?\? ""\)\.Contains\(value, StringComparison\.OrdinalIgnoreCase\)\);\s*\}/i;

    const replacement = `private long? _selectedSkuUnitId;

    private async Task OnSkuChanged(SkuLookupDto val)
    {
        _editor.SkuId = val?.SkuId ?? 0;
        if (val != null)
        {
            var skuDetails = await ApiClient.GetSkuByIdAsync(val.SkuId);
            if (skuDetails != null)
            {
                _selectedSkuUnitId = skuDetails.UnitId;
                _editor.FillingPerBottleUnit = skuDetails.UnitId;
            }
        }
        else
        {
            _selectedSkuUnitId = null;
            _editor.FillingPerBottleUnit = 0;
        }
    }

    private async Task<IEnumerable<UnitLookupDto>> SearchUnits(string value, CancellationToken token)
    {
        if (_selectedSkuUnitId == null || _selectedSkuUnitId <= 0) return _units;
        var filtered = _units.Where(x => x.UnitId == _selectedSkuUnitId || x.BaseUnit == _selectedSkuUnitId);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;
    
    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated ProductionEdit.razor");
}

// 10. BomEdit.razor
function updateBom() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\BomEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    content = content.replace(/Label="Yield Unit"([^>]*?)SearchFunc="@SearchUnits"/s, 'Label="Yield Unit"$1SearchFunc="@SearchBaseUnits"');
    content = content.replace(/T="UnitDto"([^>]*?)SearchFunc="@SearchUnits"/s, 'T="UnitDto"$1SearchFunc="@((string val, CancellationToken token) => SearchUnitsForProduct(itemContext.ItemID, val, token))"');
    
    const regex = /private async Task<IEnumerable<UnitDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)[^}]*?\}\s*return _units\.Where\(x => \(x\.UnitName \?\? ""\)\.Contains\(value, StringComparison\.OrdinalIgnoreCase\)\);\s*\}/i;

    const replacement = `private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || _units.Any(x => (x.UnitName ?? "").Equals(value, StringComparison.OrdinalIgnoreCase))) return _units;
        return _units.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }

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

    content = content.replace(regex, replacement);
    fs.writeFileSync(file, normalizeNewlines(content), 'utf8');
    console.log("Updated BomEdit.razor");
}

updateRawMaterial();
updateSemiFinished();
updateFinished();
updatePurchaseEdit();
updatePurchaseOrder();
updatePurchaseRequest();
updatePreProcessing();
updateProduction();
updateBom();
