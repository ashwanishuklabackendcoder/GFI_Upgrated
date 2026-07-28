const fs = require('fs');
const content = fs.readFileSync('D:/GFI/GFI_Upgrated/src/GFI_Upgrated.UI/Modules/Store/Pages/SemiFinishedProductEdit.razor', 'utf8');

const regex = /(\s*<MudItem xs="12" md="4">\s*<MudAutocomplete (?:Clearable="true" )?MaxItems="1000" T="UnitDto" Label="Unit" Value="@\(_units\.FirstOrDefault\(x => x\.UnitId == _batchModel\.UnitId\)\)"[\s\S]*?<\/MudItem>)/;
const match = content.match(regex);
console.log("Match: ", match ? match[0] : "null");
