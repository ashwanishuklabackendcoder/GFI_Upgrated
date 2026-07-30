$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = "SELECT OBJECT_NAME(referencing_id) AS ReferencingObject FROM sys.sql_expression_dependencies WHERE referenced_entity_name = 'Inv_ItemStockByBatchModify'"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "Referenced by: $($reader['ReferencingObject'])"
}
$reader.Close()
$connection.Close()
