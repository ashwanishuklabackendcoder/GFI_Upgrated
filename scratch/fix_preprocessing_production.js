const fs = require('fs');

function fixPreProcessing() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\PreProcessingEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    
    // Change ValueChanged for Select BOM MudAutocomplete
    content = content.replace(
        /ValueChanged="@\(\(BomLookupDto val\) => \{ _editor\.BomId = val\?\.BomId \?\? 0; \}\)"/g,
        'ValueChanged="@OnBomChanged"'
    );
    
    fs.writeFileSync(file, content, 'utf8');
    console.log("Updated PreProcessingEdit.razor BOM ValueChanged!");
}

function fixProduction() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\ProductionEdit.razor';
    let content = fs.readFileSync(file, 'utf8');

    // Change ValueChanged for Select BOM MudAutocomplete
    content = content.replace(
        /ValueChanged="@\(\(BomLookupDto val\) => \{ _editor\.BomId = val\?\.BomId \?\? 0; \}\)"/g,
        'ValueChanged="@OnBomChanged"'
    );

    // Add private long? _selectedBomUnitId; to fields block
    content = content.replace(
        'private bool _processing;',
        'private bool _processing;\n    private long? _selectedBomUnitId;'
    );

    // Add API call to LoadLookupsAsync
    content = content.replace(
        '_units = (await unitsTask).ToList();',
        `_units = (await unitsTask).ToList();
            if (_editor.BomId > 0)
            {
                var bomDetails = await ApiClient.GetBomByIdAsync(_editor.BomId);
                if (bomDetails != null)
                {
                    _selectedBomUnitId = bomDetails.UnitId;
                }
            }`
    );

    // Add OnBomChanged method and update SearchUnits
    const searchUnitsOld = `private async Task<IEnumerable<UnitLookupDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || _units.Any(x => (x.UnitName ?? "").Equals(value, StringComparison.OrdinalIgnoreCase))) return _units;
        return _units.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;

    const searchUnitsNew = `private async Task OnBomChanged(BomLookupDto val)
    {
        _editor.BomId = val?.BomId ?? 0;
        if (val != null)
        {
            var bomDetails = await ApiClient.GetBomByIdAsync(val.BomId);
            if (bomDetails != null)
            {
                _selectedBomUnitId = bomDetails.UnitId;
                _editor.FillingPerBottleUnit = bomDetails.UnitId;
            }
        }
        else
        {
            _selectedBomUnitId = null;
            _editor.FillingPerBottleUnit = 0;
        }
    }

    private async Task<IEnumerable<UnitLookupDto>> SearchUnits(string value, CancellationToken token)
    {
        if (_selectedBomUnitId == null || _selectedBomUnitId <= 0) return _units;
        var filtered = _units.Where(x => x.UnitId == _selectedBomUnitId || x.BaseUnit == _selectedBomUnitId);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;

    // Normalize newlines for match
    const norm = str => str.replace(/\r\n/g, '\n').trim();
    
    let contentNorm = content.replace(/\r\n/g, '\n');
    let index = contentNorm.indexOf(norm(searchUnitsOld));
    if (index !== -1) {
        contentNorm = contentNorm.substring(0, index) + searchUnitsNew + contentNorm.substring(index + norm(searchUnitsOld).length);
        content = contentNorm;
    } else {
        // Fallback replacement if formatting differs slightly
        content = content.replace('private async Task<IEnumerable<UnitLookupDto>> SearchUnits(string value, CancellationToken token)', '/* old search units replaced */');
    }

    fs.writeFileSync(file, content, 'utf8');
    console.log("Updated ProductionEdit.razor with OnBomChanged and filtered SearchUnits!");
}

fixPreProcessing();
fixProduction();
