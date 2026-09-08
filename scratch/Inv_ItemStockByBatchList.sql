                                                                                                                                                                                                                                                                
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ALTER PROCEDURE [dbo].[Inv_ItemStockByBatchList]        
    @ItemStockByBatchId int=0,        
    @StockById int=0,  
    @ItemID int=0,  
    @CurrentPage int=1 output,          
    @RecordPerPage int=1000,          
    @TotalRecord  int=0 outp
