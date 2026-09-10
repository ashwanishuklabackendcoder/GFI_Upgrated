-- 2. UPDATE SP: ItemBatchNumberList
ALTER PROCEDURE [dbo].[ItemBatchNumberList]
    @ItemID int=0,
    @CurrentPage INT = 1 OUTPUT,
    @RecordPerPage INT = 10,
    @TotalRecord INT = 0 OUTPUT,
    @SortOrd VARCHAR(5) = 'DESC'
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Query AS VARCHAR(1000)
    DECLARE @RecQuery AS NVARCHAR(MAX)
    SET @Query = ''
    
    SET @RecQuery = 'SELECT @TotalRecord = COUNT(DISTINCT BatchNo)
                     FROM Inv_ItemStockByBatch WHERE ItemId = @ItemID
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
    
    EXEC dbo.sp_ExecuteSql @RecQuery, N'@ItemID INT, @TotalRecord INT OUTPUT', @ItemID, @TotalRecord OUTPUT;
    
    DECLARE @Top INT = ((@CurrentPage - 1) * @RecordPerPage) + 1;
    DECLARE @Bottom INT = @CurrentPage * @RecordPerPage;

    SET @RecQuery = 'SELECT * FROM (
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY t5.BatchNo ' + @SortOrd + ') AS RowNumber,
                            MAX(t5.ItemStockByBatchId) AS Id,
                            t5.BatchNo,
                            MAX(CONVERT(VARCHAR(10), t5.ExpiryDate, 120)) AS ExpiryDate,
                            MAX(CONVERT(VARCHAR(10), COALESCE(t2.GoodsRecievedDate, pre.ProcessingDate, prod.CookingDate, stock.OpeningStockDate), 120)) AS ProcessingDate,
                            MAX(t3.ItemName) AS ItemName,
                            MAX(COALESCE(t4.AccountName, ''Production'', ''Opening Stock'')) AS AccountName,
                            MAX(t5.FinalQuantityLeft) AS AvailableQty,
                            MAX(mu.UnitName) AS UnitName
                        FROM Inv_ItemStockByBatch t5
                        LEFT JOIN W_MasterItem t3 ON t5.ItemId = t3.ItemID
                        LEFT JOIN W_MasterUnit mu ON ISNULL(t3.PurchaseUnit, t5.Unit) = mu.UnitId
                        LEFT JOIN W_PurchaseChild t1 ON t5.IdFrom = t1.PurchaseItemID AND t5.StockById = 1
                        LEFT JOIN W_PurchaseMaster t2 ON t1.PurchaseID = t2.PurchaseID
                        LEFT JOIN A_MasterAccounts t4 ON t4.AccountId = t2.AccountID
                        LEFT JOIN W_PreProcessing pre ON t5.IdFrom = pre.PreProcessingId AND t5.StockById = 2
                        LEFT JOIN W_Production prod ON t5.IdFrom = prod.ProductionId AND t5.StockById = 4
                        LEFT JOIN W_ItemStock stock ON t5.IdFrom = stock.StockID AND t5.StockById = 3
                        WHERE t5.ItemID = @ItemID
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
                        GROUP BY t5.BatchNo
                    ) AS t1
                    WHERE t1.RowNumber >= ' + CAST(@Top AS VARCHAR) + ' AND t1.RowNumber <= ' + CAST(@Bottom AS VARCHAR);
    
    EXEC dbo.sp_ExecuteSql @RecQuery, N'@ItemID INT', @ItemID;
END