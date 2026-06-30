using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class PayrollManagementModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AttendanceBonus",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "FoodAllowance",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HouseRent",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MedicalAllowance",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductionBonus",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RoutingNumber",
                table: "Employees",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SpecialAllowance",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TransportAllowance",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AbsentDeduction",
                table: "dailysalaryrecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AdvanceDeduction",
                table: "dailysalaryrecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyGross",
                table: "dailysalaryrecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HolidayBillPay",
                table: "dailysalaryrecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LateDeduction",
                table: "dailysalaryrecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanDeduction",
                table: "dailysalaryrecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LwopDeduction",
                table: "dailysalaryrecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NightBillPay",
                table: "dailysalaryrecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "employee_loans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeRefId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    MonthlyEmi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisbursedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_loans_Employees_EmployeeRefId",
                        column: x => x.EmployeeRefId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employee_salary_assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeRefId = table.Column<int>(type: "int", nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HouseRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedicalAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FoodAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpecialAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AttendanceBonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductionBonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_salary_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_salary_assignments_Employees_EmployeeRefId",
                        column: x => x.EmployeeRefId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "holiday_bill_entries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HolidayPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_bill_entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "night_bill_entries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NightHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NightPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_night_bill_entries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payroll_runs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PayrollMonth = table.Column<int>(type: "int", nullable: false),
                    PayrollYear = table.Column<int>(type: "int", nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TotalEmployees = table.Column<int>(type: "int", nullable: false),
                    TotalGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalNet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOvertime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalNightBill = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalHolidayBill = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalculatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CalculatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VerifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LockedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payroll_runs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "salary_advances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeRefId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdvanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    MonthlyDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_advances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salary_advances_Employees_EmployeeRefId",
                        column: x => x.EmployeeRefId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "salary_grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GradeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    MinGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salary_history",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeRefId = table.Column<int>(type: "int", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SourceRefId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salary_increments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeRefId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId1 = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IncrementType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousBasic = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncrementAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncrementPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewBasic = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_increments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salary_increments_Employees_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "payroll_lines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayrollRunId = table.Column<int>(type: "int", nullable: false),
                    EmployeeRefId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DesignationId = table.Column<int>(type: "int", nullable: false),
                    WorkingDays = table.Column<int>(type: "int", nullable: false),
                    PresentDays = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AbsentDays = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LeaveDays = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtHours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HouseRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedicalAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FoodAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpecialAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AttendanceBonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductionBonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimePay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NightBillPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HolidayBillPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AbsentDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LwopDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdvanceDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RoutingNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payroll_lines_payroll_runs_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalTable: "payroll_runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employee_loans_EmployeeRefId",
                table: "employee_loans",
                column: "EmployeeRefId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_salary_assignments_EmployeeRefId_EffectiveFrom",
                table: "employee_salary_assignments",
                columns: new[] { "EmployeeRefId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_lines_PayrollRunId",
                table: "payroll_lines",
                column: "PayrollRunId");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_runs_CompanyId_PayrollYear_PayrollMonth",
                table: "payroll_runs",
                columns: new[] { "CompanyId", "PayrollYear", "PayrollMonth" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salary_advances_EmployeeRefId",
                table: "salary_advances",
                column: "EmployeeRefId");

            migrationBuilder.CreateIndex(
                name: "IX_salary_increments_EmployeeId1",
                table: "salary_increments",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_salary_increments_EmployeeRefId",
                table: "salary_increments",
                column: "EmployeeRefId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_loans");

            migrationBuilder.DropTable(
                name: "employee_salary_assignments");

            migrationBuilder.DropTable(
                name: "holiday_bill_entries");

            migrationBuilder.DropTable(
                name: "night_bill_entries");

            migrationBuilder.DropTable(
                name: "payroll_lines");

            migrationBuilder.DropTable(
                name: "salary_advances");

            migrationBuilder.DropTable(
                name: "salary_grades");

            migrationBuilder.DropTable(
                name: "salary_history");

            migrationBuilder.DropTable(
                name: "salary_increments");

            migrationBuilder.DropTable(
                name: "payroll_runs");

            migrationBuilder.DropColumn(
                name: "AttendanceBonus",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "FoodAllowance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "HouseRent",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "MedicalAllowance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ProductionBonus",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RoutingNumber",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SpecialAllowance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TransportAllowance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AbsentDeduction",
                table: "dailysalaryrecords");

            migrationBuilder.DropColumn(
                name: "AdvanceDeduction",
                table: "dailysalaryrecords");

            migrationBuilder.DropColumn(
                name: "DailyGross",
                table: "dailysalaryrecords");

            migrationBuilder.DropColumn(
                name: "HolidayBillPay",
                table: "dailysalaryrecords");

            migrationBuilder.DropColumn(
                name: "LateDeduction",
                table: "dailysalaryrecords");

            migrationBuilder.DropColumn(
                name: "LoanDeduction",
                table: "dailysalaryrecords");

            migrationBuilder.DropColumn(
                name: "LwopDeduction",
                table: "dailysalaryrecords");

            migrationBuilder.DropColumn(
                name: "NightBillPay",
                table: "dailysalaryrecords");
        }
    }
}
