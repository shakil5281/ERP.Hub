using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ERPHub.Data;
using ERPHub.Models;

#pragma warning disable CA1416 // OleDb is Windows-only; this app targets Windows

namespace ERPHub.Services
{
    public class ErpService : IErpService
    {
        private readonly ErpDbContext _context;
        private readonly IConfiguration _configuration;

        public ErpService(ErpDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // --- Products Operations ---
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.OrderByDescending(p => p.Id).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            product.Id = 0; // Reset ID to let SQL Server identity handle it
            product.LastUpdated = DateTime.Now;
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Sku = product.Sku;
                existing.Category = product.Category;
                existing.Price = product.Price;
                existing.Stock = product.Stock;
                existing.Description = product.Description;
                existing.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing != null)
            {
                _context.Products.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Invoices Operations ---
        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddInvoiceAsync(Invoice invoice)
        {
            invoice.Id = 0; // Let SQL Server identity handle it
            foreach (var item in invoice.Items)
            {
                item.Id = 0;
            }
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            var existing = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == invoice.Id);

            if (existing != null)
            {
                existing.InvoiceNumber = invoice.InvoiceNumber;
                existing.ClientName = invoice.ClientName;
                existing.ClientEmail = invoice.ClientEmail;
                existing.Status = invoice.Status;
                existing.DueDate = invoice.DueDate;
                existing.TaxRate = invoice.TaxRate;
                existing.DiscountAmount = invoice.DiscountAmount;

                // Remove existing items from database context
                _context.InvoiceItems.RemoveRange(existing.Items);
                existing.Items.Clear();

                // Add the updated items with reset IDs to avoid constraint issues
                foreach (var item in invoice.Items)
                {
                    item.Id = 0;
                    existing.Items.Add(item);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteInvoiceAsync(int id)
        {
            var existing = await _context.Invoices.FindAsync(id);
            if (existing != null)
            {
                _context.Invoices.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Tasks Operations ---
        public async Task<List<ProjectTask>> GetTasksAsync()
        {
            return await _context.ProjectTasks.OrderByDescending(t => t.Id).ToListAsync();
        }

        public async Task<ProjectTask?> GetTaskByIdAsync(int id)
        {
            return await _context.ProjectTasks.FindAsync(id);
        }

        public async Task AddTaskAsync(ProjectTask task)
        {
            task.Id = 0; // Let SQL Server identity handle it
            task.CreatedAt = DateTime.Now;
            await _context.ProjectTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(ProjectTask task)
        {
            var existing = await _context.ProjectTasks.FindAsync(task.Id);
            if (existing != null)
            {
                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.Status = task.Status;
                existing.Priority = task.Priority;
                existing.Assignee = task.Assignee;
                existing.DueDate = task.DueDate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTaskStatusAsync(int taskId, Models.TaskStatus status)
        {
            var existing = await _context.ProjectTasks.FindAsync(taskId);
            if (existing != null)
            {
                existing.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            var existing = await _context.ProjectTasks.FindAsync(id);
            if (existing != null)
            {
                _context.ProjectTasks.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Companies Operations ---
        public async Task<List<Company>> GetCompaniesAsync()
        {
            return await _context.Companies.OrderByDescending(c => c.Id).ToListAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task AddCompanyAsync(Company company)
        {
            company.Id = 0; // Let SQL Server identity handle it
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            var existing = await _context.Companies.FindAsync(company.Id);
            if (existing != null)
            {
                existing.CompanyNameEn = company.CompanyNameEn;
                existing.CompanyNameBn = company.CompanyNameBn;
                existing.AddressEn = company.AddressEn;
                existing.AddressBn = company.AddressBn;
                existing.Email = company.Email;
                existing.PhoneNumber = company.PhoneNumber;
                existing.Signature = company.Signature;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var existing = await _context.Companies.FindAsync(id);
            if (existing != null)
            {
                _context.Companies.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedDemoCompaniesAsync()
        {
            var demoCompanies = new List<Company>
            {
                new Company
                {
                    CompanyNameEn = "Acme Corporation",
                    CompanyNameBn = "একমি কর্পোরেশন",
                    AddressEn = "123 Industrial Area, Dhaka",
                    AddressBn = "১২৩ শিল্প এলাকা, ঢাকা",
                    Email = "contact@acme.com",
                    PhoneNumber = "+880 1711-000001",
                    Signature = "John Doe"
                },
                new Company
                {
                    CompanyNameEn = "TechNova Solutions",
                    CompanyNameBn = "টেকনোভা সলিউশনস",
                    AddressEn = "45 Tech Park, Chittagong",
                    AddressBn = "৪৫ টেক পার্ক, চট্টগ্রাম",
                    Email = "info@technova.io",
                    PhoneNumber = "+880 1819-000002",
                    Signature = "Jane Smith"
                },
                new Company
                {
                    CompanyNameEn = "GreenSprout Organics",
                    CompanyNameBn = "গ্রীনস্প্রাউট অর্গানিকস",
                    AddressEn = "78 Vertical Farm Road, Sylhet",
                    AddressBn = "৭৮ ভার্টিকাল ফার্ম রোড, সিলেট",
                    Email = "support@greensprout.com",
                    PhoneNumber = "+880 1912-000003",
                    Signature = "Abu Sayed"
                },
                new Company
                {
                    CompanyNameEn = "Apex Global Logistics",
                    CompanyNameBn = "এপেক্স গ্লোবাল লজিস্টিকস",
                    AddressEn = "12 Ocean Terminal, Mongla",
                    AddressBn = "১২ ওশান টার্মিনাল, মংলা",
                    Email = "logistics@apex.com",
                    PhoneNumber = "+880 1613-000004",
                    Signature = "Robert Chen"
                },
                new Company
                {
                    CompanyNameEn = "Skyline Real Estate",
                    CompanyNameBn = "স্কাইলাইন রিয়েল এস্টেট",
                    AddressEn = "56 Commercial Avenue, Gulshan, Dhaka",
                    AddressBn = "৫৬ কমার্শিয়াল এভিনিউ, গুলশান, ঢাকা",
                    Email = "sales@skylinere.com",
                    PhoneNumber = "+880 1515-000005",
                    Signature = "Sarah Khan"
                }
            };

            foreach (var company in demoCompanies)
            {
                var exists = await _context.Companies.AnyAsync(c => c.Email == company.Email || c.CompanyNameEn == company.CompanyNameEn);
                if (!exists)
                {
                    await _context.Companies.AddAsync(company);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDemoCompaniesAsync()
        {
            var demoEmails = new List<string>
            {
                "contact@acme.com",
                "info@technova.io",
                "support@greensprout.com",
                "logistics@apex.com",
                "sales@skylinere.com"
            };

            var toRemove = await _context.Companies
                .Where(c => demoEmails.Contains(c.Email))
                .ToListAsync();

            if (toRemove.Any())
            {
                _context.Companies.RemoveRange(toRemove);
                await _context.SaveChangesAsync();
            }
        }

        // --- Dashboard / Statistics ---
        public async Task<decimal> GetTotalRevenueAsync()
        {
            var paidInvoices = await _context.Invoices
                .Include(i => i.Items)
                .Where(i => i.Status == InvoiceStatus.Paid)
                .ToListAsync();

            return paidInvoices.Sum(i => i.GrandTotal);
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _context.Products.SumAsync(p => p.Stock);
        }

        public async Task<int> GetPendingTasksCountAsync()
        {
            return await _context.ProjectTasks.CountAsync(t => t.Status != Models.TaskStatus.Done);
        }

        public async Task<int> GetPaidInvoicesCountAsync()
        {
            return await _context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Paid);
        }

        // --- Organogram Tree ---
        public async Task<List<CompanyNodeDto>> GetOrganogramTreeAsync()
        {
            var companies = await _context.Companies.ToListAsync();
            var departments = await _context.Departments.ToListAsync();
            var sections = await _context.Sections.ToListAsync();
            var designations = await _context.Designations.ToListAsync();
            var lines = await _context.Lines.ToListAsync();

            var tree = new List<CompanyNodeDto>();

            // For now, attach all departments to all companies, 
            // since there is no direct Company-Department relationship in the DB.
            foreach (var company in companies)
            {
                var companyNode = new CompanyNodeDto
                {
                    Id = company.Id,
                    NameEn = company.CompanyNameEn,
                    NameBn = company.CompanyNameBn,
                    Departments = departments.Select(d => new DepartmentNodeDto
                    {
                        Id = d.Id,
                        NameEn = d.NameEn,
                        NameBn = d.NameBn,
                        Sections = sections.Where(s => s.DepartmentId == d.Id).Select(s => new SectionNodeDto
                        {
                            Id = s.Id,
                            NameEn = s.NameEn,
                            NameBn = s.NameBn,
                            Designations = designations.Where(deg => deg.SectionId == s.Id).Select(deg => new DesignationNodeDto
                            {
                                Id = deg.Id,
                                NameEn = deg.NameEn,
                                NameBn = deg.NameBn
                            }).ToList(),
                            Lines = lines.Where(l => l.SectionId == s.Id).Select(l => new LineNodeDto
                            {
                                Id = l.Id,
                                NameEn = l.NameEn,
                                NameBn = l.NameBn
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };
                tree.Add(companyNode);
            }

            return tree;
        }

        // --- Shifts Operations ---
        public async Task<List<Shift>> GetShiftsAsync()
        {
            return await _context.Shifts.OrderBy(s => s.ShiftName).ToListAsync();
        }

        public async Task<Shift?> GetShiftByIdAsync(int id)
        {
            return await _context.Shifts.FindAsync(id);
        }

        public async Task AddShiftAsync(Shift shift)
        {
            shift.Id = 0;
            await _context.Shifts.AddAsync(shift);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateShiftAsync(Shift shift)
        {
            var existing = await _context.Shifts.FindAsync(shift.Id);
            if (existing != null)
            {
                existing.ShiftName = shift.ShiftName;
                existing.InTime    = shift.InTime;
                existing.OutTime   = shift.OutTime;
                existing.LateTime  = shift.LateTime;
                existing.OffDay    = shift.OffDay;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteShiftAsync(int id)
        {
            var existing = await _context.Shifts.FindAsync(id);
            if (existing != null)
            {
                _context.Shifts.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Employees Operations ---
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Section)
                .Include(e => e.Designation)
                .Include(e => e.Line)
                .Include(e => e.Shift)
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Section)
                .Include(e => e.Designation)
                .Include(e => e.Line)
                .Include(e => e.Shift)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            employee.Id = 0;
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var existing = await _context.Employees.FindAsync(employee.Id);
            if (existing != null)
            {
                existing.EmployeeId = employee.EmployeeId;
                existing.EmployeeName = employee.EmployeeName;
                existing.FatherName = employee.FatherName;
                existing.MotherName = employee.MotherName;
                existing.Address = employee.Address;
                existing.NID = employee.NID;
                existing.MobileNo = employee.MobileNo;
                existing.Email = employee.Email;
                existing.DepartmentId = employee.DepartmentId;
                existing.SectionId = employee.SectionId;
                existing.DesignationId = employee.DesignationId;
                existing.LineId = employee.LineId;
                existing.ShiftId = employee.ShiftId;
                existing.JoiningDate = employee.JoiningDate;
                existing.BasicSalary = employee.BasicSalary;
                existing.IsActive = employee.IsActive;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var existing = await _context.Employees.FindAsync(id);
            if (existing != null)
            {
                _context.Employees.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Lookups Operations ---
        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments.OrderBy(d => d.NameEn).ToListAsync();
        }

        public async Task<List<Section>> GetSectionsAsync()
        {
            return await _context.Sections.OrderBy(s => s.NameEn).ToListAsync();
        }

        public async Task<List<Designation>> GetDesignationsAsync()
        {
            return await _context.Designations.OrderBy(d => d.NameEn).ToListAsync();
        }

        public async Task<List<Line>> GetLinesAsync()
        {
            return await _context.Lines.OrderBy(l => l.NameEn).ToListAsync();
        }

        // --- Punch Records (ZK Device) ---
        public async Task<List<PunchRecord>> GetPunchRecordsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? employeeId = null)
        {
            var query = _context.PunchRecords.AsNoTracking().AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.LogDateTime >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.LogDateTime <= toDate.Value);

            if (!string.IsNullOrEmpty(employeeId))
                query = query.Where(p => p.EmployeeId == employeeId);

            return await query.OrderByDescending(p => p.LogDateTime).ToListAsync();
        }

        public async Task<PunchRecord?> GetPunchRecordByIdAsync(int id)
        {
            return await _context.PunchRecords.FindAsync(id);
        }

        public async Task<int> ImportPunchRecordsFromMdbAsync(string mdbFilePath, DateTime? syncDate = null)
        {
            return await ImportPunchRecordsFromMdbAsync(mdbFilePath, syncDate, syncDate);
        }

        public async Task<int> ImportPunchRecordsFromMdbAsync(string mdbFilePath, DateTime? fromDate, DateTime? toDate)
        {
            var connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={mdbFilePath};";
            const int batchSize = 500;

            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var schema = connection.GetSchema("Tables");
            var tableNames = new List<string>();
            foreach (System.Data.DataRow row in schema.Rows)
            {
                var name = row["TABLE_NAME"]?.ToString();
                if (!string.IsNullOrEmpty(name))
                    tableNames.Add(name);
            }

            var tableName = tableNames.FirstOrDefault(t =>
                t.Equals("CHECKINOUT", StringComparison.OrdinalIgnoreCase));

            if (tableName == null && tableNames.Count > 0)
                tableName = tableNames[0];

            if (tableName == null)
                throw new Exception("No tables found in MDB file.");

            var columns = new List<string>();
            var colSchema = connection.GetSchema("Columns", new[] { null, null, tableName });
            foreach (System.Data.DataRow row in colSchema.Rows)
                columns.Add(row["COLUMN_NAME"]?.ToString() ?? "");

            var hasSensorCol = columns.Any(c => c.Equals("SensorID", StringComparison.OrdinalIgnoreCase));

            var selectCols = "a.USERID, a.CHECKTIME";
            if (hasSensorCol) selectCols += ", a.SensorID";

            var query = $"SELECT {selectCols} FROM [{tableName}] a";

            var existingKeys = new HashSet<string>(
                await _context.PunchRecords
                    .Select(p => p.EmployeeId + "|" + p.LogDateTime.Ticks)
                    .ToListAsync(),
                StringComparer.Ordinal);

            var batch = new List<PunchRecord>(batchSize);
            int importedCount = 0;

            using var command = new OleDbCommand(query, connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var userId = reader["USERID"]?.ToString()?.Trim() ?? string.Empty;
                    var logTime = Convert.ToDateTime(reader["CHECKTIME"]);
                    var sensorId = hasSensorCol ? reader["SensorID"]?.ToString()?.Trim() ?? string.Empty : string.Empty;

                    if (fromDate.HasValue && logTime.Date < fromDate.Value.Date)
                        continue;
                    if (toDate.HasValue && logTime.Date > toDate.Value.Date)
                        continue;

                    var dedupKey = userId + "|" + logTime.Ticks;
                    if (!existingKeys.Add(dedupKey))
                        continue;

                    batch.Add(new PunchRecord
                    {
                        EmployeeId = userId,
                        LogDateTime = logTime,
                        DeviceId = sensorId
                    });

                    if (batch.Count >= batchSize)
                    {
                        _context.ChangeTracker.AutoDetectChangesEnabled = false;
                        await _context.PunchRecords.AddRangeAsync(batch);
                        await _context.SaveChangesAsync();
                        _context.ChangeTracker.AutoDetectChangesEnabled = true;
                        _context.ChangeTracker.Clear();
                        importedCount += batch.Count;
                        batch.Clear();
                    }
                }
            }

            if (batch.Count > 0)
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = false;
                await _context.PunchRecords.AddRangeAsync(batch);
                await _context.SaveChangesAsync();
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
                _context.ChangeTracker.Clear();
                importedCount += batch.Count;
            }

            return importedCount;
        }

        public async Task<int> SyncPunchRecordsFromZKDeviceAsync(DateTime? syncDate = null)
        {
            return await SyncPunchRecordsFromZKDeviceAsync(null, syncDate);
        }

        public async Task<int> SyncPunchRecordsFromZKDeviceAsync(DateTime? fromDate, DateTime? toDate)
        {
            var useSqlServer = _configuration.GetValue<bool>("ZKDevice:UseSqlServer");

            if (useSqlServer)
            {
                var sqlConnStr = _configuration["ZKDevice:SqlConnection"];
                if (string.IsNullOrEmpty(sqlConnStr))
                    return 0;

                return await ImportPunchRecordsFromSqlServerAsync(sqlConnStr, fromDate, toDate);
            }

            var mdbPath = _configuration["ZKDevice:MdbFilePath"];
            if (string.IsNullOrEmpty(mdbPath) || !File.Exists(mdbPath))
                return 0;

            return await ImportPunchRecordsFromMdbAsync(mdbPath, fromDate, toDate);
        }

        public async Task<int> ImportPunchRecordsFromSqlServerAsync(string connectionString, DateTime? syncDate = null)
        {
            return await ImportPunchRecordsFromSqlServerAsync(connectionString, syncDate, syncDate);
        }

        public async Task<int> ImportPunchRecordsFromSqlServerAsync(string connectionString, DateTime? fromDate, DateTime? toDate)
        {
            const int batchSize = 500;

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT USERID, CHECKTIME, SensorID FROM CHECKINOUT WHERE 1=1";

            if (fromDate.HasValue)
                query += " AND CAST(CHECKTIME AS DATE) >= @fromDate";
            if (toDate.HasValue)
                query += " AND CAST(CHECKTIME AS DATE) <= @toDate";

            query += " ORDER BY CHECKTIME";

            var existingKeys = new HashSet<string>(
                await _context.PunchRecords
                    .Select(p => p.EmployeeId + "|" + p.LogDateTime.Ticks)
                    .ToListAsync(),
                StringComparer.Ordinal);

            var batch = new List<PunchRecord>(batchSize);
            int importedCount = 0;

            using (var command = new SqlCommand(query, connection))
            {
                if (fromDate.HasValue)
                    command.Parameters.AddWithValue("@fromDate", fromDate.Value.Date);
                if (toDate.HasValue)
                    command.Parameters.AddWithValue("@toDate", toDate.Value.Date);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var userId = reader["USERID"]?.ToString()?.Trim() ?? string.Empty;
                        var logTime = Convert.ToDateTime(reader["CHECKTIME"]);
                        var sensorId = reader["SensorID"]?.ToString()?.Trim() ?? string.Empty;

                        var dedupKey = userId + "|" + logTime.Ticks;
                        if (!existingKeys.Add(dedupKey))
                            continue;

                        batch.Add(new PunchRecord
                        {
                            EmployeeId = userId,
                            LogDateTime = logTime,
                            DeviceId = sensorId
                        });
                    }
                }
            }

            if (batch.Count > 0)
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = false;

                for (int i = 0; i < batch.Count; i += batchSize)
                {
                    var chunk = batch.Skip(i).Take(batchSize).ToList();
                    await _context.PunchRecords.AddRangeAsync(chunk);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                    importedCount += chunk.Count;
                }

                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }

            return importedCount;
        }
    }
}

#pragma warning restore CA1416
