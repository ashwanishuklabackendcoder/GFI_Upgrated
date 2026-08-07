-- 1. Add UnitId column to Inv_ItemStockUsed if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Inv_ItemStockUsed') AND name = 'UnitId')
BEGIN
    ALTER TABLE dbo.Inv_ItemStockUsed ADD UnitId BIGINT NULL;
END
GO

-- 2. Update Inv_ItemStockUsedModify Stored Procedure
ALTER PROCEDURE [dbo].[Inv_ItemStockUsedModify]         
@ItemStockUsedID bigint =0 ,      
@ItemStockByBatchId bigint ,      
@UsedFor int ,      
@UsedForId bigint ,   
@Quantity float ,      
@Description nvarchar(1000) ,      
@CreatedBy nvarchar(200) ,      
@CreatedDate datetime,      
@UnitId bigint = null, -- Added Parameter
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
      
      UPDATE Inv_ItemStockByBatch       
      SET FinalQuantityLeft=FinalQuantityLeft-@Quantity WHERE ItemStockByBatchId=@ItemStockByBatchId      
      
      SET @ReturnVal=SCOPE_IDENTITY()        
        
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
    
        UPDATE Inv_ItemStockUsed       
        SET     
            ItemStockByBatchId=@ItemStockByBatchId,    
            UsedFor=@UsedFor,     
            UsedForId=@UsedForId,     
            Quantity=@Quantity,     
            [Description]=@Description,
            UnitId=@UnitId -- Updated UnitId
        WHERE ItemStockUsedID=@ItemStockUsedID      
    
        SET @ReturnVal = @ItemStockUsedID                  
    END                  
    ELSE                  
        SET @ReturnVal = @ItemStockUsedID                 
  END      
END
GO

-- 3. Update Inv_ItemStockUsedById Stored Procedure
ALTER PROCEDURE [dbo].[Inv_ItemStockUsedById]
@UsedForId int =0,
@UsedFor int =0
AS
BEGIN
  IF(@UsedForId>0)
    SELECT a.*, c.ItemID, b.BatchNo, 
           a.CreatedDate AS TransactionDate, 
           ISNULL(mu.UnitName, '') AS UnitName, 
           (CAST(b.Amount AS float) / NULLIF(CAST(b.Quantity AS float), 0)) * CAST(a.Quantity AS float) AS Amount
    FROM Inv_ItemStockUsed a
    INNER JOIN Inv_ItemStockByBatch b ON a.ItemStockByBatchId=b.ItemStockByBatchId
    INNER JOIN W_MasterItem c ON c.ItemID=b.ItemId
    LEFT JOIN W_MasterUnit mu ON COALESCE(a.UnitId, b.Unit) = mu.UnitId
    WHERE a.UsedForId=@UsedForId AND a.UsedFor=@UsedFor
  ELSE
    SELECT a.*, c.ItemID, b.BatchNo, 
           a.CreatedDate AS TransactionDate, 
           ISNULL(mu.UnitName, '') AS UnitName, 
           (CAST(b.Amount AS float) / NULLIF(CAST(b.Quantity AS float), 0)) * CAST(a.Quantity AS float) AS Amount
    FROM Inv_ItemStockUsed a
    INNER JOIN Inv_ItemStockByBatch b ON a.ItemStockByBatchId=b.ItemStockByBatchId
    INNER JOIN W_MasterItem c ON c.ItemID=b.ItemId
    LEFT JOIN W_MasterUnit mu ON COALESCE(a.UnitId, b.Unit) = mu.UnitId
END
GO
