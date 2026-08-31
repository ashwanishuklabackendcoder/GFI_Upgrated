Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_ProductionOperation]  
                                                                                                                                                                                                             
@ID varchar(2000)='',  
                                                                                                                                                                                                                                      
@OprType smallint=2,  
                                                                                                                                                                                                                                       
@UpdatedBy nvarchar(200)='',  
                                                                                                                                                                                                                               
@Iserror int=0 output  
                                                                                                                                                                                                                                      
as  
                                                                                                                                                                                                                                                         
begin  
                                                                                                                                                                                                                                                      
set nocount on  
                                                                                                                                                                                                                                             
if @OprType=1   
                                                                                                                                                                                                                                             
begin  
                                                                                                                                                                                                                                                      
    set @Iserror=1  
                                                                                                                                                                                                                                         
    
                                                                                                                                                                                                                                                         
    DECLARE @ProductionId BIGINT
                                                                                                                                                                                                                             
    DECLARE id_cursor CURSOR FOR 
                                                                                                                                                                                                                            
    SELECT items FROM dbo.Fun_SplitStr(@ID,',')
                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
    OPEN id_cursor
                                                                                                                                                                                                                                           
    FETCH NEXT FROM id_cursor INTO @ProductionId
                                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
    WHILE @@FETCH_STATUS = 0
                                                                                                                                                                                                                                 
    BEGIN
                                                                                                                                                                                                                                                    
        DECLARE @ItemId BIGINT, @TotalQty FLOAT, @IsComplete INT
                                                                                                                                                                                             
      
                                                                                                                                                                                                                                                       
        -- Fetch item and actual produced quantity
                                                                                                                                                                                                           
        SELECT @ItemId = ItemID, @IsComplete = ISNULL(IsComplete, 0), @TotalQty = (ISNULL(FillingBottles, 0) * ISNULL(FillingPerBottleUnit, 0)) + ISNULL(ExtraBottles, 0)
                                                                                    
        FROM dbo.W_Production
                                                                                                                                                                                                                                
        WHERE ProductionId = @ProductionId
                                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
        IF @ItemId IS NOT NULL
                                                                                                                                                                                                                               
        BEGIN
                                                                                                                                                                                                                                                
            IF @IsComplete = 1
                                                                                                                                                                                                                               
            BEGIN
                                                                                                                                                                                                                                            
                -- Revert ingredient batch stock
                                                                                                                                                                                                             
                UPDATE b
                                                                                                                                                                                                                                     
                SET b.FinalQuantityLeft = b.FinalQuantityLeft + u.Quantity
                                                                                                                                                                                   
                FROM dbo.Inv_ItemStockByBatch b
                                                                                                                                                                                                              
                INNER JOIN dbo.Inv_ItemStockUsed u ON b.ItemStockByBatchId = u.ItemStockByBatchId
                                                                                                                                                            
                WHERE u.UsedFor = 3 AND u.UsedForId = @ProductionId;
                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
                -- Revert ingredient main stock (deterministic update using grouped SUM)
                                                                                                                                                                     
                UPDATE s
                                                                                                                                                                                                                                     
                SET s.IssuedQuantity = ISNULL(s.IssuedQuantity,0) - t.TotalQtyUsed
                                                                                                                                                                           
                FROM dbo.W_ItemStock s
                                                                                                                                                                                                                       
                INNER JOIN (
                                                                                                                                                                                                                                 
                    SELECT b.ItemId, SUM(ISNULL(u.Quantity, 0)) AS TotalQtyUsed
                                                                                                                                                                              
                    FROM dbo.Inv_ItemStockUsed u
                                                                                                                                                                                                             
                    INNER JOIN dbo.Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
                                                                                                                                                     
                    WHERE u.UsedFor = 3 AND u.UsedForId = @ProductionId
                                                                                                                                                                                      
                    GROUP BY b.ItemId
                                                                                                                                                                                                                        
                ) t ON s.ItemID = t.ItemId;
                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
                -- Revert produced main stock (subtract from ProducedQuantity instead of PurchasedQuantity)
                                                                                                                                                  
                UPDATE dbo.W_ItemStock
                                                                                                                                                                                                                       
                SET 
                                                                                                                                                                                                                                         
                    ProducedQuantity = ISNULL(ProducedQuantity,0) - @TotalQty,
                                                                                                                                                                               
                    FinalStock = ISNULL(FinalStock,0) - @TotalQty
                                                                                                                                                                                            
                WHERE ItemID = @ItemId
                                                                                                                                                                                                                       
            END
                                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
            DELETE FROM dbo.Inv_ItemStockUsed
                                                                                                                                                                                                                
            WHERE UsedFor = 3 AND UsedForId = @ProductionId
                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
            -- Correctly delete produced batch from Inv_ItemStockByBatch (StockById = 4 for production)
                                                                                                                                                      
            DELETE FROM dbo.Inv_ItemStockByBatch
                                                                                                                                                                                                             
            WHERE IdFrom = @ProductionId AND StockById = 4
                                                                                                                                                                                                   

                                                                                                                                                                                                                                                             
            -- Delete produced batch from Inv_ItemStockByBatchForBOM
                                                                                                                                                                                         
            DELETE FROM dbo.Inv_ItemStockByBatchForBOM
                                                                                                                                                                                                       
            WHERE IDFrom = @ProductionId
                                                                                                                                                                                                                     
        END
                                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
        DELETE FROM dbo.W_Production WHERE ProductionId = @ProductionId
                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
        FETCH NEXT FROM id_cursor INTO @ProductionId
                                                                                                                                                                                                         
    END
                                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
    CLOSE id_cursor
                                                                                                                                                                                                                                          
    DEALLOCATE id_cursor
                                                                                                                                                                                                                                     
end  
                                                                                                                                                                                                                                                        
end                                                                                                                                                                                                                                                            
