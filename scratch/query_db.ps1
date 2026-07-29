$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = "SELECT UnitID, UnitName, BaseUnit, IsActive FROM W_MasterUnit"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "UnitID: $($reader['UnitID']) | UnitName: $($reader['UnitName']) | BaseUnit: $($reader['BaseUnit']) | IsActive: $($reader['IsActive'])"
}
$connection.Close()
