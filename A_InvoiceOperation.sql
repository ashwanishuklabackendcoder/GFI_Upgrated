ALTER PROCEDURE [dbo].[A_InvoiceOperation]      
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
        DECLARE @TempStockUsed TABLE (
            ItemStockByBatchId BIGINT,
            Quantity FLOAT,
            ItemID BIGINT
        );

        INSERT INTO @TempStockUsed (ItemStockByBatchId, Quantity, ItemID)
        SELECT u.ItemStockByBatchId, u.Quantity, b.ItemID
        FROM dbo.Inv_ItemStockUsed u
        INNER JOIN dbo.Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
        WHERE u.UsedFor = 1 AND u.UsedForId IN (SELECT CAST(items AS BIGINT) FROM dbo.Fun_SplitStr(@ID, ','));

        -- Update FinalQuantityLeft in Inv_ItemStockByBatch
        UPDATE b
        SET b.FinalQuantityLeft = b.FinalQuantityLeft + t.Quantity
        FROM dbo.Inv_ItemStockByBatch b
        INNER JOIN @TempStockUsed t ON b.ItemStockByBatchId = t.ItemStockByBatchId;

        -- Update IssuedQuantity in W_ItemStock
        UPDATE s
        SET s.IssuedQuantity = ISNULL(s.IssuedQuantity, 0) - t.Quantity
        FROM dbo.W_ItemStock s
        INNER JOIN (
            SELECT ItemID, SUM(Quantity) AS Quantity
            FROM @TempStockUsed
            GROUP BY ItemID
        ) t ON s.ItemID = t.ItemID;

        -- Delete records from Inv_ItemStockUsed
        DELETE FROM dbo.Inv_ItemStockUsed
        WHERE UsedFor = 1 AND UsedForId IN (SELECT CAST(items AS BIGINT) FROM dbo.Fun_SplitStr(@ID, ','));

        -- Delete from child table
        DELETE FROM dbo.A_InvoiceChild
        WHERE InvoiceID IN (SELECT CAST(items AS BIGINT) FROM dbo.Fun_SplitStr(@ID, ','));

        -- Delete from master table
        DELETE FROM dbo.A_InvoiceMaster
        WHERE InvoiceID IN (SELECT CAST(items AS BIGINT) FROM dbo.Fun_SplitStr(@ID, ','));      
    END      
END
