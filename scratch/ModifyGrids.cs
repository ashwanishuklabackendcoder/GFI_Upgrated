using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    static void Main()
    {
        string modulesDir = @"D:\GFI\GFI_Upgrated\src\GFI_Upgrated.UI\Modules";
        var files = Directory.GetFiles(modulesDir, "*.razor", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            try
            {
                string content = File.ReadAllText(file);
                if (!content.Contains("ServerData=\"LoadServerDataAsync\""))
                    continue;

                Console.WriteLine($"Processing {Path.GetFileName(file)}...");

                // Determine DTO type
                var dtoMatch = Regex.Match(content, @"TableData<([A-Za-z0-9_]+)>");
                if (!dtoMatch.Success)
                    dtoMatch = Regex.Match(content, @"MudTable<([A-Za-z0-9_]+)>");
                if (!dtoMatch.Success)
                    dtoMatch = Regex.Match(content, @"T=""([A-Za-z0-9_]+)""");
                
                if (!dtoMatch.Success)
                {
                    Console.WriteLine($"Could not determine DTO type for {Path.GetFileName(file)}");
                    continue;
                }

                string dtoType = dtoMatch.Groups[1].Value;

                // Determine search variable
                string searchVar = "_searchString";
                if (content.Contains("_search"))
                    searchVar = "_search";

                // 1. Insert cache field in @code block
                // Find `@code {` or `@code\s*{`
                var codeStartMatch = Regex.Match(content, @"@code\s*\{");
                if (!codeStartMatch.Success)
                {
                    Console.WriteLine($"Could not find @code block in {Path.GetFileName(file)}");
                    continue;
                }

                int codeStartIdx = codeStartMatch.Index + codeStartMatch.Length;
                string cacheFieldDecl = $"\n    private List<{dtoType}>? _cachedItems;\n";
                if (!content.Contains("_cachedItems"))
                {
                    content = content.Insert(codeStartIdx, cacheFieldDecl);
                }

                // 2. Identify the LoadServerDataAsync method and replace it
                // We'll search for the method body.
                var methodPattern = @"private\s+async\s+Task<TableData<" + dtoType + @">>\s+LoadServerDataAsync\s*\(\s*TableState\s+state\s*,\s*CancellationToken\s+cancellationToken\s*\)\s*\{([\s\S]*?)\}";
                var methodMatch = Regex.Match(content, methodPattern);
                if (!methodMatch.Success)
                {
                    // Try without CancellationToken
                    methodPattern = @"private\s+async\s+Task<TableData<" + dtoType + @">>\s+LoadServerDataAsync\s*\(\s*TableState\s+state\s*\)\s*\{([\s\S]*?)\}";
                    methodMatch = Regex.Match(content, methodPattern);
                }

                if (methodMatch.Success)
                {
                    string oldMethod = methodMatch.Value;
                    string oldBody = methodMatch.Groups[1].Value;

                    // Extract the API client call
                    var apiCallMatch = Regex.Match(oldBody, @"(var\s+result\s*=\s*await\s+ApiClient\.[A-Za-z0-9_]+\Async\([\s\S]*?\);)");
                    if (apiCallMatch.Success)
                    {
                        string apiCall = apiCallMatch.Groups[1].Value;

                        // Modify API Call to fetch all (pageSize: 100000, RecordPerPage = 100000)
                        string modifiedApiCall = apiCall;
                        // Replace page: state.Page + 1, and pageSize: state.PageSize with default/large values if present
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"page:\s*state\.Page\s*\+\s*1\s*,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"pageSize:\s*state\.PageSize\s*,?", "pageSize: 100000,");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"CurrentPage\s*=\s*state\.Page\s*\+\s*1\s*,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"RecordPerPage\s*=\s*state\.PageSize\s*,?", "RecordPerPage = 100000,");
                        // Remove search strings from API parameter to fetch all items
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"brandName:\s*[^,\)]+,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"groupName:\s*[^,\)]+,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"accountName:\s*[^,\)]+,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"symbol:\s*[^,\)]+,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"orderNo:\s*[^,\)]+,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"search:\s*[^,\)]+,?", "");

                        // Standardize sorting parameters in API call to default if present
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"sortCol:\s*state\.SortLabel\s*\?\?\s*""[^""]+""\s*,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"sortOrd:\s*state\.SortDirection\s*==\s*SortDirection\.Ascending\s*\?\s*""ASC""\s*:\s*""DESC""\s*,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"SortColumn\s*=\s*state\.SortLabel\s*\?\?\s*""[^""]+""\s*,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"SortType\s*=\s*state\.SortDirection\s*==\s*SortDirection\.Ascending\s*\?\s*""ASC""\s*:\s*""DESC""\s*,?", "");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"SortOrd\s*=\s*state\.SortDirection\s*==\s*SortDirection\.Ascending\s*\?\s*""ASC""\s*:\s*""DESC""\s*,?", "");

                        // Clean up double commas or formatting issues
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @",\s*,", ",");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @",\s*\)", ")");
                        modifiedApiCall = Regex.Replace(modifiedApiCall, @"\(\s*,", "(");

                        // Find default SortLabel from the old code
                        var defaultSortMatch = Regex.Match(oldBody, @"SortLabel\s*\?\?\s*""([A-Za-z0-9_]+)""");
                        if (!defaultSortMatch.Success)
                            defaultSortMatch = Regex.Match(oldBody, @"sortCol:\s*state\.SortLabel\s*\?\?\s*""([A-Za-z0-9_]+)""");
                        if (!defaultSortMatch.Success)
                            defaultSortMatch = Regex.Match(oldBody, @"sortCol\s*=\s*""([A-Za-z0-9_]+)""");
                        string defaultSort = defaultSortMatch.Success ? defaultSortMatch.Groups[1].Value : "1";

                        string newMethod = $@"private async Task<TableData<{dtoType}>> LoadServerDataAsync(TableState state, CancellationToken cancellationToken)
    {{
        try
        {
            if (_cachedItems == null)
            {{
                {modifiedApiCall.Replace("\r\n", "\r\n                ").Replace("\n", "\n                ")}
                _cachedItems = result.Items.ToList();
            }}

            var filtered = _cachedItems.FilterAllColumns({searchVar});
            var sorted = filtered.SortAllColumns(state.SortLabel ?? ""{defaultSort}"", state.SortDirection == SortDirection.Ascending ? ""ASC"" : ""DESC"");
            var itemsList = sorted.ToList();

            return new TableData<{dtoType}>
            {{
                TotalItems = itemsList.Count,
                Items = itemsList.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray()
            }};
        }}
        catch (Exception ex)
        {{
            Snackbar.Add($""Error loading: {{ex.Message}}"", Severity.Error);
            return new TableData<{dtoType}> {{ TotalItems = 0, Items = Array.Empty<{dtoType}>() }};
        }}
    }}";

                        content = content.Replace(oldMethod, newMethod);
                    }
                    else
                    {
                        Console.WriteLine($"Could not find API Call in LoadServerDataAsync for {Path.GetFileName(file)}");
                    }
                }
                else
                {
                    Console.WriteLine($"Could not find LoadServerDataAsync method for {Path.GetFileName(file)}");
                }

                // 3. Replace Search Button and Text Field for instant search
                // Find MudTextField that binds to _search or _searchString
                // If it binds to _search, we want:
                // T="string" Value="@_search" ValueChanged="@OnSearch" Label="..." Variant="Variant.Outlined" Immediate="true" ...
                // And define OnSearch if not already defined:
                // private async Task OnSearch(string text) { _search = text; if (_table is not null) await _table.ReloadServerData(); }
                if (!content.Contains("private async Task OnSearch") && !content.Contains("private Task OnSearch"))
                {
                    string onSearchMethod = $"\n    private async Task OnSearch(string text)\n    {{\n        {searchVar} = text;\n        if (_table is not null) await _table.ReloadServerData();\n    }}\n";
                    int idx = content.IndexOf(cacheFieldDecl);
                    if (idx >= 0)
                    {
                        content = content.Insert(idx + cacheFieldDecl.Length, onSearchMethod);
                    }
                    else
                    {
                        content = content.Insert(codeStartIdx, onSearchMethod);
                    }
                }

                // Update MudTextField binding
                // Replace: <MudTextField @bind-Value="_search" ... />
                // with: <MudTextField T="string" Value="@_search" ValueChanged="@OnSearch" Immediate="true" ... />
                content = Regex.Replace(content, @"<MudTextField\s+@bind-Value=""_search""([^>]*?)/>", @"<MudTextField T=""string"" Value=""@_search"" ValueChanged=""@OnSearch"" Immediate=""true""$1/>");
                content = Regex.Replace(content, @"<MudTextField\s+@bind-Value=""_search""([^>]*?)>", @"<MudTextField T=""string"" Value=""@_search"" ValueChanged=""@OnSearch"" Immediate=""true""$1>");

                // Also replace the Search Button
                // Remove: <MudButton Variant="Variant.Outlined" OnClick="SearchAsync">Search</MudButton>
                content = Regex.Replace(content, @"<MudButton\s+[^>]*?OnClick=""SearchAsync""[^>]*?>Search</MudButton>", "");

                // 4. Invalidate cache on add/edit/delete/toggle
                // We'll search for `ReloadServerData()` and append `_cachedItems = null;` before it.
                // Replace `await _table.ReloadServerData()` with `_cachedItems = null; await _table.ReloadServerData()`
                // Avoid replacing when loading server data or searching
                content = Regex.Replace(content, @"(?<!OnSearch\([\s\S]*?)(?<!LoadServerDataAsync\([\s\S]*?)await\s+_table\.ReloadServerData\(\)", "_cachedItems = null; await _table.ReloadServerData()");
                // Also handle cases without await (e.g. state.ReloadServerData() if any, or _table?.ReloadServerData())
                content = Regex.Replace(content, @"(?<!OnSearch\([\s\S]*?)(?<!LoadServerDataAsync\([\s\S]*?)_table\?\.ReloadServerData\(\)", "_cachedItems = null; _table?.ReloadServerData()");

                File.WriteAllText(file, content, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {Path.GetFileName(file)}: {ex.Message}");
            }
        }

        Console.WriteLine("Done!");
    }
}
