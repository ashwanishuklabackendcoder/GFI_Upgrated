# Restore empty files from GFI_Upgrated.zip
Add-Type -AssemblyName System.IO.Compression.FileSystem

$zipPath = "D:\GFI\GFI_Upgrated.zip"
$extractDir = "D:\GFI\GFI_Upgrated"
$zip = [System.IO.Compression.ZipFile]::OpenRead($zipPath)

$emptyFiles = @(
    "Staff.razor",
    "ItemStockList.razor",
    "ReportBatchWiseItems.razor",
    "SemiFinishedProductList.razor",
    "Skus.razor",
    "Warehouses.razor"
)

foreach ($entry in $zip.Entries) {
    foreach ($emptyFile in $emptyFiles) {
        if ($entry.FullName -like "*$emptyFile") {
            # Strip the leading "GFI_Upgrated/" if present
            $relPath = $entry.FullName
            if ($relPath.StartsWith("GFI_Upgrated/")) {
                $relPath = $relPath.Substring("GFI_Upgrated/".Length)
            }
            
            Write-Output "Restoring to: $relPath"
            $targetPath = [System.IO.Path]::Combine($extractDir, $relPath)
            
            # Create directory if it doesn't exist
            $dir = [System.IO.Path]::GetDirectoryName($targetPath)
            if (-not (Test-Path $dir)) {
                New-Item -ItemType Directory -Path $dir -Force | Out-Null
            }
            
            # Extract and overwrite
            [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $targetPath, $true)
        }
    }
}
$zip.Dispose()
