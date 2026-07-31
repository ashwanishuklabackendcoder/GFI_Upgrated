$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()
$transaction = $connection.BeginTransaction()

try {
    # 1. Alter W_MasterStatusModify stored procedure
    Write-Output "Altering W_MasterStatusModify stored procedure..."
    $command = $connection.CreateCommand()
    $command.Transaction = $transaction
    $command.CommandText = @"
ALTER PROCEDURE [dbo].[W_MasterStatusModify]        
@StatusID bigint=0,        
@StatusName NVarChar(500),
@StatusOf int=0,
@IsEditable bit,  
@IsActive bit,  
@CreatedDate DateTime,  
@CreatedBy NVarChar(200),  
@ReturnVal int=0 output        
as        
begin        
set nocount on        
set @ReturnVal = 0        
        
IF @StatusID = 0        
Begin        
if not exists(select 1 from dbo.W_MasterStatus where StatusName = @StatusName and StatusOf = @StatusOf)        
Begin        
insert into W_MasterStatus(StatusName,StatusOf,IsEditable,IsActive,CreatedDate,CreatedBy)   
values (@StatusName,@StatusOf,@IsEditable,@IsActive,@CreatedDate,@CreatedBy)  
set @ReturnVal=scope_Identity()        
End        
else        
set @ReturnVal= -1        
End        
Else        
Begin        
if not exists(select 1 from dbo.W_MasterStatus where StatusName = @StatusName and StatusOf = @StatusOf and StatusID <> @StatusID)        
Begin        
update W_MasterStatus  
set StatusName=@StatusName,IsEditable=@IsEditable,  
CreatedDate=@CreatedDate,CreatedBy=@CreatedBy where StatusID=@StatusID  
        
set @ReturnVal =@StatusID        
End        
else        
set @ReturnVal= -1        
End        
        
End
"@
    $command.ExecuteNonQuery()
    Write-Output "Successfully altered stored procedure."

    # 2. Insert Active status (StatusOf = 4) if not exists
    $command.CommandText = "IF NOT EXISTS(SELECT 1 FROM W_MasterStatus WHERE StatusName='Active' AND StatusOf=4) BEGIN INSERT INTO W_MasterStatus(StatusName,StatusOf,IsEditable,IsActive,CreatedDate,CreatedBy) VALUES('Active', 4, 1, 1, GETDATE(), 'System') END"
    $command.ExecuteNonQuery()

    # Get Active StatusID
    $command.CommandText = "SELECT StatusID FROM W_MasterStatus WHERE StatusName='Active' AND StatusOf=4"
    $activeId = $command.ExecuteScalar()
    Write-Output "Active Status ID for Staff: $activeId"

    # 3. Insert Inactive status (StatusOf = 4) if not exists
    $command.CommandText = "IF NOT EXISTS(SELECT 1 FROM W_MasterStatus WHERE StatusName='Inactive' AND StatusOf=4) BEGIN INSERT INTO W_MasterStatus(StatusName,StatusOf,IsEditable,IsActive,CreatedDate,CreatedBy) VALUES('Inactive', 4, 1, 1, GETDATE(), 'System') END"
    $command.ExecuteNonQuery()

    # Get Inactive StatusID
    $command.CommandText = "SELECT StatusID FROM W_MasterStatus WHERE StatusName='Inactive' AND StatusOf=4"
    $inactiveId = $command.ExecuteScalar()
    Write-Output "Inactive Status ID for Staff: $inactiveId"

    # 4. Update Hr_StaffMaster status links
    Write-Output "Updating Hr_StaffMaster record statuses..."
    $command.CommandText = "UPDATE Hr_StaffMaster SET Status = $activeId WHERE Status = 1"
    $rows = $command.ExecuteNonQuery()
    Write-Output "Updated $rows staff record(s) from Status 1 to Status $activeId"

    $command.CommandText = "UPDATE Hr_StaffMaster SET Status = $inactiveId WHERE Status = 2"
    $rows2 = $command.ExecuteNonQuery()
    Write-Output "Updated $rows2 staff record(s) from Status 2 to Status $inactiveId"

    $transaction.Commit()
    Write-Output "Transaction committed successfully."
}
catch {
    $transaction.Rollback()
    Write-Error $_.Exception.Message
}

$connection.Close()
