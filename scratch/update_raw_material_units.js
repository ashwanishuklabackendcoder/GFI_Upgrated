const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Replace SearchFunc for Base Unit
content = content.replace(
    'Label="Base Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _model.PurchaseUnit))" ValueChanged="@((UnitDto val) => { _model.PurchaseUnit = val?.UnitId ?? 0; })" SearchFunc="@SearchUnits"',
    'Label="Base Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _model.PurchaseUnit))" ValueChanged="@((UnitDto val) => { _model.PurchaseUnit = val?.UnitId ?? 0; })" SearchFunc="@SearchBaseUnits"'
);

// 2. Replace SearchFunc for Batch Unit
content = content.replace(
    'Label="Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId))" ValueChanged="@((UnitDto val) => { _batchModel.UnitId = (int)(val?.UnitId ?? 0); })" SearchFunc="@SearchUnits"',
    'Label="Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId))" ValueChanged="@((UnitDto val) => { _batchModel.UnitId = (int)(val?.UnitId ?? 0); })" SearchFunc="@SearchBatchUnits"'
);

// 3. Replace/Add search functions at the bottom
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

    private async Task<IEnumerable<UnitDto>> SearchBatchUnits(string value, CancellationToken token)
    {
        var purchaseUnit = _model.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => (x.UnitName ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;

content = content.replace(searchUnitsOld, searchUnitsNew);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated RawMaterialEdit.razor units filtering!");
