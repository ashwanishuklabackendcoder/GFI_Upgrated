$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# Query rows in W_PurchaseChild to see columns
$query = "SELECT top 5 * FROM W_PurchaseChild"

$command = $connection.CreateCommand()
$command.CommandText = $query
$reader = $command.ExecuteReader()

Write-Output "=== Rows in W_PurchaseChild ==="
while ($reader.Read()) {
    $row = @{}
    for ($i = 0; $i -lt $reader.FieldCount; $i++) {
        $row[$reader.GetName($i)] = $reader.GetValue($i)
    }
    Write-Output ([PSCustomObject]$row | Out-String)
}

$reader.Close()
$connection.Close()
