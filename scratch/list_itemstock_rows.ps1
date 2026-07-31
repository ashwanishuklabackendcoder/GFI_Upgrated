$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

$query = "SELECT * FROM W_ItemStock"
$command = $connection.CreateCommand()
$command.CommandText = $query
$reader = $command.ExecuteReader()

Write-Output "=== W_ItemStock Rows ==="
while ($reader.Read()) {
    Write-Output "StockID: $($reader['StockID']) | ItemID: $($reader['ItemID']) | WarehouseID: $($reader['WarehouseID']) | OpeningQty: $($reader['OpeningQuantity']) | PurchasedQty: $($reader['PurchasedQuantity'])"
}
$reader.Close()

$connection.Close()
