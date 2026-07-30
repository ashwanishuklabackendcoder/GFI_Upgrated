$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = "SELECT definition FROM sys.sql_modules WHERE object_id = OBJECT_ID('Inv_ItemStockUsedModify')"
$definition = $command.ExecuteScalar()
Write-Output $definition
$connection.Close()
