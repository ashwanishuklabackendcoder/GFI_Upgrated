Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_ProductionModifyFromMaster]     
                                                                                                                                                                                                   
    @DeletedBatchIds NVARCHAR(500) = NULL,
                                                                                                                                                                                                                   
    @ProductionId BIGINT = 0,                    
                                                                                                                                                                                                            
    @BomQty float = 0,      
                                                                                                                                                                                                                                 
    @ItemID BIGINT = 0,    
                                                                                                                                                                                                                                  
    @BatchNumberMade NVARCHAR(200),      
                                                                                                                                                                                                                    
    @ExpiryDate DATETIME,      
                                                                                                                                                                                                                              
    @WarehouseId BIGINT,      
                                                                                                                                                                                                                               
    @CreatedBy NVARCHAR(200), 
                                                                                                                                                                                                                               
    @FillingPerBottleUnit BIGINT = 0,
                                                                                                                                                                                                                        
    @CookingDate DATETIME,
                                                                                                                                                                                                                                   
    @ReturnVal INT = 0 OUTPUT                    
                                                                                                                                                                                                            
AS                    
                                                                                                                                                                                                                                       
BEGIN                    
                                                                                                                                                                                                                                    
    SET NOCOUNT ON                    
                                                                                                                                                                                                                       
    SET @ReturnVal = 0                    
                                                                                                                                                                                                                   
    
                                                                                                                                                                                                                                                         
    BEGIN TRY    
                                                                                                                                                                                                                                            
        BEGIN TRAN    
                                                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
        ----------------------------------------
                                                                                                                                                                                                             
        -- ?? DELETE CASE (WITH STOCK REVERSAL)
                                                                                                                                                                                                              
        ----------------------------------------
                                                                                                                                                                                                             
        IF (@DeletedBatchIds IS NOT NULL AND @DeletedBatchIds <> '')
                                                                                                                                                                                         
        BEGIN
                                                                                                                                                                                                                                                
            DECLARE @DeletedTable TABLE (Id BIGINT)
                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
            INSERT INTO @DeletedTable
                                                                                                                                                                                                                        
            SELECT value FROM STRING_SPLIT(@DeletedBatchIds, ',')
                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
            DECLARE @TotalDeletedQty FLOAT = 0
                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
            SELECT @TotalDeletedQty = ISNULL(SUM(BomQty),0)
                                                                                                                                                                                                  
            FROM W_Production
                                                                                                                                                                                                                                
            WHERE ProductionId IN (SELECT Id FROM @DeletedTable)
                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- ? REVERSE STOCK (FIXED)
                                                                                                                                                                                                                       
            ----------------------------------------
                                                                                                                                                                                                         
            UPDATE W_ItemStock
                                                                                                                                                                                                                               
            SET 
                                                                                                                                                                                                                                             
                OpeningQuantity = ISNULL(OpeningQuantity,0) - @TotalDeletedQty,
                                                                                                                                                                              
                FinalStock = ISNULL(FinalStock,0) - @TotalDeletedQty
                                                                                                                                                                                         
            WHERE ItemID = @ItemID
                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- DELETE CHILD
                                                                                                                                                                                                                                  
            ----------------------------------------
                                                                                                                                                                                                         
            DELETE FROM Inv_ItemStockByBatch
                                                                                                                                                                                                                 
            WHERE IdFrom IN (SELECT Id FROM @DeletedTable)
                                                                                                                                                                                                   
            AND StockById = 3
                                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- DELETE PARENT
                                                                                                                                                                                                                                 
            ----------------------------------------
                                                                                                                                                                                                         
            DELETE FROM W_Production
                                                                                                                                                                                                                         
            WHERE ProductionId IN (SELECT Id FROM @DeletedTable)
                                                                                                                                                                                             
        END
                                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
        ----------------------------------------
                                                                                                                                                                                                             
        -- ?? INSERT CASE
                                                                                                                                                                                                                                    
        ----------------------------------------
                                                                                                                                                                                                             
        IF @ProductionId = 0                    
                                                                                                                                                                                                             
        BEGIN                    
                                                                                                                                                                                                                            
            INSERT INTO W_Production    
                                                                                                                                                                                                                     
            (BomId, BomQty, CookingDate, FillingPerBottleUnit, BatchNo, ExpiryDate, ProcessEmployees, Remarks, DocumentUpload, WarehouseId, ItemID)      
                                                                                                    
            VALUES     
                                                                                                                                                                                                                                      
            (NULL, @BomQty, @CookingDate, @FillingPerBottleUnit, @BatchNumberMade, @ExpiryDate, 0, '', NULL, @WarehouseId, @ItemID)             
                                                                                                             

                                                                                                                                                                                                                                                             
            SET @ReturnVal = SCOPE_IDENTITY()               
                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- INSERT STOCK BY BATCH
                                                                                                                                                                                                                         
            ----------------------------------------
                                                                                                                                                                                                         
            INSERT INTO Inv_ItemStockByBatch    
                                                                                                                                                                                                             
            (StockById, IdFrom, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)      
                                                                                                                                           
            VALUES     
                                                                                                                                                                                                                                      
            (3, @ReturnVal, @ItemId, @BomQty, @FillingPerBottleUnit, @BatchNumberMade, @ExpiryDate, @WarehouseId, @BomQty)      
                                                                                                                             

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- ? UPDATE MAIN STOCK (FIXED)
                                                                                                                                                                                                                   
            ----------------------------------------
                                                                                                                                                                                                         
            IF NOT EXISTS (SELECT 1 FROM W_ItemStock WHERE ItemID = @ItemId)              
                                                                                                                                                                   
            BEGIN              
                                                                                                                                                                                                                              
                INSERT INTO W_ItemStock    
                                                                                                                                                                                                                  
                (OpeningQuantity, PurchasedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity)        
                                                                                                       
                VALUES    
                                                                                                                                                                                                                                   
                (@BomQty, 0, @ItemId, @FillingPerBottleUnit, 0, @CreatedBy, @BomQty, GETDATE(), 0)    
                                                                                                                                                       
            END        
                                                                                                                                                                                                                                      
            ELSE       
                                                                                                                                                                                                                                      
            BEGIN      
                                                                                                                                                                                                                                      
                UPDATE W_ItemStock     
                                                                                                                                                                                                                      
                SET OpeningQuantity = ISNULL(OpeningQuantity,0) + @BomQty,
                                                                                                                                                                                   
                    FinalStock = ISNULL(FinalStock,0) + @BomQty      
                                                                                                                                                                                        
                WHERE ItemID = @ItemId      
                                                                                                                                                                                                                 
            END      
                                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
        END                    
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
        ----------------------------------------
                                                                                                                                                                                                             
        -- ?? UPDATE CASE
                                                                                                                                                                                                                                    
        ----------------------------------------
                                                                                                                                                                                                             
        ELSE                    
                                                                                                                                                                                                                             
        BEGIN                    
                                                                                                                                                                                                                            
            IF EXISTS (SELECT 1 FROM W_Production WHERE ProductionId = @ProductionId)                    
                                                                                                                                                    
            BEGIN                    
                                                                                                                                                                                                                        
                DECLARE @OldQuantity FLOAT    
                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
                SELECT @OldQuantity = ISNULL(BomQty,0)
                                                                                                                                                                                                       
                FROM W_Production    
                                                                                                                                                                                                                        
                WHERE ProductionId = @ProductionId    
                                                                                                                                                                                                       

                                                                                                                                                                                                                                                             
                ----------------------------------------
                                                                                                                                                                                                     
                -- UPDATE MAIN TABLE
                                                                                                                                                                                                                         
                ----------------------------------------
                                                                                                                                                                                                     
                UPDATE W_Production       
                                                                                                                                                                                                                   
                SET     
                                                                                                                                                                                                                                     
                    BomId = NULL,    
                                                                                                                                                                                                                        
                    BomQty = @BomQty,    
                                                                                                                                                                                                                    
                    ExpiryDate = @ExpiryDate,    
                                                                                                                                                                                                            
                    CookingDate = @CookingDate,
                                                                                                                                                                                                              
                    FillingPerBottleUnit = @FillingPerBottleUnit,
                                                                                                                                                                                            
                    WarehouseId = @WarehouseId     
                                                                                                                                                                                                          
                WHERE ProductionId = @ProductionId      
                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
                SET @ReturnVal = @ProductionId           
                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
                ----------------------------------------
                                                                                                                                                                                                     
                -- UPDATE BATCH TABLE
                                                                                                                                                                                                                        
                ----------------------------------------
                                                                                                                                                                                                     
                UPDATE Inv_ItemStockByBatch    
                                                                                                                                                                                                              
                SET     
                                                                                                                                                                                                                                     
                    Quantity = @BomQty,    
                                                                                                                                                                                                                  
                    Unit = @FillingPerBottleUnit,    
                                                                                                                                                                                                        
                    BatchNo = @BatchNumberMade,    
                                                                                                                                                                                                          
                    ExpiryDate = @ExpiryDate,    
                                                                                                                                                                                                            
                    WarehouseId = @WarehouseId,    
                                                                                                                                                                                                          
                    FinalQuantityLeft = @BomQty    
                                                                                                                                                                                                          
                WHERE IdFrom = @ProductionId    
                                                                                                                                                                                                             
                AND StockById = 3    
                                                                                                                                                                                                                        

                                                                                                                                                                                                                                                             
                ----------------------------------------
                                                                                                                                                                                                     
                -- ? DELTA STOCK UPDATE (FIXED)
                                                                                                                                                                                                              
                ----------------------------------------
                                                                                                                                                                                                     
                DECLARE @DiffQty FLOAT    
                                                                                                                                                                                                                   
                SET @DiffQty = ISNULL(@BomQty,0) - ISNULL(@OldQuantity,0)
                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
                IF NOT EXISTS (SELECT 1 FROM W_ItemStock WHERE ItemID = @ItemID)    
                                                                                                                                                                         
                BEGIN    
                                                                                                                                                                                                                                    
                    INSERT INTO W_ItemStock    
                                                                                                                                                                                                              
                    (OpeningQuantity, PurchasedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity)    
                                                                                                       
                    VALUES    
                                                                                                                                                                                                                               
                    (@BomQty, 0, @ItemID, @FillingPerBottleUnit, 0, @CreatedBy, @BomQty, GETDATE(), 0)    
                                                                                                                                                   
                END    
                                                                                                                                                                                                                                      
                ELSE    
                                                                                                                                                                                                                                     
                BEGIN    
                                                                                                                                                                                                                                    
                    UPDATE W_ItemStock    
                                                                                                                                                                                                                   
                    SET     
                                                                                                                                                                                                                                 
                        OpeningQuantity = ISNULL(OpeningQuantity,0) + @DiffQty,
                                                                                                                                                                              
                        FinalStock = ISNULL(FinalStock,0) + @DiffQty    
                                                                                                                                                                                     
                    WHERE ItemID = @ItemID    
                                                                                                                                                                                                               
                END    
                                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
            END                    
                                                                                                                                                                                                                          
            ELSE                    
                                                                                                                                                                                                                         
                SET @ReturnVal = -1                    
                                                                                                                                                                                                      
        END                    
                                                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
        COMMIT    
                                                                                                                                                                                                                                           
    END TRY    
                                                                                                                                                                                                                                              
    BEGIN CATCH    
                                                                                                                                                                                                                                          
        ROLLBACK    
                                                                                                                                                                                                                                         
        SET @ReturnVal = -99    
                                                                                                                                                                                                                             
    END CATCH    
                                                                                                                                                                                                                                            
END                                                                                                                                                                                                                                                            
