$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

Write-Output "Creating Z_EmailLog table if not exists..."
$command = $connection.CreateCommand()
$command.CommandText = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Z_EmailLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Z_EmailLog] (
        [EmailLogID] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [RecipientEmail] NVARCHAR(200) NOT NULL,
        [Subject] NVARCHAR(500) NOT NULL,
        [Body] NVARCHAR(MAX) NOT NULL,
        [SentDate] DATETIME NOT NULL,
        [IsSuccess] BIT NOT NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [StaffId] BIGINT NULL
    )
END
"@
$command.ExecuteNonQuery()
Write-Output "Successfully created Z_EmailLog table."

$connection.Close()
