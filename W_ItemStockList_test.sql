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
                                                                                                                                                                                                
       
                                                                                                                                                                                                                                                      
    declare @Query as varchar(1000)          
                                                                                                                                                                                                                
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
                                                                                            
    PRINT @RecQuery,N'@TotalRecord int output',@TotalRecord output      
                                                                                                                                                                    
   
                                                                                                                                                                                                                                                          
    
                                                                                                                                                                                                                                                         
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
                                                                                                                         
 ' W_ItemStock.StockID, ROUND(W_ItemStock.OpeningQuantity, 2), ROUND(W_ItemStock.PurchasedQuantity, 2), ROUND(W_ItemStock.ProducedQuantity, 2), W_ItemStock.ItemID, W_ItemStock.WarehouseID, W_ItemStock.UnitId, ROUND(W_ItemStock.IssuedQuantity, 2), W_ItemStock.CreatedBy, W_ItemStock.OpeningStockDate, ROUND(W_ItemStock.RemovedQuantity, 2), ' +
                                                                                                                                                                                                                        
 ' isnull((select sum(FinalQuantityLeft) from Inv_ItemStockByBatch b where b.ItemId = W_ItemStock.ItemID AND (b.StockById NOT IN (2, 4) OR (b.StockById = 2 AND EXISTS (SELECT 1 FROM dbo.W_PreProcessing pp WHERE pp.PreProcessingId = b.IdFrom AND pp.IsComplete = 1)) OR (b.StockById = 4 AND EXISTS (SELECT 1 FROM dbo.W_Production p WHERE p.ProductionId = b.IdFrom AND p.IsComplete = 1)))), 0) as FinalStock, ' +
                                                                                                   
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
                                          
    
                                                                                                                                                                                                                                                         
    PRINT @RecQuery      
                                                                                                                                                                                                                   
end                                                                                                                                                                                                                                                            
