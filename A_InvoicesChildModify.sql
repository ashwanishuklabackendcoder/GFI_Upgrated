Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[A_InvoicesChildModify]            
                                                                                                                                                                                                   
@InvoiceID bigint=0,
                                                                                                                                                                                                                                         
@InvoiceChildID bigint=0,
                                                                                                                                                                                                                                    
@OrderID bigint=0,
                                                                                                                                                                                                                                           
@ItemId bigint=0,
                                                                                                                                                                                                                                            
@Amount float,
                                                                                                                                                                                                                                               
@Quantity float,
                                                                                                                                                                                                                                             
@UnitPrice float,
                                                                                                                                                                                                                                            
@Description NVarChar(500) ,
                                                                                                                                                                                                                                 
@BatchNumber nvarchar(50),
                                                                                                                                                                                                                                   
@ReturnVal int=0 output            
                                                                                                                                                                                                                          
as            
                                                                                                                                                                                                                                               
begin            
                                                                                                                                                                                                                                            
set nocount on            
                                                                                                                                                                                                                                   
set @ReturnVal = 0            
                                                                                                                                                                                                                               
            
                                                                                                                                                                                                                                                 
            
                                                                                                                                                                                                                                                 
IF @InvoiceChildID = 0            
                                                                                                                                                                                                                           
  Begin            
                                                                                                                                                                                                                                          
    if not exists(select 1 from dbo.A_InvoiceChild where ( InvoiceChildID = @InvoiceChildID ))            
                                                                                                                                                   
      Begin            
                                                                                                                                                                                                                                      
         
                                                                                                                                                                                                                                                    

                                                                                                                                                                                                                                                             
		 insert into A_InvoiceChild( InvoiceID,OrderID, ItemId,PrintHeading, Amount, Quantity, UnitPrice, [Description], BatchNumber)
                                                                                                                              
         values ( @InvoiceID,@OrderID, @ItemId,NULL, @Amount, @Quantity, @UnitPrice, @Description, @BatchNumber)
                                                                                                                                             
		 set @ReturnVal=scope_Identity()
                                                                                                                                                                                                                           
		 
                                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
      End            
                                                                                                                                                                                                                                        
    else            
                                                                                                                                                                                                                                         
    set @ReturnVal= -1            
                                                                                                                                                                                                                           
  End            
                                                                                                                                                                                                                                            
Else            
                                                                                                                                                                                                                                             
  Begin            
                                                                                                                                                                                                                                          
    if not exists(select 1 from dbo.A_InvoiceChild where ( InvoiceChildID = @InvoiceChildID ) and OrderID <> @OrderID)            
                                                                                                                           
      Begin            
        -- Sync stock modification
        DECLARE @OldQuantity FLOAT, @OldBatchNumber NVARCHAR(50), @OldItemId BIGINT
        SELECT @OldQuantity = Quantity, @OldBatchNumber = BatchNumber, @OldItemId = ItemId
        FROM dbo.A_InvoiceChild WHERE InvoiceChildID=@InvoiceChildID
        
        DECLARE @ItemStockUsedID BIGINT
        SELECT @ItemStockUsedID = u.ItemStockUsedID
        FROM dbo.Inv_ItemStockByBatch b
        INNER JOIN dbo.Inv_ItemStockUsed u ON u.ItemStockByBatchId = b.ItemStockByBatchId AND u.UsedForId = @InvoiceID
        WHERE b.ItemID = @OldItemId AND b.BatchNo = @OldBatchNumber AND u.UsedFor = 1

        update A_InvoiceChild      
                                                                                                                                                                                                                           
        set OrderID=@OrderID,ItemId=@ItemId,Amount=@Amount,Quantity=@Quantity,UnitPrice=@UnitPrice,
                                                                                                                                                          
        [Description]=@Description,BatchNumber=@BatchNumber where InvoiceChildID=@InvoiceChildID      
                                                                                                                                                       
        IF @ItemStockUsedID IS NOT NULL
        BEGIN
           DECLARE @NewItemStockByBatchId BIGINT
           SELECT TOP 1 @NewItemStockByBatchId = ItemStockByBatchId FROM dbo.Inv_ItemStockByBatch WHERE ItemID = @ItemId AND BatchNo = @BatchNumber
           
           IF @NewItemStockByBatchId IS NOT NULL
           BEGIN
               DECLARE @DummyReturn INT
               DECLARE @Now DATETIME = GETDATE()
               EXEC dbo.Inv_ItemStockUsedModify 
                   @ItemStockUsedID = @ItemStockUsedID,
                   @ItemStockByBatchId = @NewItemStockByBatchId,
                   @UsedFor = 1,
                   @UsedForId = @InvoiceID,
                   @Quantity = @Quantity,
                   @Description = @Description,
                   @CreatedBy = 'System',
                   @CreatedDate = @Now,
                   @UnitId = NULL,
                   @ReturnVal = @DummyReturn OUTPUT
           END
        END
        
                                                                                                                                                                                                                                                      
        set @ReturnVal =@InvoiceChildID            
                                                                                                                                                                                                          
      End            
                                                                                                                                                                                                                                        
    else            
                                                                                                                                                                                                                                         
      set @ReturnVal= @InvoiceChildID
                                                                                                                                                                                                                        
   End
                                                                                                                                                                                                                                                       
end                                                                                                                                                                                                                                                            
