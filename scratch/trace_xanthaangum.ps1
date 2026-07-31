$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# 1. Query W_ItemStock for StockID = 35
$query1 = "SELECT * FROM W_ItemStock WHERE StockID = 35"
$command = $connection.CreateCommand()
$command.CommandText = $query1
$reader = $command.ExecuteReader()
Write-Output "=== W_ItemStock Row for StockID 35 ==="
$itemId = 0
while ($reader.Read()) {
    $itemId = $reader["ItemID"]
    $row = @{}
    for ($i = 0; $i -lt $reader.FieldCount; $i++) {
        $row[$reader.GetName($i)] = $reader.GetValue($i)
    }
    Write-Output ([PSCustomObject]$row | Out-String)
}
$reader.Close()

Write-Output "ItemID for Xanthaangum Mix (100 gram) is: $itemId"

# 2. Query Inv_ItemStockByBatch rows for this ItemID (StockById = 3 or IdFrom = 0 is Opening Stock)
$query2 = "SELECT * FROM Inv_ItemStockByBatch WHERE ItemID = $itemId"
$command.CommandText = $query2
$reader = $command.ExecuteReader()
Write-Output "`n=== Inv_ItemStockByBatch rows for ItemID $itemId ==="
while ($reader.Read()) {
    $row = @{}
    for ($i = 0; $i -lt $reader.FieldCount; $i++) {
        $row[$reader.GetName($i)] = $reader.GetValue($i)
    }
    Write-Output ([PSCustomObject]$row | Out-String)
}
$reader.Close()

# 3. Query W_PurchaseChild records for this ItemID
$query3 = "SELECT pc.*, pm.VoucherNumber, pm.GoodsRecievedDate FROM W_PurchaseChild pc INNER JOIN W_PurchaseMaster pm ON pc.PurchaseID = pm.PurchaseID WHERE pc.ItemID = $itemId"
$command.CommandText = $query3
$reader = $command.ExecuteReader()
Write-Output "`n=== Purchase Records in W_PurchaseChild for ItemID $itemId ==="
while ($reader.Read()) {
    $row = @{}
    for ($i = 0; $i -lt $reader.FieldCount; $i++) {
        $row[$reader.GetName($i)] = $reader.GetValue($i)
    }
    Write-Output ([PSCustomObject]$row | Out-String)
}
$reader.Close()

$connection.Close()
