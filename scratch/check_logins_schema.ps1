$connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$conn.Open()
$cmd = New-Object System.Data.SqlClient.SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Z_UsersLogins'", $conn)
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Output "Column: $($reader['COLUMN_NAME'])"
}
$reader.Close()
$conn.Close()
