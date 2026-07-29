import os
import re

ui_dir = r"D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI"

def process_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    modified = False

    # 1. Update MudDatePicker
    # Regex to match MudDatePicker tag
    # We want to find <MudDatePicker ... >
    # and if it doesn't contain Editable=, insert Editable="true"
    def repl_datepicker(match):
        tag = match.group(0)
        if 'Editable=' not in tag:
            # Insert Editable="true" after <MudDatePicker
            return tag.replace('<MudDatePicker', '<MudDatePicker Editable="true"')
        return tag

    new_content = re.sub(r'<MudDatePicker[^>]*>', repl_datepicker, content)
    if new_content != content:
        content = new_content
        modified = True

    # 2. Update MudAutocomplete
    # Regex to match MudAutocomplete tag
    # and if it doesn't contain Clearable=, insert Clearable="true"
    def repl_autocomplete(match):
        tag = match.group(0)
        if 'Clearable=' not in tag:
            # Insert Clearable="true" after <MudAutocomplete
            return tag.replace('<MudAutocomplete', '<MudAutocomplete Clearable="true"')
        return tag

    new_content = re.sub(r'<MudAutocomplete[^>]*>', repl_autocomplete, content)
    if new_content != content:
        content = new_content
        modified = True

    if modified:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"Modified: {filepath}")

for root, dirs, files in os.walk(ui_dir):
    for file in files:
        if file.endswith('.razor'):
            process_file(os.path.join(root, file))
