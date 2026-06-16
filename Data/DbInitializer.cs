using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPHub.Models;

namespace ERPHub.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ErpDbContext context)
        {
            // Seed Companies if empty
            if (!await context.Companies.AnyAsync())
            {
                await context.Companies.AddRangeAsync(new List<Company>
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
                });
            }

            // Seed Products if empty
            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(new List<Product>
                {
                    new Product { Name = "MacBook Pro M3 Max 16\"", Sku = "MAC-M3MAX-16", Category = "Hardware", Price = 3499.00m, Stock = 14, Description = "16-inch liquid retina XDR display, 36GB unified RAM, 1TB SSD.", LastUpdated = DateTime.Now.AddDays(-2) },
                    new Product { Name = "LG UltraFine 5K Display", Sku = "LG-5K-DISP", Category = "Hardware", Price = 1299.99m, Stock = 8, Description = "27-inch 5K monitor optimized for macOS and Windows development.", LastUpdated = DateTime.Now.AddDays(-5) },
                    new Product { Name = "Herman Miller Aeron Chair", Sku = "HM-AERON-CH", Category = "Office Equipment", Price = 1695.00m, Stock = 5, Description = "Ergonomic office chair with posturefit support and breathable mesh.", LastUpdated = DateTime.Now.AddDays(-1) },
                    new Product { Name = "Keychron Q1 Max Keyboard", Sku = "KEY-Q1MAX-BL", Category = "Accessories", Price = 219.00m, Stock = 32, Description = "Full metal QMK custom wireless mechanical keyboard with banana switches.", LastUpdated = DateTime.Now.AddDays(-10) },
                    new Product { Name = "Logitech MX Master 3S", Sku = "LOG-MX3S-MOU", Category = "Accessories", Price = 99.99m, Stock = 45, Description = "Ergonomic wireless mouse with ultra-fast scrolling and quiet clicks.", LastUpdated = DateTime.Now }
                });
            }

            // Seed Invoices if empty
            if (!await context.Invoices.AnyAsync())
            {
                var invoice1 = new Invoice
                {
                    InvoiceNumber = "INV-2026-001",
                    ClientName = "Acme Global Solutions",
                    ClientEmail = "billing@acmeglobal.com",
                    IssueDate = DateTime.Now.AddDays(-15),
                    DueDate = DateTime.Now.AddDays(15),
                    Status = InvoiceStatus.Paid,
                    TaxRate = 0.15m,
                    DiscountAmount = 250.00m
                };
                invoice1.Items.AddRange(new[]
                {
                    new InvoiceItem { Description = "MacBook Pro M3 Max 16\"", Quantity = 2, UnitPrice = 3499.00m },
                    new InvoiceItem { Description = "LG UltraFine 5K Display", Quantity = 1, UnitPrice = 1299.99m }
                });

                var invoice2 = new Invoice
                {
                    InvoiceNumber = "INV-2026-002",
                    ClientName = "Globex Industries",
                    ClientEmail = "accounts@globex.co",
                    IssueDate = DateTime.Now.AddDays(-40),
                    DueDate = DateTime.Now.AddDays(-10),
                    Status = InvoiceStatus.Overdue,
                    TaxRate = 0.15m,
                    DiscountAmount = 0m
                };
                invoice2.Items.AddRange(new[]
                {
                    new InvoiceItem { Description = "Herman Miller Aeron Chair", Quantity = 4, UnitPrice = 1695.00m }
                });

                var invoice3 = new Invoice
                {
                    InvoiceNumber = "INV-2026-003",
                    ClientName = "Stark Enterprises",
                    ClientEmail = "finance@stark.com",
                    IssueDate = DateTime.Now.AddDays(-2),
                    DueDate = DateTime.Now.AddDays(28),
                    Status = InvoiceStatus.Draft,
                    TaxRate = 0.15m,
                    DiscountAmount = 500m
                };
                invoice3.Items.AddRange(new[]
                {
                    new InvoiceItem { Description = "MacBook Pro M3 Max 16\"", Quantity = 5, UnitPrice = 3499.00m },
                    new InvoiceItem { Description = "LG UltraFine 5K Display", Quantity = 5, UnitPrice = 1299.99m },
                    new InvoiceItem { Description = "Logitech MX Master 3S", Quantity = 10, UnitPrice = 99.99m }
                });

                var invoice4 = new Invoice
                {
                    InvoiceNumber = "INV-2026-004",
                    ClientName = "Wayne Enterprises",
                    ClientEmail = "accounts@waynecorp.com",
                    IssueDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(25),
                    Status = InvoiceStatus.Sent,
                    TaxRate = 0.15m,
                    DiscountAmount = 100m
                };
                invoice4.Items.AddRange(new[]
                {
                    new InvoiceItem { Description = "MacBook Pro M3 Max 16\"", Quantity = 1, UnitPrice = 3499.00m },
                    new InvoiceItem { Description = "Keychron Q1 Max Keyboard", Quantity = 3, UnitPrice = 219.00m }
                });

                await context.Invoices.AddRangeAsync(new[] { invoice1, invoice2, invoice3, invoice4 });
            }

            // Seed Tasks if empty
            if (!await context.ProjectTasks.AnyAsync())
            {
                await context.ProjectTasks.AddRangeAsync(new List<ProjectTask>
                {
                    new ProjectTask { Title = "Update Q2 Inventory Audits", Description = "Reconcile physical warehouse stocks with ERPHub records for accessories.", Status = Models.TaskStatus.Todo, Priority = TaskPriority.High, Assignee = "Peter Parker", DueDate = DateTime.Now.AddDays(3) },
                    new ProjectTask { Title = "Stark Enterprises Onboarding", Description = "Configure database replication and custom invoicing templates for client workspace.", Status = Models.TaskStatus.InProgress, Priority = TaskPriority.Critical, Assignee = "Tony Stark", DueDate = DateTime.Now.AddDays(2) },
                    new ProjectTask { Title = "Review Globex Overdue Invoices", Description = "Initiate dunning notifications and coordinate with client relations manager.", Status = Models.TaskStatus.Review, Priority = TaskPriority.Medium, Assignee = "Selina Kyle", DueDate = DateTime.Now.AddDays(1) },
                    new ProjectTask { Title = "Release Blazor Dashboard v1.0", Description = "Complete UI components, layout optimization, and finalize interactive Kanban behaviors.", Status = Models.TaskStatus.Done, Priority = TaskPriority.High, Assignee = "Clark Kent", DueDate = DateTime.Now.AddDays(-2) },
                    new ProjectTask { Title = "Draft Security Guidelines Policy", Description = "Standardize credential access configurations and encryption keys settings.", Status = Models.TaskStatus.Todo, Priority = TaskPriority.Low, Assignee = "Bruce Wayne", DueDate = DateTime.Now.AddDays(10) }
                });
            }

            // Create Users table via raw SQL if it doesn't exist
            await context.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                BEGIN
                    CREATE TABLE Users (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Username NVARCHAR(100) NOT NULL,
                        PasswordHash NVARCHAR(255) NOT NULL,
                        FullName NVARCHAR(200) NOT NULL,
                        Role NVARCHAR(50) NOT NULL
                    );
                    CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);
                END
            ");

            // Seed Users if empty
            if (!await context.Users.AnyAsync())
            {
                await context.Users.AddRangeAsync(new List<User>
                {
                    new User
                    {
                        Username = "admin",
                        PasswordHash = HashPassword("admin123"),
                        FullName = "System Administrator",
                        Role = "Admin"
                    },
                    new User
                    {
                        Username = "user",
                        PasswordHash = HashPassword("user123"),
                        FullName = "Staff User",
                        Role = "User"
                    }
                });
            }

            // Seed Departments if empty
            if (!await context.Departments.AnyAsync())
            {
                await context.Departments.AddRangeAsync(new List<Department>
                {
                    new Department { NameEn = "Production", NameBn = "উৎপাদন" },
                    new Department { NameEn = "Human Resources", NameBn = "মানবসম্পদ" },
                    new Department { NameEn = "Finance & Accounts", NameBn = "অর্থ ও হিসাব" },
                    new Department { NameEn = "Quality Assurance", NameBn = "মান নিশ্চিতকরণ" },
                    new Department { NameEn = "Logistics", NameBn = "লজিস্টিক্স" }
                });
                await context.SaveChangesAsync();
            }

            // Seed Sections if empty
            if (!await context.Sections.AnyAsync())
            {
                var prodDept = await context.Departments.FirstOrDefaultAsync(d => d.NameEn == "Production");
                var hrDept = await context.Departments.FirstOrDefaultAsync(d => d.NameEn == "Human Resources");
                var finDept = await context.Departments.FirstOrDefaultAsync(d => d.NameEn == "Finance & Accounts");
                var qaDept = await context.Departments.FirstOrDefaultAsync(d => d.NameEn == "Quality Assurance");

                var sectionsToSeed = new List<Section>();

                if (prodDept != null)
                {
                    sectionsToSeed.Add(new Section { NameEn = "Cutting", NameBn = "কাটিং", DepartmentId = prodDept.Id, DepartmentNameEn = prodDept.NameEn });
                    sectionsToSeed.Add(new Section { NameEn = "Sewing", NameBn = "সেয়িং", DepartmentId = prodDept.Id, DepartmentNameEn = prodDept.NameEn });
                    sectionsToSeed.Add(new Section { NameEn = "Finishing", NameBn = "ফিনিশিং", DepartmentId = prodDept.Id, DepartmentNameEn = prodDept.NameEn });
                }
                if (hrDept != null)
                {
                    sectionsToSeed.Add(new Section { NameEn = "Recruitment", NameBn = "নিয়োগ", DepartmentId = hrDept.Id, DepartmentNameEn = hrDept.NameEn });
                }
                if (finDept != null)
                {
                    sectionsToSeed.Add(new Section { NameEn = "Accounts Payable", NameBn = "বকেয়া পরিশোধ", DepartmentId = finDept.Id, DepartmentNameEn = finDept.NameEn });
                }
                if (qaDept != null)
                {
                    sectionsToSeed.Add(new Section { NameEn = "Final Audit", NameBn = "চূড়ান্ত নিরীক্ষা", DepartmentId = qaDept.Id, DepartmentNameEn = qaDept.NameEn });
                }

                if (sectionsToSeed.Any())
                {
                    await context.Sections.AddRangeAsync(sectionsToSeed);
                    await context.SaveChangesAsync();
                }
            }

            // Seed Designations if empty
            if (!await context.Designations.AnyAsync())
            {
                var sewingSection = await context.Sections.FirstOrDefaultAsync(s => s.NameEn == "Sewing");
                var cuttingSection = await context.Sections.FirstOrDefaultAsync(s => s.NameEn == "Cutting");
                var recruitmentSection = await context.Sections.FirstOrDefaultAsync(s => s.NameEn == "Recruitment");

                var desigsToSeed = new List<Designation>();

                if (sewingSection != null)
                {
                    desigsToSeed.Add(new Designation { NameEn = "Operator", NameBn = "অপারেটর", SectionId = sewingSection.Id, SectionNameEn = sewingSection.NameEn });
                    desigsToSeed.Add(new Designation { NameEn = "Line Supervisor", NameBn = "লাইন সুপারভাইজার", SectionId = sewingSection.Id, SectionNameEn = sewingSection.NameEn });
                }
                if (cuttingSection != null)
                {
                    desigsToSeed.Add(new Designation { NameEn = "Cutting Master", NameBn = "কাটিং মাস্টার", SectionId = cuttingSection.Id, SectionNameEn = cuttingSection.NameEn });
                }
                if (recruitmentSection != null)
                {
                    desigsToSeed.Add(new Designation { NameEn = "HR Officer", NameBn = "এইচআর কর্মকর্তা", SectionId = recruitmentSection.Id, SectionNameEn = recruitmentSection.NameEn });
                }

                if (desigsToSeed.Any())
                {
                    await context.Designations.AddRangeAsync(desigsToSeed);
                    await context.SaveChangesAsync();
                }
            }

            // Seed Lines if empty
            if (!await context.Lines.AnyAsync())
            {
                var sewingSection = await context.Sections.FirstOrDefaultAsync(s => s.NameEn == "Sewing");
                var finishingSection = await context.Sections.FirstOrDefaultAsync(s => s.NameEn == "Finishing");

                var linesToSeed = new List<Line>();

                if (sewingSection != null)
                {
                    linesToSeed.Add(new Line { NameEn = "Assembly Line A", NameBn = "অ্যাসেম্বলি লাইন এ", SectionId = sewingSection.Id, SectionNameEn = sewingSection.NameEn });
                    linesToSeed.Add(new Line { NameEn = "Assembly Line B", NameBn = "অ্যাসেম্বলি লাইন বি", SectionId = sewingSection.Id, SectionNameEn = sewingSection.NameEn });
                }
                if (finishingSection != null)
                {
                    linesToSeed.Add(new Line { NameEn = "Packing Line 1", NameBn = "প্যাকিং লাইন ১", SectionId = finishingSection.Id, SectionNameEn = finishingSection.NameEn });
                    linesToSeed.Add(new Line { NameEn = "Inspection Line", NameBn = "নিরীক্ষা লাইন", SectionId = finishingSection.Id, SectionNameEn = finishingSection.NameEn });
                }

                if (linesToSeed.Any())
                {
                    await context.Lines.AddRangeAsync(linesToSeed);
                    await context.SaveChangesAsync();
                }
            }

            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }
    }
}
