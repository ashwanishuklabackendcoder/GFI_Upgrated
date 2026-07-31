$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# 1. Query W_ItemStock for StockID = 41
$query1 = "SELECT * FROM W_ItemStock WHERE StockID = 41"
$command = $connection.CreateCommand()
$command.CommandText = $query1
$reader = $command.ExecuteReader()
Write-Output "=== W_ItemStock Row for 41 ==="
while ($reader.Read()) {
    $row = @{}
    for ($i = 0; $i -lt $reader.FieldCount; $i++) {
        $row[$reader.GetName($i)] = $reader.GetValue($i)
    }
    Write-Output ([PSCustomObject]$row | Out-String)
}
$reader.Close()

# Let's say ItemID is retrieved. Let's query all Inv_ItemStockByBatch records for that ItemID
$query2 = "SELECT * FROM Inv_ItemStockByBatch WHERE ItemID = (SELECT ItemID FROM W_ItemStock WHERE StockID = 41)"
$command.CommandText = $query2
$reader = $command.ExecuteReader()
Write-Output "=== Inv_ItemStockByBatch Rows ==="
while ($reader.Read()) {
    $row = @{}
    for ($i = 0; $i -lt $reader.FieldCount; $i++) {
        $row[$reader.GetName($i)] = $reader.GetValue($i)
    }
    Write-Output ([PSCustomObject]$row | Out-String)
}
$reader.Close()

$connection.Close()
