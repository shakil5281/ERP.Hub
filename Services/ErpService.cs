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

        // --- Business Groups Operations ---
        public async Task<List<BusinessGroup>> GetBusinessGroupsAsync()
        {
            return await _context.BusinessGroups.OrderByDescending(g => g.Id).ToListAsync();
        }

        public async Task<BusinessGroup?> GetBusinessGroupByIdAsync(int id)
        {
            return await _context.BusinessGroups.FindAsync(id);
        }

        public async Task AddBusinessGroupAsync(BusinessGroup group)
        {
            group.Id = 0;
            await _context.BusinessGroups.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBusinessGroupAsync(BusinessGroup group)
        {
            var existing = await _context.BusinessGroups.FindAsync(group.Id);
            if (existing != null)
            {
                existing.GroupName = group.GroupName;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBusinessGroupAsync(int id)
        {
            var existing = await _context.BusinessGroups.FindAsync(id);
            if (existing != null)
            {
                _context.BusinessGroups.Remove(existing);
                await _context.SaveChangesAsync();
            }
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
                existing.NID = employee.NID;
                existing.MobileNo = employee.MobileNo;
                existing.Email = employee.Email;
                existing.DateOfBirth = employee.DateOfBirth;
                existing.Gender = employee.Gender;
                existing.SpouseName = employee.SpouseName;
                existing.ChildrenCount = employee.ChildrenCount;
                existing.AccountType = employee.AccountType;
                existing.AccountNumber = employee.AccountNumber;
                existing.PresentVillage = employee.PresentVillage;
                existing.PresentPostOffice = employee.PresentPostOffice;
                existing.PresentDivisionId = employee.PresentDivisionId;
                existing.PresentDistrictId = employee.PresentDistrictId;
                existing.PresentUpazilaId = employee.PresentUpazilaId;
                existing.PresentPostalCode = employee.PresentPostalCode;
                existing.PermanentVillage = employee.PermanentVillage;
                existing.PermanentPostOffice = employee.PermanentPostOffice;
                existing.PermanentDivisionId = employee.PermanentDivisionId;
                existing.PermanentDistrictId = employee.PermanentDistrictId;
                existing.PermanentUpazilaId = employee.PermanentUpazilaId;
                existing.PermanentPostalCode = employee.PermanentPostalCode;
                existing.DepartmentId = employee.DepartmentId;
                existing.SectionId = employee.SectionId;
                existing.DesignationId = employee.DesignationId;
                existing.LineId = employee.LineId;
                existing.ShiftId = employee.ShiftId;
                existing.CompanyId = employee.CompanyId;
                existing.JoiningDate = employee.JoiningDate;
                existing.BasicSalary = employee.BasicSalary;
                existing.GrossSalary = employee.GrossSalary;
                existing.PhotoBase64 = employee.PhotoBase64;
                existing.SignatureBase64 = employee.SignatureBase64;
                existing.EmployeeStatus = employee.EmployeeStatus;
                existing.OverTimeStatus = employee.OverTimeStatus;
                existing.EmployeeType = employee.EmployeeType;
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
            {
                var punchNumber = await _context.Employees
                    .Where(e => e.EmployeeId == employeeId)
                    .Select(e => (int?)e.PunchNumber)
                    .FirstOrDefaultAsync();
                if (punchNumber.HasValue)
                    query = query.Where(p => p.PunchNumber == punchNumber.Value);
            }

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
            var hasUserInfo = tableNames.Any(t => t.Equals("USERINFO", StringComparison.OrdinalIgnoreCase));

            var selectCols = "a.USERID, a.CHECKTIME";
            if (hasSensorCol) selectCols += ", a.SensorID";
            if (hasUserInfo) selectCols += ", u.BADGENUMBER";

            var query = hasUserInfo
                ? $"SELECT {selectCols} FROM [{tableName}] a LEFT JOIN [USERINFO] u ON a.USERID = u.USERID"
                : $"SELECT {selectCols} FROM [{tableName}] a";

            var existingKeys = await LoadExistingPunchDedupKeysAsync(fromDate, toDate);

            var batch = new List<PunchRecord>(batchSize);
            int importedCount = 0;

            using var command = new OleDbCommand(query, connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var userPunchId = ParseZkUserId(reader["USERID"]);
                    if (userPunchId <= 0)
                        continue;

                    var logTime = Convert.ToDateTime(reader["CHECKTIME"]);
                    var sensorId = hasSensorCol ? reader["SensorID"]?.ToString()?.Trim() ?? string.Empty : string.Empty;
                    var punchNumber = hasUserInfo ? ParseZkBadgeNumber(reader["BADGENUMBER"]) : 0;

                    if (fromDate.HasValue && logTime.Date < fromDate.Value.Date)
                        continue;
                    if (toDate.HasValue && logTime.Date > toDate.Value.Date)
                        continue;

                    var dedupKey = MakePunchDedupKey(userPunchId, logTime);
                    if (!existingKeys.Add(dedupKey))
                        continue;

                    batch.Add(new PunchRecord
                    {
                        UserPunchId = userPunchId,
                        PunchNumber = punchNumber,
                        LogDateTime = logTime,
                        DeviceId = sensorId
                    });

                    if (batch.Count >= batchSize)
                    {
                        importedCount += await SavePunchBatchAsync(batch);
                    }
                }
            }

            if (batch.Count > 0)
                importedCount += await SavePunchBatchAsync(batch);

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
            var commandTimeout = _configuration.GetValue<int?>("ZKDevice:CommandTimeoutSeconds") ?? 300;
            var previousTimeout = _context.Database.GetCommandTimeout();
            _context.Database.SetCommandTimeout(commandTimeout);

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                await BackfillPunchNumbersFromZkAsync(connection, commandTimeout);

                var query = @"SELECT c.USERID, u.BADGENUMBER, c.CHECKTIME, c.SensorID
                              FROM CHECKINOUT c
                              LEFT JOIN USERINFO u ON c.USERID = u.USERID
                              WHERE 1=1";

                if (fromDate.HasValue)
                    query += " AND c.CHECKTIME >= @fromDate";
                if (toDate.HasValue)
                    query += " AND c.CHECKTIME < @toDateEnd";

                query += " ORDER BY c.CHECKTIME";

                var existingKeys = await LoadExistingPunchDedupKeysAsync(fromDate, toDate);

                var batch = new List<PunchRecord>(batchSize);
                int importedCount = 0;

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandTimeout = commandTimeout;
                    if (fromDate.HasValue)
                        command.Parameters.AddWithValue("@fromDate", fromDate.Value.Date);
                    if (toDate.HasValue)
                        command.Parameters.AddWithValue("@toDateEnd", toDate.Value.Date.AddDays(1));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var userPunchId = ParseZkUserId(reader["USERID"]);
                            if (userPunchId <= 0)
                                continue;

                            var punchNumber = ParseZkBadgeNumber(reader["BADGENUMBER"]);
                            var logTime = Convert.ToDateTime(reader["CHECKTIME"]);
                            var sensorId = reader["SensorID"]?.ToString()?.Trim() ?? string.Empty;

                            var dedupKey = MakePunchDedupKey(userPunchId, logTime);
                            if (!existingKeys.Add(dedupKey))
                                continue;

                            batch.Add(new PunchRecord
                            {
                                UserPunchId = userPunchId,
                                PunchNumber = punchNumber,
                                LogDateTime = logTime,
                                DeviceId = sensorId
                            });

                            if (batch.Count >= batchSize)
                            {
                                importedCount += await SavePunchBatchAsync(batch);
                            }
                        }
                    }
                }

                if (batch.Count > 0)
                    importedCount += await SavePunchBatchAsync(batch);

                return importedCount;
            }
            finally
            {
                _context.Database.SetCommandTimeout(previousTimeout);
            }
        }

        private async Task<HashSet<string>> LoadExistingPunchDedupKeysAsync(DateTime? fromDate, DateTime? toDate)
        {
            var existingQuery = _context.PunchRecords.AsNoTracking();
            if (fromDate.HasValue)
                existingQuery = existingQuery.Where(p => p.LogDateTime >= fromDate.Value.Date);
            if (toDate.HasValue)
                existingQuery = existingQuery.Where(p => p.LogDateTime < toDate.Value.Date.AddDays(1));

            return new HashSet<string>(
                await existingQuery
                    .Select(p => p.UserPunchId + "|" + p.LogDateTime.Ticks)
                    .ToListAsync(),
                StringComparer.Ordinal);
        }

        private async Task<int> SavePunchBatchAsync(List<PunchRecord> batch)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            await _context.PunchRecords.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            _context.ChangeTracker.Clear();
            var count = batch.Count;
            batch.Clear();
            return count;
        }

        private async Task BackfillPunchNumbersFromZkAsync(SqlConnection zkConnection, int commandTimeout)
        {
            var needsBackfill = await _context.PunchRecords.AnyAsync(p => p.PunchNumber == 0 && p.UserPunchId > 0);
            if (!needsBackfill)
                return;

            var badgeMap = new Dictionary<int, int>();
            using (var command = new SqlCommand("SELECT USERID, BADGENUMBER FROM USERINFO", zkConnection))
            {
                command.CommandTimeout = commandTimeout;
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var userId = ParseZkUserId(reader["USERID"]);
                    var badge = ParseZkBadgeNumber(reader["BADGENUMBER"]);
                    if (userId > 0 && badge > 0)
                        badgeMap[userId] = badge;
                }
            }

            if (badgeMap.Count == 0)
                return;

            var records = await _context.PunchRecords
                .Where(p => p.PunchNumber == 0 && p.UserPunchId > 0)
                .ToListAsync();

            foreach (var record in records)
            {
                if (badgeMap.TryGetValue(record.UserPunchId, out var badge))
                    record.PunchNumber = badge;
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        private static string MakePunchDedupKey(int userPunchId, DateTime logTime)
            => userPunchId + "|" + logTime.Ticks;

        private static int ParseZkUserId(object? value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            return int.TryParse(value.ToString()?.Trim(), out var id) ? id : 0;
        }

        private static int ParseZkBadgeNumber(object? value)
        {
            if (value == null || value == DBNull.Value)
                return 0;
            return int.TryParse(value.ToString()?.Trim(), out var badge) ? badge : 0;
        }

        // --- Manual Punch Logs Operations ---
        public async Task<List<ManualPunchLog>> GetManualPunchLogsAsync()
        {
            return await _context.ManualPunchLogs.OrderByDescending(m => m.PunchDate).ThenByDescending(m => m.Id).ToListAsync();
        }

        public async Task AddManualPunchLogAsync(ManualPunchLog log)
        {
            log.Id = 0;
            await _context.ManualPunchLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task ApproveManualPunchLogAsync(int id)
        {
            var log = await _context.ManualPunchLogs.FindAsync(id);
            if (log != null)
            {
                log.Status = "Approved";

                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == log.EmployeeId);

                var punchTime = log.PunchDate.Date.Add(log.PunchTime);
                var punchNumber = employee?.PunchNumber ?? 0;
                var punchRecordExists = await _context.PunchRecords
                    .AnyAsync(p => p.PunchNumber == punchNumber && p.LogDateTime == punchTime);
                if (!punchRecordExists)
                {
                    await _context.PunchRecords.AddAsync(new PunchRecord
                    {
                        UserPunchId = 0,
                        PunchNumber = punchNumber,
                        LogDateTime = punchTime,
                        DeviceId = "Manual"
                    });
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteManualPunchLogAsync(int id)
        {
            var log = await _context.ManualPunchLogs.FindAsync(id);
            if (log != null)
            {
                _context.ManualPunchLogs.Remove(log);
                await _context.SaveChangesAsync();
            }
        }

        // --- Overtime Deductions Operations ---
        public async Task<List<OvertimeDeduction>> GetOvertimeDeductionsAsync()
        {
            return await _context.OvertimeDeductions.OrderByDescending(o => o.DeductionDate).ToListAsync();
        }

        public async Task AddOvertimeDeductionAsync(OvertimeDeduction deduction)
        {
            deduction.Id = 0;
            await _context.OvertimeDeductions.AddAsync(deduction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOvertimeDeductionAsync(OvertimeDeduction deduction)
        {
            var existing = await _context.OvertimeDeductions.FindAsync(deduction.Id);
            if (existing != null)
            {
                existing.DeductedHours = deduction.DeductedHours;
                existing.Reason = deduction.Reason;
                existing.Status = deduction.Status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApproveOvertimeDeductionAsync(int id)
        {
            var deduction = await _context.OvertimeDeductions.FindAsync(id);
            if (deduction != null)
            {
                deduction.Status = "Approved";
                await _context.SaveChangesAsync();
            }
        }

        // --- Daily Salary Records Operations ---
        public async Task<List<DailySalaryRecord>> GetDailySalaryRecordsAsync(DateTime date)
        {
            return await _context.DailySalaryRecords
                .Where(d => d.SalaryDate == date.Date)
                .ToListAsync();
        }

        public async Task CalculateDailySalariesAsync(DateTime date)
        {
            var employees = await _context.Employees.Where(e => e.EmployeeStatus == "Regular").ToListAsync();
            var attendanceRecords = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate == date.Date)
                .ToListAsync();

            var existingRecords = await _context.DailySalaryRecords
                .Where(d => d.SalaryDate == date.Date)
                .ToDictionaryAsync(d => d.EmployeeId);

            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            foreach (var emp in employees)
            {
                var att = attendanceRecords.FirstOrDefault(a => a.EmployeeId == emp.EmployeeId);

                decimal dailyBasic = Math.Round(emp.BasicSalary / daysInMonth, 2);
                double otHours = att != null ? Math.Round(att.OvertimeMinutes / 60.0, 1) : 0.0;
                decimal otPay = Math.Round((decimal)otHours * (dailyBasic / 8.0m) * 1.5m, 2);

                decimal allowances = 0;
                decimal deductions = 0;

                if (att != null)
                {
                    if (att.AttendanceStatus == "Absent")
                    {
                        deductions = dailyBasic;
                    }
                    else
                    {
                        allowances = 10.00m;
                        if (att.AttendanceStatus == "Late")
                        {
                            deductions = 10.00m;
                        }
                    }
                }
                else
                {
                    deductions = dailyBasic;
                }

                decimal netPay = dailyBasic + otPay + allowances - deductions;
                if (netPay < 0) netPay = 0;

                if (existingRecords.TryGetValue(emp.EmployeeId, out var existing))
                {
                    if (existing.Status != "Approved")
                    {
                        existing.DailyBasic = dailyBasic;
                        existing.OtHours = otHours;
                        existing.OtPay = otPay;
                        existing.Allowances = allowances;
                        existing.Deductions = deductions;
                        existing.NetPay = netPay;
                    }
                }
                else
                {
                    await _context.DailySalaryRecords.AddAsync(new DailySalaryRecord
                    {
                        EmployeeId = emp.EmployeeId,
                        SalaryDate = date.Date,
                        DailyBasic = dailyBasic,
                        OtHours = otHours,
                        OtPay = otPay,
                        Allowances = allowances,
                        Deductions = deductions,
                        NetPay = netPay,
                        Status = "Processed"
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateDailySalaryRecordAsync(DailySalaryRecord record)
        {
            var existing = await _context.DailySalaryRecords.FindAsync(record.Id);
            if (existing != null)
            {
                existing.DailyBasic = record.DailyBasic;
                existing.OtHours = record.OtHours;
                existing.OtPay = record.OtPay;
                existing.Allowances = record.Allowances;
                existing.Deductions = record.Deductions;
                existing.NetPay = record.NetPay;
                existing.Status = record.Status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApproveDailySalaryRecordAsync(int id)
        {
            var record = await _context.DailySalaryRecords.FindAsync(id);
            if (record != null)
            {
                record.Status = "Approved";
                await _context.SaveChangesAsync();
            }
        }

        // Manpower
        public async Task<List<Manpower>> GetManpowersAsync()
        {
            return await _context.Manpowers
                .Include(m => m.Department)
                .Include(m => m.Section)
                .Include(m => m.Designation)
                .OrderBy(m => m.Department != null ? m.Department.NameEn : "")
                .ThenBy(m => m.Section != null ? m.Section.NameEn : "")
                .ThenBy(m => m.Designation != null ? m.Designation.NameEn : "")
                .ToListAsync();
        }

        public async Task<Manpower?> GetManpowerByIdAsync(int id)
        {
            return await _context.Manpowers
                .Include(m => m.Department)
                .Include(m => m.Section)
                .Include(m => m.Designation)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddManpowerAsync(Manpower manpower)
        {
            manpower.Id = 0;
            manpower.LastUpdated = DateTime.Now;
            await _context.Manpowers.AddAsync(manpower);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateManpowerAsync(Manpower manpower)
        {
            var existing = await _context.Manpowers.FindAsync(manpower.Id);
            if (existing != null)
            {
                existing.DepartmentId = manpower.DepartmentId;
                existing.SectionId = manpower.SectionId;
                existing.DesignationId = manpower.DesignationId;
                existing.TargetCapacity = manpower.TargetCapacity;
                existing.CurrentHeadcount = manpower.CurrentHeadcount;
                existing.Vacancies = manpower.Vacancies;
                existing.Remarks = manpower.Remarks;
                existing.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteManpowerAsync(int id)
        {
            var existing = await _context.Manpowers.FindAsync(id);
            if (existing != null)
            {
                _context.Manpowers.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RecalculateManpowerAsync()
        {
            var employees = await _context.Employees.Where(e => e.EmployeeStatus == "Regular").ToListAsync();
            var manpowers = await _context.Manpowers.ToListAsync();

            foreach (var mp in manpowers)
            {
                var count = employees.Count(e =>
                    e.DepartmentId == mp.DepartmentId &&
                    (mp.SectionId == null || e.SectionId == mp.SectionId) &&
                    (mp.DesignationId == null || e.DesignationId == mp.DesignationId));

                mp.CurrentHeadcount = count;
                mp.Vacancies = Math.Max(0, mp.TargetCapacity - count);
                mp.LastUpdated = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        // Manpower Requirements
        public async Task<List<ManpowerRequirement>> GetManpowerRequirementsAsync()
        {
            return await _context.ManpowerRequirements
                .Include(m => m.Department)
                .Include(m => m.Section)
                .Include(m => m.Designation)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<ManpowerRequirement?> GetManpowerRequirementByIdAsync(int id)
        {
            return await _context.ManpowerRequirements
                .Include(m => m.Department)
                .Include(m => m.Section)
                .Include(m => m.Designation)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddManpowerRequirementAsync(ManpowerRequirement requirement)
        {
            requirement.Id = 0;
            requirement.CreatedAt = DateTime.Now;
            await _context.ManpowerRequirements.AddAsync(requirement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateManpowerRequirementAsync(ManpowerRequirement requirement)
        {
            var existing = await _context.ManpowerRequirements.FindAsync(requirement.Id);
            if (existing != null)
            {
                existing.RoleTitle = requirement.RoleTitle;
                existing.DepartmentId = requirement.DepartmentId;
                existing.SectionId = requirement.SectionId;
                existing.DesignationId = requirement.DesignationId;
                existing.HeadcountNeeded = requirement.HeadcountNeeded;
                existing.TargetDate = requirement.TargetDate;
                existing.Priority = requirement.Priority;
                existing.Status = requirement.Status;
                existing.Description = requirement.Description;
                existing.Requirements = requirement.Requirements;
                existing.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteManpowerRequirementAsync(int id)
        {
            var existing = await _context.ManpowerRequirements.FindAsync(id);
            if (existing != null)
            {
                _context.ManpowerRequirements.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // Separations
        public async Task<List<Separation>> GetSeparationsAsync()
        {
            return await _context.Separations
                .Include(s => s.Department)
                .Include(s => s.Section)
                .Include(s => s.Designation)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Separation?> GetSeparationByIdAsync(int id)
        {
            return await _context.Separations
                .Include(s => s.Department)
                .Include(s => s.Section)
                .Include(s => s.Designation)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddSeparationAsync(Separation separation)
        {
            separation.Id = 0;
            separation.CreatedAt = DateTime.Now;
            await _context.Separations.AddAsync(separation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSeparationAsync(Separation separation)
        {
            var existing = await _context.Separations.FindAsync(separation.Id);
            if (existing != null)
            {
                existing.EmployeeId = separation.EmployeeId;
                existing.EmployeeName = separation.EmployeeName;
                existing.DepartmentId = separation.DepartmentId;
                existing.SectionId = separation.SectionId;
                existing.DesignationId = separation.DesignationId;
                existing.SeparationType = separation.SeparationType;
                existing.ResignDate = separation.ResignDate;
                existing.LastWorkingDay = separation.LastWorkingDay;
                existing.ExitInterviewDate = separation.ExitInterviewDate;
                existing.Reason = separation.Reason;
                existing.HandoverNotes = separation.HandoverNotes;
                existing.Status = separation.Status;
                existing.ClearanceProgress = separation.ClearanceProgress;
                existing.UpdatedAt = DateTime.Now;
                existing.ApprovedBy = separation.ApprovedBy;
                existing.ApprovedDate = separation.ApprovedDate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteSeparationAsync(int id)
        {
            var existing = await _context.Separations.FindAsync(id);
            if (existing != null)
            {
                _context.Separations.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}

#pragma warning restore CA1416
