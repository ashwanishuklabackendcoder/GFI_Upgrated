$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- W_MasterStatus rows in DB ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT StatusID, StatusName, StatusOf FROM W_MasterStatus"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "StatusID: $($reader['StatusID']) | StatusName: $($reader['StatusName']) | StatusOf: $($reader['StatusOf'])"
}
$reader.Close()

Write-Output "`n--- HR_StaffModify definition ---"
$command.CommandText = "SELECT definition FROM sys.sql_modules WHERE object_id = OBJECT_ID('HR_StaffModify')"
$definition = $command.ExecuteScalar()
Write-Output $definition

$connection.Close()
