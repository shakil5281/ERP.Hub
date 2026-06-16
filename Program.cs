using ERPHub;
using ERPHub.Components;
using ERPHub.Services;
using ERPHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using OfficeOpenXml;

ExcelPackage.License.SetNonCommercialOrganization("ERPHub");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controller support
builder.Services.AddControllers();

// Add Cascading Authentication & Authorization services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Add HttpClient support
builder.Services.AddHttpClient();

// Register SQL Server Database Context
builder.Services.AddDbContext<ErpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our custom ERP Service as Scoped (since database contexts are scoped)
builder.Services.AddScoped<IErpService, ErpService>();

var app = builder.Build();

// Seed admin user on first run
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
    if (!await context.Users.AnyAsync())
    {
        context.Users.Add(new ERPHub.Models.User
        {
            Username = "admin",
            PasswordHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes("admin123"))),
            FullName = "System Administrator",
            Role = "Admin"
        });
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/notfound");

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map Controller API Endpoints
app.MapControllers();

app.Run();
