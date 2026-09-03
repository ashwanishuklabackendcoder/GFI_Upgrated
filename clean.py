import re
with open('W_ItemStockList_rounded.sql', 'r', encoding='utf-8') as f:
    content = f.read()

content = re.sub(r'IsCompl\s+ete', 'IsComplete', content)
content = re.sub(r'OpeningStockD\s+ate', 'OpeningStockDate', content)

with open('W_ItemStockList_clean.sql', 'w', encoding='utf-8') as f:
    f.write(content)
