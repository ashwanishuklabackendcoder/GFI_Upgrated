$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

# 1. Update W_ProductionModify
$prodSp = @"
ALTER PROCEDURE [dbo].[W_ProductionModify]      
    @ProductionId bigint,
    @BomId bigint,
    @CookingDate Date,
    @FilledDate Date,
    @ExpiryDate Date,
    @PackedCountry NVarChar(200),
    @SkuId NVarChar(100),
    @Colli NVarChar(100),
    @PalletNumber NVarChar(100),
    @ProcessEmployees NVarChar(200),
    @KettleId NVarChar(200),
    @KettleRun Int,
    @BatchNo NVarChar(200),
    @FillingBottles Int,
    @FillingPerBottleUnit bigint,
    @ExtraBottles float,
    @Remarks NVarChar(1000),
    @DocumentUpload NVarChar(500),
    @WarehouseId bigint,
    @BomQty float,
    @ReturnVal int=0 output      
As      
begin      
    set nocount on      
    set @ReturnVal = 0      
      
    IF @ProductionId = 0      
    Begin      
        if not exists(select 1 from dbo.W_Production where ( ProductionId =0 ))      
        Begin  

            declare @ShortName nvarchar(10), @KettleNo nvarchar(500)
            select @ShortName=b.ShortName from W_MasterBom a inner join W_MasterItem b on a.ItemId=b.ItemID where BomId=@BomId
            
            set @KettleNo = ''
            if (isnull(@KettleId, '') <> '')
            BEGIN
                SELECT @KettleNo = @KettleNo + KettleNumber + ', ' 
                FROM W_MasterKettle 
                WHERE KettleId IN (SELECT CAST(value AS BIGINT) FROM STRING_SPLIT(@KettleId, ','))

                if (LEN(@KettleNo) > 1)
                    set @KettleNo = SUBSTRING(@KettleNo, 1, LEN(@KettleNo) - 1)
            END

            declare @Batch nvarchar(100)
            IF (TRIM(ISNULL(@BatchNo, '')) != '')
                SET @Batch = @BatchNo
            ELSE
                SET @Batch = @ShortName +' | ' + FORMAT(@CookingDate, 'yyMMdd') + '.' + CAST(@KettleNo AS NVARCHAR(100)) + '.' + CAST(@KettleRun AS NVARCHAR(10));

            insert into W_Production(BomId,CookingDate,FilledDate,ExpiryDate,PackedCountry,SkuId,Colli,PalletNumber,ProcessEmployees,KettleId,KettleRun,BatchNo,FillingBottles,FillingPerBottleUnit,ExtraBottles,Remarks,DocumentUpload,WarehouseId,BomQty)
            values (@BomId,@CookingDate,@FilledDate,@ExpiryDate,@PackedCountry,@SkuId,@Colli,@PalletNumber,@ProcessEmployees,@KettleId,@KettleRun,@Batch,@FillingBottles,@FillingPerBottleUnit,@ExtraBottles,@Remarks,@DocumentUpload,@WarehouseId,@BomQty)
            set @ReturnVal=scope_Identity()  

        End      
        else      
            set @ReturnVal= -1      
    End      
    Else      
    Begin      
        if exists(select 1 from dbo.W_Production where ProductionId = @ProductionId)      
        Begin     
            update W_Production set BomId=@BomId,BomQty=@BomQty,CookingDate=@CookingDate,FilledDate=@FilledDate,
            ExpiryDate=@ExpiryDate,PackedCountry=@PackedCountry,SkuId=@SkuId,Colli=@Colli,PalletNumber=@PalletNumber,
            ProcessEmployees=@ProcessEmployees,KettleId=@KettleId,KettleRun=@KettleRun,
            BatchNo=@BatchNo,FillingBottles=@FillingBottles,
            FillingPerBottleUnit=@FillingPerBottleUnit,ExtraBottles=@ExtraBottles,Remarks=@Remarks,DocumentUpload=@DocumentUpload,WarehouseId=@WarehouseId where ProductionId=@ProductionId
      
            set @ReturnVal =@ProductionId      
        End      
        else      
            set @ReturnVal= -1      
    End      
end
"@

$command = $connection.CreateCommand()
$command.CommandText = $prodSp
$command.ExecuteNonQuery()
Write-Output "Successfully updated stored procedure: W_ProductionModify"

# 2. Update W_PreProcessingModify
$preSp = @"
ALTER PROCEDURE [dbo].[W_PreProcessingModify]              
@PreProcessingId bigint=0,              
@BomId bigint=0,
@BomQty float=0,
@ProcessingDate Date,
@QuantityMade float,
@UnitMade bigint,
@BatchNumberMade NVarChar(200),
@ExpiryDate DateTime,
@ProcessEmployees NVarChar(200),
@Remarks NVarChar(2000),
@DocumentUpload NVarChar(1000),
@WarehouseId bigint,
@CreatedBy NVarChar(200),
@ReturnVal int=0 output              
as              
begin              
set nocount on              
set @ReturnVal = 0              
              
              
IF @PreProcessingId = 0              
Begin              
if not exists(select 1 from dbo.W_PreProcessing where (PreProcessingId =@PreProcessingId))              
Begin              

declare @ShortName nvarchar(10),@ItemID INT,@TentativeExpiryDays int,@Expiry date
select @ShortName=b.ShortName,@ItemID =b.ItemID, @TentativeExpiryDays=b.TentativeExpiryDays 
from W_MasterBom a inner join W_MasterItem b on a.ItemId=b.ItemID 
where BomId=@BomId

-- Fix: Handle NULL TentativeExpiryDays by falling back to the passed @ExpiryDate or a default
SET @Expiry = DATEADD(DAY, ISNULL(@TentativeExpiryDays, 365), @ProcessingDate); 
IF @Expiry IS NULL SET @Expiry = ISNULL(@ExpiryDate, GETDATE());
		

DECLARE @Prefix INT;

begin
           SET @Prefix = (SELECT COUNT(*)+1 FROM W_PreProcessing  a inner join W_MasterBom b on a.BomId=b.BomId 
           inner join W_MasterItem c on b.ItemId=c.ItemID
		   where b.ItemID =@ItemID and a.ProcessingDate=@ProcessingDate);
end


declare @Batch nvarchar(100)
    IF (TRIM(ISNULL(@BatchNumberMade, '')) != '')
        SET @Batch = @BatchNumberMade
    ELSE
        SET @Batch = ISNULL(@ShortName, 'PRD') +' | ' + FORMAT(@ProcessingDate, 'yyMMdd') + '.' + CAST(@Prefix AS NVARCHAR(10)) ;

insert into W_PreProcessing(BomId,BomQty,ProcessingDate,QuantityMade,UnitMade,BatchNumberMade,ExpiryDate,ProcessEmployees,Remarks,DocumentUpload,WarehouseId,CreatedBy)
values (@BomId,@BomQty,@ProcessingDate,@QuantityMade,@UnitMade,@Batch,@Expiry,@ProcessEmployees,@Remarks,@DocumentUpload,@WarehouseId,@CreatedBy)       
set @ReturnVal=scope_Identity()         

SELECT @ItemId=ItemId,@UnitMade=UnitId FROM W_MasterBom WHERE BomId=@BomId
insert into Inv_ItemStockByBatch(StockById, IdFrom, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)
values (2, @ReturnVal, @ItemId, @QuantityMade, @UnitMade, @Batch, @Expiry, @WarehouseId, @QuantityMade)


End              
else              
set @ReturnVal= -1              
End              
Else              
Begin              
if  exists(select 1 from dbo.W_PreProcessing where  PreProcessingId = @PreProcessingId)              
Begin              
update W_PreProcessing 
set BomId=@BomId,BomQty=@BomQty,ProcessingDate=@ProcessingDate,QuantityMade=@QuantityMade,
UnitMade=@UnitMade,ExpiryDate=@ExpiryDate,
ProcessEmployees=@ProcessEmployees,Remarks=@Remarks,DocumentUpload=@DocumentUpload,
WarehouseId=@WarehouseId where PreProcessingId=@PreProcessingId
set @ReturnVal =@PreProcessingId              
End              
else              
set @ReturnVal= -1              
End              
              
End
"@

$command = $connection.CreateCommand()
$command.CommandText = $preSp
$command.ExecuteNonQuery()
Write-Output "Successfully updated stored procedure: W_PreProcessingModify"

$connection.Close()
