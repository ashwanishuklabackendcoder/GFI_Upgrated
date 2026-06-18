$connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$conn.Open()
$query = "SELECT LoginID, LoginName, RoleID FROM Z_UsersLogins"
$cmd = New-Object System.Data.SqlClient.SqlCommand($query, $conn)
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Output "LoginID: $($reader['LoginID']) | LoginName: $($reader['LoginName']) | RoleID: $($reader['RoleID'])"
}
$conn.Close()
