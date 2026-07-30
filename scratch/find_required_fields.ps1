$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# We want tables that are related to Account or Purchase modules.
# Let's query tables and their non-nullable columns.
$query = @"
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.max_length AS MaxLength
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE c.is_nullable = 0 
  AND c.is_identity = 0
  AND c.is_computed = 0
  AND (
      t.name LIKE '%Acc%' OR 
      t.name LIKE '%Pur%' OR 
      t.name LIKE '%Cust%' OR 
      t.name LIKE '%Vend%' OR 
      t.name LIKE '%Invoice%' OR 
      t.name LIKE '%Order%' OR
      t.name LIKE '%Bill%'
  )
ORDER BY t.name, c.column_id
"@

$command = $connection.CreateCommand()
$command.CommandText = $query
$reader = $command.ExecuteReader()

Write-Output "=== Non-Nullable Columns in Account & Purchase Tables ==="
$currentTable = ""
while ($reader.Read()) {
    $tableName = $reader["TableName"].ToString()
    if ($tableName -ne $currentTable) {
        $currentTable = $tableName
        Write-Output "`nTable: $currentTable"
    }
    $colName = $reader["ColumnName"].ToString()
    $dataType = $reader["DataType"].ToString()
    $maxLen = $reader["MaxLength"].ToString()
    Write-Output "  - $colName ($dataType, max_len: $maxLen)"
}
$reader.Close()
$connection.Close()
