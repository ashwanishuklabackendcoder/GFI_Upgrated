Text                                                                                                                                                                                                                                                           
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Rpt_ItemStockTraceability]
                                                                                                                                                                                                           
@ItemId bigint
                                                                                                                                                                                                                                               
AS
                                                                                                                                                                                                                                                           
BEGIN
                                                                                                                                                                                                                                                        
    SET NOCOUNT ON;
                                                                                                                                                                                                                                          

                                                                                                                                                                                                                                                             
    SELECT 
                                                                                                                                                                                                                                                  
        COALESCE(
                                                                                                                                                                                                                                            
            CASE WHEN b.StockById = 1 THEN (SELECT GoodsRecievedDate FROM W_PurchaseMaster p JOIN W_PurchaseChild pc ON p.PurchaseID = pc.PurchaseID WHERE pc.PurchaseItemID = b.IdFrom) END,
                                                                
            CASE WHEN b.StockById = 2 THEN (SELECT ProcessingDate FROM W_PreProcessing pp WHERE pp.PreProcessingId = b.IdFrom) END,
                                                                                                                          
            CASE WHEN b.StockById = 4 THEN (SELECT CookingDate FROM W_Production p WHERE p.ProductionId = b.IdFrom) END,
                                                                                                                                     
            CASE WHEN b.StockById = 3 THEN (SELECT TOP 1 OpeningStockDate FROM W_ItemStock s WHERE s.ItemID = b.ItemId) END,
                                                                                                                                 
            NULL
                                                                                                                                                                                                                                             
        ) AS TransactionDate,
                                                                                                                                                                                                                                
        b.BatchNo,
                                                                                                                                                                                                                                           
        CASE 
                                                                                                                                                                                                                                                
            WHEN b.StockById = 1 THEN 'Purchased'
                                                                                                                                                                                                            
            WHEN b.StockById = 2 OR b.StockById = 4 THEN 'Produced'
                                                                                                                                                                                          
            WHEN b.StockById = 3 OR b.StockById = 0 THEN 'Opening Stock'
                                                                                                                                                                                     
            ELSE 'Added (Other)'
                                                                                                                                                                                                                             
        END AS TransactionType,
                                                                                                                                                                                                                              
        'Added to Stock' AS Reference,
                                                                                                                                                                                                                       
        CAST(b.Quantity AS float) AS InQty,
                                                                                                                                                                                                                  
        CAST(0.0 AS float) AS OutQty,
                                                                                                                                                                                                                        
        CAST(b.Amount AS float) AS TotalValue,
                                                                                                                                                                                                               
        ISNULL(mu.UnitName, '') AS UnitName,
                                                                                                                                                                                                                 
        0 AS RefUsedFor,
                                                                                                                                                                                                                                     
        CAST(0 AS bigint) AS RefUsedForId
                                                                                                                                                                                                                    
    FROM Inv_ItemStockByBatch b
                                                                                                                                                                                                                              
    LEFT JOIN W_MasterUnit mu ON b.Unit = mu.UnitId
                                                                                                                                                                                                          
    WHERE b.ItemId = @ItemId
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
    UNION ALL
                                                                                                                                                                                                                                                

                                                                                                                                                                                                                                                             
    SELECT 
                                                                                                                                                                                                                                                  
        u.CreatedDate AS TransactionDate,
                                                                                                                                                                                                                    
        b.BatchNo,
                                                                                                                                                                                                                                           
        CASE 
                                                                                                                                                                                                                                                
            WHEN u.UsedFor = 1 THEN 'Issued (Sales)'
                                                                                                                                                                                                         
            WHEN u.UsedFor = 2 THEN 'Issued (Pre-Processing)'
                                                                                                                                                                                                
            WHEN u.UsedFor = 3 THEN 'Issued (Production)'
                                                                                                                                                                                                    
            ELSE 'Removed'
                                                                                                                                                                                                                                   
        END AS TransactionType,
                                                                                                                                                                                                                              
        COALESCE(
                                                                                                                                                                                                                                            
            CASE 
                                                                                                                                                                                                                                            
                WHEN u.UsedFor = 2 THEN 
                                                                                                                                                                                                                     
                    (SELECT 'Used to produce ' + ISNULL(preMi.ItemName, '') + ' (Batch: ' + ISNULL(pre.BatchNumberMade, '') + ')' 
                                                                                                                           
                     FROM W_PreProcessing pre 
                                                                                                                                                                                                               
                     JOIN W_MasterBom preBom ON pre.BomId = preBom.BomId 
                                                                                                                                                                                    
                     JOIN W_MasterItem preMi ON preBom.ItemId = preMi.ItemID 
                                                                                                                                                                                
                     WHERE pre.PreProcessingId = u.UsedForId)
                                                                                                                                                                                                
                WHEN u.UsedFor = 3 THEN 
                                                                                                                                                                                                                     
                    (SELECT 'Used to produce ' + ISNULL(prodMi.ItemName, '') + ' (Batch: ' + ISNULL(prod.BatchNo, '') + ')' 
                                                                                                                                 
                     FROM W_Production prod 
                                                                                                                                                                                                                 
                     JOIN W_MasterBom prodBom ON prod.BomId = prodBom.BomId 
                                                                                                                                                                                 
                     JOIN W_MasterItem prodMi ON prodBom.ItemId = prodMi.ItemID 
                                                                                                                                                                             
                     WHERE prod.ProductionId = u.UsedForId)
                                                                                                                                                                                                  
                WHEN u.UsedFor = 1 THEN 
                                                                                                                                                                                                                     
                    'Issued for Sales Order ID: ' + CAST(u.UsedForId AS NVARCHAR(50))
                                                                                                                                                                        
                ELSE NULL 
                                                                                                                                                                                                                                   
            END,
                                                                                                                                                                                                                                             
            ISNULL(NULLIF(CAST(u.Description AS NVARCHAR(1000)), ''), 'Used')
                                                                                                                                                                                
        ) AS Reference,
                                                                                                                                                                                                                                      
        CAST(0.0 AS float) AS InQty,
                                                                                                                                                                                                                         
        CAST(u.Quantity AS float) AS OutQty,
                                                                                                                                                                                                                 
        CAST((b.Amount / NULLIF(b.Quantity, 0)) * u.Quantity AS float) AS TotalValue,
                                                                                                                                                                        
        ISNULL(mu.UnitName, '') AS UnitName,
                                                                                                                                                                                                                 
        ISNULL(u.UsedFor, 0) AS RefUsedFor,
                                                                                                                                                                                                                  
        ISNULL(u.UsedForId, 0) AS RefUsedForId
                                                                                                                                                                                                               
    FROM Inv_ItemStockUsed u
                                                                                                                                                                                                                                 
    JOIN Inv_ItemStockByBatch b ON u.ItemStockByBatchId = b.ItemStockByBatchId
                                                                                                                                                                               
    LEFT JOIN W_MasterUnit mu ON b.Unit = mu.UnitId
                                                                                                                                                                                                          
    WHERE b.ItemId = @ItemId
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                             
    ORDER BY BatchNo, TransactionType DESC
                                                                                                                                                                                                                   
END                                                                                                                                                                                                                                                            
