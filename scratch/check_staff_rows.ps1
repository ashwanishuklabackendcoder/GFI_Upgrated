$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "--- Hr_StaffMaster rows ---"
$command = $connection.CreateCommand()
$command.CommandText = "SELECT StaffId, StaffFirstName, StaffLastName, Status FROM Hr_StaffMaster"
$reader = $command.ExecuteReader()
while ($reader.Read()) {
    Write-Output "StaffId: $($reader['StaffId']) | Name: $($reader['StaffFirstName']) $($reader['StaffLastName']) | Status: $($reader['Status'])"
}
$reader.Close()

$connection.Close()
