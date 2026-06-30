using ERPHub;
using ERPHub.Components;
using ERPHub.Services;
using ERPHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using OfficeOpenXml;
using Hangfire;
using ERPHub.Jobs;
using ERPHub.Security;

ExcelPackage.License.SetNonCommercialOrganization("ERPHub");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add controller support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Add Cascading Authentication & Authorization services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Add HttpClient support
builder.Services.AddHttpClient();

// Add In-Memory Cache for LookupDataService
builder.Services.AddMemoryCache();

// Register SQL Server Database Context
builder.Services.AddDbContext<ErpDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContextFactory<ErpDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}, ServiceLifetime.Scoped);

// Register our custom ERP Service as Scoped (since database contexts are scoped)
builder.Services.AddScoped<IErpService, ErpService>();
builder.Services.AddScoped<ISeparationService, SeparationService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<JobCardService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<LookupDataService>();
builder.Services.AddScoped<EmployeeFilterService>();

// Register Hangfire services
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

var app = builder.Build();

// Seed default users on first run
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
    if (!await context.Roles.AnyAsync())
    {
        var adminRole = new ERPHub.Models.Role { Name = "Super Admin" };
        var userRole = new ERPHub.Models.Role { Name = "User" };
        context.Roles.AddRange(adminRole, userRole);
        await context.SaveChangesAsync();
    }

    if (!await context.Users.AnyAsync())
    {
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Super Admin");
        var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

        var adminUser = new ERPHub.Models.User
        {
            Username = "admin",
            PasswordHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes("admin123"))),
            FullName = "System Administrator"
        };
        var regularUser = new ERPHub.Models.User
        {
            Username = "user",
            PasswordHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes("user123"))),
            FullName = "Staff User"
        };

        context.Users.AddRange(adminUser, regularUser);
        await context.SaveChangesAsync();

        if (adminRole != null)
        {
            context.UserRoles.Add(new ERPHub.Models.UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
        }
        if (userRole != null)
        {
            context.UserRoles.Add(new ERPHub.Models.UserRole { UserId = regularUser.Id, RoleId = userRole.Id });
        }
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStatusCodePagesWithRedirects("/notfound");

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map Controller API Endpoints
app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAuthorizationFilter()]
});

// Schedule recurring daily attendance job
RecurringJob.AddOrUpdate<DailyAttendanceProcessor>("Process-Daily-Attendance", x => x.ProcessYesterdayAttendanceAsync(), Cron.Daily(1));

app.Run();
