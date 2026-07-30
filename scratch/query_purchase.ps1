$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- W_PurchaseChild for PurchaseID = 12 ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT PurchaseItemID, PurchaseID, ItemID, Quantity, UnitId, UnitPrice, Amount FROM W_PurchaseChild WHERE PurchaseID = 12"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "PurchaseItemID: $($reader['PurchaseItemID']) | ItemID: $($reader['ItemID']) | Quantity: $($reader['Quantity']) | UnitId: $($reader['UnitId']) | UnitPrice: $($reader['UnitPrice']) | Amount: $($reader['Amount'])"
}
$reader.Close()

Write-Output "`n--- EXEC W_PurchaseChildBatchSelectByID @PurchaseID = 12 ---"
$command.CommandText = "EXEC W_PurchaseChildBatchSelectByID @PurchaseID = 12"
$reader = $command.ExecuteReader()
# Print column names
$cols = @()
for ($i = 0; $i -lt $reader.FieldCount; $i++) {
    $cols += $reader.GetName($i)
}
Write-Output "Columns: $($cols -join ', ')"

while ($reader.Read()) {
    $rowStr = ""
    foreach ($col in $cols) {
        $rowStr += "$($col): $($reader[$col]) | "
    }
    Write-Output $rowStr
}
$reader.Close()

$connection.Close()
