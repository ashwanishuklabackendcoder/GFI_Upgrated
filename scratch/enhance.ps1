$srcDir = "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI"
$files = Get-ChildItem -Path $srcDir -Filter "*.razor" -Recurse

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false

    # 1. Update MudAutocomplete instances to add Clearable="true" if not present
    if ($content -like "*<MudAutocomplete*") {
        # Find MudAutocomplete tags and inject Clearable="true" if they don't have Clearable already
        $pattern = "(<MudAutocomplete(?![^>]*Clearable)[^>]*)"
        if ($content -match $pattern) {
            $content = [regex]::Replace($content, $pattern, '$1 Clearable="true"')
            $modified = $true
        }
    }

    # 2. Update MudDatePicker instances to add Editable="true" if not present
    if ($content -like "*<MudDatePicker*") {
        # Find MudDatePicker tags and inject Editable="true" if they don't have Editable already
        $pattern = "(<MudDatePicker(?![^>]*Editable)[^>]*)"
        if ($content -match $pattern) {
            $content = [regex]::Replace($content, $pattern, '$1 Editable="true"')
            $modified = $true
        }
    }

    # 3. Search and replace search functions to return all values if pre-filled
    # Pattern to find standard search functions:
    # private async Task<IEnumerable<T>> SearchX(string value, CancellationToken token) { ... }
    # Let's match multiline and singleline functions.
    $searchFuncRegex = "(?ms)private\s+async\s+Task<IEnumerable<(?<type>[^>]+)>>\s+(?<funcName>Search[a-zA-Z0-9_]+)\(string\s+value,\s+CancellationToken\s+token\)\s*\{(?<body1>\s*if\s*\(\s*string\.IsNullOrEmpty\(value\)\s*\)\s*(?:\{\s*)?return\s+(?<coll>[a-zA-Z0-9_]+);(?:\s*\})?)\s*return\s+\k<coll>\.Where\(x\s*=>\s*(?:\(x\.(?<prop>[a-zA-Z0-9_]+)\s*(?:\?\?\s*\"\"\s*)?\)|x\.(?<prop>[a-zA-Z0-9_]+))\.Contains\(value,\s*StringComparison\.OrdinalIgnoreCase\)\);\s*\}"
    
    $matches = [regex]::Matches($content, $searchFuncRegex)
    foreach ($m in $matches) {
        $type = $m.Groups["type"].Value
        $funcName = $m.Groups["funcName"].Value
        $coll = $m.Groups["coll"].Value
        $prop = $m.Groups["prop"].Value
        
        # Build replacement body
        $newBody = "private async Task<IEnumerable<$type>> $funcName(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || $coll.Any(x => (x.$prop ?? \"\").Equals(value, StringComparison.OrdinalIgnoreCase))) return $coll;
        return $coll.Where(x => (x.$prop ?? \"\").Contains(value, StringComparison.OrdinalIgnoreCase));
    }"
        
        # Replace this specific match
        $oldString = $m.Value
        $content = $content.Replace($oldString, $newBody)
        $modified = $true
    }

    # Also handle another search function pattern with minor syntax variations
    $searchFuncRegex2 = "(?ms)private\s+async\s+Task<IEnumerable<(?<type>[^>]+)>>\s+(?<funcName>Search[a-zA-Z0-9_]+)\(string\s+value,\s+CancellationToken\s+token\)\s*\{(?<body1>\s*if\s*\(\s*string\.IsNullOrEmpty\(value\)\s*\)\s*(?:\{\s*)?return\s+(?<coll>[a-zA-Z0-9_]+);(?:\s*\})?)\s*return\s+\k<coll>\.Where\(x\s*=>\s*\(?(?:x\.(?<prop>[a-zA-Z0-9_]+)\s*(?:\?\?\s*\"\"\s*)?)\)?\.Contains\(value,\s*StringComparison\.OrdinalIgnoreCase\)\);\s*\}"
    $matches2 = [regex]::Matches($content, $searchFuncRegex2)
    foreach ($m in $matches2) {
        $type = $m.Groups["type"].Value
        $funcName = $m.Groups["funcName"].Value
        $coll = $m.Groups["coll"].Value
        $prop = $m.Groups["prop"].Value
        
        $newBody = "private async Task<IEnumerable<$type>> $funcName(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || $coll.Any(x => (x.$prop ?? \"\").Equals(value, StringComparison.OrdinalIgnoreCase))) return $coll;
        return $coll.Where(x => (x.$prop ?? \"\").Contains(value, StringComparison.OrdinalIgnoreCase));
    }"
        $oldString = $m.Value
        $content = $content.Replace($oldString, $newBody)
        $modified = $true
    }

    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -Encoding utf8
        Write-Output "Enhanced: $($file.Name)"
    }
}
