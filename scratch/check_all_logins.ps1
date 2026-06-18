$connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$conn.Open()
$query = @"
SELECT 
    L.LoginID,
    L.LoginName,
    R.RoleName,
    R.RoleID,
    R.IsAdmin
FROM Z_UsersLogins L
INNER JOIN Z_UsersRolesMore RM ON L.LoginID = RM.LoginID AND RM.IsDefault = 1
INNER JOIN Z_UsersRoles R ON RM.RoleID = R.RoleID
"@
$cmd = New-Object System.Data.SqlClient.SqlCommand($query, $conn)
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Output "LoginID: $($reader['LoginID']) | LoginName: $($reader['LoginName']) | RoleName: $($reader['RoleName']) (ID: $($reader['RoleID'])) | IsAdmin: $($reader['IsAdmin'])"
}
$conn.Close()
