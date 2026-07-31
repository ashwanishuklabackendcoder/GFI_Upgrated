$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

$query = @"
SELECT 
 (isnull((select sum(isnull(Amount, 0)) from Inv_ItemStockByBatch where ItemID = 81 and (StockById = 3 or IdFrom = 0)), 0) + 
  isnull((select sum(isnull(pc.Quantity, 0) * isnull(pc.UnitPrice, 0)) from Inv_ItemStockByBatch sb inner join W_PurchaseChild pc on sb.IdFrom = pc.PurchaseItemID where sb.ItemID = 81 and sb.StockById = 1 and sb.IdFrom > 0), 0)) as TotalValue
"@

$command = $connection.CreateCommand()
$command.CommandText = $query
$val = $command.ExecuteScalar()
Write-Output "TotalValue returned: $val"

# Let's also print individual parts
$q1 = "select sum(isnull(Amount, 0)) from Inv_ItemStockByBatch where ItemID = 81 and (StockById = 3 or IdFrom = 0)"
$command.CommandText = $q1
$val1 = $command.ExecuteScalar()
Write-Output "Opening Stock Sum: $val1"

$q2 = "select sb.ItemStockByBatchId, sb.IdFrom, pc.PurchaseItemID, pc.Quantity, pc.UnitPrice, (pc.Quantity * pc.UnitPrice) as RowVal from Inv_ItemStockByBatch sb inner join W_PurchaseChild pc on sb.IdFrom = pc.PurchaseItemID where sb.ItemID = 81 and sb.StockById = 1 and sb.IdFrom > 0"
$command.CommandText = $q2
$reader = $command.ExecuteReader()
Write-Output "=== Joined Rows ==="
while ($reader.Read()) {
    Write-Output "ItemStockByBatchId: $($reader['ItemStockByBatchId']) | IdFrom: $($reader['IdFrom']) | PurchaseItemID: $($reader['PurchaseItemID']) | Qty: $($reader['Quantity']) | Price: $($reader['UnitPrice']) | RowVal: $($reader['RowVal'])"
}
$reader.Close()

$connection.Close()
