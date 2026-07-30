$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

$command = $connection.CreateCommand()
$command.CommandText = "DELETE FROM Inv_ItemStockByBatch WHERE IdFrom IN (14, 15, 16, 17, 18)"
$rows = $command.ExecuteNonQuery()
Write-Output "Deleted $rows incorrect/duplicate batch records for purchase 12."

$connection.Close()
