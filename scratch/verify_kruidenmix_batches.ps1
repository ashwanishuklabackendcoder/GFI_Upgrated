$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# Print item name for ID 20
$command = $connection.CreateCommand()
$command.CommandText = "SELECT ItemName FROM W_MasterItem WHERE ItemID = 20"
$name = $command.ExecuteScalar()
Write-Output "ItemID 20 ItemName: $name"

# Let's count all rows in Inv_ItemStockByBatch for ItemID = 20
$command.CommandText = "SELECT COUNT(*) FROM Inv_ItemStockByBatch WHERE ItemID = 20"
$count = $command.ExecuteScalar()
Write-Output "Total rows in Inv_ItemStockByBatch for ItemID 20: $count"

# Let's print all of them again to be absolutely sure
$command.CommandText = "SELECT ItemStockByBatchId, StockById, BatchNo, Quantity, Amount, IdFrom FROM Inv_ItemStockByBatch WHERE ItemID = 20"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "ItemStockByBatchId: $($reader['ItemStockByBatchId']) | StockById: $($reader['StockById']) | BatchNo: $($reader['BatchNo']) | Qty: $($reader['Quantity']) | Amount: $($reader['Amount']) | IdFrom: $($reader['IdFrom'])"
}
$reader.Close()

$connection.Close()
