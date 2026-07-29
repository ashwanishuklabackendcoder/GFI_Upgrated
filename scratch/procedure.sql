
CREATE PROCEDURE [dbo].[Inv_ItemStockByBatchModifyFromMaster]                  
@ItemStockByBatchId bigint = 0,                  
@PurchaseID bigint = 0,                  
@ItemId bigint,                  
@WarehouseId bigint,                  
@Quantity float,                  
@UnitId bigint,                  
@BatchNo nvarchar(100),                  
@ExpiryDate datetime = null,                  
@StockById int = 1,                 
@CreatedBy nvarchar(200),                  
@DeletedBatchIds nvarchar(max) = null,                
@Amount float = null,
@ReturnVal int = 0 output                  
AS                  
BEGIN                  
    SET NOCOUNT ON                  
    SET @ReturnVal = 0                

    IF (@DeletedBatchIds IS NOT NULL AND @DeletedBatchIds <> '')
    BEGIN
        DECLARE @DeletedTable TABLE (Id BIGINT)

        INSERT INTO @DeletedTable
        SELECT value FROM STRING_SPLIT(@DeletedBatchIds, ',')

        DELETE FROM Inv_ItemStockByBatchForBOM
        WHERE ItemId = @ItemId AND BatchNo IN (
            SELECT BatchNo FROM Inv_ItemStockByBatch WHERE ItemStockByBatchId IN (SELECT Id FROM @DeletedTable)
        )

        DELETE FROM Inv_ItemStockByBatch
        WHERE ItemStockByBatchId IN (SELECT Id FROM @DeletedTable)

        UPDATE W_ItemStock
        SET 
            OpeningQuantity = OQ,
            PurchasedQuantity = PQ,
            FinalStock = OQ + PQ - ISNULL(IssuedQuantity, 0) - ISNULL(RemovedQuantity, 0)
        FROM W_ItemStock
        CROSS APPLY (
            SELECT 
                ISNULL((SELECT SUM(Quantity) FROM Inv_ItemStockByBatch WHERE ItemID = @ItemId AND (IdFrom = 0 OR ISNULL(StockById, 1) IN (2, 3))), 0) as OQ,
                ISNULL((SELECT SUM(Quantity) FROM Inv_ItemStockByBatch WHERE ItemID = @ItemId AND ISNULL(StockById, 1) = 1 AND IdFrom > 0), 0) as PQ
        ) ca
        WHERE ItemID = @ItemId
    END  

    DECLARE @IdFrom BIGINT, @GoodReceiveDate DATE, @TentativeExpiryDays INT, @ShortName NVARCHAR(10)   

    IF(@PurchaseID <> 0)
        SELECT @IdFrom = PurchaseItemID FROM W_PurchaseChild WHERE PurchaseItemID = @PurchaseID AND ItemID = @ItemID    
    ELSE
        SELECT @IdFrom = 0
    
    DECLARE @ActualUnitId BIGINT = @UnitId
    IF NOT EXISTS (SELECT 1 FROM W_MasterUnit WHERE UnitId = @ActualUnitId)
    BEGIN
        SELECT TOP 1 @ActualUnitId = UnitId FROM W_MasterUnit
    END

    DECLARE @ActualWarehouseId BIGINT = @WarehouseId
    IF NOT EXISTS (SELECT 1 FROM W_MasterWarehouse WHERE WarehouseId = @ActualWarehouseId)
    BEGIN
        SELECT TOP 1 @ActualWarehouseId = WarehouseId FROM W_MasterWarehouse
    END

    IF @ItemStockByBatchId = 0 
    BEGIN                
        IF NOT EXISTS (SELECT 1 FROM Inv_ItemStockByBatch WHERE ItemStockByBatchId = @ItemStockByBatchId)                
        BEGIN       

            IF(ISNULL(@BatchNo,'') = '')  
                SET @BatchNo = @ShortName + ' | ' + FORMAT(@GoodReceiveDate, 'yyMMdd')  

            INSERT INTO Inv_ItemStockByBatch
            (StockById, IdFrom, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft, Amount)     
            VALUES 
            (@StockById, ISNULL(@IdFrom,0), @ItemId, @Quantity, @ActualUnitId, @BatchNo, @ExpiryDate, @ActualWarehouseId, @Quantity, @Amount)    

            SET @ReturnVal = SCOPE_IDENTITY()       

            DECLARE @ValBomId BIGINT
            SELECT TOP 1 @ValBomId = BomId FROM W_MasterBom WHERE ItemId = @ItemId
            IF @ValBomId IS NULL
                SELECT TOP 1 @ValBomId = BomId FROM W_MasterBom

            INSERT INTO Inv_ItemStockByBatchForBOM
            (IDFrom, BomId, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)     
            VALUES 
            (ISNULL(@IdFrom,0), @ValBomId, @ItemId, @Quantity, @ActualUnitId, @BatchNo, @ExpiryDate, @ActualWarehouseId, @Quantity)

            IF NOT EXISTS (SELECT 1 FROM W_ItemStock WHERE ItemID = @ItemId)                
            BEGIN                
                INSERT INTO W_ItemStock      
                (OpeningQuantity, PurchasedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity)          
                VALUES      
                (
                    CASE WHEN @IdFrom = 0 OR @StockById IN (2, 3) THEN @Quantity ELSE 0 END, 
                    CASE WHEN @StockById = 1 AND @IdFrom > 0 THEN @Quantity ELSE 0 END, 
                    @ItemId, 
                    @ActualUnitId, 
                    0, 
                    @CreatedBy, 
                    @Quantity, 
                    GETDATE(), 
                    0
                )      
            END          
            ELSE         
            BEGIN        
                UPDATE W_ItemStock       
                SET 
                    OpeningQuantity = OQ,
                    PurchasedQuantity = PQ,
                    FinalStock = OQ + PQ - ISNULL(IssuedQuantity, 0) - ISNULL(RemovedQuantity, 0)
                FROM W_ItemStock
                CROSS APPLY (
                    SELECT 
                        ISNULL((SELECT SUM(Quantity) FROM Inv_ItemStockByBatch WHERE ItemID = @ItemId AND (IdFrom = 0 OR ISNULL(StockById, 1) IN (2, 3))), 0) as OQ,
                        ISNULL((SELECT SUM(Quantity) FROM Inv_ItemStockByBatch WHERE ItemID = @ItemId AND ISNULL(StockById, 1) = 1 AND IdFrom > 0), 0) as PQ
                ) ca
                WHERE ItemID = @ItemId        
            END   

        END                
        ELSE                
            SET @ReturnVal = -1                
    END                

    ELSE                
    BEGIN        
        IF EXISTS (SELECT 1 FROM Inv_ItemStockByBatch WHERE ItemStockByBatchId = @ItemStockByBatchId)                
        BEGIN     
   
            DECLARE @OldBatchNo NVARCHAR(50), @OldItemId BIGINT
            SELECT @OldBatchNo = BatchNo, @OldItemId = ItemId FROM Inv_ItemStockByBatch WHERE ItemStockByBatchId = @ItemStockByBatchId

            UPDATE Inv_ItemStockByBatch     
            SET 
                StockById = @StockById,
                IdFrom = ISNULL(@IdFrom,0),
                ItemId = @ItemId,
                Quantity = @Quantity,
                FinalQuantityLeft = @Quantity, 
                Unit = @ActualUnitId,
                BatchNo = @BatchNo,
                ExpiryDate = @ExpiryDate,
                WarehouseId = @ActualWarehouseId,
                Amount = @Amount
            WHERE ItemStockByBatchId = @ItemStockByBatchId    

            DECLARE @UpdBomId BIGINT
            SELECT TOP 1 @UpdBomId = BomId FROM W_MasterBom WHERE ItemId = @ItemId
            IF @UpdBomId IS NULL
                SELECT TOP 1 @UpdBomId = BomId FROM W_MasterBom

            IF EXISTS (SELECT 1 FROM Inv_ItemStockByBatchForBOM WHERE ItemId = @OldItemId AND BatchNo = @OldBatchNo)
            BEGIN
                UPDATE Inv_ItemStockByBatchForBOM
                SET 
                    IDFrom = ISNULL(@IdFrom,0),
                    BomId = @UpdBomId,
                    ItemId = @ItemId,
                    Quantity = @Quantity,
                    FinalQuantityLeft = @Quantity,
                    Unit = @ActualUnitId,
                    BatchNo = @BatchNo,
                    ExpiryDate = @ExpiryDate,
                    WarehouseId = @ActualWarehouseId
                WHERE ItemId = @OldItemId AND BatchNo = @OldBatchNo
            END
            ELSE
            BEGIN
                INSERT INTO Inv_ItemStockByBatchForBOM
                (IDFrom, BomId, ItemId, Quantity, Unit, BatchNo, ExpiryDate, WarehouseId, FinalQuantityLeft)     
                VALUES 
                (ISNULL(@IdFrom,0), @UpdBomId, @ItemId, @Quantity, @ActualUnitId, @BatchNo, @ExpiryDate, @ActualWarehouseId, @Quantity)
            END

            IF NOT EXISTS (SELECT 1 FROM W_ItemStock WHERE ItemID = @ItemID)      
            BEGIN      
                INSERT INTO W_ItemStock      
                (OpeningQuantity, PurchasedQuantity, ItemID, UnitId, IssuedQuantity, CreatedBy, FinalStock, OpeningStockDate, RemovedQuantity)      
                VALUES      
                (
                    CASE WHEN @IdFrom = 0 OR @StockById IN (2, 3) THEN @Quantity ELSE 0 END, 
                    CASE WHEN @StockById = 1 AND @IdFrom > 0 THEN @Quantity ELSE 0 END, 
                    @ItemID, 
                    @ActualUnitId, 
                    0, 
                    @CreatedBy, 
                    @Quantity, 
                    GETDATE(), 
                    0
                )      
            END      
            ELSE      
            BEGIN      
                UPDATE W_ItemStock      
                SET 
                    OpeningQuantity = OQ,
                    PurchasedQuantity = PQ,
                    FinalStock = OQ + PQ - ISNULL(IssuedQuantity, 0) - ISNULL(RemovedQuantity, 0)
                FROM W_ItemStock
                CROSS APPLY (
                    SELECT 
                        ISNULL((SELECT SUM(Quantity) FROM Inv_ItemStockByBatch WHERE ItemID = @ItemID AND (IdFrom = 0 OR ISNULL(StockById, 1) IN (2, 3))), 0) as OQ,
                        ISNULL((SELECT SUM(Quantity) FROM Inv_ItemStockByBatch WHERE ItemID = @ItemID AND ISNULL(StockById, 1) = 1 AND IdFrom > 0), 0) as PQ
                ) ca
                WHERE ItemID = @ItemID   
            END  

            SET @ReturnVal = @ItemStockByBatchId                
        END                
        ELSE                
            SET @ReturnVal = @ItemStockByBatchId               
    END                
END
