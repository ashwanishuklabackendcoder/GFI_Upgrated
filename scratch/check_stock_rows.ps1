$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# Query rows in Inv_ItemStockByBatch where IdFrom = 12 (or related to purchase / stock)
$query = @"
SELECT top 100
    ItemStockByBatchId,
    StockById,
    IdFrom,
    ItemId,
    Quantity,
    Unit,
    BatchNo,
    ExpiryDate,
    WarehouseId,
    FinalQuantityLeft,
    Amount
FROM Inv_ItemStockByBatch
WHERE IdFrom = 12 OR StockById = 1
"@

$command = $connection.CreateCommand()
$command.CommandText = $query
$reader = $command.ExecuteReader()

Write-Output "=== Rows in Inv_ItemStockByBatch ==="
while ($reader.Read()) {
    $row = [PSCustomObject]@{
        ItemStockByBatchId = $reader["ItemStockByBatchId"]
        StockById          = $reader["StockById"]
        IdFrom             = $reader["IdFrom"]
        ItemId             = $reader["ItemId"]
        Quantity           = $reader["Quantity"]
        Unit               = $reader["Unit"]
        BatchNo            = $reader["BatchNo"]
        WarehouseId        = $reader["WarehouseId"]
        FinalQuantityLeft  = $reader["FinalQuantityLeft"]
        Amount             = $reader["Amount"]
    }
    Write-Output ($row | Out-String)
}

$reader.Close()
$connection.Close()
