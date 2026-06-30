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
using OfficeOpenXml;
using ERPHub.Data;
using ERPHub.Models;

#pragma warning disable CA1416 // OleDb is Windows-only; this app targets Windows

namespace ERPHub.Services
{
    public class ErpService : IErpService
    {
        private readonly ErpDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPayrollService _payrollService;

        public ErpService(ErpDbContext context, IConfiguration configuration, IPayrollService payrollService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _payrollService = payrollService ?? throw new ArgumentNullException(nameof(payrollService));
        }

        // --- Groups Operations ---
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
                existing.InTime = shift.InTime;
                existing.OutTime = shift.OutTime;
                existing.LateTime = shift.LateTime;
                existing.OffDay = shift.OffDay;
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
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Section)
                .Include(e => e.Designation)
                .Include(e => e.Line)
                .Include(e => e.Shift)
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetFilteredEmployeesAsync(EmployeeFilter filter)
        {
            IQueryable<Employee> query = _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Section)
                .Include(e => e.Designation)
                .Include(e => e.Line)
                .Include(e => e.Shift)
                .Include(e => e.Company);

            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                var q = filter.SearchQuery.Trim().ToLower();
                query = query.Where(e =>
                    e.EmployeeName.ToLower().Contains(q) ||
                    e.Email.ToLower().Contains(q) ||
                    (e.EmployeeId != null && e.EmployeeId.ToLower().Contains(q)));
            }

            if (!string.IsNullOrWhiteSpace(filter.EmployeeId))
                query = query.Where(e => e.EmployeeId != null && e.EmployeeId == filter.EmployeeId);

            if (!string.IsNullOrWhiteSpace(filter.Department))
                query = query.Where(e => e.Department != null && e.Department.NameEn == filter.Department);

            if (!string.IsNullOrWhiteSpace(filter.Section))
                query = query.Where(e => e.Section != null && e.Section.NameEn == filter.Section);

            if (!string.IsNullOrWhiteSpace(filter.Designation))
                query = query.Where(e => e.Designation != null && e.Designation.NameEn == filter.Designation);

            if (!string.IsNullOrWhiteSpace(filter.Line))
                query = query.Where(e => e.Line != null && e.Line.NameEn == filter.Line);

            if (!string.IsNullOrWhiteSpace(filter.Company))
                query = query.Where(e => e.Company != null && e.Company.CompanyNameEn == filter.Company);

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(e => e.Status == filter.Status);

            if (!string.IsNullOrWhiteSpace(filter.Year))
            {
                var year = int.Parse(filter.Year);
                query = query.Where(e => e.JoiningDate.Year == year);
            }

            return await query.OrderByDescending(e => e.Id).ToListAsync();
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
                existing.HouseRent = employee.HouseRent;
                existing.MedicalAllowance = employee.MedicalAllowance;
                existing.TransportAllowance = employee.TransportAllowance;
                existing.FoodAllowance = employee.FoodAllowance;
                existing.SpecialAllowance = employee.SpecialAllowance;
                existing.AttendanceBonus = employee.AttendanceBonus;
                existing.ProductionBonus = employee.ProductionBonus;
                existing.BankName = employee.BankName;
                existing.BranchName = employee.BranchName;
                existing.RoutingNumber = employee.RoutingNumber;
                existing.PhotoBase64 = employee.PhotoBase64;
                existing.SignatureBase64 = employee.SignatureBase64;
                existing.Status = employee.Status;
                existing.SeparationDate = employee.SeparationDate;
                existing.SeparationType = employee.SeparationType;
                existing.SeparationReason = employee.SeparationReason;
                existing.SeparationRemarks = employee.SeparationRemarks;
                existing.SeparationApprovedBy = employee.SeparationApprovedBy;
                existing.SeparationApprovedDate = employee.SeparationApprovedDate;
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

        public async Task<ImportResultDto> ImportEmployeesFromExcelAsync(Stream fileStream)
        {
            var result = new ImportResultDto();
            try
            {
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Success = false;
                    result.Errors.Add("Excel file is empty (no worksheets found).");
                    return result;
                }

                int rowCount = worksheet.Dimension?.End.Row ?? 0;
                int colCount = worksheet.Dimension?.End.Column ?? 0;

                if (rowCount < 2)
                {
                    result.Success = false;
                    result.Errors.Add("Excel sheet must contain a header row and at least one data row.");
                    return result;
                }

                // Map headers
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int col = 1; col <= colCount; col++)
                {
                    var headerText = worksheet.Cells[1, col].Text?.Trim();
                    if (!string.IsNullOrEmpty(headerText))
                    {
                        headerMap[headerText] = col;
                    }
                }

                // Validate headers
                var requiredHeaders = new[] { "Employee ID", "Employee Name", "Punch Number", "Mobile No", "Company", "Department", "Section", "Designation", "Line", "Shift" };
                var missingHeaders = new List<string>();
                foreach (var req in requiredHeaders)
                {
                    if (!headerMap.ContainsKey(req))
                    {
                        missingHeaders.Add(req);
                    }
                }

                if (missingHeaders.Any())
                {
                    result.Success = false;
                    result.Errors.Add($"Missing required headers in Excel file: {string.Join(", ", missingHeaders)}");
                    return result;
                }

                // Load all lookups to match by name
                var dbCompanies = await _context.Companies.ToListAsync();
                var dbDepartments = await _context.Departments.ToListAsync();
                var dbSections = await _context.Sections.ToListAsync();
                var dbDesignations = await _context.Designations.ToListAsync();
                var dbLines = await _context.Lines.ToListAsync();
                var dbShifts = await _context.Shifts.ToListAsync();

                // Load all existing employees to check duplicates (EmployeeId and PunchNumber)
                var existingEmployees = await _context.Employees.ToListAsync();

                var newEmployees = new List<Employee>();
                var updatedEmployees = new List<Employee>();

                // Keep track of Employee IDs and Punch Numbers processed in this file to prevent internal duplicates
                var fileEmployeeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var filePunchNumbers = new HashSet<int>();

                string GetValue(int row, string headerName)
                {
                    if (headerMap.TryGetValue(headerName, out int col))
                    {
                        return worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
                    }
                    return string.Empty;
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    // Check if row is empty
                    bool isRowEmpty = true;
                    for (int col = 1; col <= colCount; col++)
                    {
                        if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                        {
                            isRowEmpty = false;
                            break;
                        }
                    }
                    if (isRowEmpty) continue;

                    result.TotalRows++;

                    var empId = GetValue(row, "Employee ID");
                    var name = GetValue(row, "Employee Name");
                    var punchStr = GetValue(row, "Punch Number");
                    var mobile = GetValue(row, "Mobile No");
                    var email = GetValue(row, "Email");
                    var companyName = GetValue(row, "Company");
                    var deptName = GetValue(row, "Department");
                    var secName = GetValue(row, "Section");
                    var desName = GetValue(row, "Designation");
                    var lineName = GetValue(row, "Line");
                    var shiftName = GetValue(row, "Shift");
                    var joiningDateStr = GetValue(row, "Joining Date");
                    var basicSalaryStr = GetValue(row, "Basic Salary");
                    var grossSalaryStr = GetValue(row, "Gross Salary");
                    var gender = GetValue(row, "Gender");
                    var dobStr = GetValue(row, "Date of Birth");
                    var status = GetValue(row, "Employee Status");
                    var empType = GetValue(row, "Employee Type");
                    var otStatusStr = GetValue(row, "Overtime Status");

                    var rowErrors = new List<string>();

                    // 1. Basic validation
                    if (string.IsNullOrEmpty(empId))
                        rowErrors.Add("Employee ID is required.");
                    if (string.IsNullOrEmpty(name))
                        rowErrors.Add("Employee Name is required.");
                    if (string.IsNullOrEmpty(mobile))
                        rowErrors.Add("Mobile No is required.");

                    // 2. Punch number validation
                    int punchNum = 0;
                    if (!string.IsNullOrEmpty(punchStr))
                    {
                        if (!int.TryParse(punchStr, out punchNum) || punchNum <= 0)
                        {
                            rowErrors.Add("Punch Number must be a valid positive integer.");
                        }
                    }
                    else
                    {
                        rowErrors.Add("Punch Number is required.");
                    }

                    // 3. Resolve Company
                    var company = dbCompanies.FirstOrDefault(c => c.CompanyNameEn.Trim().Equals(companyName, StringComparison.OrdinalIgnoreCase));
                    if (company == null)
                        rowErrors.Add($"Company '{companyName}' not found.");

                    // 4. Resolve Department
                    var department = dbDepartments.FirstOrDefault(d => d.NameEn.Trim().Equals(deptName, StringComparison.OrdinalIgnoreCase));
                    if (department == null)
                        rowErrors.Add($"Department '{deptName}' not found.");

                    // 5. Resolve Section
                    var section = dbSections.FirstOrDefault(s => s.NameEn.Trim().Equals(secName, StringComparison.OrdinalIgnoreCase));
                    if (section == null)
                        rowErrors.Add($"Section '{secName}' not found.");

                    // 6. Resolve Designation
                    var designation = dbDesignations.FirstOrDefault(d => d.NameEn.Trim().Equals(desName, StringComparison.OrdinalIgnoreCase));
                    if (designation == null)
                        rowErrors.Add($"Designation '{desName}' not found.");

                    // 7. Resolve Line
                    var line = dbLines.FirstOrDefault(l => l.NameEn.Trim().Equals(lineName, StringComparison.OrdinalIgnoreCase));
                    if (line == null)
                        rowErrors.Add($"Line '{lineName}' not found.");

                    // 8. Resolve Shift
                    var shift = dbShifts.FirstOrDefault(s => s.ShiftName.Trim().Equals(shiftName, StringComparison.OrdinalIgnoreCase));
                    if (shift == null)
                        rowErrors.Add($"Shift '{shiftName}' not found.");

                    // Date parsing
                    var joiningDate = DateTime.Today;
                    if (!string.IsNullOrEmpty(joiningDateStr))
                    {
                        if (DateTime.TryParse(joiningDateStr, out DateTime jd))
                            joiningDate = jd;
                        else
                            rowErrors.Add($"Invalid Joining Date format '{joiningDateStr}'. Use YYYY-MM-DD.");
                    }

                    // Decimal salaries parsing
                    decimal basicSalary = 0;
                    if (!string.IsNullOrEmpty(basicSalaryStr))
                    {
                        if (!decimal.TryParse(basicSalaryStr, out basicSalary) || basicSalary < 0)
                            rowErrors.Add($"Invalid Basic Salary '{basicSalaryStr}'.");
                    }

                    decimal grossSalary = 0;
                    if (!string.IsNullOrEmpty(grossSalaryStr))
                    {
                        if (!decimal.TryParse(grossSalaryStr, out grossSalary) || grossSalary < 0)
                            rowErrors.Add($"Invalid Gross Salary '{grossSalaryStr}'.");
                    }

                    bool overTimeStatus = false;
                    if (!string.IsNullOrEmpty(otStatusStr))
                    {
                        if (otStatusStr.Equals("yes", StringComparison.OrdinalIgnoreCase) || 
                            otStatusStr.Equals("true", StringComparison.OrdinalIgnoreCase) || 
                            otStatusStr.Equals("1"))
                        {
                            overTimeStatus = true;
                        }
                    }

                    // Duplicate ID check in same file
                    if (!string.IsNullOrEmpty(empId))
                    {
                        if (!fileEmployeeIds.Add(empId))
                        {
                            rowErrors.Add($"Duplicate Employee ID '{empId}' found in the Excel file.");
                        }
                    }

                    // Duplicate Punch check in same file
                    if (punchNum > 0)
                    {
                        if (!filePunchNumbers.Add(punchNum))
                        {
                            rowErrors.Add($"Duplicate Punch Number '{punchNum}' found in the Excel file.");
                        }
                    }

                    // Check duplicate punch number in Database (assigned to someone else)
                    if (punchNum > 0)
                    {
                        var dbDupPunch = existingEmployees.FirstOrDefault(e => e.PunchNumber == punchNum && !e.EmployeeId.Equals(empId, StringComparison.OrdinalIgnoreCase));
                        if (dbDupPunch != null)
                        {
                            rowErrors.Add($"Punch Number '{punchNum}' is already assigned to another employee: {dbDupPunch.EmployeeName} ({dbDupPunch.EmployeeId}).");
                        }
                    }

                    if (rowErrors.Any())
                    {
                        result.Errors.AddRange(rowErrors.Select(err => $"Row {row}: {err}"));
                        result.ErrorCount++;
                        continue;
                    }

                    // Build / update employee object
                    var existing = existingEmployees.FirstOrDefault(e => e.EmployeeId.Equals(empId, StringComparison.OrdinalIgnoreCase));
                    if (existing != null)
                    {
                        // Update existing
                        existing.EmployeeName = name;
                        existing.PunchNumber = punchNum;
                        existing.MobileNo = mobile;
                        existing.Email = email;
                        existing.CompanyId = company!.Id;
                        existing.DepartmentId = department!.Id;
                        existing.SectionId = section!.Id;
                        existing.DesignationId = designation!.Id;
                        existing.LineId = line!.Id;
                        existing.ShiftId = shift!.Id;
                        existing.JoiningDate = joiningDate;
                        existing.BasicSalary = basicSalary;
                        existing.GrossSalary = grossSalary;
                        existing.Gender = gender;
                        existing.DateOfBirth = dobStr;
                        existing.Status = EmploymentEligibility.NormalizeImportStatus(status, out var sepType);
                        if (sepType != null)
                        {
                            existing.SeparationType = sepType;
                            existing.SeparationDate ??= DateTime.Today;
                        }
                        existing.EmployeeType = empType;
                        existing.OverTimeStatus = overTimeStatus;

                        updatedEmployees.Add(existing);
                    }
                    else
                    {
                        // Create new
                        var emp = new Employee
                        {
                            EmployeeId = empId,
                            EmployeeName = name,
                            PunchNumber = punchNum,
                            MobileNo = mobile,
                            Email = email,
                            CompanyId = company!.Id,
                            DepartmentId = department!.Id,
                            SectionId = section!.Id,
                            DesignationId = designation!.Id,
                            LineId = line!.Id,
                            ShiftId = shift!.Id,
                            JoiningDate = joiningDate,
                            BasicSalary = basicSalary,
                            GrossSalary = grossSalary,
                            Gender = gender,
                            DateOfBirth = dobStr,
                            Status = EmploymentEligibility.NormalizeImportStatus(status, out var newSepType),
                            SeparationType = newSepType,
                            SeparationDate = newSepType != null ? DateTime.Today : null,
                            EmployeeType = empType,
                            OverTimeStatus = overTimeStatus
                        };

                        newEmployees.Add(emp);
                    }

                    result.SuccessCount++;
                }

                if (result.ErrorCount > 0)
                {
                    result.Success = false;
                    return result;
                }

                // Start transaction to save all changes safely
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (newEmployees.Any())
                    {
                        await _context.Employees.AddRangeAsync(newEmployees);
                    }

                    if (updatedEmployees.Any())
                    {
                        _context.Employees.UpdateRange(updatedEmployees);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    result.Success = true;
                }
                catch (Exception dbEx)
                {
                    await transaction.RollbackAsync();
                    result.Success = false;
                    result.Errors.Add($"Database save error: {dbEx.Message}");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"System parsing error: {ex.Message}");
            }

            return result;
        }

        public async Task<ImportResultDto> ImportOrganogramFromExcelAsync(Stream fileStream)
        {
            var result = new ImportResultDto();
            try
            {
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Success = false;
                    result.Errors.Add("Excel file is empty (no worksheets found).");
                    return result;
                }

                int rowCount = worksheet.Dimension?.End.Row ?? 0;
                int colCount = worksheet.Dimension?.End.Column ?? 0;

                if (rowCount < 2)
                {
                    result.Success = false;
                    result.Errors.Add("Excel sheet must contain a header row and at least one data row.");
                    return result;
                }

                // Map headers
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int col = 1; col <= colCount; col++)
                {
                    var headerText = worksheet.Cells[1, col].Text?.Trim();
                    if (!string.IsNullOrEmpty(headerText))
                    {
                        headerMap[headerText] = col;
                    }
                }

                // Validate headers
                var requiredHeaders = new[] { "Department (EN)" };
                var missingHeaders = new List<string>();
                foreach (var req in requiredHeaders)
                {
                    if (!headerMap.ContainsKey(req))
                    {
                        missingHeaders.Add(req);
                    }
                }

                if (missingHeaders.Any())
                {
                    result.Success = false;
                    result.Errors.Add($"Missing required headers in Excel file: {string.Join(", ", missingHeaders)}");
                    return result;
                }

                // Start transaction to save all changes safely and generate IDs incrementally
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Load existing lookups
                    var dbDepartments = await _context.Departments.ToListAsync();
                    var dbSections = await _context.Sections.ToListAsync();
                    var dbDesignations = await _context.Designations.ToListAsync();
                    var dbLines = await _context.Lines.ToListAsync();

                    var tempDepartments = new Dictionary<string, Department>(StringComparer.OrdinalIgnoreCase);
                    foreach (var d in dbDepartments)
                    {
                        tempDepartments[d.NameEn] = d;
                    }

                    var tempSections = new Dictionary<string, Section>(StringComparer.OrdinalIgnoreCase);
                    foreach (var s in dbSections)
                    {
                        var key = $"{s.DepartmentNameEn}|{s.NameEn}";
                        tempSections[key] = s;
                    }

                    var tempDesignations = new Dictionary<string, Designation>(StringComparer.OrdinalIgnoreCase);
                    foreach (var d in dbDesignations)
                    {
                        var sectionObj = dbSections.FirstOrDefault(s => s.Id == d.SectionId);
                        var deptName = sectionObj?.DepartmentNameEn ?? string.Empty;
                        var key = $"{deptName}|{d.SectionNameEn}|{d.NameEn}";
                        tempDesignations[key] = d;
                    }

                    var tempLines = new Dictionary<string, Line>(StringComparer.OrdinalIgnoreCase);
                    foreach (var l in dbLines)
                    {
                        var sectionObj = dbSections.FirstOrDefault(s => s.Id == l.SectionId);
                        var deptName = sectionObj?.DepartmentNameEn ?? string.Empty;
                        var key = $"{deptName}|{l.SectionNameEn}|{l.NameEn}";
                        tempLines[key] = l;
                    }

                    string GetValue(int row, string headerName)
                    {
                        if (headerMap.TryGetValue(headerName, out int col))
                        {
                            return worksheet.Cells[row, col].Text?.Trim() ?? string.Empty;
                        }
                        return string.Empty;
                    }

                    for (int row = 2; row <= rowCount; row++)
                    {
                        // Check if row is empty
                        bool isRowEmpty = true;
                        for (int col = 1; col <= colCount; col++)
                        {
                            if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }
                        if (isRowEmpty) continue;

                        result.TotalRows++;

                        var deptEn = GetValue(row, "Department (EN)");
                        var deptBn = GetValue(row, "Department (BN)");
                        var secEn = GetValue(row, "Section (EN)");
                        var secBn = GetValue(row, "Section (BN)");
                        var desigEn = GetValue(row, "Designation (EN)");
                        var desigBn = GetValue(row, "Designation (BN)");
                        var lineEn = GetValue(row, "Line (EN)");
                        var lineBn = GetValue(row, "Line (BN)");

                        var rowErrors = new List<string>();

                        // Validate row rules
                        if (string.IsNullOrEmpty(deptEn))
                        {
                            rowErrors.Add("Department (EN) name is required.");
                        }

                        if (string.IsNullOrEmpty(secEn) && (!string.IsNullOrEmpty(desigEn) || !string.IsNullOrEmpty(lineEn)))
                        {
                            rowErrors.Add("Section (EN) is required when defining a Designation or Line.");
                        }

                        if (rowErrors.Any())
                        {
                            result.Errors.AddRange(rowErrors.Select(err => $"Row {row}: {err}"));
                            result.ErrorCount++;
                            continue;
                        }

                        // 1. Resolve Department
                        if (!tempDepartments.TryGetValue(deptEn, out var dept))
                        {
                            dept = new Department
                            {
                                NameEn = deptEn,
                                NameBn = string.IsNullOrEmpty(deptBn) ? deptEn : deptBn
                            };
                            await _context.Departments.AddAsync(dept);
                            await _context.SaveChangesAsync();
                            tempDepartments[deptEn] = dept;
                        }

                        // 2. Resolve Section (if provided)
                        Section? section = null;
                        if (!string.IsNullOrEmpty(secEn))
                        {
                            var secKey = $"{dept.NameEn}|{secEn}";
                            if (!tempSections.TryGetValue(secKey, out section))
                            {
                                section = new Section
                                {
                                    NameEn = secEn,
                                    NameBn = string.IsNullOrEmpty(secBn) ? secEn : secBn,
                                    DepartmentId = dept.Id,
                                    DepartmentNameEn = dept.NameEn
                                };
                                await _context.Sections.AddAsync(section);
                                await _context.SaveChangesAsync();
                                tempSections[secKey] = section;
                            }
                        }

                        // 3. Resolve Designation (if provided)
                        if (!string.IsNullOrEmpty(desigEn) && section != null)
                        {
                            var desigKey = $"{dept.NameEn}|{section.NameEn}|{desigEn}";
                            if (!tempDesignations.TryGetValue(desigKey, out var desig))
                            {
                                desig = new Designation
                                {
                                    NameEn = desigEn,
                                    NameBn = string.IsNullOrEmpty(desigBn) ? desigEn : desigBn,
                                    SectionId = section.Id,
                                    SectionNameEn = section.NameEn
                                };
                                await _context.Designations.AddAsync(desig);
                                await _context.SaveChangesAsync();
                                tempDesignations[desigKey] = desig;
                            }
                        }

                        // 4. Resolve Line (if provided)
                        if (!string.IsNullOrEmpty(lineEn) && section != null)
                        {
                            var lineKey = $"{dept.NameEn}|{section.NameEn}|{lineEn}";
                            if (!tempLines.TryGetValue(lineKey, out var line))
                            {
                                line = new Line
                                {
                                    NameEn = lineEn,
                                    NameBn = string.IsNullOrEmpty(lineBn) ? lineEn : lineBn,
                                    SectionId = section.Id,
                                    SectionNameEn = section.NameEn
                                };
                                await _context.Lines.AddAsync(line);
                                await _context.SaveChangesAsync();
                                tempLines[lineKey] = line;
                            }
                        }

                        result.SuccessCount++;
                    }

                    if (result.ErrorCount > 0)
                    {
                        await transaction.RollbackAsync();
                        result.Success = false;
                        return result;
                    }

                    await transaction.CommitAsync();
                    result.Success = true;
                }
                catch (Exception dbEx)
                {
                    await transaction.RollbackAsync();
                    result.Success = false;
                    result.Errors.Add($"Database save error: {dbEx.Message}");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"System parsing error: {ex.Message}");
            }

            return result;
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
            await _payrollService.CalculateDailyPayrollAsync(date);
        }

        public async Task UpdateDailySalaryRecordAsync(DailySalaryRecord record)
        {
            var existing = await _context.DailySalaryRecords.FindAsync(record.Id);
            if (existing != null)
            {
                existing.DailyBasic = record.DailyBasic;
                existing.DailyGross = record.DailyGross;
                existing.OtHours = record.OtHours;
                existing.OtPay = record.OtPay;
                existing.NightBillPay = record.NightBillPay;
                existing.HolidayBillPay = record.HolidayBillPay;
                existing.Allowances = record.Allowances;
                existing.AbsentDeduction = record.AbsentDeduction;
                existing.LateDeduction = record.LateDeduction;
                existing.LwopDeduction = record.LwopDeduction;
                existing.AdvanceDeduction = record.AdvanceDeduction;
                existing.LoanDeduction = record.LoanDeduction;
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
            var count = await _context.Manpowers.CountAsync();
            if (count == 0)
            {
                var employees = await _context.Employees.CurrentlyActive().ToListAsync();
                var groups = employees.GroupBy(e => new { e.DepartmentId, e.SectionId, e.DesignationId }).ToList();
                int idx = 1;
                foreach (var g in groups)
                {
                    int headcount = g.Count();
                    int extra = (idx % 3) == 0 ? 0 : (idx % 3);
                    int target = headcount + extra;
                    var mp = new Manpower
                    {
                        DepartmentId = g.Key.DepartmentId,
                        SectionId = g.Key.SectionId,
                        DesignationId = g.Key.DesignationId,
                        CurrentHeadcount = headcount,
                        TargetCapacity = target,
                        Vacancies = target - headcount,
                        Remarks = "Auto-seeded based on employee distribution",
                        LastUpdated = DateTime.Now
                    };
                    await _context.Manpowers.AddAsync(mp);
                    idx++;
                }
                await _context.SaveChangesAsync();
            }

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
            var employees = await _context.Employees.CurrentlyActive().ToListAsync();
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
            var count = await _context.ManpowerRequirements.CountAsync();
            if (count == 0)
            {
                var departments = await _context.Departments.Take(3).ToListAsync();
                var designations = await _context.Designations.Take(3).ToListAsync();
                if (departments.Count > 0 && designations.Count > 0)
                {
                    var req1 = new ManpowerRequirement
                    {
                        RoleTitle = "Senior Software Architect",
                        DepartmentId = departments[0].Id,
                        DesignationId = designations[0].Id,
                        HeadcountNeeded = 2,
                        TargetDate = DateTime.Today.AddDays(30),
                        Priority = "High",
                        Status = "Approved",
                        Description = "We are looking for a Senior Software Architect to lead the transition of our ERP systems into Blazor native components.",
                        Requirements = "8+ years in C#/.NET, strong database normalization skills, and clean architecture expertise.",
                        CreatedAt = DateTime.Now.AddDays(-5),
                        CreatedBy = "HR Manager"
                    };

                    var req2 = new ManpowerRequirement
                    {
                        RoleTitle = "Production Lead Supervisor",
                        DepartmentId = departments.Count > 1 ? departments[1].Id : departments[0].Id,
                        DesignationId = designations.Count > 1 ? designations[1].Id : designations[0].Id,
                        HeadcountNeeded = 5,
                        TargetDate = DateTime.Today.AddDays(45),
                        Priority = "Medium",
                        Status = "Recruiting",
                        Description = "Urgent requirement for a shift lead to manage machinery throughput and safety protocols.",
                        Requirements = "Prior supervisory experience, familiar with shop floor scheduling systems.",
                        CreatedAt = DateTime.Now.AddDays(-3),
                        CreatedBy = "HR Manager"
                    };

                    var req3 = new ManpowerRequirement
                    {
                        RoleTitle = "Technical Recruiter",
                        DepartmentId = departments.Count > 2 ? departments[2].Id : departments[0].Id,
                        DesignationId = designations.Count > 2 ? designations[2].Id : designations[0].Id,
                        HeadcountNeeded = 1,
                        TargetDate = DateTime.Today.AddDays(15),
                        Priority = "Low",
                        Status = "Pending",
                        Description = "Responsible for hiring technical engineers, developers, and administrators.",
                        Requirements = "Strong communication, experience in C# tech sourcing.",
                        CreatedAt = DateTime.Now.AddDays(-1),
                        CreatedBy = "HR Manager"
                    };

                    await _context.ManpowerRequirements.AddRangeAsync(req1, req2, req3);
                    await _context.SaveChangesAsync();
                }
            }

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
        public async Task<List<Separation>> GetSeparationsAsync(string? type = null, string? status = null, int? deptId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Separations
                .Include(s => s.Department)
                .Include(s => s.Section)
                .Include(s => s.Designation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(type) && type != "All")
                query = query.Where(s => s.SeparationType == type);
            if (!string.IsNullOrEmpty(status) && status != "All")
                query = query.Where(s => s.Status == status);
            if (deptId.HasValue && deptId > 0)
                query = query.Where(s => s.DepartmentId == deptId.Value);
            if (fromDate.HasValue)
                query = query.Where(s => s.SeparationDate >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(s => s.SeparationDate <= toDate.Value.Date);

            return await query.OrderByDescending(s => s.CreatedDate).ToListAsync();
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
            separation.CreatedDate = DateTime.UtcNow;
            await _context.Separations.AddAsync(separation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSeparationAsync(Separation separation)
        {
            var existing = await _context.Separations.FindAsync(separation.Id);
            if (existing != null)
            {
                existing.EmployeeRefId = separation.EmployeeRefId;
                existing.EmployeeId = separation.EmployeeId;
                existing.EmployeeName = separation.EmployeeName;
                existing.DepartmentId = separation.DepartmentId;
                existing.SectionId = separation.SectionId;
                existing.DesignationId = separation.DesignationId;
                existing.CompanyId = separation.CompanyId;
                existing.SeparationType = separation.SeparationType;
                existing.SeparationDate = separation.SeparationDate;
                existing.ExitInterviewDate = separation.ExitInterviewDate;
                existing.Reason = separation.Reason;
                existing.Remarks = separation.Remarks;
                existing.Status = separation.Status;
                existing.ClearanceProgress = separation.ClearanceProgress;
                existing.IsCancelled = separation.IsCancelled;
                existing.IsSettled = separation.IsSettled;
                existing.UpdatedAt = DateTime.UtcNow;
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

        // ─────────────────────────────────────────────────────────────────────
        // Leave Types
        // ─────────────────────────────────────────────────────────────────────

        public async Task<List<LeaveType>> GetLeaveTypesAsync()
        {
            var count = await _context.LeaveTypes.CountAsync();
            if (count == 0)
            {
                var seeds = new List<LeaveType>
                {
                    new LeaveType { Name = "Annual Leave",    Code = "AL",  MaxDaysPerYear = 15, IsPaid = true,  AccrualType = "Monthly", IsActive = true, Color = "#6366f1" },
                    new LeaveType { Name = "Medical Leave",   Code = "ML",  MaxDaysPerYear = 10, IsPaid = true,  AccrualType = "Yearly",  IsActive = true, Color = "#ef4444" },
                    new LeaveType { Name = "Casual Leave",    Code = "CL",  MaxDaysPerYear = 5,  IsPaid = true,  AccrualType = "Yearly",  IsActive = true, Color = "#f59e0b" },
                    new LeaveType { Name = "Maternity Leave", Code = "MAT", MaxDaysPerYear = 90, IsPaid = true,  AccrualType = "OnRequest", IsActive = true, Color = "#ec4899", RequiresMedicalCertificate = true },
                    new LeaveType { Name = "Unpaid Leave",    Code = "UL",  MaxDaysPerYear = 30, IsPaid = false, AccrualType = "OnRequest", IsActive = true, Color = "#71717a" }
                };
                await _context.LeaveTypes.AddRangeAsync(seeds);
                await _context.SaveChangesAsync();
            }
            return await _context.LeaveTypes.OrderBy(lt => lt.Name).ToListAsync();
        }

        public async Task<LeaveType?> GetLeaveTypeByIdAsync(int id)
            => await _context.LeaveTypes.FindAsync(id);

        public async Task AddLeaveTypeAsync(LeaveType leaveType)
        {
            leaveType.Id = 0;
            leaveType.CreatedAt = DateTime.Now;
            await _context.LeaveTypes.AddAsync(leaveType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeaveTypeAsync(LeaveType leaveType)
        {
            var existing = await _context.LeaveTypes.FindAsync(leaveType.Id);
            if (existing != null)
            {
                existing.Name = leaveType.Name;
                existing.Code = leaveType.Code;
                existing.MaxDaysPerYear = leaveType.MaxDaysPerYear;
                existing.IsPaid = leaveType.IsPaid;
                existing.AccrualType = leaveType.AccrualType;
                existing.IsActive = leaveType.IsActive;
                existing.Color = leaveType.Color;
                existing.RequiresMedicalCertificate = leaveType.RequiresMedicalCertificate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteLeaveTypeAsync(int id)
        {
            var existing = await _context.LeaveTypes.FindAsync(id);
            if (existing != null) { _context.LeaveTypes.Remove(existing); await _context.SaveChangesAsync(); }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Leave Applications
        // ─────────────────────────────────────────────────────────────────────

        public async Task<List<LeaveApplication>> GetLeaveApplicationsAsync(int? month = null, int? year = null, string? status = null)
        {

            var query = _context.LeaveApplications
                .Include(la => la.Department)
                .Include(la => la.Designation)
                .Include(la => la.LeaveTypeNav)
                .AsQueryable();

            if (month.HasValue) query = query.Where(la => la.LeaveDate.Month == month.Value);
            if (year.HasValue)  query = query.Where(la => la.LeaveDate.Year  == year.Value);
            if (!string.IsNullOrEmpty(status) && status != "All")
                query = query.Where(la => la.Status == status);

            return await query.OrderByDescending(la => la.CreatedAt).ToListAsync();
        }

        public async Task<LeaveApplication?> GetLeaveApplicationByIdAsync(int id)
            => await _context.LeaveApplications
                .Include(la => la.Department)
                .Include(la => la.Designation)
                .Include(la => la.LeaveTypeNav)
                .FirstOrDefaultAsync(la => la.Id == id);

        public async Task AddLeaveApplicationAsync(LeaveApplication application)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == application.EmployeeId);

            if (employee == null)
                throw new InvalidOperationException("Employee not found.");

            if (employee.Status == EmployeeStatuses.Separation)
                throw new InvalidOperationException("Separated employees cannot apply for leave.");

            if (!EmploymentEligibility.IsEligibleForProcessing(employee, application.LeaveDate))
                throw new InvalidOperationException("Leave cannot be applied after separation date.");

            application.Id = 0;
            application.CreatedAt = DateTime.Now;
            application.Status = "Pending";
            await _context.LeaveApplications.AddAsync(application);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeaveApplicationAsync(LeaveApplication application)
        {
            var existing = await _context.LeaveApplications.FindAsync(application.Id);
            if (existing != null)
            {
                existing.EmployeeId    = application.EmployeeId;
                existing.EmployeeName  = application.EmployeeName;
                existing.DepartmentId  = application.DepartmentId;
                existing.DesignationId = application.DesignationId;
                existing.LeaveTypeId   = application.LeaveTypeId;
                existing.LeaveType     = application.LeaveType;
                existing.LeaveDate     = application.LeaveDate;
                existing.EndDate       = application.EndDate;
                existing.TotalDays     = application.TotalDays;
                existing.Reason        = application.Reason;
                existing.UpdatedAt     = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApproveLeaveApplicationAsync(int id, string approvedBy)
        {
            var existing = await _context.LeaveApplications.FindAsync(id);
            if (existing != null)
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == existing.EmployeeId);

                if (employee?.Status == EmployeeStatuses.Separation)
                    throw new InvalidOperationException("Cannot approve leave for a separated employee.");

                if (employee != null && !EmploymentEligibility.IsEligibleForProcessing(employee, existing.LeaveDate))
                    throw new InvalidOperationException("Cannot approve leave after employee separation date.");

                existing.Status      = "Approved";
                existing.ApprovedBy  = approvedBy;
                existing.ApprovedDate = DateTime.Now;
                existing.UpdatedAt   = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectLeaveApplicationAsync(int id, string rejectedBy, string reason)
        {
            var existing = await _context.LeaveApplications.FindAsync(id);
            if (existing != null)
            {
                existing.Status         = "Rejected";
                existing.ApprovedBy     = rejectedBy;
                existing.RejectedReason = reason;
                existing.ApprovedDate   = DateTime.Now;
                existing.UpdatedAt      = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteLeaveApplicationAsync(int id)
        {
            var existing = await _context.LeaveApplications.FindAsync(id);
            if (existing != null) { _context.LeaveApplications.Remove(existing); await _context.SaveChangesAsync(); }
        }
    }
}

#pragma warning restore CA1416
