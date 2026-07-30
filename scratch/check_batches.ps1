$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- Item details for BE Mix ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT ItemID, ItemName, MasterItemTypeId FROM W_MasterItem WHERE ItemName LIKE '%BE%Mix%'"
$reader = $command.ExecuteReader()
$itemId = 0
while ($reader.Read()) {
    $itemId = $reader['ItemID']
    Write-Output "ItemID: $($reader['ItemID']) | ItemName: $($reader['ItemName']) | ItemTypeId: $($reader['MasterItemTypeId'])"
}
$reader.Close()

if ($itemId -gt 0) {
    Write-Output "`n--- Batches for ItemID = $itemId in Inv_ItemStockByBatch ---"
    $command.CommandText = "SELECT ItemStockByBatchId, StockById, IdFrom, ItemId, Quantity, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft FROM Inv_ItemStockByBatch WHERE ItemId = $itemId"
    $reader = $command.ExecuteReader()
    while ($reader.Read()) {
        Write-Output "BatchID: $($reader['ItemStockByBatchId']) | BatchNo: $($reader['BatchNo']) | FinalQuantityLeft: $($reader['FinalQuantityLeft']) | ExpiryDate: $($reader['ExpiryDate'])"
    }
    $reader.Close()
}

$connection.Close()
