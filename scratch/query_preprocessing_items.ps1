$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- Ingredients in W_PreProcessingItem for HeaderId = 5 ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT p.ItemStockUsedID, p.ItemID, m.ItemName, m.MasterItemTypeId, p.Quantity, p.BatchNo FROM W_PreProcessingItem p INNER JOIN W_MasterItem m ON p.ItemID = m.ItemID WHERE p.PreProcessingId = 5"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "ItemStockUsedID: $($reader['ItemStockUsedID']) | ItemID: $($reader['ItemID']) | ItemName: $($reader['ItemName']) | ItemTypeId: $($reader['MasterItemTypeId']) | Quantity: $($reader['Quantity']) | BatchNo: $($reader['BatchNo'])"
}
$reader.Close()

$connection.Close()
