using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GFI_Upgrated.UI;
using GFI_Upgrated.UI.Services;
using GFI_Upgrated.UI.State;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddSingleton<AppSessionState>();
builder.Services.AddSingleton<LocalizationState>();
builder.Services.AddScoped(sp =>
{
    var apiBase = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
    return new HttpClient { BaseAddress = new Uri(apiBase) };
});
builder.Services.AddScoped<AdminSecurityApiClient>();
builder.Services.AddScoped<StoreApiClient>();
builder.Services.AddScoped<AccountApiClient>();

await builder.Build().RunAsync();
