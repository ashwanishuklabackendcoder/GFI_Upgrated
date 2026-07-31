$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- Z_UsersLogins columns ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Z_UsersLogins'"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "Col: $($reader['COLUMN_NAME']) | Type: $($reader['DATA_TYPE'])"
}
$reader.Close()

$connection.Close()
