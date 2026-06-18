# Find all empty razor files under src/GFI_Upgrated.UI/Modules
$files = Get-ChildItem -Path "D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules" -Filter "*.razor" -Recurse
foreach ($file in $files) {
    if ($file.Length -eq 0) {
        Write-Output "Empty file: $($file.FullName)"
    }
}
