using Microsoft.EntityFrameworkCore;
using ERPHub.Models;

namespace ERPHub.Data
{
    public class ErpDbContext : DbContext
    {
        public ErpDbContext(DbContextOptions<ErpDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<BusinessGroup> BusinessGroups => Set<BusinessGroup>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Designation> Designations => Set<Designation>();
        public DbSet<Line> Lines => Set<Line>();
        public DbSet<Shift> Shifts => Set<Shift>();
        public DbSet<PunchRecord> PunchRecords => Set<PunchRecord>();
        public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
        public DbSet<Holiday> Holidays => Set<Holiday>();
        public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();
        public DbSet<JobCard> JobCards => Set<JobCard>();
        public DbSet<ManualPunchLog> ManualPunchLogs => Set<ManualPunchLog>();
        public DbSet<OvertimeDeduction> OvertimeDeductions => Set<OvertimeDeduction>();
        public DbSet<DailySalaryRecord> DailySalaryRecords => Set<DailySalaryRecord>();
        public DbSet<Manpower> Manpowers => Set<Manpower>();
        public DbSet<ManpowerRequirement> ManpowerRequirements => Set<ManpowerRequirement>();
        public DbSet<Separation> Separations => Set<Separation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Invoice -> InvoiceItems one-to-many cascading delete relationship
            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Items)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Product price precision (decimal 18, 2)
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            // Configure Invoice tax rate & discount amount precision
            modelBuilder.Entity<Invoice>()
                .Property(i => i.TaxRate)
                .HasColumnType("decimal(18, 4)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.DiscountAmount)
                .HasColumnType("decimal(18, 2)");

            // Configure InvoiceItem unit price precision
            modelBuilder.Entity<InvoiceItem>()
                .Property(ii => ii.UnitPrice)
                .HasColumnType("decimal(18, 2)");

            // Configure Section -> Department relationship
            modelBuilder.Entity<Section>()
                .HasOne<Department>()
                .WithMany()
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Designation -> Section relationship
            modelBuilder.Entity<Designation>()
                .HasOne<Section>()
                .WithMany()
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Line -> Section relationship
            modelBuilder.Entity<Line>()
                .HasOne<Section>()
                .WithMany()
                .HasForeignKey(l => l.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Employee relationships and precision
            modelBuilder.Entity<Employee>()
                .Property(e => e.BasicSalary)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Employee>()
                .Property(e => e.GrossSalary)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Section)
                .WithMany()
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Designation)
                .WithMany()
                .HasForeignKey(e => e.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Line)
                .WithMany()
                .HasForeignKey(e => e.LineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Shift)
                .WithMany()
                .HasForeignKey(e => e.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Company)
                .WithMany()
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.PunchNumber)
                .IsUnique();

            modelBuilder.Entity<PunchRecord>()
                .HasIndex(p => new { p.UserPunchId, p.LogDateTime })
                .IsUnique();

            modelBuilder.Entity<PunchRecord>()
                .HasIndex(p => p.PunchNumber);

            modelBuilder.Entity<PunchRecord>()
                .HasIndex(p => p.LogDateTime);

            modelBuilder.Entity<PunchRecord>()
                .HasIndex(p => p.DeviceId);

            modelBuilder.Entity<AttendanceRecord>()
                .HasIndex(a => new { a.EmployeeId, a.AttendanceDate })
                .IsUnique();

            modelBuilder.Entity<AttendanceRecord>()
                .HasIndex(a => a.AttendanceDate);

            modelBuilder.Entity<Holiday>()
                .HasIndex(h => h.HolidayDate)
                .IsUnique();

            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.DailyBasic)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.OtPay)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.Allowances)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.Deductions)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.NetPay)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Manpower>()
                .HasOne(m => m.Department)
                .WithMany()
                .HasForeignKey(m => m.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Manpower>()
                .HasOne(m => m.Section)
                .WithMany()
                .HasForeignKey(m => m.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Manpower>()
                .HasOne(m => m.Designation)
                .WithMany()
                .HasForeignKey(m => m.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Manpower>()
                .HasIndex(m => new { m.DepartmentId, m.SectionId, m.DesignationId })
                .IsUnique();

            modelBuilder.Entity<ManpowerRequirement>()
                .HasOne(m => m.Department)
                .WithMany()
                .HasForeignKey(m => m.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManpowerRequirement>()
                .HasOne(m => m.Section)
                .WithMany()
                .HasForeignKey(m => m.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManpowerRequirement>()
                .HasOne(m => m.Designation)
                .WithMany()
                .HasForeignKey(m => m.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ManpowerRequirement>()
                .HasIndex(m => m.Status);

            modelBuilder.Entity<ManpowerRequirement>()
                .HasIndex(m => m.Priority);

            modelBuilder.Entity<Separation>()
                .HasOne(s => s.Department)
                .WithMany()
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Separation>()
                .HasOne(s => s.Section)
                .WithMany()
                .HasForeignKey(s => s.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Separation>()
                .HasOne(s => s.Designation)
                .WithMany()
                .HasForeignKey(s => s.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Separation>()
                .HasIndex(s => s.EmployeeId);

            modelBuilder.Entity<Separation>()
                .HasIndex(s => s.Status);

            modelBuilder.Entity<Separation>()
                .HasIndex(s => s.ResignDate);
        }
    }
}
