import os

file = r'D:\GFI_Upgrated\src\GFI_Upgrated.UI\Layout\MainLayout.razor'
with open(file, 'r', encoding='utf-8') as f:
    content = f.read()

old = '<img src="assets/img/branding/nuvotrace-custom-logo.png" alt="Logo" style="height: 55px; width: auto; object-fit: contain;" />'
new = '<img src="assets/img/branding/nuvotrace-horizontal-logo.png" alt="Logo" style="height: 65px; width: auto; object-fit: contain;" />'

content = content.replace(old, new)

with open(file, 'w', encoding='utf-8') as f:
    f.write(content)
