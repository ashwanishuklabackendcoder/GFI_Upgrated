-- Patch to update trg_SyncBatchQuantity trigger on Inv_ItemStockUsed table

ALTER TRIGGER dbo.trg_SyncBatchQuantity
ON dbo.Inv_ItemStockUsed
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE b
    SET b.FinalQuantityLeft = b.Quantity - ISNULL((
        SELECT SUM(u.Quantity)
        FROM dbo.Inv_ItemStockUsed u
        LEFT JOIN dbo.W_PreProcessing p ON u.UsedFor = 2 AND u.UsedForId = p.PreProcessingId
        LEFT JOIN dbo.W_Production pr ON u.UsedFor = 3 AND u.UsedForId = pr.ProductionId
        WHERE u.ItemStockByBatchId = b.ItemStockByBatchId
          AND (
               u.UsedFor = 1
               OR (u.UsedFor = 2 AND ISNULL(p.IsComplete, 0) = 1)
               OR (u.UsedFor = 3 AND ISNULL(pr.IsComplete, 0) = 1)
              )
    ), 0)
    FROM dbo.Inv_ItemStockByBatch b
    WHERE b.ItemStockByBatchId IN (SELECT ItemStockByBatchId FROM inserted WHERE ItemStockByBatchId IS NOT NULL)
       OR b.ItemStockByBatchId IN (SELECT ItemStockByBatchId FROM deleted WHERE ItemStockByBatchId IS NOT NULL);
END;
GO
