CREATE OR ALTER PROCEDURE [dbo].[Inv_ItemStockPreProcessingAndProductModify] 
    @UsedFor INT, 
    @UsedForId BIGINT, -- PreProcessingID / ProductionID
    @CreatedBy NVARCHAR (200), 
    @CreatedDate DATETIME, 
    @ReturnVal INT = 0 OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;
    SET @ReturnVal = 0;

    -- ============================================================
    -- STRICT BLOCK 1: Validate individual batch stock availability
    -- ============================================================
    DECLARE @ShortfallMsg NVARCHAR(MAX) = '';

    SELECT @ShortfallMsg = STRING_AGG(
        CONCAT(m.ItemName, ' (Batch: ', ISNULL(b.BatchNo, 'N/A'), ', Available: ', b.FinalQuantityLeft, ', Required: ', u.Quantity, ')'),
        '; '
    )
    FROM dbo.Inv_ItemStockByBatch b 
    INNER JOIN dbo.Inv_ItemStockUsed u ON b.ItemStockByBatchId = u.ItemStockByBatchId 
    INNER JOIN dbo.W_MasterItem m ON b.ItemId = m.ItemID
    WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId
      AND (ROUND(b.FinalQuantityLeft, 4) - ROUND(u.Quantity, 4)) < 0;

    IF (@ShortfallMsg <> '' AND @ShortfallMsg IS NOT NULL)
    BEGIN
        DECLARE @ErrorText NVARCHAR(2048) = CONCAT('STRICT BLOCK: Cannot complete production. Insufficient batch stock for: ', @ShortfallMsg);
        RAISERROR(@ErrorText, 16, 1);
        RETURN;
    END

    -- ============================================================
    -- STRICT BLOCK 2: Validate total warehouse stock availability
    -- ============================================================
    DECLARE @TotalShortfallMsg NVARCHAR(MAX) = '';

    SELECT @TotalShortfallMsg = STRING_AGG(
        CONCAT(m.ItemName, ' (Total Available: ', s.FinalStock, ', Total Required: ', Req.TotalQty, ')'),
        '; '
    )
    FROM (
        SELECT b.ItemId, SUM(u.Quantity) AS TotalQty
        FROM dbo.Inv_ItemStockUsed u
        INNER JOIN dbo.Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
        WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId
        GROUP BY b.ItemId
    ) Req
    INNER JOIN dbo.W_ItemStock s ON Req.ItemId = s.ItemID
    INNER JOIN dbo.W_MasterItem m ON s.ItemID = m.ItemID
    WHERE (ROUND(s.FinalStock, 4) - ROUND(Req.TotalQty, 4)) < 0;

    IF (@TotalShortfallMsg <> '' AND @TotalShortfallMsg IS NOT NULL)
    BEGIN
        DECLARE @TotalErrorText NVARCHAR(2048) = CONCAT('STRICT BLOCK: Cannot complete production. Overall stock insufficient for: ', @TotalShortfallMsg);
        RAISERROR(@TotalErrorText, 16, 1);
        RETURN;
    END

    -- ============================================================
    -- PROCEED WITH COMPLETION
    -- ============================================================
    DECLARE @BomItemId BIGINT, @BomItemQty FLOAT = 0;
    DECLARE @BomId BIGINT, @UnitId BIGINT, @BatchNo NVARCHAR(100), @ExpiryDate DATETIME, @WarehouseId BIGINT;

    IF (@UsedFor = 3) -- PRODUCTION
    BEGIN
        SELECT TOP 1 
              @BomItemId = B.ItemId
            , @BomItemQty = (ISNULL(B.Quantity, 0) * ISNULL(P.BomQty, 0))
            , @BomId = B.BomId
            , @UnitId = B.UnitId
            , @BatchNo = P.BatchNo
            , @ExpiryDate = P.ExpiryDate
            , @WarehouseId = P.WarehouseId
        FROM W_Production P 
        INNER JOIN W_MasterBom B ON P.BomId = B.BomId
        WHERE P.ProductionId = @UsedForId;

        INSERT INTO Inv_ItemStockByBatchForBOM (IDFrom, BomID, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)
        VALUES (@UsedForId, @BomId, @BomItemId, @BomItemQty, @UnitId, @BatchNo, @ExpiryDate, @WarehouseId, @BomItemQty);
            
        INSERT INTO Inv_ItemStockByBatch (StockById, IdFrom, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)
        VALUES (4, @UsedForId, @BomItemId, @BomItemQty, @UnitId, @BatchNo, @ExpiryDate, @WarehouseId, @BomItemQty);
            
        SET @ReturnVal = SCOPE_IDENTITY();

        IF @ReturnVal > 0
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.W_ItemStock WHERE (ItemID = @BomItemId))
            BEGIN
                INSERT INTO W_ItemStock (OpeningQuantity, ProducedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity)
                VALUES (0, @BomItemQty, @BomItemId, @UnitId, 0, @CreatedBy, @BomItemQty, GETUTCDATE(), 0);
            END
            ELSE
            BEGIN
                UPDATE W_ItemStock
                SET ProducedQuantity = ISNULL(ProducedQuantity, 0) + @BomItemQty,
                    FinalStock = FinalStock + @BomItemQty
                WHERE ItemID = @BomItemId;
            END
        END
    END
    ELSE IF (@UsedFor = 2) -- PRE-PROCESSING
    BEGIN
        SELECT TOP 1 
              @BomItemId = B.ItemId
            , @BomItemQty = (ISNULL(B.Quantity, 0) * ISNULL(P.BomQty, 0))
            , @BomId = B.BomId
            , @UnitId = B.UnitId
            , @BatchNo = P.BatchNumberMade
            , @ExpiryDate = P.ExpiryDate
            , @WarehouseId = P.WarehouseId
        FROM W_PreProcessing P 
        INNER JOIN W_MasterBom B ON P.BomId = B.BomId
        WHERE P.PreProcessingId = @UsedForId;
        
        INSERT INTO Inv_ItemStockByBatchForBOM (IDFrom, BomID, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)
        VALUES (@UsedForId, @BomId, @BomItemId, @BomItemQty, @UnitId, @BatchNo, @ExpiryDate, @WarehouseId, @BomItemQty);
            
        SET @ReturnVal = SCOPE_IDENTITY();

        IF @ReturnVal > 0
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.W_ItemStock WHERE (ItemID = @BomItemId))
            BEGIN
                INSERT INTO W_ItemStock (OpeningQuantity, ProducedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity)
                VALUES (0, @BomItemQty, @BomItemId, @UnitId, 0, @CreatedBy, @BomItemQty, GETUTCDATE(), 0);
            END
            ELSE
            BEGIN
                UPDATE W_ItemStock
                SET ProducedQuantity = ISNULL(ProducedQuantity, 0) + @BomItemQty, 
                    FinalStock = FinalStock + @BomItemQty
                WHERE ItemID = @BomItemId;
            END
        END
    END

    -- =======================================================
    -- DEDUCT INGREDIENT STOCK ON FINALIZE
    -- =======================================================
    UPDATE b
        SET b.FinalQuantityLeft = ROUND(b.FinalQuantityLeft - u.Quantity, 4) 
    FROM dbo.Inv_ItemStockByBatch b 
    INNER JOIN dbo.Inv_ItemStockUsed u ON b.ItemStockByBatchId = u.ItemStockByBatchId
    WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId;

    UPDATE s
        SET s.IssuedQuantity = ROUND(ISNULL(s.IssuedQuantity, 0) + u.Quantity, 4), 
            s.FinalStock = ROUND(ISNULL(s.FinalStock, 0) - u.Quantity, 4) 
    FROM dbo.W_ItemStock s 
    INNER JOIN dbo.Inv_ItemStockByBatch b ON s.ItemID = b.ItemID 
    INNER JOIN dbo.Inv_ItemStockUsed u ON b.ItemStockByBatchId = u.ItemStockByBatchId
    WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId;
    
    -- =======================================================
    -- CALCULATE AND APPLY TOTAL MANUFACTURING COST
    -- =======================================================
    DECLARE @TotalCost FLOAT;
    
    SELECT @TotalCost = SUM(u.Quantity * (ISNULL(b.Amount, 0) / NULLIF(b.Quantity, 0)))
    FROM Inv_ItemStockUsed u 
    INNER JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
    WHERE u.UsedFor = @UsedFor AND u.UsedForId = @UsedForId;
    
    UPDATE Inv_ItemStockByBatch
        SET Amount = @TotalCost
    WHERE IdFrom = @UsedForId AND StockById = CASE WHEN @UsedFor = 3 THEN 4 ELSE 2 END;

    UPDATE Inv_ItemStockByBatchForBOM
        SET Amount = @TotalCost
    WHERE IdFrom = @UsedForId; 

END
