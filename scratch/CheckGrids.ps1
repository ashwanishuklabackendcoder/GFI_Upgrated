$files = Get-ChildItem -Path 'd:\GFI_Upgrated\src\GFI_Upgrated.UI' -Filter '*.razor' -Recurse

$results = @()
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match '<MudTable') {
        $matches = [regex]::Matches($content, '(?s)<MudTh.*?>.*?</MudTh>')
        $missingSort = $false
        $totalHeaders = 0
        $sortableHeaders = 0
        
        foreach ($match in $matches) {
            $headerContent = $match.Value
            if ($headerContent -match 'Action' -or $headerContent -match 'Edit' -or $headerContent -match 'Delete' -or $headerContent -match 'Select' -or $headerContent -match '<MudIcon' -or $headerContent -match 'Image') {
                continue
            }
            if ($headerContent -match '(?s)<MudTh[^>]*>\s*</MudTh>') {
                continue
            }

            $totalHeaders++
            if ($headerContent -notmatch '<MudTableSortLabel') {
                $missingSort = $true
            } else {
                $sortableHeaders++
            }
        }
        
        if ($totalHeaders -gt 0 -and $missingSort) {
            $results += [PSCustomObject]@{
                File = $file.Name
                Path = $file.FullName
                TotalHeaders = $totalHeaders
                SortableHeaders = $sortableHeaders
            }
        }
    }
}
$results | Format-Table -AutoSize
