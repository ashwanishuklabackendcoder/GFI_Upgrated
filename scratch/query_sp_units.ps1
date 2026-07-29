$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$command = $connection.CreateCommand()
$command.CommandText = "EXEC W_MasterUnitList @UnitId=0, @UnitName='', @CurrentPage=1, @RecordPerPage=100, @TotalRecord=0"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "UnitID: $($reader['UnitId']) | UnitName: $($reader['UnitName']) | BaseUnit: $($reader['BaseUnit']) | BaseName: $($reader['BaseName'])"
}
$connection.Close()
