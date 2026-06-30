using ERPHub.Data;
using ERPHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ERPHub.Services
{
    public class LookupDataService
    {
        private readonly IMemoryCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

        public LookupDataService(IMemoryCache cache, IServiceScopeFactory scopeFactory)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        private static string CacheKey(string prefix) => $"Lookup_{prefix}";

        public async Task<List<Company>> GetCompaniesAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKey(nameof(Company)), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTtl;
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
                return await ctx.Companies.OrderBy(c => c.CompanyNameEn).ToListAsync();
            }) ?? [];
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKey(nameof(Department)), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTtl;
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
                return await ctx.Departments.OrderBy(d => d.NameEn).ToListAsync();
            }) ?? [];
        }

        public async Task<List<Section>> GetSectionsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKey(nameof(Section)), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTtl;
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
                return await ctx.Sections.OrderBy(s => s.NameEn).ToListAsync();
            }) ?? [];
        }

        public async Task<List<Designation>> GetDesignationsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKey(nameof(Designation)), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTtl;
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
                return await ctx.Designations.OrderBy(d => d.NameEn).ToListAsync();
            }) ?? [];
        }

        public async Task<List<Line>> GetLinesAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKey(nameof(Line)), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTtl;
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
                return await ctx.Lines.OrderBy(l => l.NameEn).ToListAsync();
            }) ?? [];
        }

        public async Task<List<Shift>> GetShiftsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKey(nameof(Shift)), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTtl;
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
                return await ctx.Shifts.OrderBy(s => s.ShiftName).ToListAsync();
            }) ?? [];
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKey(nameof(Employee)), async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTtl;
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<ErpDbContext>();
                return await ctx.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Line)
                    .Include(e => e.Shift)
                    .ToListAsync();
            }) ?? [];
        }

        public void InvalidateCompanies() => _cache.Remove(CacheKey(nameof(Company)));
        public void InvalidateDepartments() => _cache.Remove(CacheKey(nameof(Department)));
        public void InvalidateSections() => _cache.Remove(CacheKey(nameof(Section)));
        public void InvalidateDesignations() => _cache.Remove(CacheKey(nameof(Designation)));
        public void InvalidateLines() => _cache.Remove(CacheKey(nameof(Line)));
        public void InvalidateShifts() => _cache.Remove(CacheKey(nameof(Shift)));
        public void InvalidateEmployees() => _cache.Remove(CacheKey(nameof(Employee)));
        public void InvalidateAll()
        {
            InvalidateCompanies();
            InvalidateDepartments();
            InvalidateSections();
            InvalidateDesignations();
            InvalidateLines();
            InvalidateShifts();
            InvalidateEmployees();
        }
    }
}
