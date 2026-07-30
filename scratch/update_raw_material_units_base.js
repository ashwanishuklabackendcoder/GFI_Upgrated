const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\RawMaterialEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

// 1. Replace SearchFunc for Base Unit using regex
content = content.replace(
    /Label="Base Unit"\s*Value="@\(_units\.FirstOrDefault\(x => x\.UnitId == _model\.PurchaseUnit\)\)"\s*ValueChanged="@\(\(UnitDto val\) => \{ _model\.PurchaseUnit = val\?\.UnitId \?\? 0; \}\)"\s*SearchFunc="@SearchUnits"/i,
    'Label="Base Unit" Value="@(_units.FirstOrDefault(x => x.UnitId == _model.PurchaseUnit))" ValueChanged="@((UnitDto val) => { _model.PurchaseUnit = val?.UnitId ?? 0; })" SearchFunc="@SearchBaseUnits"'
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully ran robust Base Unit SearchFunc replacement on RawMaterialEdit.razor!");
