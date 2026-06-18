$connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$conn.Open()
$query = "SELECT r.RoleName, p.LinkID, p.ViewPer, p.InsertPer, p.UpdatePer, p.DeletePer FROM Z_UsersRoleForm p JOIN Z_UsersRoles r ON p.RoleID = r.RoleID WHERE p.LinkID = 1"
$cmd = New-Object System.Data.SqlClient.SqlCommand($query, $conn)
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Output "Role: $($reader['RoleName']) | LinkID: $($reader['LinkID']) | View: $($reader['ViewPer']) | Insert: $($reader['InsertPer'])"
}
$conn.Close()
