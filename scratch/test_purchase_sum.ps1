$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

$query = "SELECT PurchaseItemID, PurchaseID, Quantity, UnitPrice, (Quantity * UnitPrice) as RowAmt FROM W_PurchaseChild WHERE ItemID = 81"
$command = $connection.CreateCommand()
$command.CommandText = $query
$reader = $command.ExecuteReader()

Write-Output "=== Purchases for ItemID 81 ==="
$total = 0
while ($reader.Read()) {
    $rowAmt = $reader["RowAmt"]
    $total += $rowAmt
    Write-Output "PurchaseItemID: $($reader['PurchaseItemID']) | PurchaseID: $($reader['PurchaseID']) | Qty: $($reader['Quantity']) | Price: $($reader['UnitPrice']) | RowAmt: $rowAmt"
}
$reader.Close()
Write-Output "Total sum: $total"

$connection.Close()
