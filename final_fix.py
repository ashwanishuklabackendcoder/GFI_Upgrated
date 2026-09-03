with open('W_ItemStockList_clean.sql', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace('isnull(ROUND(W_ItemStock.OpeningQuantity, 2) as OpeningQuantity, 0)', 'isnull(ROUND(W_ItemStock.OpeningQuantity, 2), 0)')
content = content.replace('isnull(ROUND(W_ItemStock.PurchasedQuantity, 2) as PurchasedQuantity, 0)', 'isnull(ROUND(W_ItemStock.PurchasedQuantity, 2), 0)')
content = content.replace('isnull(ROUND(W_ItemStock.ProducedQuantity, 2) as ProducedQuantity, 0)', 'isnull(ROUND(W_ItemStock.ProducedQuantity, 2), 0)')
content = content.replace('isnull(ROUND(W_ItemStock.IssuedQuantity, 2) as IssuedQuantity, 0)', 'isnull(ROUND(W_ItemStock.IssuedQuantity, 2), 0)')
content = content.replace('isnull(ROUND(W_ItemStock.RemovedQuantity, 2) as RemovedQuantity, 0)', 'isnull(ROUND(W_ItemStock.RemovedQuantity, 2), 0)')

with open('W_ItemStockList_clean.sql', 'w', encoding='utf-8') as f:
    f.write(content)
