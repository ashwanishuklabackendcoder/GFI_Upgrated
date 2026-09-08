-- ============================================================================
-- PRODUCTION DATABASE PATCH SCRIPT
-- Purpose:
-- 1. Update trg_SyncBatchQuantity to exclude draft consumption rows from deducting stock.
-- 2. Update report SPs to dynamically display unit from W_MasterItem.PurchaseUnit.
-- Assurance:
-- - NO historical stock quantities (Quantity, FinalQuantityLeft, OpeningQuantity) are changed.
-- - NO past transaction records or financial values are altered.
-- ============================================================================

-- 1. UPDATE TRIGGER: trg_SyncBatchQuantity
-- ============================================================================
-- PRODUCTION DATABASE PATCH SCRIPT
-- Purpose:
-- 1. Update trg_SyncBatchQuantity to exclude draft consumption rows from deducting stock.
-- 2. Update report SPs to dynamically display unit from W_MasterItem.PurchaseUnit.
-- Assurance:
-- - NO historical stock quantities (Quantity, FinalQuantityLeft, OpeningQuantity) are changed.
-- - NO past transaction records or financial values are altered.
-- ============================================================================

-- 1. UPDATE TRIGGER: trg_SyncBatchQuantity
CREATE TRIGGER trg_SyncBatchQuantity ON Inv_ItemStockUsed AFTER INSERT, UPDATE, DELETE AS BEGIN SET NOCOUNT ON; UPDATE b SET b.FinalQuantityLeft = b.Quantity - ISNULL((SELECT SUM(u.Quantity) FROM dbo.Inv_ItemStockUsed u LEFT JOIN dbo.W_PreProcessing p ON u.UsedFor = 2 AND u.UsedForId = p.PreProcessingId LEFT JOIN dbo.W_Production pr ON u.UsedFor = 3 AND u.UsedForId = pr.ProductionId WHERE u.ItemStockByBatchId = b.ItemStockByBatchId AND (u.UsedFor = 1 OR (u.UsedFor = 2 AND ISNULL(p.IsComplete, 0) = 1) OR (u.UsedFor = 3 AND ISNULL(pr.IsComplete, 0) = 1))), 0) FROM dbo.Inv_ItemStockByBatch b WHERE b.ItemStockByBatchId IN (SELECT ItemStockByBatchId FROM inserted WHERE ItemStockByBatchId IS NOT NULL) OR b.ItemStockByBatchId IN (SELECT ItemStockByBatchId FROM deleted WHERE ItemStockByBatchId IS NOT NULL); END;
GO

-- 2. UPDATE SP: ItemBatchNumberList
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
                            MAX(CONVERT(VARCHAR(10), COALESCE(t2.GoodsRecievedDate, prod.CreatedDate, stock.OpeningStockDate), 120)) AS ProcessingDate,
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
                        LEFT JOIN W_Production prod ON t5.IdFrom = prod.ProductionId AND (t5.StockById = 2 OR t5.StockById = 4)
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
GO

-- 3. UPDATE SP: Inv_ItemStockByBatchList
-- 3. UPDATE SP: Inv_ItemStockByBatchList
ALTER PROCEDURE [dbo].[Inv_ItemStockByBatchList]        
    @ItemStockByBatchId int=0,        
    @StockById int=0,  
    @ItemID int=0,  
    @CurrentPage int=1 output,          
    @RecordPerPage int=1000,          
    @TotalRecord  int=0 output        
AS        
BEGIN        
    SET NOCOUNT ON;        
    DECLARE @Query AS varchar(1000)        
    DECLARE @RecQuery AS nvarchar(MAX)        
    Set @Query=''        
        
    IF @ItemStockByBatchId > 0        
        SET @Query = @Query + ' and Inv_ItemStockByBatch.ItemStockByBatchId=' + cast(@ItemStockByBatchId as varchar)        
            
    IF @StockById > 0        
        SET @Query=@Query+' and Inv_ItemStockByBatch.StockById=' + cast(@StockById as varchar)        
          
    IF @ItemID > 0        
        SET @Query=@Query+' and Inv_ItemStockByBatch.ItemID=' + cast(@ItemID as varchar)        
        
    SET @RecQuery='SELECT @TotalRecord=count(1) FROM Inv_ItemStockByBatch WHERE 1=1' + @Query        
    EXEC dbo.sp_ExecuteSql @RecQuery, N'@TotalRecord INT OUTPUT', @TotalRecord OUTPUT        
            
    DECLARE @Top INT = ((@CurrentPage - 1) * @RecordPerPage) + 1;        
    DECLARE @Bottom INT = @CurrentPage * @RecordPerPage;        
        
    SET @RecQuery='SELECT * FROM (
        SELECT ROW_NUMBER() OVER (ORDER BY Inv_ItemStockByBatch.ItemStockByBatchId DESC) AS RowNumber, 
               Inv_ItemStockByBatch.ItemStockByBatchId, Inv_ItemStockByBatch.StockById, Inv_ItemStockByBatch.IdFrom, 
               Inv_ItemStockByBatch.ItemId, Inv_ItemStockByBatch.Quantity, Inv_ItemStockByBatch.Unit, Inv_ItemStockByBatch.BatchNo, 
               convert(varchar(12),Inv_ItemStockByBatch.ExpiryDate,106) ExpiryDate, Inv_ItemStockByBatch.WarehouseId, Inv_ItemStockByBatch.FinalQuantityLeft,
               ItemName, UnitName, WarehouseName,
               (Inv_ItemStockByBatch.Quantity - Inv_ItemStockByBatch.FinalQuantityLeft) AS IssuedStock,
               Inv_ItemStockByBatch.Amount AS StockValue
        FROM (
            SELECT b.ItemStockByBatchId, b.StockById, b.IdFrom, b.ItemId, b.Quantity, b.Unit, b.BatchNo, b.ExpiryDate, b.WarehouseId, 
                   b.FinalQuantityLeft, b.Amount 
            FROM dbo.Inv_ItemStockByBatch b
            UNION ALL
            SELECT bom.ItemStockByBatchID as ItemStockByBatchId, 
                   CASE WHEN bom.IDFrom = 0 THEN 3 WHEN EXISTS (SELECT 1 FROM dbo.W_MasterItem i WHERE i.ItemID = bom.ItemId AND i.ItemTypeId = 2) THEN 2 ELSE 4 END as StockById, 
                   IDFrom as IdFrom, ItemId, Quantity, Unit, BatchNo, CAST(ExpiryDate AS date) as ExpiryDate, WarehouseId, FinalQuantityLeft, Amount 
            FROM dbo.Inv_ItemStockByBatchForBOM bom
            WHERE not exists (
                SELECT 1 FROM dbo.Inv_ItemStockByBatch b 
                WHERE b.IdFrom = bom.IDFrom and b.ItemId = bom.ItemId and b.BatchNo = bom.BatchNo and b.StockById = (CASE WHEN bom.IDFrom = 0 THEN 3 WHEN EXISTS (SELECT 1 FROM dbo.W_MasterItem i WHERE i.ItemID = bom.ItemId AND i.ItemTypeId = 2) THEN 2 ELSE 4 END)
            )
        ) Inv_ItemStockByBatch  
        INNER JOIN W_MasterItem ON W_MasterItem.ItemID=Inv_ItemStockByBatch.ItemId  
        LEFT JOIN W_MasterUnit ON W_MasterUnit.UnitId=ISNULL(W_MasterItem.PurchaseUnit, Inv_ItemStockByBatch.Unit)  
        LEFT JOIN W_MasterWarehouse ON W_MasterWarehouse.WarehouseId= Inv_ItemStockByBatch.WarehouseId  
        WHERE 1=1 ' + @Query + '
    ) t1 WHERE t1.RowNumber>=' + cast(@Top as varchar) + ' and t1.RowNumber <=' + cast(@Bottom as varchar);
        
    EXEC dbo.sp_ExecuteSql @RecQuery;
END;
GO

-- 4. UPDATE SP: Rpt_ItemStockTraceability
-- 4. UPDATE SP: Rpt_ItemStockTraceability
ALTER PROCEDURE [dbo].[Rpt_ItemStockTraceability]
@ItemId bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        COALESCE(
            CASE WHEN b.StockById = 1 THEN (SELECT GoodsRecievedDate FROM W_PurchaseMaster p JOIN W_PurchaseChild pc ON p.PurchaseID = pc.PurchaseID WHERE pc.PurchaseItemID = b.IdFrom) END,
            CASE WHEN b.StockById = 2 THEN (SELECT ProcessingDate FROM W_PreProcessing pp WHERE pp.PreProcessingId = b.IdFrom) END,
            CASE WHEN b.StockById = 4 THEN (SELECT CookingDate FROM W_Production p WHERE p.ProductionId = b.IdFrom) END,
            CASE WHEN b.StockById = 3 THEN (SELECT TOP 1 OpeningStockDate FROM W_ItemStock s WHERE s.ItemID = b.ItemId) END,
            NULL
        ) AS TransactionDate,
        b.BatchNo,
        CASE 
            WHEN b.StockById = 1 THEN 'Purchased'
            WHEN b.StockById = 2 OR b.StockById = 4 THEN 'Produced'
            WHEN b.StockById = 3 OR b.StockById = 0 THEN 'Opening Stock'
            ELSE 'Added (Other)'
        END AS TransactionType,
        'Added to Stock' AS Reference,
        CAST(b.Quantity AS float) AS InQty,
        CAST(0.0 AS float) AS OutQty,
        CAST(b.Amount AS float) AS TotalValue,
        ISNULL(mu.UnitName, '') AS UnitName,
        0 AS RefUsedFor,
        CAST(0 AS bigint) AS RefUsedForId
    FROM Inv_ItemStockByBatch b
    LEFT JOIN W_MasterItem mi ON b.ItemId = mi.ItemID
    LEFT JOIN W_MasterUnit mu ON ISNULL(mi.PurchaseUnit, b.Unit) = mu.UnitId
    WHERE b.ItemId = @ItemId

    UNION ALL

    SELECT 
        u.CreatedDate AS TransactionDate,
        b.BatchNo,
        CASE 
            WHEN u.UsedFor = 1 THEN 'Issued (Sales)'
            WHEN u.UsedFor = 2 THEN 'Issued (Pre-Processing)'
            WHEN u.UsedFor = 3 THEN 'Issued (Production)'
            ELSE 'Removed'
        END AS TransactionType,
        COALESCE(
            CASE 
                WHEN u.UsedFor = 2 THEN 
                    (SELECT 'Used to produce ' + ISNULL(preMi.ItemName, '') + ' (Batch: ' + ISNULL(pre.BatchNumberMade, '') + ')' 
                     FROM W_PreProcessing pre 
                     JOIN W_MasterBom preBom ON pre.BomId = preBom.BomId 
                     JOIN W_MasterItem preMi ON preBom.ItemId = preMi.ItemID 
                     WHERE pre.PreProcessingId = u.UsedForId)
                WHEN u.UsedFor = 3 THEN 
                    (SELECT 'Used to produce ' + ISNULL(prodMi.ItemName, '') + ' (Batch: ' + ISNULL(prod.BatchNo, '') + ')' 
                     FROM W_Production prod 
                     JOIN W_MasterBom prodBom ON prod.BomId = prodBom.BomId 
                     JOIN W_MasterItem prodMi ON prodBom.ItemId = prodMi.ItemID 
                     WHERE prod.ProductionId = u.UsedForId)
                WHEN u.UsedFor = 1 THEN 
                    'Issued for Sales Order ID: ' + CAST(u.UsedForId AS NVARCHAR(50))
                ELSE NULL 
            END,
            ISNULL(NULLIF(CAST(u.Description AS NVARCHAR(1000)), ''), 'Used')
        ) AS Reference,
        CAST(0.0 AS float) AS InQty,
        CAST(u.Quantity AS float) AS OutQty,
        CAST((b.Amount / NULLIF(b.Quantity, 0)) * u.Quantity AS float) AS TotalValue,
        ISNULL(mu.UnitName, '') AS UnitName,
        ISNULL(u.UsedFor, 0) AS RefUsedFor,
        ISNULL(u.UsedForId, 0) AS RefUsedForId
    FROM Inv_ItemStockUsed u
    JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
    LEFT JOIN W_MasterItem mi ON b.ItemId = mi.ItemID
    LEFT JOIN W_MasterUnit mu ON ISNULL(mi.PurchaseUnit, b.Unit) = mu.UnitId
    WHERE b.ItemId = @ItemId

    ORDER BY BatchNo, TransactionType DESC
END
GO

-- 5. UPDATE SP: W_ItemStockList
ALTER PROCEDURE [dbo].[W_ItemStockList]      
                                                                                                                                                                                                               
    @StockID int=0,      
                                                                                                                                                                                                                                    
    @CreatedBy nvarchar(200)=null,   
                                                                                                                                                                                                                        
    @ItemID int=0,
                                                                                                                                                                                                                                           
    @WarehouseID int=0,
                                                                                                                                                                                                                                      
    @ItemTypeId int=0,
                                                                                                                                                                                                                                       
    @FromDate date=null,
                                                                                                                                                                                                                                     
    @ToDate date=null,
                                                                                                                                                                                                                                       
    @CurrentPage int=1 output,
                                                                                                                                                                                                                               
        
                                                                                                                                                                                                                                                     
    @RecordPerPage int=10,        
                                                                                                                                                                                                                           
    @TotalRecord  int=0 output,           
                                                                                                                                                                                                                   
    @SortOrd varchar(5)='DESC',          
                                                                                                                                                                                                                    
    @SortColumn varchar(20)='StockID'        
                                                                                                                                                                                                                
As      
                                                                                                                                                                                                                                                     
begin      
                                                                                                                                                                                                                                                  
    set nocount on          
                                                                                                                                                                                                                                 
    set @CreatedBy = dbo.ReplaceSingleQuote(@CreatedBy)      
                                                                                                                                                                                                
       
                                                                                                                                                                                                                                                      
    declare @Query as nvarchar(max)          
                                                                                                                                                                                                                
    declare @RecQuery as nvarchar(max)          
                                                                                                                                                                                                             
    set @Query=''       
                                                                                                                                                                                                                                     
          
                                                                                                                                                                                                                                                   
    if @StockID<>0          
                                                                                                                                                                                                                                 
        set @Query=@Query + ' and W_ItemStock.StockID =' + cast(@StockID as varchar)  
                                                                                                                                                                       
    if @ItemID<>0          
                                                                                                                                                                                                                                  
        set @Query=@Query + ' and W_ItemStock.ItemID =' + cast(@ItemID as varchar)  
                                                                                                                                                                         
    if @WarehouseID<>0          
                                                                                                                                                                                                                             
        set @Query=@Query + ' and (W_ItemStock.WarehouseID =' + cast(@WarehouseID as varchar) + ' or exists (select 1 from Inv_ItemStockByBatch b where b.ItemId = W_ItemStock.ItemID and b.WarehouseId = ' + cast(@WarehouseID as varchar) + '))'
           
    if @ItemTypeId<>0          
                                                                                                                                                                                                                              
        set @Query=@Query + ' and IM.ItemTypeId =' + cast(@ItemTypeId as varchar)  
                                                                                                                                                                          
        
                                                                                                                                                                                                                                                     
    if @FromDate is not null
        set @Query=@Query + ' and EXISTS (SELECT 1 FROM Inv_ItemStockByBatch b WHERE b.ItemId = W_ItemStock.ItemID AND ( ' +
        ' (b.StockById = 1 AND EXISTS (SELECT 1 FROM W_PurchaseMaster p JOIN W_PurchaseChild pc ON p.PurchaseID = pc.PurchaseID WHERE pc.PurchaseItemID = b.IdFrom AND cast(p.GoodsRecievedDate as date) >= ''' + cast(@FromDate as varchar(50)) + ''')) OR ' +
        ' (b.StockById = 2 AND EXISTS (SELECT 1 FROM W_PreProcessing pp WHERE pp.PreProcessingId = b.IdFrom AND cast(pp.ProcessingDate as date) >= ''' + cast(@FromDate as varchar(50)) + ''')) OR ' +
        ' (b.StockById = 4 AND EXISTS (SELECT 1 FROM W_Production p WHERE p.ProductionId = b.IdFrom AND cast(p.CookingDate as date) >= ''' + cast(@FromDate as varchar(50)) + ''')) OR ' +
        ' (b.StockById = 3 AND cast(W_ItemStock.OpeningStockDate as date) >= ''' + cast(@FromDate as varchar(50)) + ''') ' +
        '))'
                                                                                                                                   
    if @ToDate is not null
        set @Query=@Query + ' and EXISTS (SELECT 1 FROM Inv_ItemStockByBatch b WHERE b.ItemId = W_ItemStock.ItemID AND ( ' +
        ' (b.StockById = 1 AND EXISTS (SELECT 1 FROM W_PurchaseMaster p JOIN W_PurchaseChild pc ON p.PurchaseID = pc.PurchaseID WHERE pc.PurchaseItemID = b.IdFrom AND cast(p.GoodsRecievedDate as date) <= ''' + cast(@ToDate as varchar(50)) + ''')) OR ' +
        ' (b.StockById = 2 AND EXISTS (SELECT 1 FROM W_PreProcessing pp WHERE pp.PreProcessingId = b.IdFrom AND cast(pp.ProcessingDate as date) <= ''' + cast(@ToDate as varchar(50)) + ''')) OR ' +
        ' (b.StockById = 4 AND EXISTS (SELECT 1 FROM W_Production p WHERE p.ProductionId = b.IdFrom AND cast(p.CookingDate as date) <= ''' + cast(@ToDate as varchar(50)) + ''')) OR ' +
        ' (b.StockById = 3 AND cast(W_ItemStock.OpeningStockDate as date) <= ''' + cast(@ToDate as varchar(50)) + ''') ' +
        '))'
                                                                                                                                     

                                                                                                                                                                                                                                                             
    if @CreatedBy<>''          
                                                                                                                                                                                                                              
        set @Query=@Query + ' and W_ItemStock.CreatedBy like ''%'' + @CreatedBy + ''%'''        
                                                                                                                                                             
        
                                                                                                                                                                                                                                                     
    declare @MaxPage int          
                                                                                                                                                                                                                           

                                                                                                                                                                                                                                                             
    set @RecQuery='select @TotalRecord=count(1) from dbo.W_ItemStock inner join W_MasterItem IM on IM.ItemID=W_ItemStock.ItemID where 1=1 ' + @Query             
                                                                                            
    exec dbo.sp_ExecuteSql @RecQuery,N'@TotalRecord int output',@TotalRecord output      
                                                                                                                                                                    
   
                                                                                                                                                                                                                                                          
    
                                                                                                                                                                                                                                                         
    set @MaxPage=ceiling(isnull(@TotalRecord,0) / (@RecordPerPage * 1.0))            
                                                                                                                                                                        
           
                                                                                                                                                                                                                                                  
    if @MaxPage < @CurrentPage          
                                                                                                                                                                                                                     
    begin          
                                                                                                                                                                                                                                          
        if @MaxPage<=0          
                                                                                                                                                                                                                             
            set @CurrentPage = 1          
                                                                                                                                                                                                                   
        else   
                                                                                                                                                                                                                                              
            set @CurrentPage = @MaxPage                         
                                                                                                                                                                                             
    end          
                                                                                                                                                                                                                                            
           
                                                                                                                                                                                                                                                  
    declare @Top as int          
                                                                                                                                                                                                                            
    declare @Bottom as int          
                                                                                                                                                                                                                         
    set @Top=((@CurrentPage - 1) * @RecordPerPage + 1)          
                                                                                                                                                                                             
    set @Bottom = (@CurrentPage * @RecordPerPage)           
                                                                                                                                                                                                 
           
                                                                                                                                                                                                                                                  
    declare @QualifiedSortColumn varchar(50) = @SortColumn
                                                                                                                                                                                                   
    if @SortColumn = 'StockID' or @SortColumn = 'ItemID' or @SortColumn = 'WarehouseID' or @SortColumn = 'UnitId' or @SortColumn = 'CreatedBy'
                                                                                                               
        set @QualifiedSortColumn = 'W_ItemStock.' + @SortColumn
                                                                                                                                                                                              

                                                                                                                                                                                                                                                             
    -- NEW FILTER APPLIED HERE: ONLY SUM COMPLETED BATCHES FOR FINAL STOCK, AND SELECT ProducedQuantity
                                                                                                                                                      
    set @RecQuery='select * from (select row_number() over (order by '+ @QualifiedSortColumn + ' ' + @SortOrd + ') as RowNumber, ' +
                                                                                                                         
 ' W_ItemStock.StockID, ROUND(W_ItemStock.OpeningQuantity, 2) as OpeningQuantity, ROUND(W_ItemStock.PurchasedQuantity, 2) as PurchasedQuantity, ROUND(W_ItemStock.ProducedQuantity, 2) as ProducedQuantity, W_ItemStock.ItemID, W_ItemStock.WarehouseID, ISNULL(IM.PurchaseUnit, W_ItemStock.UnitId) AS UnitId, ROUND(W_ItemStock.IssuedQuantity, 2) as IssuedQuantity, W_ItemStock.CreatedBy, W_ItemStock.OpeningStockDate, ROUND(W_ItemStock.RemovedQuantity, 2) as RemovedQuantity, ' +
                                                                                                                                                                                                                        
 ' ROUND(W_ItemStock.FinalStock, 2) as FinalStock, ' +
                                                                                                   
 ' ItemName,ItemCode, ' +
                                                                                                                                                                                                                                    
 ' coalesce(WarehouseName, (select top 1 w.WarehouseName from Inv_ItemStockByBatch b inner join W_MasterWarehouse w on b.WarehouseId = w.WarehouseId where b.ItemId = W_ItemStock.ItemID order by b.ItemStockByBatchId desc), ''N/A'') as WarehouseName, ' +
 
 ' (select case when (isnull(ROUND(W_ItemStock.OpeningQuantity, 2), 0) + isnull(ROUND(W_ItemStock.PurchasedQuantity, 2), 0) + isnull(ROUND(W_ItemStock.ProducedQuantity, 2), 0)) = 0 then 0 else ' +
                                                                                       
 '  ((isnull((select sum(isnull(Amount, 0)) from Inv_ItemStockByBatch where ItemID = W_ItemStock.ItemID and (StockById = 3 or IdFrom = 0)), 0) + ' +
                                                                                                         
 '   isnull((select sum(isnull(pc.Quantity, 0) * isnull(pc.UnitPrice, 0)) from W_PurchaseChild pc where pc.ItemID = W_ItemStock.ItemID), 0) + ' +
                                                                                                            
 '   isnull((select sum(isnull(Amount, 0)) from Inv_ItemStockByBatch b where b.ItemId = W_ItemStock.ItemID AND (b.StockById = 2 AND EXISTS (SELECT 1 FROM dbo.W_PreProcessing pp WHERE pp.PreProcessingId = b.IdFrom AND pp.IsComplete = 1))), 0) + ' +
      
 '   isnull((select sum(isnull(Amount, 0)) from Inv_ItemStockByBatch b where b.ItemId = W_ItemStock.ItemID AND (b.StockById = 4 AND EXISTS (SELECT 1 FROM dbo.W_Production p WHERE p.ProductionId = b.IdFrom AND p.IsComplete = 1))), 0)) ' + 
               
 '  / (isnull(ROUND(W_ItemStock.OpeningQuantity, 2), 0) + isnull(ROUND(W_ItemStock.PurchasedQuantity, 2), 0) + isnull(ROUND(W_ItemStock.ProducedQuantity, 2), 0))) ' +
                                                                                                                     
 ' * (isnull(ROUND(W_ItemStock.OpeningQuantity, 2), 0) + isnull(ROUND(W_ItemStock.PurchasedQuantity, 2), 0) + isnull(ROUND(W_ItemStock.ProducedQuantity, 2), 0) - isnull(ROUND(W_ItemStock.IssuedQuantity, 2), 0) - isnull(ROUND(W_ItemStock.RemovedQuantity, 2), 0)) end) as TotalValue ' +
                   
 ' from dbo.W_ItemStock inner join W_MasterItem IM on IM.ItemID=W_ItemStock.ItemID ' +
                                                                                                                                                                       
 ' left join W_MasterWarehouse MW on Mw.WarehouseId=W_ItemStock.WarehouseID where 1=1 ' + @Query + ')t1 where t1.RowNumber>=' + cast(@Top as varchar) + ' and t1.RowNumber <=' + cast(@Bottom as varchar)          
                                          
    
                                                                                                                                                                                                                                                         
    exec dbo.sp_ExecuteSql @RecQuery      
                                                                                                                                                                                                                   
end
GO
