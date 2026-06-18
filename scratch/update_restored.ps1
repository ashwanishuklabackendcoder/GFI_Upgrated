# Update the 6 restored files safely
$files = @(
    "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules\Admin\Pages\Staff.razor",
    "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules\Store\Pages\ItemStockList.razor",
    "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules\Store\Pages\ReportBatchWiseItems.razor",
    "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules\Store\Pages\SemiFinishedProductList.razor",
    "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules\Store\Pages\Skus.razor",
    "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules\Store\Pages\Warehouses.razor"
)

foreach ($filePath in $files) {
    if (-not (Test-Path $filePath)) {
        Write-Output "File not found: $filePath"
        continue
    }
    
    $content = Get-Content -Path $filePath -Raw
    if ([string]::IsNullOrEmpty($content)) {
        Write-Output "File is empty: $filePath"
        continue
    }

    $modified = $false

    # 1. Add RowsPerPage="25" to MudTable
    if ($content -match '<MudTable\s+[^>]*ServerData=' -and $content -notmatch 'RowsPerPage=') {
        $content = $content -replace '(<MudTable\s+)', '$1RowsPerPage="25" '
        $modified = $true
        Write-Output "Added RowsPerPage to $filePath"
    }

    # 2. Update SortType to default to DESC
    if ($content -match 'SortDirection\.Descending\s*\?\s*["'']DESC["'']\s*:\s*["'']ASC["'']') {
        $content = $content -replace 'SortDirection\.Descending(\s*\?\s*["'']DESC["'']\s*:\s*["'']ASC["''])', 'SortDirection.Ascending$1'
        $content = $content -replace 'SortDirection\.Ascending\s*\?\s*(["''])DESC(["''])\s*:\s*(["''])ASC(["''])', 'SortDirection.Ascending ? ${1}ASC${2} : ${3}DESC${4}'
        $modified = $true
        Write-Output "Updated SortDirection ternary in $filePath"
    }

    if ($modified) {
        # Write back safely
        [System.IO.File]::WriteAllText($filePath, $content)
        Write-Output "Saved changes to $filePath"
    }
}
