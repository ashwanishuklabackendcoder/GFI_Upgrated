Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_ItemStockModify]      
                                                                                                                                                                                                             
@StockID bigint=0,      
                                                                                                                                                                                                                                     
@OpeningQuantity float,
                                                                                                                                                                                                                                      
@PurchasedQuantity float,
                                                                                                                                                                                                                                    
@ItemID int,
                                                                                                                                                                                                                                                 
@WarehouseID int,
                                                                                                                                                                                                                                            
@UnitId int,
                                                                                                                                                                                                                                                 
@IssuedQuantity float,
                                                                                                                                                                                                                                       
@CreatedBy NVarChar(200),
                                                                                                                                                                                                                                    
@FinalStock float  ,
                                                                                                                                                                                                                                         
@OpeningStockDate datetime,
                                                                                                                                                                                                                                  
@RemovedQuantity float,
                                                                                                                                                                                                                                      
@ReturnVal int=0 output      
                                                                                                                                                                                                                                
as      
                                                                                                                                                                                                                                                     
begin      
                                                                                                                                                                                                                                                  
set nocount on      
                                                                                                                                                                                                                                         
set @ReturnVal = 0      
                                                                                                                                                                                                                                     
      
                                                                                                                                                                                                                                                       
      
                                                                                                                                                                                                                                                       
IF @StockID = 0      
                                                                                                                                                                                                                                        
Begin      
                                                                                                                                                                                                                                                  
if not exists(select 1 from dbo.W_ItemStock where ( ItemID =@ItemID and WarehouseID=@WarehouseID ))      
                                                                                                                                                    
Begin      
                                                                                                                                                                                                                                                  
      
                                                                                                                                                                                                                                                       
insert into W_ItemStock(OpeningQuantity,PurchasedQuantity,ItemID,WarehouseID,UnitId,IssuedQuantity,CreatedBy,FinalStock,OpeningStockDate,RemovedQuantity)
                                                                                                    
values (@OpeningQuantity,@PurchasedQuantity,@ItemID,@WarehouseID,1,@IssuedQuantity,@CreatedBy,@FinalStock,@OpeningStockDate,@RemovedQuantity)    
                                                                                                            
set @ReturnVal=scope_Identity()      
                                                                                                                                                                                                                        
End      
                                                                                                                                                                                                                                                    
else      
                                                                                                                                                                                                                                                   
set @ReturnVal= -1      
                                                                                                                                                                                                                                     
End      
                                                                                                                                                                                                                                                    
Else      
                                                                                                                                                                                                                                                   
Begin      
                                                                                                                                                                                                                                                  
if not exists(select 1 from dbo.W_ItemStock where ( ItemID =@ItemID and WarehouseID=@WarehouseID) and StockID <> @StockID)      
                                                                                                                             
Begin     
                                                                                                                                                                                                                                                   
update W_ItemStock set
                                                                                                                                                                                                                                       
OpeningQuantity=@OpeningQuantity,
                                                                                                                                                                                                                            
PurchasedQuantity=@PurchasedQuantity,
                                                                                                                                                                                                                        
ItemID=@ItemID,
                                                                                                                                                                                                                                              
WarehouseID=@WarehouseID,
                                                                                                                                                                                                                                    
RemovedQuantity=@RemovedQuantity,
                                                                                                                                                                                                                            
UnitId=1,
                                                                                                                                                                                                                                                    
IssuedQuantity=@IssuedQuantity,
                                                                                                                                                                                                                              
OpeningStockDate=@OpeningStockDate,
                                                                                                                                                                                                                          
FinalStock=@FinalStock where StockID=@StockID
                                                                                                                                                                                                                
   
                                                                                                                                                                                                                                                          
      
                                                                                                                                                                                                                                                       
set @ReturnVal =@StockID      
                                                                                                                                                                                                                               
End      
                                                                                                                                                                                                                                                    
else      
                                                                                                                                                                                                                                                   
set @ReturnVal= -1      
                                                                                                                                                                                                                                     
End      
                                                                                                                                                                                                                                                    
      
                                                                                                                                                                                                                                                       
End                                                                                                                                                                                                                                                            
