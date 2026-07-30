$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- Columns of Inv_ItemStockByBatch ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Inv_ItemStockByBatch'"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "Column: $($reader['COLUMN_NAME']) | Type: $($reader['DATA_TYPE']) | Nullable: $($reader['IS_NULLABLE'])"
}
$reader.Close()

Write-Output "`n--- Columns of W_PurchaseChild ---"
$command.CommandText = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'W_PurchaseChild'"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "Column: $($reader['COLUMN_NAME']) | Type: $($reader['DATA_TYPE']) | Nullable: $($reader['IS_NULLABLE'])"
}
$reader.Close()

$connection.Close()
