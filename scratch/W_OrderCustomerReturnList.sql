CREATE    PROCEDURE [dbo].[W_OrderCustomerReturnList]                  
    @OrderReturnID BIGINT = 0,    
    @OrderID       BIGINT = 0,                  
    @CurrentPage   INT = 1 OUTPUT,                    
    @RecordPerPage INT = 10,                    
    @TotalRecord   INT = 0 OUTPUT,                       
    @SortOrd       VARCHAR(5) = 'DESC',                      
    @SortColumn    VARCHAR(30) = 'OrderReturnID'                    
AS                  
BEGIN                  
    SET NOCOUNT ON                      
    
    SET @OrderReturnID = dbo.ReplaceSingleQuote(@OrderReturnID)                  
    
    DECLARE @Query VARCHAR(1000)                      
    DECLARE @RecQuery NVARCHAR(3000)                      
    SET @Query = ''                   
    
    -- Filters    
    IF @OrderID <> 0                      
        SET @Query = @Query + ' AND W_OrderCustomerReturn.OrderID = ' + CAST(@OrderID AS VARCHAR)    
    
    IF @OrderReturnID <> 0                      
        SET @Query = @Query + ' AND W_OrderCustomerReturn.OrderReturnID = ' + CAST(@OrderReturnID AS VARCHAR)    
    
    -- ================= Total Record Count =================    
    DECLARE @MaxPage INT                      
    SET @RecQuery = 'SELECT @TotalRecord = COUNT(1)     
                     FROM dbo.W_OrderCustomerReturn     
                     WHERE 1=1 ' + @Query                         
    
    EXEC dbo.sp_ExecuteSql @RecQuery, N'@TotalRecord INT OUTPUT', @TotalRecord OUTPUT                  
    
    SET @MaxPage = CEILING(ISNULL(@TotalRecord,0) / (@RecordPerPage * 1.0))                        
                       
    IF @MaxPage < @CurrentPage                      
    BEGIN                      
        IF @MaxPage <= 0                      
            SET @CurrentPage = 1                      
        ELSE                      
            SET @CurrentPage = @MaxPage                                     
    END                      
                       
    DECLARE @Top INT                      
    DECLARE @Bottom INT                      
    
    SET @Top = ((@CurrentPage - 1) * @RecordPerPage + 1)                      
    SET @Bottom = (@CurrentPage * @RecordPerPage)                       
    
    -- ================= Data Query =================    
  SET @RecQuery = '    
SELECT * FROM (    
    SELECT ROW_NUMBER() OVER (ORDER BY ' + @SortColumn + ' ' + @SortOrd + ') AS RowNumber,    

    W_OrderCustomerReturn.OrderReturnID,    
    W_OrderCustomerReturn.OrderID,    
    W_OrderCustomerReturn.OrderDetailsID,    
    W_OrderCustomerReturn.ItemStockByBatchId,   
    W_OrderCustomerReturn.ItemStockUsedID, 

    CONVERT(VARCHAR(12), W_OrderCustomerReturn.ReturnDate, 106) AS ReturnDate,    
    W_OrderCustomerReturn.Quantity,    

    ISNULL(R.TotalReturnQty,0) AS TotalReturnedQty,  -- ?? ADDED

    W_OrderCustomerReturn.CreatedDate,    
    W_OrderCustomerReturn.CreatedBy,    
    W_OrderCustomerReturn.ReturnReason,    

    A_MasterAccounts.AccountName,  
    Inv_ItemStockByBatchForBOM.BatchNo,  
    W_MasterItem.ItemName, 
    Inv_ItemStockUsedForBOM.Quantity AS OrderedQty

    FROM dbo.W_OrderCustomerReturn    

    INNER JOIN W_OrderCustomer 
        ON W_OrderCustomer.OrderID = W_OrderCustomerReturn.OrderID 

    INNER JOIN W_OrderCustomerDetails 
        ON W_OrderCustomerDetails.ID = W_OrderCustomerReturn.OrderDetailsID

    INNER JOIN Inv_ItemStockByBatchForBOM 
        ON Inv_ItemStockByBatchForBOM.ItemStockByBatchID = W_OrderCustomerReturn.ItemStockByBatchId 
		
		INNER JOIN Inv_ItemStockUsedForBOM
		ON Inv_ItemStockUsedForBOM.ItemStockUsedID=W_OrderCustomerReturn.ItemStockUsedID

    INNER JOIN A_MasterAccounts 
        ON A_MasterAccounts.AccountId = W_OrderCustomer.CustomerID   

    INNER JOIN W_MasterItem 
        ON W_MasterItem.ItemID = Inv_ItemStockByBatchForBOM.ItemId  

    LEFT JOIN (
        SELECT ItemStockUsedID, SUM(Quantity) AS TotalReturnQty
        FROM W_OrderCustomerReturn
        GROUP BY ItemStockUsedID
    ) R ON R.ItemStockUsedID = W_OrderCustomerReturn.ItemStockUsedID

    WHERE 1=1 ' + @Query + '    
) t1    
WHERE t1.RowNumber >= ' + CAST(@Top AS VARCHAR) + '    
AND t1.RowNumber <= ' + CAST(@Bottom AS VARCHAR)
   
    
    PRINT @RecQuery              
    EXEC dbo.sp_ExecuteSql @RecQuery                  
END 

