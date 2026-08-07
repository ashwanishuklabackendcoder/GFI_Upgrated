-- 1. Add IsComplete column to W_PreProcessing if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.W_PreProcessing') AND name = 'IsComplete')
BEGIN
    ALTER TABLE dbo.W_PreProcessing ADD IsComplete INT NOT NULL DEFAULT 0;
END
GO

-- 2. Update W_PreProcessingList to return the actual IsComplete column
ALTER PROCEDURE [dbo].[W_PreProcessingList]          
    @PreProcessingId int=0,          
    @CreatedBy nvarchar(200)=null,  
    @BomId int=0,   
    @CurrentPage int=1 output,            
    @RecordPerPage int=10,            
    @TotalRecord  int=0 output,               
    @SortOrd varchar(5)='DESC',              
    @SortColumn varchar(20)='PreProcessingId'            
AS          
BEGIN          
 SET NOCOUNT ON              
 SET @CreatedBy = dbo.ReplaceSingleQuote(@CreatedBy)          
           
 DECLARE @Query as varchar(1000)              
 DECLARE @RecQuery as nvarchar(2500)              
 SET @Query=''           
              
 IF @PreProcessingId<>0              
  SET @Query=@Query + ' and W_PreProcessing.PreProcessingId =' + cast(@PreProcessingId as varchar)      
    
 IF @BomId<>0              
  SET @Query=@Query + ' and W_PreProcessing.BomId =' + cast(@BomId as varchar)      
  
 IF @CreatedBy<>''              
  SET @Query=@Query + ' and (W_PreProcessing.CreatedBy like ''%' + @CreatedBy + '%'' or BomName like ''%' + @CreatedBy + '%'' or ItemName like ''%' + @CreatedBy + '%'' or BatchNumberMade like ''%' + @CreatedBy + '%''' + ')'            
            
 DECLARE @MaxPage int              
 SET @RecQuery='select @TotalRecord=count(1) from dbo.W_PreProcessing left join dbo.W_MasterBom on W_PreProcessing.BomId=W_MasterBom.BomId left join dbo.W_MasterItem on W_MasterItem.ItemID=W_MasterBom.ItemId where 1=1 ' + @Query                 
 EXEC dbo.sp_ExecuteSql @RecQuery,N'@TotalRecord int output',@TotalRecord output          
           
 SET @MaxPage=ceiling(isnull(@TotalRecord,0) / (@RecordPerPage * 1.0))                
               
 IF @MaxPage < @CurrentPage              
 BEGIN              
  IF @MaxPage<=0              
   SET @CurrentPage = 1              
  ELSE              
   SET @CurrentPage = @MaxPage                             
 END              
               
 DECLARE @Top as int              
 DECLARE @Bottom as int              
 SET @Top=((@CurrentPage - 1) * @RecordPerPage + 1)              
 SET @Bottom = (@CurrentPage * @RecordPerPage)               
               
 SET @RecQuery='select * from (select row_number() over (order by W_PreProcessing.' + @SortColumn + ' ' + @SortOrd + ') as RowNumber,PreProcessingId,W_PreProcessing.BomId,convert(varchar(12),ProcessingDate,106) ProcessingDate,QuantityMade,W_PreProcessing.UnitMade,BatchNumberMade,convert(varchar(12),ExpiryDate,106) ExpiryDate,
 ProcessEmployees,Remarks,DocumentUpload,W_PreProcessing.WarehouseId,W_PreProcessing.CreatedDate,W_PreProcessing.CreatedBy, 
 BomName,UnitName,WarehouseName,ItemName,BomQty,
 W_PreProcessing.IsComplete -- Changed to use the physical status column
 from dbo.W_PreProcessing 
   left join W_MasterBom on W_PreProcessing.BomId=W_MasterBom.BomId
  left join W_MasterItem on W_MasterItem.ItemID=W_MasterBom.ItemId
 left join W_MasterUnit on W_MasterUnit.UnitId=W_PreProcessing.UnitMade
 left join W_MasterWarehouse on W_PreProcessing.WarehouseId=W_MasterWarehouse.WarehouseId where 1=1 ' + @Query + ')t1 where t1.RowNumber>=' + cast(@Top as varchar) + ' and t1.RowNumber <=' + cast(@Bottom as varchar)              
 PRINT @RecQuery      
 EXEC dbo.sp_ExecuteSql @RecQuery          
END
GO
