const fs = require('fs');

const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Store\\Pages\\ItemStockList.razor';
let content = fs.readFileSync(filePath, 'utf8');

// 1. Add Opening Stock to the filter dropdown
content = content.replace(
    `<MudSelectItem Value="@((long?)2)">Pre-Processing</MudSelectItem>`,
    `<MudSelectItem Value="@((long?)2)">Pre-Processing</MudSelectItem>
                <MudSelectItem Value="@((long?)3)">Opening Stock</MudSelectItem>`
);

// 2. Map StockById = 3 to "Opening Stock" in row template
content = content.replace(
    `Color="@(context.StockById == 1 ? Color.Info : Color.Warning)"`,
    `Color="@(context.StockById == 1 ? Color.Info : context.StockById == 2 ? Color.Warning : Color.Success)"`
);
content = content.replace(
    `@(context.StockById == 1 ? "Purchase" : context.StockById == 2 ? "PreProcessing" : "Unknown")`,
    `@(context.StockById == 1 ? "Purchase" : context.StockById == 2 ? "PreProcessing" : context.StockById == 3 ? "Opening Stock" : "Unknown")`
);

// 3. Map StockById = 3 in printing summary details
content = content.replace(
    `Stock From: @(_selectedStockById == 1 ? "Purchase" : _selectedStockById == 2 ? "Pre-Processing" : "All Sources")`,
    `Stock From: @(_selectedStockById == 1 ? "Purchase" : _selectedStockById == 2 ? "Pre-Processing" : _selectedStockById == 3 ? "Opening Stock" : "All Sources")`
);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully mapped StockById = 3 to Opening Stock!");
