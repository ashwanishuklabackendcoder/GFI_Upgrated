import re
with open('W_ItemStockList_clean.sql', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace('exec dbo.sp_ExecuteSql @RecQuery', 'PRINT @RecQuery')
with open('W_ItemStockList_test.sql', 'w', encoding='utf-8') as f:
    f.write(content)
