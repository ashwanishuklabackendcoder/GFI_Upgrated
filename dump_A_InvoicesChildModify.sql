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
@Description NVarChar(500),
@BatchNumber nvarchar(50),
@ItemSt
ockUsedID bigint=NULL,
@ReturnVal int=0 output            
as            
begin            
    set nocount on            
    set @ReturnVal = 0            

    IF @InvoiceChildID = 0            
    Begin            
        if not exists(select 1 from
 dbo.A_InvoiceChild where ( InvoiceChildID = @InvoiceChildID ))            
        Begin            
            insert into A_InvoiceChild( InvoiceID,OrderID, ItemId,PrintHeading, Amount, Quantity, UnitPrice, [Description], BatchNumber, ItemStockUsedID)

            values ( @InvoiceID,@OrderID, @ItemId,NULL, @Amount, @Quantity, @UnitPrice, @Description, @BatchNumber, @ItemStockUsedID)
            
            set @ReturnVal=scope_Identity()
        End            
        else            
            se
t @ReturnVal= -1            
    End            
    Else            
    Begin            
        if not exists(select 1 from dbo.A_InvoiceChild where ( InvoiceChildID = @InvoiceChildID ) and OrderID <> @OrderID)            
        Begin            
  
          update A_InvoiceChild      
            set OrderID=@OrderID,ItemId=@ItemId,Amount=@Amount,Quantity=@Quantity,UnitPrice=@UnitPrice,
            [Description]=@Description,BatchNumber=@BatchNumber, ItemStockUsedID=@ItemStockUsedID where InvoiceCh
ildID=@InvoiceChildID      
            
            set @ReturnVal =@InvoiceChildID            
        End            
        else            
            set @ReturnVal= @InvoiceChildID
    End
end
                                                     
