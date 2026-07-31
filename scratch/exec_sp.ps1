$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# Execute W_ItemStockList for ItemID = 81
$query = @"
DECLARE @CurrentPage INT = 1
DECLARE @TotalRecord INT = 0
EXEC W_ItemStockList 
    @ItemID = 81,
    @CurrentPage = @CurrentPage OUTPUT,
    @TotalRecord = @TotalRecord OUTPUT
"@

$command = $connection.CreateCommand()
$command.CommandText = $query
$reader = $command.ExecuteReader()

Write-Output "=== SP Result ==="
while ($reader.Read()) {
    $row = @{}
    for ($i = 0; $i -lt $reader.FieldCount; $i++) {
        $row[$reader.GetName($i)] = $reader.GetValue($i)
    }
    Write-Output ([PSCustomObject]$row | Out-String)
}

$reader.Close()
$connection.Close()
