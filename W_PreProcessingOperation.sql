Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_PreProcessingOperation]      
                                                                                                                                                                                                      
@ID varchar(2000)='',      
                                                                                                                                                                                                                                  
@OprType smallint=2,      
                                                                                                                                                                                                                                   
@UpdatedBy nvarchar(200)='',      
                                                                                                                                                                                                                           
@Iserror int=0 output      
                                                                                                                                                                                                                                  
AS      
                                                                                                                                                                                                                                                     
BEGIN      
                                                                                                                                                                                                                                                  
    SET NOCOUNT ON;      
                                                                                                                                                                                                                                    
    IF @OprType = 1       
                                                                                                                                                                                                                                   
    BEGIN      
                                                                                                                                                                                                                                              
        SET @Iserror = 1;
                                                                                                                                                                                                                                    
        
                                                                                                                                                                                                                                                     
        DECLARE @PreProcessingId BIGINT;
                                                                                                                                                                                                                     
        DECLARE id_cursor CURSOR FOR 
                                                                                                                                                                                                                        
        SELECT items FROM dbo.Fun_SplitStr(@ID, ',');
                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
        OPEN id_cursor;
                                                                                                                                                                                                                                      
        FETCH NEXT FROM id_cursor INTO @PreProcessingId;
                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
        WHILE @@FETCH_STATUS = 0
                                                                                                                                                                                                                             
        BEGIN
                                                                                                                                                                                                                                                
            DECLARE @ItemId BIGINT, @TotalQty FLOAT, @IsComplete INT;
                                                                                                                                                                                        
            SELECT @ItemId = ItemID, @TotalQty = ISNULL(QuantityMade, 0), @IsComplete = ISNULL(IsComplete, 0)
                                                                                                                                                
            FROM dbo.W_PreProcessing
                                                                                                                                                                                                                         
            WHERE PreProcessingId = @PreProcessingId;
                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
            IF @ItemId IS NOT NULL
                                                                                                                                                                                                                           
            BEGIN
                                                                                                                                                                                                                                            
                -- Only revert ingredient stock and output stock if it was finalized (IsComplete = 1)
                                                                                                                                                        
                IF @IsComplete = 1
                                                                                                                                                                                                                           
                BEGIN
                                                                                                                                                                                                                                        
                    -- Revert ingredient batch stock
                                                                                                                                                                                                         
                    UPDATE b
                                                                                                                                                                                                                                 
                    SET b.FinalQuantityLeft = b.FinalQuantityLeft + u.Quantity
                                                                                                                                                                               
                    FROM dbo.Inv_ItemStockByBatch b
                                                                                                                                                                                                          
                    INNER JOIN dbo.Inv_ItemStockUsed u ON b.ItemStockByBatchId = u.ItemStockByBatchId
                                                                                                                                                        
                    WHERE u.UsedFor = 2 AND u.UsedForId = @PreProcessingId;
                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
                    -- Revert ingredient main stock (deterministic update using grouped SUM)
                                                                                                                                                                 
                    UPDATE s
                                                                                                                                                                                                                                 
                    SET s.IssuedQuantity = ISNULL(s.IssuedQuantity, 0) - t.TotalQtyUsed
                                                                                                                                                                      
                    FROM dbo.W_ItemStock s
                                                                                                                                                                                                                   
                    INNER JOIN (
                                                                                                                                                                                                                             
                        SELECT b.ItemId, SUM(ISNULL(u.Quantity, 0)) AS TotalQtyUsed
                                                                                                                                                                          
                        FROM dbo.Inv_ItemStockUsed u
                                                                                                                                                                                                         
                        INNER JOIN dbo.Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
                                                                                                                                                 
                        WHERE u.UsedFor = 2 AND u.UsedForId = @PreProcessingId
                                                                                                                                                                               
                        GROUP BY b.ItemId
                                                                                                                                                                                                                    
                    ) t ON s.ItemID = t.ItemId;
                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
                    -- Revert produced main stock (subtract from ProducedQuantity instead of PurchasedQuantity)
                                                                                                                                              
                    UPDATE dbo.W_ItemStock
                                                                                                                                                                                                                   
                    SET 
                                                                                                                                                                                                                                     
                        ProducedQuantity = ISNULL(ProducedQuantity, 0) - @TotalQty,
                                                                                                                                                                          
                        FinalStock = ISNULL(FinalStock, 0) - @TotalQty
                                                                                                                                                                                       
                    WHERE ItemID = @ItemId;
                                                                                                                                                                                                                  
                END
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
                -- Delete records from Inv_ItemStockUsed (always clean up draft/finalized usages)
                                                                                                                                                            
                DELETE FROM dbo.Inv_ItemStockUsed
                                                                                                                                                                                                            
                WHERE UsedFor = 2 AND UsedForId = @PreProcessingId;
                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
                -- Delete produced batch from Inv_ItemStockByBatch
                                                                                                                                                                                           
                DELETE FROM dbo.Inv_ItemStockByBatch
                                                                                                                                                                                                         
                WHERE IdFrom = @PreProcessingId AND StockById = 2;
                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
                -- Delete produced batch from Inv_ItemStockByBatchForBOM
                                                                                                                                                                                     
                DELETE FROM dbo.Inv_ItemStockByBatchForBOM
                                                                                                                                                                                                   
                WHERE IDFrom = @PreProcessingId;
                                                                                                                                                                                                             
            END
                                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
            DELETE FROM dbo.W_PreProcessing WHERE PreProcessingId = @PreProcessingId;
                                                                                                                                                                        

                                                                                                                                                                                                                                                             
            FETCH NEXT FROM id_cursor INTO @PreProcessingId;
                                                                                                                                                                                                 
        END
                                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
        CLOSE id_cursor;
                                                                                                                                                                                                                                     
        DEALLOCATE id_cursor;
                                                                                                                                                                                                                                
    END      
                                                                                                                                                                                                                                                
END                                                                                                                                                                                                                                                            
