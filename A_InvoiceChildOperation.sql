ALTER PROCEDURE [dbo].[A_InvoiceChildOperation]      
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

        -- 1. Revert stock in Inv_ItemStockByBatch and W_ItemStock
        DECLARE @TempChildStock TABLE (
            ItemStockUsedID BIGINT,
            ItemStockByBatchId BIGINT,
            Quantity FLOAT,
            ItemID BIGINT
        );

        INSERT INTO @TempChildStock (ItemStockUsedID, ItemStockByBatchId, Quantity, ItemID)
        SELECT u.ItemStockUsedID, u.ItemStockByBatchId, u.Quantity, b.ItemID
        FROM dbo.A_InvoiceChild c
        INNER JOIN dbo.Inv_ItemStockByBatch b ON c.ItemId = b.ItemID AND c.BatchNumber = b.BatchNo
        INNER JOIN dbo.Inv_ItemStockUsed u ON u.ItemStockByBatchId = b.ItemStockByBatchId AND u.UsedForId = c.InvoiceID
        WHERE c.InvoiceChildID IN (SELECT CAST(items AS BIGINT) FROM dbo.Fun_SplitStr(@ID, ','))
          AND u.UsedFor = 1;

        -- Update FinalQuantityLeft in Inv_ItemStockByBatch
        UPDATE b
        SET b.FinalQuantityLeft = b.FinalQuantityLeft + t.Quantity
        FROM dbo.Inv_ItemStockByBatch b
        INNER JOIN @TempChildStock t ON b.ItemStockByBatchId = t.ItemStockByBatchId;

        -- Update IssuedQuantity in W_ItemStock
        UPDATE s
        SET s.IssuedQuantity = ISNULL(s.IssuedQuantity, 0) - t.Quantity
        FROM dbo.W_ItemStock s
        INNER JOIN (
            SELECT ItemID, SUM(Quantity) AS Quantity
            FROM @TempChildStock
            GROUP BY ItemID
        ) t ON s.ItemID = t.ItemID;

        -- Delete records from Inv_ItemStockUsed
        DELETE FROM dbo.Inv_ItemStockUsed
        WHERE ItemStockUsedID IN (SELECT ItemStockUsedID FROM @TempChildStock);

        -- Delete from child table
        DELETE FROM dbo.A_InvoiceChild 
        WHERE InvoiceChildID IN (SELECT CAST(items AS BIGINT) FROM dbo.Fun_SplitStr(@ID, ','));      
    END      
END
