$connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$conn.Open()
$cmd = New-Object System.Data.SqlClient.SqlCommand("Z_UsersMenuList", $conn)
$cmd.CommandType = [System.Data.CommandType]::StoredProcedure
$cmd.Parameters.AddWithValue("@LinkID", 0) | Out-Null
$cmd.Parameters.AddWithValue("@PageHeading", "") | Out-Null
$cmd.Parameters.AddWithValue("@CurrentPage", 1) | Out-Null
$cmd.Parameters.AddWithValue("@RecordPerPage", 5000) | Out-Null
$cmd.Parameters.AddWithValue("@TotalRecord", 0) | Out-Null
$cmd.Parameters.AddWithValue("@SortOrd", "ASC") | Out-Null
$cmd.Parameters.AddWithValue("@SortColumn", "PageHeading") | Out-Null
$cmd.Parameters.AddWithValue("@ModuleID", 0) | Out-Null
$cmd.Parameters.AddWithValue("@MenuName", "") | Out-Null
$cmd.Parameters.AddWithValue("@PagePath", "") | Out-Null
$cmd.Parameters.AddWithValue("@ActualName", "") | Out-Null
$cmd.Parameters.AddWithValue("@LevelNo", 0) | Out-Null
$cmd.Parameters.AddWithValue("@SequenceNo", 0) | Out-Null

$reader = $cmd.ExecuteReader()
$found = $false
while ($reader.Read()) {
    if ($reader['LinkID'] -eq 35) {
        $found = $true
        Write-Output "Found LinkID 35: Heading=$($reader['PageHeading']) | Parent=$($reader['ParentID']) | ModuleID=$($reader['ModuleID'])"
    }
}
if (-not $found) {
    Write-Output "LinkID 35 NOT returned by Z_UsersMenuList!"
}
$conn.Close()
