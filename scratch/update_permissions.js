const fs = require('fs');
const filePath = 'D:\\GFI\\GFI_Upgrated\\src\\GFI_Upgrated.UI\\Modules\\Admin\\Pages\\Permissions.razor';

let content = fs.readFileSync(filePath, 'utf8');
let lines = content.split(/\r?\n/);

let headerUpdated = false;
let rowUpdated = false;

for (let i = 0; i < lines.length; i++) {
    // 1. Replace table element and inject Collapse/Expand buttons
    if (lines[i].includes('<MudTable Items="_permissions"')) {
        lines[i] = `        <div class="d-flex gap-2 mb-2">
            <MudButton Variant="Variant.Outlined" OnClick="CollapseAll" StartIcon="@Icons.Material.Filled.UnfoldLess" Size="Size.Small">Collapse All</MudButton>
            <MudButton Variant="Variant.Outlined" OnClick="ExpandAll" StartIcon="@Icons.Material.Filled.UnfoldMore" Size="Size.Small">Expand All</MudButton>
        </div>
        <MudTable Items="GetVisiblePermissions()" Dense="true" Hover="true" FixedHeader="true" Height="600px" Elevation="0" Class="border">`;
    }

    // 2. Replace headers with checkboxes
    if (lines[i].includes('<HeaderContent>') && !headerUpdated) {
        headerUpdated = true;
        lines[i] = `            <HeaderContent>
                <MudTh Style="width: 40%">
                    <div class="d-flex align-center">
                        <MudCheckBox T="bool" Value="@IsAllEverythingChecked()" ValueChanged="@(v => OnMasterEverythingToggle(v))" Color="Color.Default" Dense="true" Class="mr-2" />
                        Menu / Page
                    </div>
                </MudTh>
                <MudTh Style="text-align: center">
                    View
                    <div class="d-flex justify-center">
                        <MudCheckBox T="bool" Value="@IsAllChecked(\"View\")" ValueChanged="@(v => OnHeaderToggle(\"View\", v))" Color="Color.Primary" Dense="true" />
                    </div>
                </MudTh>
                <MudTh Style="text-align: center">
                    Insert
                    <div class="d-flex justify-center">
                        <MudCheckBox T="bool" Value="@IsAllChecked(\"Insert\")" ValueChanged="@(v => OnHeaderToggle(\"Insert\", v))" Color="Color.Success" Dense="true" />
                    </div>
                </MudTh>
                <MudTh Style="text-align: center">
                    Update
                    <div class="d-flex justify-center">
                        <MudCheckBox T="bool" Value="@IsAllChecked(\"Update\")" ValueChanged="@(v => OnHeaderToggle(\"Update\", v))" Color="Color.Warning" Dense="true" />
                    </div>
                </MudTh>
                <MudTh Style="text-align: center">
                    Delete
                    <div class="d-flex justify-center">
                        <MudCheckBox T="bool" Value="@IsAllChecked(\"Delete\")" ValueChanged="@(v => OnHeaderToggle(\"Delete\", v))" Color="Color.Error" Dense="true" />
                    </div>
                </MudTh>`;
        // skip old headers in subsequent lines
        while (!lines[i+1].includes('</HeaderContent>')) {
            lines.splice(i+1, 1);
        }
    }

    // 3. Replace RowTemplate Menu / Page TD
    if (lines[i].includes('<MudTd>') && lines[i+1].includes('style="padding-left:') && !rowUpdated) {
        rowUpdated = true;
        lines[i] = `                <MudTd>
                    <div style="padding-left: @(context.ParentID == 0 ? "0" : "24px")" class="d-flex align-center">
                        @if (context.ParentID == 0)
                        {
                            <MudIconButton Icon="@(_collapsedParents.Contains(context.LinkID) ? Icons.Material.Filled.ChevronRight : Icons.Material.Filled.ExpandMore)"
                                           Size="Size.Small" OnClick="@(() => ToggleParentCollapse(context.LinkID))" Class="mr-1" />
                            <MudIcon Icon="@Icons.Material.Filled.Folder" Size="Size.Small" Class="mr-2" Color="Color.Primary" />
                            <strong>@FormatPageHeading(context.PageHeading)</strong>
                        }
                        else
                        {
                            <MudIcon Icon="@Icons.Material.Filled.Description" Size="Size.Small" Class="mr-2" />
                            @FormatPageHeading(context.PageHeading)
                        }
                    </div>
                </MudTd>`;
        
        // delete original targeted MudTd structure
        let deleteCount = 0;
        while (!lines[i+1].includes('</MudTd>')) {
            lines.splice(i+1, 1);
            deleteCount++;
        }
        lines.splice(i+1, 1); // remove closing tag
    }
}

content = lines.join('\n');

// 4. Inject collapsing state and header/master checkbox logic in @code block
const codeToInject = `    private HashSet<long> _collapsedParents = new();

    private IEnumerable<RolePermissionDto> GetVisiblePermissions()
    {
        return _permissions.Where(p => p.ParentID == 0 || !_collapsedParents.Contains(p.ParentID));
    }

    private void ToggleParentCollapse(long parentId)
    {
        if (_collapsedParents.Contains(parentId))
            _collapsedParents.Remove(parentId);
        else
            _collapsedParents.Add(parentId);
    }

    private void CollapseAll()
    {
        foreach (var item in _permissions.Where(x => x.ParentID == 0))
        {
            _collapsedParents.Add(item.LinkID);
        }
    }

    private void ExpandAll()
    {
        _collapsedParents.Clear();
    }

    private string FormatPageHeading(string? heading)
    {
        if (string.IsNullOrEmpty(heading)) return string.Empty;
        var clean = heading.Replace("_", " ").Replace("-", " ");
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(clean.ToLower());
    }

    private bool IsAllChecked(string type)
    {
        if (_permissions.Count == 0) return false;
        switch (type)
        {
            case "View": return _permissions.All(x => x.ViewPer);
            case "Insert": return _permissions.All(x => x.InsertPer);
            case "Update": return _permissions.All(x => x.UpdatePer);
            case "Delete": return _permissions.All(x => x.DeletePer);
            default: return false;
        }
    }

    private void OnHeaderToggle(string type, bool value)
    {
        foreach (var item in _permissions)
        {
            switch (type)
            {
                case "View": item.ViewPer = value; break;
                case "Insert": item.InsertPer = value; break;
                case "Update": item.UpdatePer = value; break;
                case "Delete": item.DeletePer = value; break;
            }
        }
    }

    private bool IsAllEverythingChecked()
    {
        if (_permissions.Count == 0) return false;
        return _permissions.All(x => x.ViewPer && x.InsertPer && x.UpdatePer && x.DeletePer);
    }

    private void OnMasterEverythingToggle(bool value)
    {
        foreach (var item in _permissions)
        {
            item.ViewPer = value;
            item.InsertPer = value;
            item.UpdatePer = value;
            item.DeletePer = value;
        }
    }

    private List<RoleDto> _roles = new();`;

content = content.replace('private List<RoleDto> _roles = new();', codeToInject);

fs.writeFileSync(filePath, content, 'utf8');
console.log("Successfully updated Permissions.razor!");
