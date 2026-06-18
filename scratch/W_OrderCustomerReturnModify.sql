CREATE     PROCEDURE [dbo].[W_OrderCustomerReturnModify]                
    @OrderReturnID       BIGINT = 0,  
    @OrderID             BIGINT,  
    @OrderDetailsID      BIGINT,  
    @ItemStockByBatchId  INT,  
    @ReturnDate          DATE,  
    @Quantity            FLOAT,  
    @CreatedDate         DATETIME,  
    @CreatedBy           NVARCHAR(200),  
    @ReturnReason        NVARCHAR(1000), 
	@ItemStockUsedID  INT,
    @ReturnVal           INT = 0 OUTPUT  
AS                
BEGIN                
    SET NOCOUNT ON                
    SET @ReturnVal = 0                
  
    -- ================= INSERT =================  
    IF @OrderReturnID = 0                
    BEGIN                
        IF NOT EXISTS (SELECT 1 FROM dbo.W_OrderCustomerReturn WHERE OrderReturnID = @OrderReturnID)                
        BEGIN                
            INSERT INTO W_OrderCustomerReturn  
            (  
                OrderID,  
                OrderDetailsID,  
                ItemStockByBatchId,  
                ReturnDate,  
                Quantity,  
                CreatedDate,  
                CreatedBy,  
                ReturnReason ,
				ItemStockUsedID
            )    
            VALUES   
            (  
                @OrderID,  
                @OrderDetailsID,  
                @ItemStockByBatchId,  
                @ReturnDate,  
                @Quantity,  
                @CreatedDate,  
                @CreatedBy,  
                @ReturnReason  ,
				@ItemStockUsedID
            )    
  
            SET @ReturnVal = SCOPE_IDENTITY()                
        END                
        ELSE                
            SET @ReturnVal = -1                
    END                
  
    -- ================= UPDATE =================  
    ELSE                
    BEGIN                
        IF NOT EXISTS   
        (  
            SELECT 1   
            FROM dbo.W_OrderCustomerReturn   
            WHERE OrderReturnID = @OrderReturnID  
              AND OrderID <> @OrderID  
        )                
        BEGIN                    
            UPDATE W_OrderCustomerReturn    
            SET   
                OrderID            = @OrderID,  
                OrderDetailsID     = @OrderDetailsID,  
                ItemStockByBatchId = @ItemStockByBatchId,  
                ReturnDate         = @ReturnDate,  
                Quantity           = @Quantity,  
                ReturnReason       = @ReturnReason ,
				ItemStockUsedID=@ItemStockUsedID
            WHERE OrderReturnID = @OrderReturnID        
  
            SET @ReturnVal = @OrderReturnID                
        END                
        ELSE                
            SET @ReturnVal = @OrderReturnID               
    END                
END  

