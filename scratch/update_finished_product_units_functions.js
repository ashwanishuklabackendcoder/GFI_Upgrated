const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

const regex = /private async Task<IEnumerable<UnitDto>> SearchUnits\(string value, CancellationToken token\)\s*\{\s*if \(string\.IsNullOrEmpty\(value\)\) return _units;\s*return _units\.Where\(x => x\.UnitName\?\.Contains\(value, StringComparison\.OrdinalIgnoreCase\) == true\);\s*\}/i;

const replacementStr = `private async Task<IEnumerable<UnitDto>> SearchUnits(string value, CancellationToken token)
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

content = content.replace(regex, replacementStr);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully added SearchBaseUnits to FinishedProductEdit.razor!");
