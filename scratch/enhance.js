const fs = require('fs');
const path = require('path');

const srcDir = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI';

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = path.join(dir, file);
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(file));
        } else if (file.endsWith('.razor')) {
            results.push(file);
        }
    });
    return results;
}

const files = walk(srcDir);

files.forEach(filePath => {
    let content = fs.readFileSync(filePath, 'utf8');
    let modified = false;

    // 1. Process MudAutocomplete tags (stopping at self-closing tag end: />)
    const autocompleteRegex = /<MudAutocomplete[\s\S]*?\/>/g;
    content = content.replace(autocompleteRegex, tag => {
        // Clean all existing Clearable attributes
        let cleaned = tag.replace(/\bClearable\s*=\s*"[^"]*"/gi, '');
        cleaned = cleaned.replace(/\bClearable\s*=\s*'[^']*'/gi, '');
        cleaned = cleaned.replace(/\bClearable\s*=\s*[^\s>]+/gi, '');
        cleaned = cleaned.replace(/\bClearable\b/gi, '');
        
        // Add single Clearable="true"
        const upgraded = cleaned.replace('<MudAutocomplete', '<MudAutocomplete Clearable="true"');
        if (upgraded !== tag) {
            modified = true;
        }
        return upgraded;
    });

    // 2. Process MudDatePicker tags (stopping at self-closing tag end: />)
    const datepickerRegex = /<MudDatePicker[\s\S]*?\/>/g;
    content = content.replace(datepickerRegex, tag => {
        // Clean all existing Editable attributes
        let cleaned = tag.replace(/\bEditable\s*=\s*"[^"]*"/gi, '');
        cleaned = cleaned.replace(/\bEditable\s*=\s*'[^']*'/gi, '');
        cleaned = cleaned.replace(/\bEditable\s*=\s*[^\s>]+/gi, '');
        cleaned = cleaned.replace(/\bEditable\b/gi, '');
        
        // Add single Editable="true"
        const upgraded = cleaned.replace('<MudDatePicker', '<MudDatePicker Editable="true"');
        if (upgraded !== tag) {
            modified = true;
        }
        return upgraded;
    });

    // 3. Search and replace search functions to return all values if pre-filled
    const searchFuncRegex = /private\s+async\s+Task<IEnumerable<([^>]+)>>\s+(Search\w+)\(string\s+value,\s+CancellationToken\s+token\)\s*\{(\s*if\s*\(\s*string\.IsNullOrEmpty\(value\)\s*\)\s*(?:\{\s*)?return\s+(\w+);(?:\s*\})?)\s*return\s+\4\.Where\(x\s*=>\s*(?:\(x\.(\w+)\s*(?:\?\?\s*""\s*)?\)|x\.(\w+))\.Contains\(value,\s*StringComparison\.OrdinalIgnoreCase\)\);\s*\}/g;

    const newContent = content.replace(searchFuncRegex, (match, type, funcName, body1, coll, prop1, prop2) => {
        const prop = prop1 || prop2;
        modified = true;
        return `private async Task<IEnumerable<${type}>> ${funcName}(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || ${coll}.Any(x => (x.${prop} ?? "").Equals(value, StringComparison.OrdinalIgnoreCase))) return ${coll};
        return ${coll}.Where(x => (x.${prop} ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;
    });
    content = newContent;

    const searchFuncRegex2 = /private\s+async\s+Task<IEnumerable<([^>]+)>>\s+(Search\w+)\(string\s+value,\s+CancellationToken\s+token\)\s*\{(\s*if\s*\(\s*string\.IsNullOrEmpty\(value\)\s*\)\s*(?:\{\s*)?return\s+(\w+);(?:\s*\})?)\s*return\s+\4\.Where\(x\s*=>\s*\(?(?:x\.(\w+)\s*(?:\?\?\s*""\s*)?)\)?\.Contains\(value,\s*StringComparison\.OrdinalIgnoreCase\)\);\s*\}/g;
    
    const newContent2 = content.replace(searchFuncRegex2, (match, type, funcName, body1, coll, prop) => {
        modified = true;
        return `private async Task<IEnumerable<${type}>> ${funcName}(string value, CancellationToken token)
    {
        if (string.IsNullOrEmpty(value) || ${coll}.Any(x => (x.${prop} ?? "").Equals(value, StringComparison.OrdinalIgnoreCase))) return ${coll};
        return ${coll}.Where(x => (x.${prop} ?? "").Contains(value, StringComparison.OrdinalIgnoreCase));
    }`;
    });
    content = newContent2;

    if (modified) {
        fs.writeFileSync(filePath, content, 'utf8');
        console.log(`Enhanced: ${path.basename(filePath)}`);
    }
});
