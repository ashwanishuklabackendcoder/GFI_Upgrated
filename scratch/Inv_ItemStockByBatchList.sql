Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Create PROCEDURE [dbo].[Inv_ItemStockByBatchList]        
                                                                                                                                                                                                    
    @ItemStockByBatchId int=0,        
                                                                                                                                                                                                                       
    @StockById int=0,  
                                                                                                                                                                                                                                      
    @ItemID int=0,  
                                                                                                                                                                                                                                         
    @CurrentPage int=1 output,          
                                                                                                                                                                                                                     
    @RecordPerPage int=1000,          
                                                                                                                                                                                                                       
    @TotalRecord int=0 output,
                                                                                                                                                                                                                               
    @SortOrd varchar(5)='DESC',
                                                                                                                                                                                                                              
    @SortColumn varchar(50)='ItemStockByBatchId'
                                                                                                                                                                                                             
AS        
                                                                                                                                                                                                                                                   
BEGIN        
                                                                                                                                                                                                                                                
    SET NOCOUNT ON;        
                                                                                                                                                                                                                                  
    DECLARE @Query AS varchar(1000) = ''        
                                                                                                                                                                                                             
    DECLARE @RecQuery AS nvarchar(MAX)        
                                                                                                                                                                                                               
        
                                                                                                                                                                                                                                                     
    IF @ItemStockByBatchId > 0        
                                                                                                                                                                                                                       
        SET @Query = @Query + ' and Inv_ItemStockByBatch.ItemStockByBatchId=' + cast(@ItemStockByBatchId as varchar)        
                                                                                                                                 
            
                                                                                                                                                                                                                                                 
    IF @StockById > 0        
                                                                                                                                                                                                                                
        SET @Query = @Query + ' and Inv_ItemStockByBatch.StockById=' + cast(@StockById as varchar)        
                                                                                                                                                   
          
                                                                                                                                                                                                                                                   
    IF @ItemID > 0        
                                                                                                                                                                                                                                   
        SET @Query = @Query + ' and Inv_ItemStockByBatch.ItemID=' + cast(@ItemID as varchar)        
                                                                                                                                                         
        
                                                                                                                                                                                                                                                     
    SET @RecQuery = 'SELECT @TotalRecord=count(1) FROM dbo.Inv_ItemStockByBatch WHERE 1=1' + @Query        
                                                                                                                                                  
    EXEC dbo.sp_ExecuteSql @RecQuery, N'@TotalRecord INT OUTPUT', @TotalRecord OUTPUT        
                                                                                                                                                                
            
                                                                                                                                                                                                                                                 
    DECLARE @Top INT = ((@CurrentPage - 1) * @RecordPerPage) + 1;        
                                                                                                                                                                                    
    DECLARE @Bottom INT = @CurrentPage * @RecordPerPage;        
                                                                                                                                                                                             

                                                                                                                                                                                                                                                             
    DECLARE @QualifiedSortColumn varchar(200) = @SortColumn
                                                                                                                                                                                                  
    IF @SortColumn IN ('ItemStockByBatchId', 'StockById', 'IdFrom', 'ItemId', 'Quantity', 'Unit', 'BatchNo', 'ExpiryDate', 'WarehouseId', 'FinalQuantityLeft', 'Amount', 'CreatedBy', 'CreatedDate', 'ModifiedBy', 'ModifiedDate')
                           
        SET @QualifiedSortColumn = 'Inv_ItemStockByBatch.' + @SortColumn
                                                                                                                                                                                     
    ELSE IF @SortColumn = 'IssuedStock'
                                                                                                                                                                                                                      
        SET @QualifiedSortColumn = 'ISNULL((SELECT SUM(u.Quantity) FROM dbo.Inv_ItemStockUsed u LEFT JOIN dbo.W_PreProcessing p ON u.UsedFor = 2 AND u.UsedForId = p.PreProcessingId LEFT JOIN dbo.W_Production pr ON u.UsedFor = 3 AND u.UsedForId = pr.Produc
tionId WHERE u.ItemStockByBatchId = Inv_ItemStockByBatch.ItemStockByBatchId AND (u.UsedFor = 1 OR (u.UsedFor = 2 AND ISNULL(p.IsComplete, 0) = 1) OR (u.UsedFor = 3 AND ISNULL(pr.IsComplete, 0) = 1))), 0)'
                                                 
    ELSE IF @SortColumn = 'StockValue'
                                                                                                                                                                                                                       
        SET @QualifiedSortColumn = 'CAST(ROUND((ISNULL(Inv_ItemStockByBatch.Amount, 0) / NULLIF(Inv_ItemStockByBatch.Quantity, 0)) * CASE WHEN Inv_ItemStockByBatch.FinalQuantityLeft < 0 THEN 0 ELSE Inv_ItemStockByBatch.FinalQuantityLeft END, 2) AS float)'

                                                                                                                                                                                                                                                             
        
                                                                                                                                                                                                                                                     
    SET @RecQuery = 'SELECT * FROM (
                                                                                                                                                                                                                         
        SELECT ROW_NUMBER() OVER (ORDER BY ' + @QualifiedSortColumn + ' ' + @SortOrd + ') AS RowNumber, 
                                                                                                                                                     
               Inv_ItemStockByBatch.ItemStockByBatchId, Inv_ItemStockByBatch.StockById, Inv_ItemStockByBatch.IdFrom, 
                                                                                                                                        
               Inv_ItemStockByBatch.ItemId, Inv_ItemStockByBatch.Quantity, Inv_ItemStockByBatch.Unit, Inv_ItemStockByBatch.BatchNo, 
                                                                                                                         
               convert(varchar(12),Inv_ItemStockByBatch.ExpiryDate,106) ExpiryDate, Inv_ItemStockByBatch.WarehouseId, CASE WHEN Inv_ItemStockByBatch.FinalQuantityLeft < 0 THEN 0 ELSE Inv_ItemStockByBatch.FinalQuantityLeft END AS FinalQuantityLeft,
      
               
                                                                                                                                                                                                                                              
               -- THE 4 NEW AUDIT COLUMNS ARE ADDED HERE:
                                                                                                                                                                                                    
               Inv_ItemStockByBatch.CreatedBy, Inv_ItemStockByBatch.CreatedDate, Inv_ItemStockByBatch.ModifiedBy, Inv_ItemStockByBatch.ModifiedDate,
                                                                                                         
               
                                                                                                                                                                                                                                              
               ItemName, UnitName, WarehouseName,
                                                                                                                                                                                                            
               ISNULL((
                                                                                                                                                                                                                                      
                   SELECT SUM(u.Quantity) 
                                                                                                                                                                                                                   
                   FROM dbo.Inv_ItemStockUsed u 
                                                                                                                                                                                                             
                   LEFT JOIN dbo.W_PreProcessing p ON u.UsedFor = 2 AND u.UsedForId = p.PreProcessingId 
                                                                                                                                                     
                   LEFT JOIN dbo.W_Production pr ON u.UsedFor = 3 AND u.UsedForId = pr.ProductionId 
                                                                                                                                                         
                   WHERE u.ItemStockByBatchId = Inv_ItemStockByBatch.ItemStockByBatchId 
                                                                                                                                                                     
                   AND (u.UsedFor = 1 OR (u.UsedFor = 2 AND ISNULL(p.IsComplete, 0) = 1) OR (u.UsedFor = 3 AND ISNULL(pr.IsComplete, 0) = 1))
                                                                                                                
               ), 0) AS IssuedStock,
                                                                                                                                                                                                                         
               CAST(ROUND((ISNULL(Inv_ItemStockByBatch.Amount, 0) / NULLIF(Inv_ItemStockByBatch.Quantity, 0)) * CASE WHEN Inv_ItemStockByBatch.FinalQuantityLeft < 0 THEN 0 ELSE Inv_ItemStockByBatch.FinalQuantityLeft END, 2) AS float) AS StockValue
      
        FROM dbo.Inv_ItemStockByBatch AS Inv_ItemStockByBatch  
                                                                                                                                                                                              
        INNER JOIN dbo.W_MasterItem ON W_MasterItem.ItemID = Inv_ItemStockByBatch.ItemId  
                                                                                                                                                                   
        LEFT JOIN dbo.W_MasterUnit ON W_MasterUnit.UnitId = ISNULL(W_MasterItem.PurchaseUnit, Inv_ItemStockByBatch.Unit)  
                                                                                                                                   
        LEFT JOIN dbo.W_MasterWarehouse ON W_MasterWarehouse.WarehouseId = Inv_ItemStockByBatch.WarehouseId  
                                                                                                                                                
        WHERE 1=1 ' + @Query + '
                                                                                                                                                                                                                             
    ) t1 WHERE t1.RowNumber >= ' + cast(@Top as varchar) + ' and t1.RowNumber <= ' + cast(@Bottom as varchar);
                                                                                                                                               
        
                                                                                                                                                                                                                                                     
    EXEC dbo.sp_ExecuteSql @RecQuery;
                                                                                                                                                                                                                        
END;
                                                                                                                                                                                                                                                         
