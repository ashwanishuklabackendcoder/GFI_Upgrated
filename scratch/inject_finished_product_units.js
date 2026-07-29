const fs = require('fs');

function injectFinished() {
    const file = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\FinishedProductEdit.razor';
    let content = fs.readFileSync(file, 'utf8');
    let lines = content.split('\n');
    let updated = false;

    for (let i = 0; i < lines.length; i++) {
        if (lines[i].includes('SearchUnits(string value, CancellationToken token)')) {
            for (let j = 1; j <= 6; j++) {
                if (lines[i + j] && lines[i + j].trim() === '}') {
                    const newCode = `
    private async Task<IEnumerable<UnitDto>> SearchBaseUnits(string value, CancellationToken token)
    {
        var baseUnits = _units.Where(x => x.BaseUnit == null);
        if (string.IsNullOrEmpty(value)) return baseUnits;
        return baseUnits.Where(x => x.UnitName?.Contains(value, StringComparison.OrdinalIgnoreCase) == true);
    }`;
                    lines[i + j] = lines[i + j] + '\n' + newCode;
                    updated = true;
                    console.log("Successfully injected SearchBaseUnits to FinishedProductEdit.razor!");
                    break;
                }
            }
            if (updated) break;
        }
    }

    if (updated) {
        fs.writeFileSync(file, lines.join('\n'), 'utf8');
    }
}

injectFinished();
