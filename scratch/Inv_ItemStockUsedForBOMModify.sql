
        
CREATE      PROCEDURE [dbo].[Inv_ItemStockUsedForBOMModify]           
@ItemStockUsedID bigint =0 ,        
@ItemStockByBatchId bigint ,        
       
@OrderCustomerID bigint ,   
@OrderCustomerDetailsID bigint ,   
@Quantity float ,       
@UsedFor int , 
@Description nvarchar(1000) ,        
@CreatedBy nvarchar(200) ,        
@CreatedDate datetime,        
@ReturnVal int=0 output         
as                    
begin                    
set nocount on                    
set @ReturnVal = 0                    
          
        
        
   
        
        
IF @ItemStockUsedID = 0                    
  Begin                    
   if not exists(select 1 from dbo.Inv_ItemStockUsedForBOM where ( ItemStockUsedID=@ItemStockUsedID))                    
    Begin           
        
  insert into Inv_ItemStockUsedForBOM(ItemStockByBatchId,UsedFor, OrderCustomerID,OrderCustomerDetailsID, Quantity, Description,CreateBy,CreateDate)         
  values (@ItemStockByBatchId,@UsedFor, @OrderCustomerID,@OrderCustomerDetailsID, @Quantity, @Description, @CreatedBy,@CreatedDate)        
        
  update Inv_ItemStockByBatchForBOM         
  set FinalQuantityLeft=FinalQuantityLeft-@Quantity where ItemStockByBatchId=@ItemStockByBatchId        
        
  set @ReturnVal=scope_Identity()          
          
  if @ReturnVal>0        
  begin        
  if(isnull((select count(*) from W_ItemStock where ItemID in(select ItemId from Inv_ItemStockUsedForBOM a 
  inner join Inv_ItemStockByBatchForBOM b on a.ItemStockByBatchId=b.ItemStockByBatchId where ItemStockUsedID=@ReturnVal)),0)>0)        
  begin         
  update t1 set IssuedQuantity=isnull(IssuedQuantity,0)+ isnull(t3.Quantity,0)        
  from W_ItemStock t1         
  inner join Inv_ItemStockByBatchForBOM t2 on t1.ItemID=t2.ItemID         
  inner join Inv_ItemStockUsedForBOM t3 on t2.ItemStockByBatchId=t3.ItemStockByBatchId        
  where t3.ItemStockUsedID=@ReturnVal        
  end         
  end  
  
  update W_OrderCustomerDetails set isComplete=1 where OrderID=@OrderCustomerDetailsID
    End                    
   else                    
    set @ReturnVal= -1                    
  End                    
      
  ELSE      
BEGIN            
    IF EXISTS (SELECT 1 FROM dbo.Inv_ItemStockUsedForBOM WHERE ItemStockUsedID=@ItemStockUsedID)      
    BEGIN         
      
        DECLARE       
            @OldBatchId BIGINT,      
            @OldQty FLOAT,      
            @OldItemId BIGINT,      
            @NewItemId BIGINT      
      
        -- Get old values      
        SELECT       
            @OldBatchId = ItemStockByBatchId,      
            @OldQty = Quantity      
        FROM Inv_ItemStockUsedForBOM       
        WHERE ItemStockUsedID=@ItemStockUsedID      
      
        -- Get item ids      
        SELECT @OldItemId = ItemID       
        FROM Inv_ItemStockByBatchForBOM       
        WHERE ItemStockByBatchId=@OldBatchId      
      
        SELECT @NewItemId = ItemID       
        FROM Inv_ItemStockByBatchForBOM       
        WHERE ItemStockByBatchId=@ItemStockByBatchId      
      
        /* 1. RETURN stock to OLD batch */      
        UPDATE Inv_ItemStockByBatchForBOM      
        SET FinalQuantityLeft = FinalQuantityLeft + @OldQty      
        WHERE ItemStockByBatchId = @OldBatchId      
      
        /* 2. DEDUCT stock from NEW batch */      
        UPDATE Inv_ItemStockByBatchForBOM      
        SET FinalQuantityLeft = FinalQuantityLeft - @Quantity      
        WHERE ItemStockByBatchId = @ItemStockByBatchId      
      
        /* 3. FIX W_ItemStock */      
        -- remove old      
        UPDATE W_ItemStock      
        SET IssuedQuantity = ISNULL(IssuedQuantity,0) - @OldQty      
        WHERE ItemID = @OldItemId      
      
        -- add new      
        UPDATE W_ItemStock      
        SET IssuedQuantity = ISNULL(IssuedQuantity,0) + @Quantity      
        WHERE ItemID = @NewItemId      
      
        /* 4. UPDATE main record */      
        UPDATE Inv_ItemStockUsedForBOM         
        SET       
            ItemStockByBatchId=@ItemStockByBatchId,      
            UsedFor=@UsedFor,       
            OrderCustomerID=OrderCustomerID, 
			OrderCustomerDetailsID=@OrderCustomerDetailsID,
            Quantity=@Quantity,       
            [Description]=@Description        
        WHERE ItemStockUsedID=@ItemStockUsedID        
      
        SET @ReturnVal = @ItemStockUsedID     
		
		update W_OrderCustomerDetails set isComplete=1 where OrderID=@OrderCustomerDetailsID
    END                    
    ELSE                    
        SET @ReturnVal = @ItemStockUsedID                   
END        
      
  
         
        
        
END 

