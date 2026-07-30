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

    const datepickerRegex = /<MudDatePicker[\s\S]*?\/>/g;
    content = content.replace(datepickerRegex, tag => {
        // Clean existing DateFormat and Placeholder
        let cleaned = tag.replace(/\bDateFormat\s*=\s*"[^"]*"/gi, '');
        cleaned = cleaned.replace(/\bDateFormat\s*=\s*'[^']*'/gi, '');
        cleaned = cleaned.replace(/\bDateFormat\s*=\s*[^\s>]+/gi, '');
        cleaned = cleaned.replace(/\bDateFormat\b/gi, '');

        cleaned = cleaned.replace(/\bPlaceholder\s*=\s*"[^"]*"/gi, '');
        cleaned = cleaned.replace(/\bPlaceholder\s*=\s*'[^']*'/gi, '');
        cleaned = cleaned.replace(/\bPlaceholder\s*=\s*[^\s>]+/gi, '');
        cleaned = cleaned.replace(/\bPlaceholder\b/gi, '');

        // Update Label to append (DD/MM/YYYY) if not already present
        cleaned = cleaned.replace(/Label="([^"]+)"/gi, (match, labelText) => {
            if (!/DD\/MM\/YYYY/i.test(labelText)) {
                return `Label="${labelText} (DD/MM/YYYY)"`;
            }
            return match;
        });

        // Add standard DateFormat and Placeholder
        const upgraded = cleaned.replace('<MudDatePicker', '<MudDatePicker DateFormat="dd/MM/yyyy" Placeholder="dd/MM/yyyy"');
        if (upgraded !== tag) {
            modified = true;
        }
        return upgraded;
    });

    if (modified) {
        fs.writeFileSync(filePath, content, 'utf8');
        console.log(`Updated DatePicker in: ${path.basename(filePath)}`);
    }
});
