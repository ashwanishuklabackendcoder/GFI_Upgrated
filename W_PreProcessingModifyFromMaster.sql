Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[W_PreProcessingModifyFromMaster]        
                                                                                                                                                                                             
    @DeletedBatchIds NVARCHAR(500) = NULL,
                                                                                                                                                                                                                   
    @PreProcessingId BIGINT = 0,                      
                                                                                                                                                                                                       
    @BomQty FLOAT = 0,        
                                                                                                                                                                                                                               
    @ItemID BIGINT = 0,      
                                                                                                                                                                                                                                
    @BatchNumberMade NVARCHAR(200),        
                                                                                                                                                                                                                  
    @ExpiryDate DATETIME,     
                                                                                                                                                                                                                               
    @WarehouseId BIGINT,        
                                                                                                                                                                                                                             
    @CreatedBy NVARCHAR(200),   
                                                                                                                                                                                                                             
    @UnitMade BIGINT = 0,  
                                                                                                                                                                                                                                  
    @ProcessingDate DATETIME,  
                                                                                                                                                                                                                              
    @ReturnVal INT = 0 OUTPUT                      
                                                                                                                                                                                                          
AS                      
                                                                                                                                                                                                                                     
BEGIN                      
                                                                                                                                                                                                                                  
    SET NOCOUNT ON                      
                                                                                                                                                                                                                     
    SET @ReturnVal = 0                      
                                                                                                                                                                                                                 
    
                                                                                                                                                                                                                                                         
    BEGIN TRY      
                                                                                                                                                                                                                                          
        BEGIN TRAN      
                                                                                                                                                                                                                                     

                                                                                                                                                                                                                                                             
        ----------------------------------------
                                                                                                                                                                                                             
        -- VALIDATE ITEMID
                                                                                                                                                                                                                                   
        ----------------------------------------
                                                                                                                                                                                                             
        IF (@ItemID IS NULL OR @ItemID = 0)
                                                                                                                                                                                                                  
        BEGIN
                                                                                                                                                                                                                                                
            SET @ReturnVal = -98
                                                                                                                                                                                                                             
            ROLLBACK
                                                                                                                                                                                                                                         
            RETURN
                                                                                                                                                                                                                                           
        END
                                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
        IF NOT EXISTS (SELECT 1 FROM W_MasterItem WHERE ItemID = @ItemID)
                                                                                                                                                                                    
        BEGIN
                                                                                                                                                                                                                                                
            SET @ReturnVal = -97
                                                                                                                                                                                                                             
            ROLLBACK
                                                                                                                                                                                                                                         
            RETURN
                                                                                                                                                                                                                                           
        END
                                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
        ----------------------------------------
                                                                                                                                                                                                             
        -- ?? DELETE CASE (WITH STOCK REVERSAL)
                                                                                                                                                                                                              
        ----------------------------------------
                                                                                                                                                                                                             
        IF (@DeletedBatchIds IS NOT NULL AND @DeletedBatchIds <> '')
                                                                                                                                                                                         
        BEGIN
                                                                                                                                                                                                                                                
            DECLARE @DeletedTable TABLE (Id BIGINT)
                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
            INSERT INTO @DeletedTable
                                                                                                                                                                                                                        
            SELECT value FROM STRING_SPLIT(@DeletedBatchIds, ',')
                                                                                                                                                                                            

                                                                                                                                                                                                                                                             
            DECLARE @TotalDeletedQty FLOAT = 0
                                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
            SELECT @TotalDeletedQty = ISNULL(SUM(QuantityMade),0)
                                                                                                                                                                                            
            FROM W_PreProcessing
                                                                                                                                                                                                                             
            WHERE PreProcessingId IN (SELECT Id FROM @DeletedTable)
                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- REVERSE STOCK (UPDATED)
                                                                                                                                                                                                                       
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
                                                                                                                                                                                                   
            AND StockById = 2
                                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- DELETE PARENT
                                                                                                                                                                                                                                 
            ----------------------------------------
                                                                                                                                                                                                         
            DELETE FROM W_PreProcessing
                                                                                                                                                                                                                      
            WHERE PreProcessingId IN (SELECT Id FROM @DeletedTable)
                                                                                                                                                                                          
        END
                                                                                                                                                                                                                                                  

                                                                                                                                                                                                                                                             
        ----------------------------------------
                                                                                                                                                                                                             
        -- ?? INSERT CASE
                                                                                                                                                                                                                                    
        ----------------------------------------
                                                                                                                                                                                                             
        IF @PreProcessingId = 0                      
                                                                                                                                                                                                        
        BEGIN                      
                                                                                                                                                                                                                          
            INSERT INTO W_PreProcessing      
                                                                                                                                                                                                                
            (BomId, BomQty, ProcessingDate, QuantityMade, UnitMade, BatchNumberMade, ExpiryDate, ProcessEmployees, Remarks, DocumentUpload, WarehouseId, CreatedBy, ItemID)        
                                                                          
            VALUES       
                                                                                                                                                                                                                                    
            (NULL, @BomQty, @ProcessingDate, @BomQty, @UnitMade, @BatchNumberMade, @ExpiryDate, 0, '', NULL, @WarehouseId, @CreatedBy, @ItemID)               
                                                                                               

                                                                                                                                                                                                                                                             
            SET @ReturnVal = SCOPE_IDENTITY()                 
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- INSERT STOCK BY BATCH
                                                                                                                                                                                                                         
            ----------------------------------------
                                                                                                                                                                                                         
            INSERT INTO Inv_ItemStockByBatch      
                                                                                                                                                                                                           
            (StockById, IdFrom, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)        
                                                                                                                                         
            VALUES       
                                                                                                                                                                                                                                    
            (2, @ReturnVal, @ItemId, @BomQty, @UnitMade, @BatchNumberMade, @ExpiryDate, @WarehouseId, @BomQty)        
                                                                                                                                       

                                                                                                                                                                                                                                                             
            ----------------------------------------
                                                                                                                                                                                                         
            -- UPDATE MAIN STOCK (UPDATED)
                                                                                                                                                                                                                   
            ----------------------------------------
                                                                                                                                                                                                         
            IF NOT EXISTS (SELECT 1 FROM W_ItemStock WHERE ItemID = @ItemId)                
                                                                                                                                                                 
            BEGIN                
                                                                                                                                                                                                                            
                INSERT INTO W_ItemStock      
                                                                                                                                                                                                                
                (OpeningQuantity, PurchasedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity)          
                                                                                                     
                VALUES      
                                                                                                                                                                                                                                 
                (@BomQty, 0, @ItemId, @UnitMade, 0, @CreatedBy, @BomQty, GETDATE(), 0)      
                                                                                                                                                                 
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
                                                                                                                                                                                                                          
            IF EXISTS (SELECT 1 FROM dbo.W_PreProcessing WHERE PreProcessingId = @PreProcessingId)                      
                                                                                                                                     
            BEGIN                      
                                                                                                                                                                                                                      
                DECLARE @OldQuantity FLOAT = 0      
                                                                                                                                                                                                         

                                                                                                                                                                                                                                                             
                SELECT @OldQuantity = ISNULL(QuantityMade,0)
                                                                                                                                                                                                 
                FROM W_PreProcessing      
                                                                                                                                                                                                                   
                WHERE PreProcessingId = @PreProcessingId      
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
                ----------------------------------------
                                                                                                                                                                                                     
                -- UPDATE MAIN TABLE
                                                                                                                                                                                                                         
                ----------------------------------------
                                                                                                                                                                                                     
                UPDATE W_PreProcessing         
                                                                                                                                                                                                              
                SET       
                                                                                                                                                                                                                                   
                    BomQty = @BomQty,      
                                                                                                                                                                                                                  
                    QuantityMade = @BomQty,        
                                                                                                                                                                                                          
                    ExpiryDate = @ExpiryDate,      
                                                                                                                                                                                                          
                    ProcessingDate = @ProcessingDate,  
                                                                                                                                                                                                      
                    UnitMade = @UnitMade,  
                                                                                                                                                                                                                  
                    WarehouseId = @WarehouseId       
                                                                                                                                                                                                        
                WHERE PreProcessingId = @PreProcessingId        
                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
                SET @ReturnVal = @PreProcessingId             
                                                                                                                                                                                               

                                                                                                                                                                                                                                                             
                ----------------------------------------
                                                                                                                                                                                                     
                -- UPDATE BATCH TABLE
                                                                                                                                                                                                                        
                ----------------------------------------
                                                                                                                                                                                                     
                UPDATE Inv_ItemStockByBatch      
                                                                                                                                                                                                            
                SET       
                                                                                                                                                                                                                                   
                    Quantity = @BomQty,      
                                                                                                                                                                                                                
                    Unit = @UnitMade,      
                                                                                                                                                                                                                  
                    BatchNo = @BatchNumberMade,      
                                                                                                                                                                                                        
                    ExpiryDate = @ExpiryDate,      
                                                                                                                                                                                                          
                    WarehouseId = @WarehouseId,      
                                                                                                                                                                                                        
                    FinalQuantityLeft = @BomQty      
                                                                                                                                                                                                        
                WHERE IdFrom = @PreProcessingId      
                                                                                                                                                                                                        
                AND StockById = 2      
                                                                                                                                                                                                                      

                                                                                                                                                                                                                                                             
                ----------------------------------------
                                                                                                                                                                                                     
                -- ?? DELTA STOCK UPDATE (UPDATED)
                                                                                                                                                                                                           
                ----------------------------------------
                                                                                                                                                                                                     
                DECLARE @DiffQty FLOAT
                                                                                                                                                                                                                       
                SET @DiffQty = ISNULL(@BomQty,0) - ISNULL(@OldQuantity,0)
                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
                IF NOT EXISTS (SELECT 1 FROM W_ItemStock WHERE ItemID = @ItemID)      
                                                                                                                                                                       
                BEGIN      
                                                                                                                                                                                                                                  
                    INSERT INTO W_ItemStock      
                                                                                                                                                                                                            
                    (OpeningQuantity, PurchasedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity)      
                                                                                                     
                    VALUES      
                                                                                                                                                                                                                             
                    (@BomQty, 0, @ItemID, @UnitMade, 0, @CreatedBy, @BomQty, GETDATE(), 0)      
                                                                                                                                                             
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
