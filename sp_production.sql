Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE    PROCEDURE [dbo].[Inv_ItemStockPreProcessingAndProductModify]       
                                                                                                                                                                                
                                                                                                                                                                                
                                                                             
   
                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                          
   
@UsedFor int ,    
                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                           
                  
@UsedForId bigint ,   --PreProcessingID/ProductID 
                                                                                                                                                                                                           
                                                                                                                                                                                                           
                                                  
  
                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                           
  
@CreatedBy nvarchar(200) ,    
                                                                                                                                                                                                                               
                                                                                                                                                                                                                               
                              
@CreatedDate datetime,    
                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                   
                          
@ReturnVal int=0 output     
                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                 
                            
as                
                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                           
                  
begin                
                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                        
                     
set nocount on                
                                                                                                                                                                                                                               
                                                                                                                                                                                                                               
                              
set @ReturnVal = 0                
                                                                                                                                                                                                                           
                                                                                                                                                                                                                           
                                  
      
                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                       
      
   
                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                          
   
  
                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                           
  
    
                                                                                                                                                                                                                                                         
                                                                                                              
                                                                                                                                               
    --AFTER managing Stock of RAW materials
                                                                                                                                                                                                                  
                                                                                                                                                                                                                  
                                           
	--ADD Stock of PreProcessing and Processing Item
                                                                                                                                                                                                            
                                                                                                                                                                                                            
                                                 
--	const PurchaseID = 1;
                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                     
                        
--const Preprocessing = 2;
                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                   
                          
--const Production = 3;
                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                      
                       
Declare @BomItemId int,@BomItemQty int=0
                                                                                                                                                                                                                     
                                                                                                                                                                                                                     
                                        
	if(@UsedFor=3)
                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                              
               
	BEGIN
                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                       
      
	
                                                                                                                                                                                                                                                            
                                                                                                                                                                                                                                                            
 
Select @BomItemId=B.ItemId,@BomItemQty=(isnull(B.Quantity,0)*isnull(P.BomQty,0))
                                                                                                                                                                             
                                                                                                                                                                             
                                                                                
from W_Production P
                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                          
                   
inner join W_MasterBom B
                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                     
                        
on P.BomId=B.BomId where P.ProductionId=@UsedForId
                                                                                                                                                                                                           
                                                                                                                                                                                                           
                                                  

                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             

 insert into Inv_ItemStockByBatchForBOM(IDFrom,BomID,  ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)     
                                                                                                                     
                                                                                                                     
                                                                                                                                        
  --values (null, null, @BomItemId, @BomItemQty, @Unit, @BatchNo, @ExpiryDate, @WarehouseId, @Quantity)    
                                                                                                                                                  
                                                                                                                                                  
                                                                                                           
 Select @UsedForId,B.BomId,B.ItemId,(isnull(B.Quantity,0)*isnull(P.BomQty,0)),B.UnitId,P.BatchNo,P.ExpiryDate,P.WarehouseId,(isnull(B.Quantity,0)*isnull(P.BomQty,0))
                                                                                        
                                                                                        
                                                                                                                                                                     
from W_Production P
                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                          
                   
inner join W_MasterBom B
                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                     
                        
on P.BomId=B.BomId where P.ProductionId=@UsedForId
                                                                                                                                                                                                           
                                                                                                                                                                                                           
                                                  

                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             

 set @ReturnVal=scope_Identity()      
                                                                                                                                                                                                                       
                                                                                                                                                                                                                       
                                      
      
                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                       
      
  if @ReturnVal>0    
                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                        
                     
  begin    
                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                  
           
    
                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                         
    
 if not exists(select 1 from dbo.W_ItemStock where ( ItemID =@BomItemId  ))        
                                                                                                                                                                          
                                                                                                                                                                          
                                                                                   
Begin        
                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                
             
        
                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                     
        
insert into W_ItemStock(OpeningQuantity,PurchasedQuantity,ItemID,UnitId,IssuedQuantity,CreatedBy,FinalStock,OpeningStockDate,RemovedQuantity)  
                                                                                                              
                                                                                                              
                                                                                                                                               
Select 0,(isnull(B.Quantity,0)*isnull(P.BomQty,0)),B.ItemId,B.UnitId,0,@CreatedBy,(isnull(B.Quantity,0)*isnull(P.BomQty,0)),GetDate(),0
                                                                                                                      
                                                                             
                                                                                                                                                                                
from W_Production P
                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                          
                   
inner join W_MasterBom B
                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                     
                        
on P.BomId=B.BomId where P.ProductionId=@UsedForId   
                                                                                                                                                                                                        
                                                                                                                                                                                                        
                                                     
    
                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                         
    
End  
                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                        
     
Else 
                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                        
     
begin
                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                        
     
update W_ItemStock set PurchasedQuantity=PurchasedQuantity+@BomItemQty,FinalStock=FinalStock+@BomItemQty
                                                                                                                                                     
                                                                                                                                                     
                                                                                                        
where ItemID=@BomItemId
                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                      
                       
end
                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                          
   
     
                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                        
     
  end  
                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                      
       

                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             


                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             

	END
                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                         
    
	else if(@UsedFor=2)
                                                                                                                                                                                                                                         
                                                                                                                                                                                             
                                                                
	BEGIN
                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                       
      

                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             

Select @BomItemId=B.ItemId,@BomItemQty=(isnull(B.Quantity,0)*isnull(P.BomQty,0))
                                                                                                                                                                             
                                                                                                                                                                             
                                                                                
from W_PreProcessing P
                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                       
                      
inner join W_MasterBom B
                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                     
                        
on P.BomId=B.BomId where P.PreProcessingId=@UsedForId
                                                                                                                                                                                                        
                                                                                                                                                                                                        
                                                     

                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             

 insert into Inv_ItemStockByBatchForBOM(IDFrom,BomID,  ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)     
                                                                                                                     
                                                                                                                     
                                                                                                                                        
  --values (null, null, @BomItemId, @BomItemQty, @Unit, @BatchNo, @ExpiryDate, @WarehouseId, @Quantity)    
                                                                                                                                                  
                                                                                                                                                  
                                                                                                           
 Select @UsedForId,B.BomId,B.ItemId,(isnull(B.Quantity,0)*isnull(P.BomQty,0)),B.UnitId,P.BatchNumberMade,P.ExpiryDate,P.WarehouseId,(isnull(B.Quantity,0)*isnull(P.BomQty,0))
                                                                                
                                                                                
                                                                                                                                                                             
from W_PreProcessing P
                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                       
                      
inner join W_MasterBom B
                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                     
                        
on P.BomId=B.BomId where P.PreProcessingId=@UsedForId
                                                                                                                                                                                                        
                                                                                                                                                                                                        
                                                     

                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             

 set @ReturnVal=scope_Identity()      
                                                                                                                                                                                                                       
                                            
                                                                                                                                                                                                                 
      
                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                       
      
  if @ReturnVal>0    
                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                        
                     
  begin    
                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                  
           
    
                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                         
    
 if not exists(select 1 from dbo.W_ItemStock where ( ItemID =@BomItemId  ))        
                                                                                                                                                                          
                                                                                                                                                                          
                                                                                   
Begin        
                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                
             
        
                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                     
        
insert into W_ItemStock(OpeningQuantity,PurchasedQuantity,ItemID,UnitId,IssuedQuantity,CreatedBy,FinalStock,OpeningStockDate,RemovedQuantity)  
                                                                                                              
                                                                                                              
                                                                                                                                               
Select 0,(isnull(B.Quantity,0)*isnull(P.BomQty,0)),B.ItemId,B.UnitId,0,@CreatedBy,(isnull(B.Quantity,0)*isnull(P.BomQty,0)),GetDate(),0
                                                                                                                      
                                                                                                                      
                                                                                                                                       
from W_PreProcessing P
                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                       
                      
inner join W_MasterBom B
                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                     
                        
on P.BomId=B.BomId where P.PreProcessingId=@UsedForId   
                                                                                                                                                                                                     
                                                                                                                                                                                                     
                                                        
    
                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                         
    
End  
                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                        
     
Else 
                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                        
     
begin
                                                                                                                                                                                                                                                        
                                                                                                                                                            
                                                                                                 
update W_ItemStock set PurchasedQuantity=PurchasedQuantity+@BomItemQty,FinalStock=FinalStock+@BomItemQty
                                                                                                                                                     
                                                                                                                                                     
                                                                                                        
where ItemID=@BomItemId
                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                      
                       
end
                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                          
   
     
                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                        
     
  end  
                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                      
       

                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             


                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                             

	END
                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                         
    
        
                                                                                                                                                                                                                                                     
    
                                                                                                                                                                                                                                                         
    -- =======================================================
                                                                                                                                                                                               
    -- CALCULATE AND APPLY TOTAL MANUFACTURING COST
                                                                                                                                                                                                          
    -- =======================================================
                                                                                                                                                                                               
    DECLARE @TotalCost FLOAT;
                                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
    SELECT @TotalCost = SUM( u.Quantity * (ISNULL(b.Amount, 0) / NULLIF(b.Quantity, 0)) )
                                                                                                                                                                    
    FROM Inv_ItemStockUsed u
                                                                                                                                                                                                                                 
    INNER JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
                                                                                                                                                                         
    WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId;
                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
    -- Update the output batch created for this manufacturing run
                                                                                                                                                                                            
    UPDATE Inv_ItemStockByBatch
                                                                                                                                                                                                                              
    SET Amount = @TotalCost
                                                                                                                                                                                                                                  
    WHERE IdFrom = @UsedForId AND StockById = 2; -- (2 = Produced/PreProcessed)
                                                                                                                                                                              

                                                                                                                                                                                                                                                             
END
                                                                                                                                                                                                                                                          
