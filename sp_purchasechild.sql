Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_PurchaseChildModify]            
                                                                                                                                                                                                   
@PurchaseItemID bigint=0,            
                                                                                                                                                                                                                        
@PurchaseID bigint,
                                                                                                                                                                                                                                          
@BrandID bigint,
                                                                                                                                                                                                                                             
@ItemID bigint,
                                                                                                                                                                                                                                              
@Quantity Float,
                                                                                                                                                                                                                                             
@UnitId bigint,
                                                                                                                                                                                                                                              
@UnitPrice float,
                                                                                                                                                                                                                                            
@Amount float,
                                                                                                                                                                                                                                               
@Description NVarChar(1000),
                                                                                                                                                                                                                                 
@CreatedBy NVarChar(200),
                                                                                                                                                                                                                                    
@ReturnVal int=0 output            
                                                                                                                                                                                                                          
as            
                                                                                                                                                                                                                                               
begin            
                                                                                                                                                                                                                                            
set nocount on            
                                                                                                                                                                                                                                   
set @ReturnVal = 0            
                                                                                                                                                                                                                               
  
                                                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
            
                                                                                                                                                                                                                                                 
IF @PurchaseItemID = 0            
                                                                                                                                                                                                                           
Begin            
                                                                                                                                                                                                                                            
if not exists(select 1 from dbo.W_PurchaseChild where ( PurchaseItemID =@PurchaseItemID ))            
                                                                                                                                                       
Begin   
                                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
insert into W_PurchaseChild(PurchaseID,BrandID,ItemID,Quantity,UnitId,UnitPrice,Amount,Description,CreatedBy) 
                                                                                                                                               
values (@PurchaseID,@BrandID,@ItemID,@Quantity,@UnitId,@UnitPrice,@Amount,@Description,@CreatedBy)
                                                                                                                                                           

                                                                                                                                                                                                                                                             
set @ReturnVal=scope_Identity()   
                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
if @ReturnVal>0
                                                                                                                                                                                                                                              
 begin
                                                                                                                                                                                                                                                       
 if(isnull((select count(*) from W_ItemStock where ItemID in(select ItemID from dbo.W_PurchaseChild where PurchaseItemID=@ReturnVal)),0)>0)
                                                                                                                  
 begin 
                                                                                                                                                                                                                                                      
 update t1 set PurchasedQuantity=isnull(PurchasedQuantity,0)+ isnull(t2.Quantity,0) from W_ItemStock t1 inner join W_PurchaseChild t2 on t1.ItemID=t2.ItemID where t2.PurchaseItemID=@ReturnVal
                                                              
 end
                                                                                                                                                                                                                                                         
 else
                                                                                                                                                                                                                                                        
 begin
                                                                                                                                                                                                                                                       
 insert  into W_ItemStock(ItemID	,WarehouseID,	OpeningQuantity,	OpeningStockDate,	PurchasedQuantity,	IssuedQuantity,	RemovedQuantity,	UnitId,	FinalStock,	CreatedBy)
                                                                                         
 select ItemID,null,null,null,Quantity,null,null,UnitId,Quantity,@CreatedBy from dbo.W_PurchaseChild where PurchaseItemID=@ReturnVal
                                                                                                                         
 end
                                                                                                                                                                                                                                                         
 end
                                                                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
End            
                                                                                                                                                                                                                                              
else            
                                                                                                                                                                                                                                             
set @ReturnVal= -1            
                                                                                                                                                                                                                               
End            
                                                                                                                                                                                                                                              
Else            
                                                                                                                                                                                                                                             
Begin            
                                                                                                                                                                                                                                            
if exists(select 1 from dbo.W_PurchaseChild where  PurchaseItemID = @PurchaseItemID)            
                                                                                                                                                             
Begin                
                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
update t1 set PurchasedQuantity=(isnull(PurchasedQuantity,0)-isnull(t2.Quantity,0)) +@Quantity from W_ItemStock t1 
                                                                                                                                          
inner join W_PurchaseChild t2 on t1.ItemID=t2.ItemID where t2.PurchaseItemID=@PurchaseItemID
                                                                                                                                                                 

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
update W_PurchaseChild 
                                                                                                                                                                                                                                      
set PurchaseID=@PurchaseID,BrandID=@BrandID,ItemID=@ItemID,Quantity=@Quantity,
                                                                                                                                                                               
UnitId=@UnitId,UnitPrice=@UnitPrice,Amount=@Amount,Description=@Description
                                                                                                                                                                                  
 where PurchaseItemID=@PurchaseItemID
                                                                                                                                                                                                                        
            
                                                                                                                                                                                                                                                 
set @ReturnVal =@PurchaseItemID  
                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
End            
                                                                                                                                                                                                                                              
else            
                                                                                                                                                                                                                                             
set @ReturnVal= @PurchaseItemID           
                                                                                                                                                                                                                   
End            
                                                                                                                                                                                                                                              
 
                                                                                                                                                                                                                                                            
 
                                                                                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
End                                                                                                                                                                                                                                                            
