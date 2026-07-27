using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=db50414.public.databaseasp.net;Database=db50414;User Id=db50414;Password=m?2TQ9f#nZ+5;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
        
        string sql = @"
ALTER PROCEDURE [dbo].[Inv_ItemStockByBatchList]        
    @ItemStockByBatchId int=0,        
    @StockById int=0,  
    @ItemID int=0,  
    @CurrentPage int=1 output,          
    @RecordPerPage int=1000,          
    @TotalRecord  int=0 output,             
    @SortOrd varchar(5)='DESC',            
    @SortColumn varchar(20)='ItemStockByBatchId'          
As        
begin        
          
 set nocount on            
 set @StockById = dbo.ReplaceSingleQuote(@StockById)        
         
 declare @Query as varchar(1000)            
    declare @RecQuery as nvarchar(2500)            
    set @Query=''         
            
            
     if @ItemStockByBatchId<>0            
  set @Query=@Query + ' and ItemStockByBatchId =' + cast(@ItemStockByBatchId as varchar)    
       if @StockById<>0            
  set @Query=@Query + ' and StockById =' + cast(@StockById as varchar)    
  if @ItemID<>0            
  set @Query=@Query + ' and Inv_ItemStockByBatch.ItemId =' + cast(@ItemID as varchar)    
          
          
declare @MaxPage int            
 set @RecQuery='select @TotalRecord=count(1) from dbo.Inv_ItemStockByBatch where 1=1 ' + @Query               
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
             
 set @RecQuery='select * from (select row_number() over (order by '+ @SortColumn + ' ' + @SortOrd + ') as RowNumber,Inv_ItemStockByBatch.ItemStockByBatchId, Inv_ItemStockByBatch.StockById, Inv_ItemStockByBatch.IdFrom, Inv_ItemStockByBatch.ItemId, Inv_ItemStockByBatch.Quantity  
 , Inv_ItemStockByBatch.Unit, Inv_ItemStockByBatch.BatchNo, Inv_ItemStockByBatch.Amount  
 ,convert(varchar(12),Inv_ItemStockByBatch.ExpiryDate,106) ExpiryDate,Inv_ItemStockByBatch.WarehouseId, Inv_ItemStockByBatch.FinalQuantityLeft  
,ItemName,UnitName,WarehouseName'+            
 ' from dbo.Inv_ItemStockByBatch  
 inner join W_MasterItem on W_MasterItem.ItemID=Inv_ItemStockByBatch.ItemId  
 left join W_MasterUnit on W_MasterUnit.UnitId=Inv_ItemStockByBatch.Unit  
 left join W_MasterWarehouse on W_MasterWarehouse.WarehouseId= Inv_ItemStockByBatch.WarehouseId  
 where 1=1 ' + @Query + ')t1 where t1.RowNumber>=' + cast(@Top as varchar) + ' and t1.RowNumber <=' + cast(@Bottom as varchar)            
print @RecQuery    
 exec dbo.sp_ExecuteSql @RecQuery        
     
end
";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("SUCCESS: Stored procedure Inv_ItemStockByBatchList altered successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
