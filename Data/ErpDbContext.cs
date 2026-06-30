using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
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
        public DbSet<EmployeeSalaryAssignment> EmployeeSalaryAssignments => Set<EmployeeSalaryAssignment>();
        public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
        public DbSet<PayrollLine> PayrollLines => Set<PayrollLine>();
        public DbSet<SalaryAdvance> SalaryAdvances => Set<SalaryAdvance>();
        public DbSet<EmployeeLoan> EmployeeLoans => Set<EmployeeLoan>();
        public DbSet<SalaryIncrement> SalaryIncrements => Set<SalaryIncrement>();
        public DbSet<SalaryHistory> SalaryHistories => Set<SalaryHistory>();
        public DbSet<NightBillEntry> NightBillEntries => Set<NightBillEntry>();
        public DbSet<HolidayBillEntry> HolidayBillEntries => Set<HolidayBillEntry>();
        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure RBAC Composite Keys
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

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

            // Configure Section <-> Department relationship (single relationship with both navigations)
            modelBuilder.Entity<Section>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Sections)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Designation <-> Section relationship (single relationship with both navigations)
            modelBuilder.Entity<Designation>()
                .HasOne(d => d.Section)
                .WithMany(s => s.Designations)
                .HasForeignKey(d => d.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Line <-> Section relationship (single relationship with both navigations)
            modelBuilder.Entity<Line>()
                .HasOne(l => l.Section)
                .WithMany(s => s.Lines)
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
                .HasOne(e => e.BusinessGroup)
                .WithMany()
                .HasForeignKey(e => e.BusinessGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.BusinessGroup)
                .WithMany()
                .HasForeignKey(c => c.BusinessGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeId)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.CompanyId);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.DepartmentId);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => new { e.Status, e.JoiningDate, e.SeparationDate });

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.PunchNumber)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Status);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.SeparationDate);

            modelBuilder.Entity<Employee>()
                .Property(e => e.SeparationType)
                .HasMaxLength(30);

            modelBuilder.Entity<Employee>()
                .Property(e => e.SeparationReason)
                .HasMaxLength(1000);

            modelBuilder.Entity<Employee>()
                .Property(e => e.SeparationRemarks)
                .HasMaxLength(500);

            modelBuilder.Entity<Employee>()
                .Property(e => e.SeparationApprovedBy)
                .HasMaxLength(100);

            modelBuilder.Entity<PunchRecord>()
                .HasIndex(p => new { p.UserPunchId, p.LogDateTime })
                .IsUnique();

            modelBuilder.Entity<PunchRecord>()
                .HasIndex(p => new { p.PunchNumber, p.LogDateTime });

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
                .HasIndex(a => new { a.AttendanceDate, a.EmployeeId });

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

            modelBuilder.Entity<DailySalaryRecord>()
                .HasIndex(d => new { d.EmployeeId, d.SalaryDate })
                .IsUnique();

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
                .HasOne(s => s.Employee)
                .WithMany(e => e.Separations)
                .HasForeignKey(s => s.EmployeeRefId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasIndex(s => s.EmployeeRefId);

            modelBuilder.Entity<Separation>()
                .HasIndex(s => s.Status);

            modelBuilder.Entity<Separation>()
                .HasIndex(s => s.SeparationDate);

            modelBuilder.Entity<Separation>()
                .HasIndex(s => new { s.SeparationType, s.SeparationDate });

            modelBuilder.Entity<Separation>()
                .HasIndex(s => new { s.CompanyId, s.SeparationDate });

            ConfigurePayrollEntities(modelBuilder);

            // LeaveApplication relationships
            modelBuilder.Entity<LeaveApplication>()
                .HasOne(la => la.Department)
                .WithMany()
                .HasForeignKey(la => la.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveApplication>()
                .HasOne(la => la.Designation)
                .WithMany()
                .HasForeignKey(la => la.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveApplication>()
                .HasOne(la => la.LeaveTypeNav)
                .WithMany()
                .HasForeignKey(la => la.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveApplication>()
                .HasIndex(la => new { la.EmployeeId, la.LeaveDate, la.Status });

            modelBuilder.Entity<LeaveApplication>()
                .HasIndex(la => la.EmployeeId);

            modelBuilder.Entity<LeaveApplication>()
                .HasIndex(la => la.Status);

            modelBuilder.Entity<LeaveApplication>()
                .HasIndex(la => la.LeaveDate);
        }

        private static void ConfigurePayrollEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.HouseRent).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Employee>()
                .Property(e => e.MedicalAllowance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Employee>()
                .Property(e => e.TransportAllowance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Employee>()
                .Property(e => e.FoodAllowance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Employee>()
                .Property(e => e.SpecialAllowance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Employee>()
                .Property(e => e.AttendanceBonus).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Employee>()
                .Property(e => e.ProductionBonus).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.DailyGross).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.NightBillPay).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.HolidayBillPay).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.AbsentDeduction).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.LateDeduction).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.LwopDeduction).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.AdvanceDeduction).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<DailySalaryRecord>()
                .Property(d => d.LoanDeduction).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<EmployeeSalaryAssignment>()
                .HasOne(a => a.Employee).WithMany().HasForeignKey(a => a.EmployeeRefId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EmployeeSalaryAssignment>()
                .HasIndex(a => new { a.EmployeeRefId, a.EffectiveFrom });

            modelBuilder.Entity<PayrollRun>()
                .HasOne(p => p.Company).WithMany().HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PayrollRun>()
                .HasIndex(p => new { p.CompanyId, p.PayrollYear, p.PayrollMonth }).IsUnique();

            modelBuilder.Entity<PayrollLine>()
                .HasOne(l => l.PayrollRun).WithMany(r => r.Lines).HasForeignKey(l => l.PayrollRunId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PayrollLine>()
                .HasIndex(l => l.PayrollRunId);

            modelBuilder.Entity<SalaryAdvance>()
                .HasOne(a => a.Employee).WithMany().HasForeignKey(a => a.EmployeeRefId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EmployeeLoan>()
                .HasOne(l => l.Employee).WithMany().HasForeignKey(l => l.EmployeeRefId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SalaryIncrement>()
                .HasOne(i => i.Employee).WithMany().HasForeignKey(i => i.EmployeeRefId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SalaryIncrement>()
                .HasIndex(i => i.EmployeeRefId);

            foreach (var entity in new[] { typeof(PayrollRun), typeof(PayrollLine), typeof(EmployeeSalaryAssignment),
                typeof(SalaryAdvance), typeof(EmployeeLoan), typeof(SalaryIncrement), typeof(SalaryHistory),
                typeof(NightBillEntry), typeof(HolidayBillEntry) })
            {
                foreach (var prop in entity.GetProperties().Where(p => p.PropertyType == typeof(decimal)))
                    modelBuilder.Entity(entity).Property(prop.Name).HasColumnType("decimal(18,2)");
            }
        }
    }
}
