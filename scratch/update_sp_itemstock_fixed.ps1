$connString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
$connection = New-Object System.Data.SqlClient.SqlConnection($connString)
$connection.Open()

$spSql = @"
ALTER PROCEDURE [dbo].[W_ItemStockList]      
    @StockID int=0,      
    @CreatedBy nvarchar(200)=null,   
    @ItemID int=0,
    @WarehouseID int=0,
    @ItemTypeId int=0,
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
    declare @RecQuery as nvarchar(3000)          
    set @Query=''       
          
    if @StockID<>0          
        set @Query=@Query + ' and W_ItemStock.StockID =' + cast(@StockID as varchar)  
    if @ItemID<>0          
        set @Query=@Query + ' and W_ItemStock.ItemID =' + cast(@ItemID as varchar)  
    if @WarehouseID<>0          
        set @Query=@Query + ' and (W_ItemStock.WarehouseID =' + cast(@WarehouseID as varchar) + ' or exists (select 1 from Inv_ItemStockByBatch b where b.ItemId = W_ItemStock.ItemID and b.WarehouseId = ' + cast(@WarehouseID as varchar) + '))'
    if @ItemTypeId<>0          
        set @Query=@Query + ' and IM.ItemTypeId =' + cast(@ItemTypeId as varchar)  

    if @CreatedBy<>''          
        set @Query=@Query + ' and W_ItemStock.CreatedBy like ''%' + @CreatedBy + '%'''        
        
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
           
    -- Prefix W_ItemStock.* columns and make sure sorting column is qualified if needed
    declare @QualifiedSortColumn varchar(50) = @SortColumn
    if @SortColumn = 'StockID' or @SortColumn = 'ItemID' or @SortColumn = 'WarehouseID' or @SortColumn = 'UnitId' or @SortColumn = 'CreatedBy'
        set @QualifiedSortColumn = 'W_ItemStock.' + @SortColumn

    set @RecQuery='select * from (select row_number() over (order by '+ @QualifiedSortColumn + ' ' + @SortOrd + ') as RowNumber, ' +
 ' W_ItemStock.StockID, W_ItemStock.OpeningQuantity, W_ItemStock.PurchasedQuantity, W_ItemStock.ItemID, W_ItemStock.WarehouseID, W_ItemStock.UnitId, W_ItemStock.IssuedQuantity, W_ItemStock.CreatedBy, W_ItemStock.OpeningStockDate, W_ItemStock.RemovedQuantity, ' +
 ' (isnull(W_ItemStock.OpeningQuantity, 0) + isnull(W_ItemStock.PurchasedQuantity, 0) - isnull(W_ItemStock.IssuedQuantity, 0) - isnull(W_ItemStock.RemovedQuantity, 0)) as FinalStock, ' +
 ' ItemName,ItemCode, ' +
 ' coalesce(WarehouseName, (select top 1 w.WarehouseName from Inv_ItemStockByBatch b inner join W_MasterWarehouse w on b.WarehouseId = w.WarehouseId where b.ItemId = W_ItemStock.ItemID order by b.ItemStockByBatchId desc), ''N/A'') as WarehouseName, ' +
 ' (isnull((select sum(isnull(Amount, 0)) from Inv_ItemStockByBatch where ItemID = W_ItemStock.ItemID and (StockById = 3 or IdFrom = 0)), 0) + ' +
 '  isnull((select sum(isnull(pc.Quantity, 0) * isnull(pc.UnitPrice, 0)) from Inv_ItemStockByBatch sb inner join W_PurchaseChild pc on sb.IdFrom = pc.PurchaseItemID where sb.ItemID = W_ItemStock.ItemID and sb.StockById = 1 and sb.IdFrom > 0), 0)) as TotalValue ' +
 ' from dbo.W_ItemStock inner join W_MasterItem IM on IM.ItemID=W_ItemStock.ItemID ' +
 ' left join W_MasterWarehouse MW on Mw.WarehouseId=W_ItemStock.WarehouseID where 1=1 ' + @Query + ')t1 where t1.RowNumber>=' + cast(@Top as varchar) + ' and t1.RowNumber <=' + cast(@Bottom as varchar)          
    
    exec dbo.sp_ExecuteSql @RecQuery      
end
"@

$command = $connection.CreateCommand()
$command.CommandText = $spSql
$command.ExecuteNonQuery()
Write-Output "Successfully updated stored procedure: W_ItemStockList"

$connection.Close()
