CREATE PROCEDURE [dbo].[BatchNumberList]
    @BatchNo NVARCHAR(50),
    @CurrentPage INT = 1 OUTPUT,
    @RecordPerPage INT = 10,
    @TotalRecord INT = 0 OUTPUT,
    @SortOrd VARCHAR(5) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RecQuery AS NVARCHAR(MAX)

    -- Counting total records for this BatchNo
    SET @RecQuery = 'SELECT @TotalRecord = COUNT(1)
                     FROM Inv_ItemStockByBatch 
                     WHERE BatchNo = @BatchNo
                     AND (
                         StockById NOT IN (2, 4)
                         OR (StockById = 2 AND EXISTS (SELECT 1 FROM dbo.W_PreProcessing pp WHERE pp.PreProcessingId = IdFrom AND pp.IsComplete = 1))
                         OR (StockById = 4 AND EXISTS (
                             SELECT 1 FROM dbo.W_Production p 
                             WHERE p.ProductionId = IdFrom 
                             AND NOT EXISTS (
                                 SELECT 1 FROM dbo.W_MasterBomItems AS Bom 
                                 WHERE Bom.BomId = p.BomId 
                                 AND NOT EXISTS (
                                     SELECT 1 FROM dbo.inv_itemstockused AS StockUsed 
                                     WHERE StockUsed.UsedFor = 3 AND StockUsed.UsedForId = p.ProductionId
                                 )
                             )
                         ))
                     )';

    EXEC dbo.sp_ExecuteSql @RecQuery, N'@BatchNo NVARCHAR(50), @TotalRecord INT OUTPUT', @BatchNo, @TotalRecord OUTPUT;

    DECLARE @MaxPage INT
    SET @MaxPage = CEILING(ISNULL(@TotalRecord, 0) / (@RecordPerPage * 1.0));

    IF @MaxPage < @CurrentPage
    BEGIN
        IF @MaxPage <= 0
            SET @CurrentPage = 1;
        ELSE
            SET @CurrentPage = @MaxPage;
    END;

    DECLARE @Top AS INT
    DECLARE @Bottom AS INT
    SET @Top = ((@CurrentPage - 1) * @RecordPerPage + 1);
    SET @Bottom = (@CurrentPage * @RecordPerPage);

    -- Final query with BatchNo filter
    SET @RecQuery = 'SELECT * FROM (
                        SELECT ROW_NUMBER() OVER (ORDER BY t5.ItemStockByBatchId ' + @SortOrd + ') AS RowNumber,
                               t5.ItemStockByBatchId AS Id,
                               t5.BatchNo,
                               t5.ExpiryDate,
                               CASE
                                   WHEN t5.StockById = 1 THEN t2.GoodsRecievedDate
                                   WHEN t5.StockById = 2 OR t5.StockById = 4 THEN prod.CookingDate
                                   WHEN t5.StockById = 3 THEN stock.OpeningStockDate
                               END AS ProcessingDate,
                               t3.ItemName,
                               CASE
                                   WHEN t5.StockById = 1 THEN t4.AccountName
                                   WHEN t5.StockById = 2 OR t5.StockById = 4 THEN ''Production''
                                   WHEN t5.StockById = 3 THEN ''Opening Stock''
                                   ELSE ''Manual Entry''
                               END AS AccountName
                        FROM Inv_ItemStockByBatch t5
                        LEFT JOIN W_MasterItem t3 ON t5.ItemId = t3.ItemID
                        LEFT JOIN W_PurchaseChild t1 ON t5.IdFrom = t1.PurchaseItemID AND t5.StockById = 1
                        LEFT JOIN W_PurchaseMaster t2 ON t1.PurchaseID = t2.PurchaseID
                        LEFT JOIN A_MasterAccounts t4 ON t4.AccountId = t2.AccountID
                        LEFT JOIN W_Production prod ON t5.IdFrom = prod.ProductionId AND (t5.StockById = 2 OR t5.StockById = 4)
                        LEFT JOIN W_ItemStock stock ON t5.IdFrom = stock.StockID AND t5.StockById = 3
                        WHERE t5.BatchNo = @BatchNo
                        AND (
                            t5.StockById NOT IN (2, 4)
                            OR (t5.StockById = 2 AND EXISTS (SELECT 1 FROM dbo.W_PreProcessing pp WHERE pp.PreProcessingId = t5.IdFrom AND pp.IsComplete = 1))
                            OR (t5.StockById = 4 AND EXISTS (
                                SELECT 1 FROM dbo.W_Production p 
                                WHERE p.ProductionId = t5.IdFrom 
                                AND NOT EXISTS (
                                    SELECT 1 FROM dbo.W_MasterBomItems AS Bom 
                                    WHERE Bom.BomId = p.BomId 
                                    AND NOT EXISTS (
                                        SELECT 1 FROM dbo.inv_itemstockused AS StockUsed 
                                        WHERE StockUsed.UsedFor = 3 AND StockUsed.UsedForId = p.ProductionId
                                    )
                                )
                            ))
                        )
                     ) AS t1
                     WHERE t1.RowNumber >= ' + CAST(@Top AS VARCHAR) + ' AND t1.RowNumber <= ' + CAST(@Bottom AS VARCHAR);

    EXEC dbo.sp_ExecuteSql @RecQuery, N'@BatchNo NVARCHAR(50)', @BatchNo;
END;
