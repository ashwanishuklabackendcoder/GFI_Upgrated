$connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$conn.Open()
$cmd = New-Object System.Data.SqlClient.SqlCommand("sp_helptext", $conn)
$cmd.CommandType = [System.Data.CommandType]::StoredProcedure
$cmd.Parameters.AddWithValue("@objname", "Z_UsersLoginsDetails") | Out-Null
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    Write-Output $reader.GetValue(0)
}
$conn.Close()
