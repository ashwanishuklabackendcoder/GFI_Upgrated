$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

$spNames = @("W_ProductionModify", "W_PreProcessingModify")

foreach ($sp in $spNames) {
    $query = "SELECT OBJECT_DEFINITION(OBJECT_ID('$sp')) AS SPText"
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $text = $command.ExecuteScalar()
    Write-Output "=== Stored Procedure: $sp ==="
    Write-Output $text
    Write-Output "`n"
}

$connection.Close()
