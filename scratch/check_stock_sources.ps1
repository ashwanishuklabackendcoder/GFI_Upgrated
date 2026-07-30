$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- Distinct StockById values in Inv_ItemStockByBatch ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT DISTINCT StockById FROM Inv_ItemStockByBatch"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "StockById: $($reader['StockById'])"
}
$reader.Close()

Write-Output "`n--- Checking W_MasterDropDown (if there is a group matching Stock From / StockById) ---"
$command.CommandText = "SELECT DropDownValueID, ValueName, GroupID FROM W_MasterDropDown WHERE GroupID IN (SELECT GroupID FROM W_MasterDropDownGroup WHERE GroupName LIKE '%Stock%')"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "ValueID: $($reader['DropDownValueID']) | ValueName: $($reader['ValueName']) | GroupID: $($reader['GroupID'])"
}
$reader.Close()

$connection.Close()
