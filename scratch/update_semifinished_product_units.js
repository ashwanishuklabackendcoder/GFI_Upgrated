const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Replace Base Unit dropdown SearchFunc
content = content.replace(
    /Label="Base Unit"\s*Value="@\(_units\.FirstOrDefault\(x => x\.UnitId == _model\.PurchaseUnit\)\)"\s*ValueChanged="@\(\(UnitDto val\) => \{ _model\.PurchaseUnit = val\?\.UnitId \?\??\s*0; \}\)"\s*SearchFunc="@SearchUnits"/i,
    'Label="Base Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _model.PurchaseUnit))" ValueChanged="@((UnitDto val) => { _model.PurchaseUnit = val?.UnitId ?? 0; })" SearchFunc="@SearchBaseUnits"'
);

// 2. Replace Batch Unit dropdown SearchFunc
content = content.replace(
    /Label="Unit"\s*Value="@\(_units\.FirstOrDefault\(x => x\.UnitId == _batchModel\.UnitId\)\)"\s*ValueChanged="@\(\(UnitDto val\) => \{ _batchModel\.UnitId = \(int\)\(val\?\.UnitId \?\??\s*0\); \}\)"\s*SearchFunc="@SearchUnits"/i,
    'Label="Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _batchModel.UnitId))" ValueChanged="@((UnitDto val) => { _batchModel.UnitId = (int)(val?.UnitId ?? 0); })" SearchFunc="@SearchBatchUnits"'
);

// 3. Add search functions
const searchUnitsOld = `    private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;

const searchUnitsNew = `    private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value)) return _units;
        return _units.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task<IEnumerable<UnitDto>> SearchBaseUnits(string value, CancellationToken token)
    {
        var baseUnits = _units.Where(x => x.BaseUnit == null);
        if (string.IsNullOrEmpty(value)) return baseUnits;
        return baseUnits.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }

    private async Task<IEnumerable<UnitDto>> SearchBatchUnits(string value, CancellationToken token)
    {
        var purchaseUnit = _model.PurchaseUnit;
        var filtered = _units.Where(x => x.UnitId == purchaseUnit || x.BaseUnit == purchaseUnit);
        if (string.IsNullOrEmpty(value)) return filtered;
        return filtered.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;

content = content.replace(searchUnitsOld, searchUnitsNew);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated SemiFinishedProductEdit.razor units filtering!");
