# Update grids sort direction ternary logic
$files = Get-ChildItem -Path "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules" -Filter "*.razor" -Recurse

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    $modified = $false

    # Update SortType to default to DESC when SortDirection is None
    if ($content -match 'SortDirection\.Descending\s*\?\s*["'']DESC["'']\s*:\s*["'']ASC["'']') {
        $content = $content -replace 'SortDirection\.Descending(\s*\?\s*["'']DESC["'']\s*:\s*["'']ASC["''])', 'SortDirection.Ascending$1'
        # Now replace the ternary values to swap them: change DESC to ASC and ASC to DESC
        # Wait, if we change the condition to SortDirection.Ascending, it should be:
        # SortDirection.Ascending ? "ASC" : "DESC"
        $content = $content -replace 'SortDirection\.Ascending\s*\?\s*(["''])DESC(["''])\s*:\s*(["''])ASC(["''])', 'SortDirection.Ascending ? ${1}ASC${2} : ${3}DESC${4}'
        $modified = $true
        Write-Output "Updated SortDirection ternary in $($file.Name)"
    }

    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}
