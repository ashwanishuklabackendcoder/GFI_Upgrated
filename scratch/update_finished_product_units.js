const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Replace Base Unit dropdown SearchFunc
content = content.replace(
    /Label="Base Unit"\s*\n?\s*Value="@\(_units\.FirstOrDefault\(x => x\.UnitId == _model\.PurchaseUnit\)\)"\s*\n?\s*ValueChanged="@\(\(UnitDto val\) => \{ _model\.PurchaseUnit = val\?\.UnitId \?\??\s*0; \}\)"\s*\n?\s*SearchFunc="@SearchUnits"/i,
    'Label="Base Unit" \n                                         Value="@(_units.FirstOrDefault(x => x.UnitId == _model.PurchaseUnit))" \n                                         ValueChanged="@((UnitDto val) => { _model.PurchaseUnit = val?.UnitId ?? 0; })" \n                                         SearchFunc="@SearchBaseUnits"'
);

// 2. Add SearchBaseUnits function
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
    }`;

content = content.replace(searchUnitsOld, searchUnitsNew);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated FinishedProductEdit.razor units filtering!");
