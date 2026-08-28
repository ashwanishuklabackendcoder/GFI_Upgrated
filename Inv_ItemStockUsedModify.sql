Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Inv_ItemStockUsedModify]         
                                                                                                                                                                                                    
@ItemStockUsedID bigint =0 ,      
                                                                                                                                                                                                                           
@ItemStockByBatchId bigint ,      
                                                                                                                                                                                                                           
@UsedFor int ,      
                                                                                                                                                                                                                                         
@UsedForId bigint ,   
                                                                                                                                                                                                                                       
@Quantity float ,      
                                                                                                                                                                                                                                      
@Description nvarchar(1000) ,      
                                                                                                                                                                                                                          
@CreatedBy nvarchar(200) ,      
                                                                                                                                                                                                                             
@CreatedDate datetime,      
                                                                                                                                                                                                                                 
@UnitId bigint = null,
                                                                                                                                                                                                                                       
@ReturnVal int=0 output       
                                                                                                                                                                                                                               
AS                  
                                                                                                                                                                                                                                         
BEGIN                  
                                                                                                                                                                                                                                      
SET NOCOUNT ON                  
                                                                                                                                                                                                                             
SET @ReturnVal = 0                  
                                                                                                                                                                                                                         
        
                                                                                                                                                                                                                                                     
IF @ItemStockUsedID = 0                  
                                                                                                                                                                                                                    
  BEGIN                  
                                                                                                                                                                                                                                    
   IF NOT EXISTS(SELECT 1 FROM dbo.Inv_ItemStockUsed WHERE ItemStockUsedID=@ItemStockUsedID)                  
                                                                                                                                               
    BEGIN         
                                                                                                                                                                                                                                           
      INSERT INTO Inv_ItemStockUsed(ItemStockByBatchId, UsedFor, UsedForId, Quantity, Description, CreatedBy, UnitId)       
                                                                                                                                 
      VALUES (@ItemStockByBatchId, @UsedFor, @UsedForId, @Quantity, @Description, @CreatedBy, @UnitId)      
                                                                                                                                                 
      
                                                                                                                                                                                                                                                       
      SET @ReturnVal=SCOPE_IDENTITY()        
                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
      -- Skip stock deduction during draft saves for pre-processing (2) and production (3)
                                                                                                                                                                   
      IF @UsedFor NOT IN (2, 3)
                                                                                                                                                                                                                              
      BEGIN
                                                                                                                                                                                                                                                  
          UPDATE Inv_ItemStockByBatch       
                                                                                                                                                                                                                 
          SET FinalQuantityLeft=FinalQuantityLeft-@Quantity WHERE ItemStockByBatchId=@ItemStockByBatchId      
                                                                                                                                               
          
                                                                                                                                                                                                                                                   
          IF @ReturnVal>0      
                                                                                                                                                                                                                              
          BEGIN      
                                                                                                                                                                                                                                        
            IF(ISNULL((SELECT COUNT(*) FROM W_ItemStock WHERE ItemID IN (SELECT ItemId FROM Inv_ItemStockUsed a INNER JOIN Inv_ItemStockByBatch b ON a.ItemStockByBatchId=b.ItemStockByBatchId WHERE ItemStockUsedID=@ReturnVal)),0)>0)      
                
            BEGIN       
                                                                                                                                                                                                                                     
              UPDATE t1 SET IssuedQuantity=ISNULL(IssuedQuantity,0) + ISNULL(t3.Quantity,0)      
                                                                                                                                                            
              FROM W_ItemStock t1       
                                                                                                                                                                                                                     
              INNER JOIN Inv_ItemStockByBatch t2 ON t1.ItemID=t2.ItemID       
                                                                                                                                                                               
              INNER JOIN Inv_ItemStockUsed t3 ON t2.ItemStockByBatchId=t3.ItemStockByBatchId      
                                                                                                                                                           
              WHERE t3.ItemStockUsedID=@ReturnVal      
                                                                                                                                                                                                      
            END       
                                                                                                                                                                                                                                       
          END      
                                                                                                                                                                                                                                          
      END
                                                                                                                                                                                                                                                    
    END                  
                                                                                                                                                                                                                                    
   ELSE                  
                                                                                                                                                                                                                                    
    SET @ReturnVal= -1                  
                                                                                                                                                                                                                     
  END                  
                                                                                                                                                                                                                                      
  ELSE    
                                                                                                                                                                                                                                                   
  BEGIN          
                                                                                                                                                                                                                                            
    IF EXISTS (SELECT 1 FROM dbo.Inv_ItemStockUsed WHERE ItemStockUsedID=@ItemStockUsedID)    
                                                                                                                                                               
    BEGIN       
                                                                                                                                                                                                                                             
        DECLARE     
                                                                                                                                                                                                                                         
            @OldBatchId BIGINT,    
                                                                                                                                                                                                                          
            @OldQty FLOAT,    
                                                                                                                                                                                                                               
            @OldItemId BIGINT,    
                                                                                                                                                                                                                           
            @NewItemId BIGINT    
                                                                                                                                                                                                                            
    
                                                                                                                                                                                                                                                         
        SELECT     
                                                                                                                                                                                                                                          
            @OldBatchId = ItemStockByBatchId,    
                                                                                                                                                                                                            
            @OldQty = Quantity    
                                                                                                                                                                                                                           
        FROM Inv_ItemStockUsed     
                                                                                                                                                                                                                          
        WHERE ItemStockUsedID=@ItemStockUsedID    
                                                                                                                                                                                                           
    
                                                                                                                                                                                                                                                         
        SELECT @OldItemId = ItemID     
                                                                                                                                                                                                                      
        FROM Inv_ItemStockByBatch     
                                                                                                                                                                                                                       
        WHERE ItemStockByBatchId=@OldBatchId    
                                                                                                                                                                                                             
    
                                                                                                                                                                                                                                                         
        SELECT @NewItemId = ItemID     
                                                                                                                                                                                                                      
        FROM Inv_ItemStockByBatch     
                                                                                                                                                                                                                       
        WHERE ItemStockByBatchId=@ItemStockByBatchId    
                                                                                                                                                                                                     
    
                                                                                                                                                                                                                                                         
        -- Skip stock deduction during draft saves for pre-processing (2) and production (3)
                                                                                                                                                                 
        IF @UsedFor NOT IN (2, 3)
                                                                                                                                                                                                                            
        BEGIN
                                                                                                                                                                                                                                                
            UPDATE Inv_ItemStockByBatch    
                                                                                                                                                                                                                  
            SET FinalQuantityLeft = FinalQuantityLeft + @OldQty    
                                                                                                                                                                                          
            WHERE ItemStockByBatchId = @OldBatchId    
                                                                                                                                                                                                       
        
                                                                                                                                                                                                                                                     
            UPDATE Inv_ItemStockByBatch    
                                                                                                                                                                                                                  
            SET FinalQuantityLeft = FinalQuantityLeft - @Quantity    
                                                                                                                                                                                        
            WHERE ItemStockByBatchId = @ItemStockByBatchId    
                                                                                                                                                                                               
        
                                                                                                                                                                                                                                                     
            UPDATE W_ItemStock    
                                                                                                                                                                                                                           
            SET IssuedQuantity = ISNULL(IssuedQuantity,0) - @OldQty    
                                                                                                                                                                                      
            WHERE ItemID = @OldItemId    
                                                                                                                                                                                                                    
        
                                                                                                                                                                                                                                                     
            UPDATE W_ItemStock    
                                                                                                                                                                                                                           
            SET IssuedQuantity = ISNULL(IssuedQuantity,0) + @Quantity    
                                                                                                                                                                                    
            WHERE ItemID = @NewItemId    
                                                                                                                                                                                                                    
        END
                                                                                                                                                                                                                                                  
    
                                                                                                                                                                                                                                                         
        UPDATE Inv_ItemStockUsed       
                                                                                                                                                                                                                      
        SET     
                                                                                                                                                                                                                                             
            ItemStockByBatchId=@ItemStockByBatchId,    
                                                                                                                                                                                                      
            UsedFor=@UsedFor,     
                                                                                                                                                                                                                           
            UsedForId=@UsedForId,     
                                                                                                                                                                                                                       
            Quantity=@Quantity,     
                                                                                                                                                                                                                         
            [Description]=@Description,
                                                                                                                                                                                                                      
            UnitId=@UnitId
                                                                                                                                                                                                                                   
        WHERE ItemStockUsedID=@ItemStockUsedID      
                                                                                                                                                                                                         
    
                                                                                                                                                                                                                                                         
        SET @ReturnVal = @ItemStockUsedID                  
                                                                                                                                                                                                  
    END                  
                                                                                                                                                                                                                                    
    ELSE                  
                                                                                                                                                                                                                                   
        SET @ReturnVal = @ItemStockUsedID                 
                                                                                                                                                                                                   
  END      
                                                                                                                                                                                                                                                  
END                                                                                                                                                                                                                                                            
