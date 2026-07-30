const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\SemiFinishedProductEdit.razor';

let content = fs.readFileSync(filePath, 'utf8');

const regex = /private async Task<bool> SaveGeneralInfo\(\)\s*\{\s*if \(_model\.TentativeExpiryDays < 0\)\s*\{\s*Snackbar\.Add\("Tentative expiry days cannot be negative\.", Severity\.Warning\);\s*return false;\s*\}\s*_savingGeneral = true;\s*try\s*\{/i;

const replacementStr = `private async Task<bool> SaveGeneralInfo()
    {
        if (string.IsNullOrWhiteSpace(_model.ItemName))
        {
            Snackbar.Add("Product Name is required.", Severity.Warning);
            return false;
        }
        if (_model.ItemCatId <= 0)
        {
            Snackbar.Add("Category is required.", Severity.Warning);
            return false;
        }
        if (_model.PurchaseUnit <= 0)
        {
            Snackbar.Add("Purchase Unit is required.", Severity.Warning);
            return false;
        }
        if (_model.StatusId <= 0)
        {
            Snackbar.Add("Status is required.", Severity.Warning);
            return false;
        }
        if (_model.TentativeExpiryDays < 0)
        {
            Snackbar.Add("Tentative expiry days cannot be negative.", Severity.Warning);
            return false;
        }

        _savingGeneral = true;
        try
        {`;

content = content.replace(regex, replacementStr);
fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated SemiFinishedProductEdit.razor!");
