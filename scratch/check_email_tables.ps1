$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- Checking tables matching Email, Mail, or Log ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '%Email%' OR TABLE_NAME LIKE '%Mail%' OR TABLE_NAME LIKE '%Log%'"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "Table: $($reader['TABLE_NAME'])"
}
$reader.Close()

$connection.Close()
