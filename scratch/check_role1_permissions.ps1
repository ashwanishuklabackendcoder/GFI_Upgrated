$connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$conn.Open()
$query = "SELECT p.LinkID, p.ViewPer, m.PageHeading, m.PagePath FROM Z_UsersRoleForm p JOIN Z_UsersMenu m ON p.LinkID = m.LinkID WHERE p.RoleID = 1"
$cmd = New-Object System.Data.SqlClient.SqlCommand($query, $conn)
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Output "LinkID: $($reader['LinkID']) | Heading: $($reader['PageHeading']) | Path: $($reader['PagePath']) | View: $($reader['ViewPer'])"
}
$conn.Close()
