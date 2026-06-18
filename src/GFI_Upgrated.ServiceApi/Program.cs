using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUi", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("YOUR_PASSWORD") || connectionString.Contains("YOUR_USER"))
{
    throw new InvalidOperationException("DefaultConnection connection string is missing or contains placeholder values. Set a valid connection string in configuration or environment variables.");
}

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey == "GFI_Upgrated_Super_Secret_Key_12345!@#")
{
    throw new InvalidOperationException("Jwt:Key is missing or uses default security fallback. A secure JWT Key must be provided in configuration or environment variables.");
}

builder.Services.AddHealthChecks()
    .AddCheck("SQLServer", () =>
    {
        try
        {
            var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connStr) || connStr.Contains("YOUR_PASSWORD"))
            {
                return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("SQL Server connection string is not configured.");
            }
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(connStr);
            conn.Open();
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("SQL Server connection failed.", ex);
        }
    })
    .AddCheck("Memory", () =>
    {
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var allocatedMb = allocated / (1024 * 1024);
        if (allocatedMb > 1024) 
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Degraded($"High memory allocation: {allocatedMb} MB");
        }
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy($"Allocated memory: {allocatedMb} MB");
    });

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "GFI_Upgrated",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "GFI_Upgrated_UI",
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();


builder.Services.AddScoped<GFI_Upgrated.Data.AdminSecurity.IAdminSecurityRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.AdminSecurity.AdminSecurityRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IBrandRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.BrandRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.ISkuRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.SkuRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IUnitRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.UnitRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IWarehouseRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.WarehouseRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IKettleRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.KettleRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IItemCategoryRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.ItemCategoryRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IStatusRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.StatusRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IItemTypeRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.ItemTypeRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IAlmirahRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.AlmirahRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IPreProcessingRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.PreProcessingRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IRawMaterialRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.RawMaterialRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.ISemiFinishedProductRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.SemiFinishedProductRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IFinishedProductRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.FinishedProductRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IBomRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.BomRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IProductionRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.ProductionRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Store.IReportRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Store.ReportRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Purchase.IPurchaseRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Purchase.PurchaseRepository(connectionString);
});
builder.Services.AddScoped<GFI_Upgrated.Data.Account.IAccountRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    return new GFI_Upgrated.Data.Account.AccountRepository(connectionString);
});

builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.IAdminSecurityService, GFI_Upgrated.ServiceApi.Services.AdminSecurityService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IBrandService, GFI_Upgrated.ServiceApi.Services.Store.BrandService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.ISkuService, GFI_Upgrated.ServiceApi.Services.Store.SkuService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IUnitService, GFI_Upgrated.ServiceApi.Services.Store.UnitService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IWarehouseService, GFI_Upgrated.ServiceApi.Services.Store.WarehouseService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IKettleService, GFI_Upgrated.ServiceApi.Services.Store.KettleService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IItemCategoryService, GFI_Upgrated.ServiceApi.Services.Store.ItemCategoryService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IStatusService, GFI_Upgrated.ServiceApi.Services.Store.StatusService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IItemTypeService, GFI_Upgrated.ServiceApi.Services.Store.ItemTypeService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IAlmirahService, GFI_Upgrated.ServiceApi.Services.Store.AlmirahService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IPreProcessingService, GFI_Upgrated.ServiceApi.Services.Store.PreProcessingService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IProductionService, GFI_Upgrated.ServiceApi.Services.Store.ProductionService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IReportService, GFI_Upgrated.ServiceApi.Services.Store.ReportService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IRawMaterialService, GFI_Upgrated.ServiceApi.Services.Store.RawMaterialService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.ISemiFinishedProductService, GFI_Upgrated.ServiceApi.Services.Store.SemiFinishedProductService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IFinishedProductService, GFI_Upgrated.ServiceApi.Services.Store.FinishedProductService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Store.IBomService, GFI_Upgrated.ServiceApi.Services.Store.BomService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Purchase.IPurchaseService, GFI_Upgrated.ServiceApi.Services.Purchase.PurchaseService>();
builder.Services.AddScoped<GFI_Upgrated.ServiceApi.Services.Account.IAccountService, GFI_Upgrated.ServiceApi.Services.Account.AccountService>();

var app = builder.Build();

app.UseCors("AllowUi");

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapHealthChecks("/health");

app.Run();
