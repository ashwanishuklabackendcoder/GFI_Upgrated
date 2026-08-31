Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Inv_ItemStockByBatchModify]       
                                                                                                                                                                                                   
@PurchaseID bigint=0,    
                                                                                                                                                                                                                                    
@ItemStockByBatchId bigint ,    
                                                                                                                                                                                                                             
@StockById int ,    
                                                                                                                                                                                                                                         
@ItemId bigint ,    
                                                                                                                                                                                                                                         
@Quantity float ,    
                                                                                                                                                                                                                                        
@BatchNo nvarchar(50) ,    
                                                                                                                                                                                                                                  
@ExpiryDate date ,    
                                                                                                                                                                                                                                       
@WarehouseId bigint ,    
                                                                                                                                                                                                                                    
@IdFrom bigint=0,
                                                                                                                                                                                                                                            
@Amount float=0,    
                                                                                                                                                                                                                                         
@ReturnVal int=0 output      
                                                                                                                                                                                                                                
    
                                                                                                                                                                                                                                                         
as                
                                                                                                                                                                                                                                           
begin                
                                                                                                                                                                                                                                        
set nocount on                
                                                                                                                                                                                                                               
set @ReturnVal = 0                
                                                                                                                                                                                                                           
      
                                                                                                                                                                                                                                                       
DECLARE @Unit bigint , @GoodReceiveDate date,@TentativeExpiryDays int,@ShortName nvarchar(10)    
                                                                                                                                                            

                                                                                                                                                                                                                                                             
IF ISNULL(@IdFrom, 0) = 0
                                                                                                                                                                                                                                    
BEGIN
                                                                                                                                                                                                                                                        
  SELECT @IdFrom=PurchaseItemID FROM W_PurchaseChild WHERE PurchaseID=@PurchaseID AND ItemID=@ItemID
                                                                                                                                                         
END
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
SELECT @Unit=UnitId FROM W_PurchaseChild WHERE PurchaseItemID=@IdFrom
                                                                                                                                                                                        
IF ISNULL(@Unit, 0) = 0
                                                                                                                                                                                                                                      
BEGIN
                                                                                                                                                                                                                                                        
  SELECT @Unit=UnitId FROM W_PurchaseChild WHERE PurchaseID=@PurchaseID AND ItemID=@ItemID
                                                                                                                                                                   
END
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
if(isnull(@WarehouseId,0)=0)
                                                                                                                                                                                                                                 
BEGIN
                                                                                                                                                                                                                                                        
  SELECT TOP 1 @WarehouseId = WarehouseId FROM dbo.W_Warehouse ORDER BY WarehouseId;
                                                                                                                                                                         
END
                                                                                                                                                                                                                                                          
    
                                                                                                                                                                                                                                                         
IF @ItemStockByBatchId = 0                
                                                                                                                                                                                                                   
  Begin                
                                                                                                                                                                                                                                      
   if not exists(select 1 from dbo.Inv_ItemStockByBatch where ( ItemStockByBatchId =@ItemStockByBatchId ) AND BatchNo=@BatchNo)                
                                                                                                              
    Begin       
                                                                                                                                                                                                                                             
    
                                                                                                                                                                                                                                                         
  SELECT @TentativeExpiryDays=TentativeExpiryDays,@ShortName=ShortName FROM W_MasterItem WHERE ItemID=@ItemId    
                                                                                                                                            
  SELECT @GoodReceiveDate=GoodsRecievedDate FROM W_PurchaseMaster WHERE PurchaseID=@PurchaseID    
                                                                                                                                                           
  
                                                                                                                                                                                                                                                           
  if(@ExpiryDate is null)
                                                                                                                                                                                                                                    
    SET @ExpiryDate = DATEADD(DAY, isnull(@TentativeExpiryDays, 365), isnull(@GoodReceiveDate, getdate()));     
                                                                                                                                             
    
                                                                                                                                                                                                                                                         
  if(isnull(@BatchNo,'')='')  
                                                                                                                                                                                                                               
    SET @BatchNo = @ShortName +' | ' + FORMAT(isnull(@GoodReceiveDate, getdate()), 'yyMMdd') ;    
                                                                                                                                                           
    
                                                                                                                                                                                                                                                         
  insert into Inv_ItemStockByBatch(StockById, IdFrom, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft, Amount)     
                                                                                                              
  values (@StockById, isnull(@IdFrom,0), @ItemId, @Quantity, @Unit, @BatchNo, @ExpiryDate, @WarehouseId, @Quantity, @Amount)    
                                                                                                                             
    
                                                                                                                                                                                                                                                         
  set @ReturnVal=scope_Identity()                
                                                                                                                                                                                                            
    End                
                                                                                                                                                                                                                                      
   else                
                                                                                                                                                                                                                                      
    set @ReturnVal= -1                
                                                                                                                                                                                                                       
  End                
                                                                                                                                                                                                                                        
Else                
                                                                                                                                                                                                                                         
  Begin        
                                                                                                                                                                                                                                              
   if not exists(select 1 from dbo.Inv_ItemStockByBatch where  (ItemStockByBatchId = @ItemStockByBatchId) and ItemId<>@ItemId)                
                                                                                                               
    Begin     
                                                                                                                                                                                                                                               
    
                                                                                                                                                                                                                                                         
     update Inv_ItemStockByBatch     
                                                                                                                                                                                                                        
     set StockById=@StockById, IdFrom=isnull(@IdFrom,0), ItemId=@ItemId, Quantity=@Quantity,     
                                                                                                                                                            
     Unit=@Unit, BatchNo=@BatchNo, ExpiryDate=@ExpiryDate, WarehouseId=@WarehouseId, Amount=@Amount    
                                                                                                                                                      
     where ItemStockByBatchId=@ItemStockByBatchId    
                                                                                                                                                                                                        
                
                                                                                                                                                                                                                                             
     set @ReturnVal =@ItemStockByBatchId                
                                                                                                                                                                                                     
    End                
                                                                                                                                                                                                                                      
   else                
                                                                                                                                                                                                                                      
     set @ReturnVal= @ItemStockByBatchId               
                                                                                                                                                                                                      
  End                
                                                                                                                                                                                                                                        
END                                                                                                                                                                                                                                                            
