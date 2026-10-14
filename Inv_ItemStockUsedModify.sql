CREATE OR ALTER PROCEDURE [dbo].[Inv_ItemStockUsedModify]         
    @ItemStockUsedID bigint = 0,      
    @ItemStockByBatchId bigint,      
    @UsedFor int,      
    @UsedForId bigint,   
    @Quantity float,      
    @Description nvarchar(1000),      
    @CreatedBy nvarchar(200),      
    @CreatedDate datetime,      
    @UnitId bigint = null,
    @ReturnVal int = 0 output       
AS                  
BEGIN                  
    SET NOCOUNT ON;                  
    SET @ReturnVal = 0;                  

    IF @ItemStockUsedID = 0                  
    BEGIN                  
        -- STRICT BLOCK on INSERT: check batch stock availability
        DECLARE @CurrentBatchQty FLOAT;
        DECLARE @ItemName NVARCHAR(200);
        DECLARE @BatchNo NVARCHAR(100);

        SELECT @CurrentBatchQty = b.FinalQuantityLeft, @ItemName = m.ItemName, @BatchNo = b.BatchNo
        FROM dbo.Inv_ItemStockByBatch b
        INNER JOIN dbo.W_MasterItem m ON b.ItemId = m.ItemID
        WHERE b.ItemStockByBatchId = @ItemStockByBatchId;

        IF (ROUND(@CurrentBatchQty, 4) - ROUND(@Quantity, 4)) < 0
        BEGIN
            DECLARE @InsertBlockMsg NVARCHAR(500) = CONCAT(
                'STRICT BLOCK: Cannot allocate ', @Quantity, ' units. Only ', 
                ISNULL(@CurrentBatchQty, 0), ' units are available in Batch "', 
                ISNULL(@BatchNo, 'N/A'), '" for item "', ISNULL(@ItemName, 'Unknown'), '".'
            );
            RAISERROR(@InsertBlockMsg, 16, 1);
            RETURN;
        END

        IF NOT EXISTS(SELECT 1 FROM dbo.Inv_ItemStockUsed WHERE ItemStockUsedID=@ItemStockUsedID)                  
        BEGIN         
            INSERT INTO Inv_ItemStockUsed(ItemStockByBatchId, UsedFor, UsedForId, Quantity, Description, CreatedBy, UnitId)       
            VALUES (@ItemStockByBatchId, @UsedFor, @UsedForId, @Quantity, @Description, @CreatedBy, @UnitId);      

            SET @ReturnVal = SCOPE_IDENTITY();        

            IF @UsedFor NOT IN (2, 3)
            BEGIN
                UPDATE Inv_ItemStockByBatch       
                SET FinalQuantityLeft = ROUND(FinalQuantityLeft - @Quantity, 4) 
                WHERE ItemStockByBatchId = @ItemStockByBatchId;      

                IF @ReturnVal > 0      
                BEGIN      
                    UPDATE t1 
                    SET IssuedQuantity = ISNULL(IssuedQuantity, 0) + ISNULL(t3.Quantity, 0),
                        FinalStock = ISNULL(FinalStock, 0) - ISNULL(t3.Quantity, 0)      
                    FROM W_ItemStock t1       
                    INNER JOIN Inv_ItemStockByBatch t2 ON t1.ItemID = t2.ItemID       
                    INNER JOIN Inv_ItemStockUsed t3 ON t2.ItemStockByBatchId = t3.ItemStockByBatchId      
                    WHERE t3.ItemStockUsedID = @ReturnVal;      
                END      
            END
        END                  
        ELSE                  
            SET @ReturnVal = -1;                  
    END                  
    ELSE    
    BEGIN          
        IF EXISTS (SELECT 1 FROM dbo.Inv_ItemStockUsed WHERE ItemStockUsedID = @ItemStockUsedID)    
        BEGIN       
            DECLARE     
                @OldBatchId BIGINT,    
                @OldQty FLOAT,    
                @OldItemId BIGINT,    
                @NewItemId BIGINT;    

            SELECT     
                @OldBatchId = ItemStockByBatchId,    
                @OldQty = Quantity    
            FROM Inv_ItemStockUsed     
            WHERE ItemStockUsedID = @ItemStockUsedID;    

            SELECT @OldItemId = ItemID     
            FROM Inv_ItemStockByBatch     
            WHERE ItemStockByBatchId = @OldBatchId;    

            SELECT @NewItemId = ItemID     
            FROM Inv_ItemStockByBatch     
            WHERE ItemStockByBatchId = @ItemStockByBatchId;    

            -- STRICT BLOCK on UPDATE
            DECLARE @AvailUpdateQty FLOAT, @UpdItemName NVARCHAR(200), @UpdBatchNo NVARCHAR(100);
            SELECT @AvailUpdateQty = b.FinalQuantityLeft, @UpdItemName = m.ItemName, @UpdBatchNo = b.BatchNo
            FROM dbo.Inv_ItemStockByBatch b
            INNER JOIN dbo.W_MasterItem m ON b.ItemId = m.ItemID
            WHERE b.ItemStockByBatchId = @ItemStockByBatchId;

            DECLARE @EffectiveAvail FLOAT = @AvailUpdateQty + (CASE WHEN @OldBatchId = @ItemStockByBatchId AND @UsedFor NOT IN (2, 3) THEN @OldQty ELSE 0 END);

            IF (ROUND(@EffectiveAvail, 4) - ROUND(@Quantity, 4)) < 0
            BEGIN
                DECLARE @UpdateBlockMsg NVARCHAR(500) = CONCAT(
                    'STRICT BLOCK: Cannot update to ', @Quantity, ' units. Only ', 
                    ISNULL(@EffectiveAvail, 0), ' units are available in Batch "', 
                    ISNULL(@UpdBatchNo, 'N/A'), '" for item "', ISNULL(@UpdItemName, 'Unknown'), '".'
                );
                RAISERROR(@UpdateBlockMsg, 16, 1);
                RETURN;
            END

            IF @UsedFor NOT IN (2, 3)
            BEGIN
                UPDATE Inv_ItemStockByBatch    
                SET FinalQuantityLeft = ROUND(FinalQuantityLeft + @OldQty, 4)    
                WHERE ItemStockByBatchId = @OldBatchId;    

                UPDATE Inv_ItemStockByBatch    
                SET FinalQuantityLeft = ROUND(FinalQuantityLeft - @Quantity, 4)    
                WHERE ItemStockByBatchId = @ItemStockByBatchId;    

                UPDATE W_ItemStock    
                SET IssuedQuantity = ISNULL(IssuedQuantity, 0) - @OldQty,
                    FinalStock = ISNULL(FinalStock, 0) + @OldQty    
                WHERE ItemID = @OldItemId;    

                UPDATE W_ItemStock    
                SET IssuedQuantity = ISNULL(IssuedQuantity, 0) + @Quantity,
                    FinalStock = ISNULL(FinalStock, 0) - @Quantity    
                WHERE ItemID = @NewItemId;    
            END

            UPDATE Inv_ItemStockUsed       
            SET     
                ItemStockByBatchId = @ItemStockByBatchId,    
                UsedFor = @UsedFor,     
                UsedForId = @UsedForId,     
                Quantity = @Quantity,     
                [Description] = @Description,
                UnitId = @UnitId
            WHERE ItemStockUsedID = @ItemStockUsedID;      

            SET @ReturnVal = @ItemStockUsedID;                  
        END                  
        ELSE                  
            SET @ReturnVal = @ItemStockUsedID;                 
    END      
END
