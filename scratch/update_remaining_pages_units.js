const fs = require('fs');

// 1. Update PreProcessingEdit.razor
function updatePreProcessing() {
    const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\PreProcessingEdit.razor';
    let content = fs.readFileSync(filePath, 'utf8');

    // Replace BOM selection ValueChanged
    content = content.replace(
        'ValueChanged="@((BomLookupDto val) => { _editor.BomId = val?.BomId ?? 0; })"',
        'ValueChanged="@(async (BomLookupDto val) => await OnBomChanged(val))"'
    );

    // Replace SearchUnits in the UnitMade autocomplete
    content = content.replace(
        'SearchFunc="@SearchUnits"',
        'SearchFunc="@SearchUnits"'
    );

    // Add OnBomChanged and _selectedBomUnitId logic
    const initializedOld = '            _boms = (await bomsTask).ToList();';
    const initializedNew = `            _boms = (await bomsTask).ToList();
            if (_editor.BomId > 0)
            {
                var bomDetails = await ApiClient.GetBomByIdAsync(_editor.BomId);
                if (bomDetails != null)
                {
                    _selectedBomUnitId = bomDetails.UnitId;
                }
            }`;
    content = content.replace(initializedOld, initializedNew);

    const searchUnitsOld = `    private async Task<IEnumerable<UnitLookupDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;

    const searchUnitsNew = `    private long? _selectedBomUnitId;

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

    content = content.replace(searchUnitsOld, searchUnitsNew);
    fs.writeFileSync(filePath, content, 'utf8');
    console.log("Updated PreProcessingEdit.razor");
}

// 2. Update ProductionEdit.razor
function updateProduction() {
    const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\ProductionEdit.razor';
    let content = fs.readFileSync(filePath, 'utf8');

    // Replace Sku selection ValueChanged
    content = content.replace(
        'ValueChanged="@((SkuLookupDto val) => { _editor.SkuId = val?.SkuId ?? 0; })"',
        'ValueChanged="@(async (SkuLookupDto val) => await OnSkuChanged(val))"'
    );

    // Add OnSkuChanged and _selectedSkuUnitId logic
    const initializedOld = '            _editor = await ApiClient.GetProductionByIdAsync(Id);';
    const initializedNew = `            _editor = await ApiClient.GetProductionByIdAsync(Id);
            if (_editor != null && _editor.SkuId > 0)
            {
                var skuDetails = await ApiClient.GetSkuByIdAsync(_editor.SkuId);
                if (skuDetails != null)
                {
                    _selectedSkuUnitId = skuDetails.UnitId;
                }
            }`;
    content = content.replace(initializedOld, initializedNew);

    const initializedNewOld = '            _editor = new SaveProductionRequest();';
    const initializedNewNew = `            _editor = new SaveProductionRequest();
            if (_editor.SkuId > 0)
            {
                var skuDetails = await ApiClient.GetSkuByIdAsync(_editor.SkuId);
                if (skuDetails != null)
                {
                    _selectedSkuUnitId = skuDetails.UnitId;
                }
            }`;
    content = content.replace(initializedNewOld, initializedNewNew);

    const searchUnitsOld = `    private async Task<IEnumerable<UnitLookupDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || _units.Any(x => (x.UnitName ?? "").Equals(value, StringComparison.OrdinalIgnoreCase))) return _units;
        return _units.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;

    const searchUnitsNew = `    private long? _selectedSkuUnitId;

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

    content = content.replace(searchUnitsOld, searchUnitsNew);
    fs.writeFileSync(filePath, content, 'utf8');
    console.log("Updated ProductionEdit.razor");
}

// 3. Update BomEdit.razor
function updateBom() {
    const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\BomEdit.razor';
    let content = fs.readFileSync(filePath, 'utf8');

    // Replace SearchFunc for Yield Unit
    content = content.replace(
        'Label="Yield Unit" \n                                  Value="@(_units.FirstOrDefault(x => x.UnitId == _model.UnitId))" \n                                  ValueChanged="@((UnitDto val) => { _model.UnitId = val?.UnitId ?? 0; })" \n                                  SearchFunc="@SearchUnits"',
        'Label="Yield Unit" \n                                  Value="@(_units.FirstOrDefault(x => x.UnitId == _model.UnitId))" \n                                  ValueChanged="@((UnitDto val) => { _model.UnitId = val?.UnitId ?? 0; })" \n                                  SearchFunc="@SearchBaseUnits"'
    );
    // Alternate whitespace check
    content = content.replace(
        'Label="Yield Unit" \r\n                                  Value="@(_units.FirstOrDefault(x => x.UnitId == _model.UnitId))" \r\n                                  ValueChanged="@((UnitDto val) => { _model.UnitId = val?.UnitId ?? 0; })" \r\n                                  SearchFunc="@SearchUnits"',
        'Label="Yield Unit" \r\n                                  Value="@(_units.FirstOrDefault(x => x.UnitId == _model.UnitId))" \r\n                                  ValueChanged="@((UnitDto val) => { _model.UnitId = val?.UnitId ?? 0; })" \r\n                                  SearchFunc="@SearchBaseUnits"'
    );

    // Replace SearchFunc for Component Unit
    content = content.replace(
        'Value="@(_units.FirstOrDefault(x => x.UnitId == itemContext.UnitId))" \n                                             ValueChanged="@((UnitDto val) => { itemContext.UnitId = val?.UnitId ?? 0; })" \n                                             SearchFunc="@SearchUnits"',
        'Value="@(_units.FirstOrDefault(x => x.UnitId == itemContext.UnitId))" \n                                             ValueChanged="@((UnitDto val) => { itemContext.UnitId = val?.UnitId ?? 0; })" \n                                             SearchFunc="@((string val, CancellationToken token) => SearchUnitsForProduct(itemContext.ItemID, val, token))"'
    );
    // Alternate whitespace check
    content = content.replace(
        'Value="@(_units.FirstOrDefault(x => x.UnitId == itemContext.UnitId))" \r\n                                             ValueChanged="@((UnitDto val) => { itemContext.UnitId = val?.UnitId ?? 0; })" \r\n                                             SearchFunc="@SearchUnits"',
        'Value="@(_units.FirstOrDefault(x => x.UnitId == itemContext.UnitId))" \r\n                                             ValueChanged="@((UnitDto val) => { itemContext.UnitId = val?.UnitId ?? 0; })" \r\n                                             SearchFunc="@((string val, CancellationToken token) => SearchUnitsForProduct(itemContext.ItemID, val, token))"'
    );

    // Add SearchBaseUnits and SearchUnitsForProduct functions
    const searchUnitsOld = `    private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || _units.Any(x => (x.UnitName ?? "").Equals(value, StringComparison.OrdinalIgnoreCase))) return _units;
        return _units.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;

    const searchUnitsNew = `    private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
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

    content = content.replace(searchUnitsOld, searchUnitsNew);
    fs.writeFileSync(filePath, content, 'utf8');
    console.log("Updated BomEdit.razor");
}

updatePreProcessing();
updateProduction();
updateBom();
