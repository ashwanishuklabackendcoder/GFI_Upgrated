$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$sqlFile = Join-Path (Resolve-Path ".\") "patch_preprocessing_draft.sql"
$sqlText = Get-Content -Raw -Path $sqlFile

# Split SQL by GO command
$commands = $sqlText -split "(?mi)^\s*GO\s*$"

$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

foreach ($cmdText in $commands) {
    if (![string]::IsNullOrWhiteSpace($cmdText)) {
        $command = $connection.CreateCommand()
        $command.CommandText = $cmdText.Trim()
        try {
            $command.ExecuteNonQuery() | Out-Null
            Write-Output "Successfully executed statement block."
        } catch {
            Write-Error "Failed to execute: $_"
        }
    }
}

$connection.Close()
Write-Output "Database patch application complete."
