$srcDir = "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI"
$files = Get-ChildItem -Path $srcDir -Filter "*.razor" -Recurse

foreach ($file in $files) {
    $content = [System.IO.File]::ReadAllText($file.FullName)
    $modified = $false

    # 1. Process MudAutocomplete tags
    # Match tags spanning multiple lines using (?s)
    $autocompleteMatches = [regex]::Matches($content, "(?s)<MudAutocomplete[^>]*?>")
    foreach ($match in $autocompleteMatches) {
        $tag = $match.Value
        
        # Remove all existing Clearable attributes (with or without quotes) to avoid duplicates
        # Pattern matches: Clearable="value", Clearable='value', Clearable=value, or just Clearable
        $cleanedTag = [regex]::Replace($tag, '(?i)\bClearable\s*=\s*"[^"]*"', "")
        $cleanedTag = [regex]::Replace($cleanedTag, "(?i)\bClearable\s*=\s*'[^']*'", "")
        $cleanedTag = [regex]::Replace($cleanedTag, '(?i)\bClearable\s*=\s*[^\s>]+', "")
        $cleanedTag = [regex]::Replace($cleanedTag, '\bClearable\b', "")
        
        # Inject single Clearable="true"
        $newTag = $cleanedTag -replace '<MudAutocomplete', '<MudAutocomplete Clearable="true"'
        
        if ($tag -ne $newTag) {
            $content = $content.Replace($tag, $newTag)
            $modified = $true
        }
    }

    # 2. Process MudDatePicker tags
    $datepickerMatches = [regex]::Matches($content, "(?s)<MudDatePicker[^>]*?>")
    foreach ($match in $datepickerMatches) {
        $tag = $match.Value
        
        # Remove all existing Editable attributes
        $cleanedTag = [regex]::Replace($tag, '(?i)\bEditable\s*=\s*"[^"]*"', "")
        $cleanedTag = [regex]::Replace($cleanedTag, "(?i)\bEditable\s*=\s*'[^']*'", "")
        $cleanedTag = [regex]::Replace($cleanedTag, '(?i)\bEditable\s*=\s*[^\s>]+', "")
        $cleanedTag = [regex]::Replace($cleanedTag, '\bEditable\b', "")
        
        # Inject single Editable="true"
        $newTag = $cleanedTag -replace '<MudDatePicker', '<MudDatePicker Editable="true"'
        
        if ($tag -ne $newTag) {
            $content = $content.Replace($tag, $newTag)
            $modified = $true
        }
    }

    # 3. Search and replace search functions to return all values if pre-filled
    $searchFuncRegex = '(?ms)private\s+async\s+Task<IEnumerable<(?<type>[^>]+)>>\s+(?<funcName>Search[a-zA-Z0-9_]+)\(string\s+value,\s+CancellationToken\s+token\)\s*\{(?<body1>\s*if\s*\(\s*string\.IsNullOrEmpty\(value\)\s*\)\s*(?:\{\s*)?return\s+(?<coll>[a-zA-Z0-9_]+);(?:\s*\})?)\s*return\s+\k<coll>\.Where\(x\s*=>\s*(?:\(x\.(?<prop>[a-zA-Z0-9_]+)\s*(?:\?\?\s*\"\"\s*)?\)|x\.(?<prop>[a-zA-Z0-9_]+))\.Contains\(value,\s*StringComparison\.OrdinalIgnoreCase\)\);\s*\}'
    
    $matches = [regex]::Matches($content, $searchFuncRegex)
    foreach ($m in $matches) {
        $type = $m.Groups["type"].Value
        $funcName = $m.Groups["funcName"].Value
        $coll = $m.Groups["coll"].Value
        $prop = $m.Groups["prop"].Value
        
        $newBody = "private async Task<IEnumerable<$type>> $funcName(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || $coll.Any(x => (x.$prop ?? """").Equals(value, StringComparison.OrdinalIgnoreCase))) return $coll;
        return $coll.Where(x => (x.$prop ?? """").Contains(value, StringComparison.OrdinalIgnoreCase));
    }"
        
        $oldString = $m.Value
        $content = $content.Replace($oldString, $newBody)
        $modified = $true
    }

    # Pattern 2 for search functions
    $searchFuncRegex2 = '(?ms)private\s+async\s+Task<IEnumerable<(?<type>[^>]+)>>\s+(?<funcName>Search[a-zA-Z0-9_]+)\(string\s+value,\s+CancellationToken\s+token\)\s*\{(?<body1>\s*if\s*\(\s*string\.IsNullOrEmpty\(value\)\s*\)\s*(?:\{\s*)?return\s+(?<coll>[a-zA-Z0-9_]+);(?:\s*\})?)\s*return\s+\k<coll>\.Where\(x\s*=>\s*\(?(?:x\.(?<prop>[a-zA-Z0-9_]+)\s*(?:\?\?\s*\"\"\s*)?)\)?\.Contains\(value,\s*StringComparison\.OrdinalIgnoreCase\)\);\s*\}'
    $matches2 = [regex]::Matches($content, $searchFuncRegex2)
    foreach ($m in $matches2) {
        $type = $m.Groups["type"].Value
        $funcName = $m.Groups["funcName"].Value
        $coll = $m.Groups["coll"].Value
        $prop = $m.Groups["prop"].Value
        
        $newBody = "private async Task<IEnumerable<$type>> $funcName(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || $coll.Any(x => (x.$prop ?? """").Equals(value, StringComparison.OrdinalIgnoreCase))) return $coll;
        return $coll.Where(x => (x.$prop ?? """").Contains(value, StringComparison.OrdinalIgnoreCase));
    }"
        
        $oldString = $m.Value
        $content = $content.Replace($oldString, $newBody)
        $modified = $true
    }

    if ($modified) {
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        Write-Output "Enhanced: $($file.Name)"
    }
}
