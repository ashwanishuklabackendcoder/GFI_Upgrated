# Update grids to show 25 records by default and sort descending by default
$files = Get-ChildItem -Path "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules" -Filter "*.razor" -Recurse

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    $modified = $false

    # 1. Add RowsPerPage="25" to MudTable if it has ServerData and doesn't already have RowsPerPage
    if ($content -match '<MudTable\s+[^>]*ServerData=' -and $content -notmatch 'RowsPerPage=') {
        # Replace <MudTable ...> with <MudTable ... RowsPerPage="25">
        # We find the start of <MudTable and insert RowsPerPage="25" right after '<MudTable'
        $content = $content -replace '(<MudTable\s+)', '$1RowsPerPage="25" '
        $modified = $true
        Write-Output "Added RowsPerPage to $($file.Name)"
    }

    # 2. Update SortType to default to DESC when SortDirection is None
    # Look for patterns like: SortType = state.SortDirection == SortDirection.Descending ? "DESC" : "ASC"
    # and replace with: SortType = state.SortDirection == SortDirection.Ascending ? "ASC" : "DESC"
    if ($content -contains 'SortDirection.Descending ? "DESC" : "ASC"') {
        $content = $content -replace 'SortDirection\.Descending \? "DESC" : "ASC"', 'SortDirection.Ascending ? "ASC" : "DESC"'
        $modified = $true
        Write-Output "Updated SortDirection ternary in $($file.Name)"
    }
    if ($content -contains 'SortDirection.Descending ? "DESC" : "ASC"') {
        $content = $content -replace 'SortDirection\.Descending \? ""DESC"" : ""ASC""', 'SortDirection.Ascending ? ""ASC"" : ""DESC""'
        $modified = $true
    }

    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}
