Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_PreProcessingModify]              
                                                                                                                                                                                                 
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
                                                                                                                                                                                                                                 
    IF (LTRIM(RTRIM(ISNULL(@BatchNumberMade, ''))) != '')
                                                                                                                                                                                                    
        SET @Batch = @BatchNumberMade;
                                                                                                                                                                                                                       
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
