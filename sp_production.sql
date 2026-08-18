CREATE PROCEDURE [dbo].[Inv_ItemStockPreProcessingAndProductModify] 
    @UsedFor int, 
    @UsedForId bigint, --PreProcessingID/ProductID 
    @CreatedBy nvarchar(200), 
    @CreatedDate datetime, 
    @ReturnVal int=0 output 
AS 
BEGIN 
    SET NOCOUNT ON; 
    SET @ReturnVal = 0; 
    
    DECLARE @BomItemId INT, @BomItemQty FLOAT = 0;
    
    IF (@UsedFor = 3) -- Production
    BEGIN 
        SELECT @BomItemId = B.ItemId, 
               @BomItemQty = (ISNULL(P.FillingBottles, 0) * ISNULL(P.FillingPerBottleUnit, 0)) + ISNULL(P.ExtraBottles, 0)
        FROM W_Production P 
        INNER JOIN W_MasterBom B ON P.BomId = B.BomId 
        WHERE P.ProductionId = @UsedForId;
        
        INSERT INTO Inv_ItemStockByBatchForBOM (IDFrom, BomId, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft) 
        SELECT @UsedForId, B.BomId, B.ItemId, @BomItemQty, B.UnitId, P.BatchNo, P.ExpiryDate, P.WarehouseId, @BomItemQty
        FROM W_Production P 
        INNER JOIN W_MasterBom B ON P.BomId = B.BomId 
        WHERE P.ProductionId = @UsedForId;
        
        SET @ReturnVal = SCOPE_IDENTITY();
        
        IF (@ReturnVal > 0) 
        BEGIN 
            IF NOT EXISTS (SELECT 1 FROM dbo.W_ItemStock WHERE ItemID = @BomItemId) 
            BEGIN 
                INSERT INTO W_ItemStock (OpeningQuantity, PurchasedQuantity, ProducedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity) 
                SELECT 0, 0, @BomItemQty, B.ItemId, B.UnitId, 0, @CreatedBy, @BomItemQty, GETDATE(), 0
                FROM W_Production P 
                INNER JOIN W_MasterBom B ON P.BomId = B.BomId 
                WHERE P.ProductionId = @UsedForId;
            END 
            ELSE 
            BEGIN 
                UPDATE W_ItemStock 
                SET ProducedQuantity = ProducedQuantity + @BomItemQty, 
                    FinalStock = FinalStock + @BomItemQty 
                WHERE ItemID = @BomItemId;
            END 
        END 
    END 
    ELSE IF (@UsedFor = 2) -- Pre-Processing
    BEGIN 
        SELECT @BomItemId = P.ItemId, 
               @BomItemQty = ISNULL(P.QuantityMade, 0)
        FROM W_PreProcessing P 
        WHERE P.PreProcessingId = @UsedForId;
        
        INSERT INTO Inv_ItemStockByBatchForBOM (IDFrom, BomId, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft) 
        SELECT @UsedForId, P.BomId, P.ItemId, P.QuantityMade, P.UnitMade, P.BatchNumberMade, P.ExpiryDate, P.WarehouseId, P.QuantityMade
        FROM W_PreProcessing P 
        WHERE P.PreProcessingId = @UsedForId;
        
        SET @ReturnVal = SCOPE_IDENTITY();
        
        IF (@ReturnVal > 0) 
        BEGIN 
            IF NOT EXISTS (SELECT 1 FROM dbo.W_ItemStock WHERE ItemID = @BomItemId) 
            BEGIN 
                INSERT INTO W_ItemStock (OpeningQuantity, PurchasedQuantity, ProducedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity) 
                SELECT 0, 0, @BomItemQty, P.ItemId, P.UnitMade, 0, @CreatedBy, @BomItemQty, GETDATE(), 0
                FROM W_PreProcessing P 
                WHERE P.PreProcessingId = @UsedForId;
            END 
            ELSE 
            BEGIN 
                UPDATE W_ItemStock 
                SET ProducedQuantity = ProducedQuantity + @BomItemQty, 
                    FinalStock = FinalStock + @BomItemQty 
                WHERE ItemID = @BomItemId;
            END 
        END 
    END 

    -- Deduct Ingredient Stock on Finalize
    UPDATE b 
    SET b.FinalQuantityLeft = b.FinalQuantityLeft - u.Quantity 
    FROM dbo.Inv_ItemStockByBatch b 
    INNER JOIN dbo.Inv_ItemStockUsed u ON b.ItemStockByBatchId = u.ItemStockByBatchId 
    WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId;

    UPDATE s 
    SET s.IssuedQuantity = ISNULL(s.IssuedQuantity, 0) + t.Quantity 
    FROM dbo.W_ItemStock s 
    INNER JOIN (
        SELECT b.ItemID, SUM(u.Quantity) AS Quantity 
        FROM dbo.Inv_ItemStockByBatch b 
        INNER JOIN dbo.Inv_ItemStockUsed u ON b.ItemStockByBatchId = u.ItemStockByBatchId 
        WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId 
        GROUP BY b.ItemID
    ) t ON s.ItemID = t.ItemID;

    -- Calculate and Apply Total Manufacturing Cost
    DECLARE @TotalCost FLOAT;
    SELECT @TotalCost = SUM(u.Quantity * (ISNULL(b.Amount, 0) / NULLIF(b.Quantity, 0))) 
    FROM Inv_ItemStockUsed u 
    INNER JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId 
    WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId;

    -- Update the output batch created for this manufacturing run 
    UPDATE Inv_ItemStockByBatch 
    SET Amount = @TotalCost 
    WHERE IdFrom = @UsedForId AND StockById = @UsedFor;
END;
