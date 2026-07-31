$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# We will query all items in W_ItemStock and compare W_ItemStock.PurchasedQuantity with the sum of W_PurchaseChild.Quantity
$query = @"
SELECT 
    ws.StockID,
    mi.ItemName,
    ws.PurchasedQuantity AS StockTablePurchasedQty,
    ISNULL((SELECT SUM(pc.Quantity) FROM W_PurchaseChild pc WHERE pc.ItemID = ws.ItemID), 0) AS PurchaseChildSumQty,
    ISNULL((SELECT SUM(pc.Quantity * pc.UnitPrice) FROM W_PurchaseChild pc WHERE pc.ItemID = ws.ItemID), 0) AS PurchaseChildValue,
    ISNULL((SELECT SUM(Amount) FROM Inv_ItemStockByBatch WHERE ItemID = ws.ItemID AND (StockById = 3 OR IdFrom = 0)), 0) AS OpeningStockValue
FROM W_ItemStock ws
INNER JOIN W_MasterItem mi ON ws.ItemID = mi.ItemID
ORDER BY ws.StockID DESC
"@

$command = $connection.CreateCommand()
$command.CommandText = $query
$reader = $command.ExecuteReader()

Write-Output "=== Auditing Purchased Quantities vs Purchase Ledger ==="
$mismatches = 0
while ($reader.Read()) {
    $stockId = $reader["StockID"]
    $itemName = $reader["ItemName"]
    $stockQty = $reader["StockTablePurchasedQty"]
    $ledgerQty = $reader["PurchaseChildSumQty"]
    $purchaseVal = $reader["PurchaseChildValue"]
    $openingVal = $reader["OpeningStockValue"]

    # If the purchased qty in W_ItemStock is positive but there is a mismatch with the ledger
    if ($stockQty -gt 0 -and $stockQty -ne $ledgerQty) {
        $mismatches++
        Write-Output "StockID: $stockId | Item: $itemName"
        Write-Output "  - Purchased Qty in Stock Table: $stockQty"
        Write-Output "  - Actual Qty in Purchase Ledger (W_PurchaseChild): $ledgerQty"
        Write-Output "  - Calculated Purchase Value: $purchaseVal SRD (Opening: $openingVal SRD)"
        Write-Output ""
    }
}
$reader.Close()

Write-Output "Total items with mismatches: $mismatches"
$connection.Close()
