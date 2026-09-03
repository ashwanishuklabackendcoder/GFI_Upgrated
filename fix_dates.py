import re

with open('W_ItemStockList_rounded.sql', 'r') as f:
    content = f.read()

new_from = ""\"    if @FromDate is not null
        set @Query=@Query + ' and EXISTS (SELECT 1 FROM Inv_ItemStockByBatch b WHERE b.ItemId = W_ItemStock.ItemID AND ( ' +
        ' (b.StockById = 1 AND EXISTS (SELECT 1 FROM W_PurchaseMaster p JOIN W_PurchaseChild pc ON p.PurchaseID = pc.PurchaseID WHERE pc.PurchaseItemID = b.IdFrom AND cast(p.GoodsRecievedDate as date) >= ''''' + cast(@FromDate as varchar(50)) + ''''')) OR ' +
        ' (b.StockById = 2 AND EXISTS (SELECT 1 FROM W_PreProcessing pp WHERE pp.PreProcessingId = b.IdFrom AND cast(pp.ProcessingDate as date) >= ''''' + cast(@FromDate as varchar(50)) + ''''')) OR ' +
        ' (b.StockById = 4 AND EXISTS (SELECT 1 FROM W_Production p WHERE p.ProductionId = b.IdFrom AND cast(p.CookingDate as date) >= ''''' + cast(@FromDate as varchar(50)) + ''''')) OR ' +
        ' (b.StockById = 3 AND cast(W_ItemStock.OpeningStockDate as date) >= ''''' + cast(@FromDate as varchar(50)) + ''''') ' +
        '))' ""\"

new_to = ""\"    if @ToDate is not null
        set @Query=@Query + ' and EXISTS (SELECT 1 FROM Inv_ItemStockByBatch b WHERE b.ItemId = W_ItemStock.ItemID AND ( ' +
        ' (b.StockById = 1 AND EXISTS (SELECT 1 FROM W_PurchaseMaster p JOIN W_PurchaseChild pc ON p.PurchaseID = pc.PurchaseID WHERE pc.PurchaseItemID = b.IdFrom AND cast(p.GoodsRecievedDate as date) <= ''''' + cast(@ToDate as varchar(50)) + ''''')) OR ' +
        ' (b.StockById = 2 AND EXISTS (SELECT 1 FROM W_PreProcessing pp WHERE pp.PreProcessingId = b.IdFrom AND cast(pp.ProcessingDate as date) <= ''''' + cast(@ToDate as varchar(50)) + ''''')) OR ' +
        ' (b.StockById = 4 AND EXISTS (SELECT 1 FROM W_Production p WHERE p.ProductionId = b.IdFrom AND cast(p.CookingDate as date) <= ''''' + cast(@ToDate as varchar(50)) + ''''')) OR ' +
        ' (b.StockById = 3 AND cast(W_ItemStock.OpeningStockDate as date) <= ''''' + cast(@ToDate as varchar(50)) + ''''') ' +
        '))' ""\"

content = re.sub(r'if @FromDate is not null.*?cast\(@FromDate as varchar\(50\)\) \+ \'\'\'', new_from, content, flags=re.DOTALL)
content = re.sub(r'if @ToDate is not null.*?cast\(@ToDate as varchar\(50\)\) \+ \'\'\'', new_to, content, flags=re.DOTALL)

with open('W_ItemStockList_FixDate.sql', 'w', encoding='utf-8') as f:
    f.write(content)
