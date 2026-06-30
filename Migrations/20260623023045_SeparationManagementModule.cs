using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class SeparationManagementModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Employees: add new columns before dropping EmployeeStatus ---
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.AddColumn<DateTime>(
                name: "SeparationDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeparationType",
                table: "Employees",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeparationReason",
                table: "Employees",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeparationRemarks",
                table: "Employees",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeparationApprovedBy",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SeparationApprovedDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Employees SET Status = 'Active' WHERE EmployeeStatus = 'Regular' OR EmployeeStatus = 'Active' OR EmployeeStatus IS NULL OR EmployeeStatus = '';
                UPDATE Employees SET Status = 'Separation', SeparationType = EmployeeStatus
                  WHERE EmployeeStatus IN ('Resign','Left','Close');
            ");

            migrationBuilder.DropColumn(
                name: "EmployeeStatus",
                table: "Employees");

            // --- Separations: reshape columns ---
            migrationBuilder.RenameColumn(
                name: "ResignDate",
                table: "separations",
                newName: "SeparationDate");

            migrationBuilder.RenameColumn(
                name: "HandoverNotes",
                table: "separations",
                newName: "Remarks");

            migrationBuilder.RenameIndex(
                name: "IX_separations_ResignDate",
                table: "separations",
                newName: "IX_separations_SeparationDate");

            migrationBuilder.Sql(@"
                UPDATE separations SET SeparationDate = LastWorkingDay
                  WHERE LastWorkingDay IS NOT NULL AND LastWorkingDay > SeparationDate;
            ");

            migrationBuilder.DropColumn(
                name: "LastWorkingDay",
                table: "separations");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "separations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.Sql(@"
                UPDATE separations SET CreatedDate = COALESCE(CreatedAt, GETUTCDATE());
            ");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "separations");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "separations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "separations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "separations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeRefId",
                table: "separations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "separations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSettled",
                table: "separations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                UPDATE s SET
                    EmployeeRefId = e.Id,
                    CompanyId = e.CompanyId
                FROM separations s
                INNER JOIN Employees e ON e.EmployeeId = s.EmployeeId;

                UPDATE e SET
                    SeparationDate = s.SeparationDate,
                    SeparationType = CASE WHEN s.SeparationType IN ('Resign','Left','Close') THEN s.SeparationType ELSE e.SeparationType END,
                    SeparationReason = s.Reason,
                    SeparationRemarks = s.Remarks,
                    SeparationApprovedBy = s.ApprovedBy,
                    SeparationApprovedDate = s.ApprovedDate,
                    Status = 'Separation'
                FROM Employees e
                INNER JOIN separations s ON s.EmployeeRefId = e.Id
                WHERE s.IsCancelled = 0;

                DELETE FROM separations
                WHERE EmployeeRefId = 0
                   OR EmployeeRefId NOT IN (SELECT Id FROM Employees);
            ");

            migrationBuilder.CreateTable(
                name: "final_settlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeparationId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BasicSalaryDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimeDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NightBillDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LeaveEncashment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BonusDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPayable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProcessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_final_settlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_final_settlements_separations_SeparationId",
                        column: x => x.SeparationId,
                        principalTable: "separations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_separations_EmployeeRefId",
                table: "separations",
                column: "EmployeeRefId");

            migrationBuilder.CreateIndex(
                name: "IX_separations_SeparationType_SeparationDate",
                table: "separations",
                columns: new[] { "SeparationType", "SeparationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SeparationDate",
                table: "Employees",
                column: "SeparationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Status",
                table: "Employees",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_final_settlements_SeparationId",
                table: "final_settlements",
                column: "SeparationId");

            migrationBuilder.AddForeignKey(
                name: "FK_separations_Employees_EmployeeRefId",
                table: "separations",
                column: "EmployeeRefId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_separations_Employees_EmployeeRefId",
                table: "separations");

            migrationBuilder.DropTable(
                name: "final_settlements");

            migrationBuilder.DropIndex(
                name: "IX_separations_EmployeeRefId",
                table: "separations");

            migrationBuilder.DropIndex(
                name: "IX_separations_SeparationType_SeparationDate",
                table: "separations");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SeparationDate",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Status",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "separations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "separations");

            migrationBuilder.DropColumn(
                name: "EmployeeRefId",
                table: "separations");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "separations");

            migrationBuilder.DropColumn(
                name: "IsSettled",
                table: "separations");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "separations");

            migrationBuilder.DropColumn(
                name: "SeparationApprovedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SeparationApprovedDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SeparationDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SeparationReason",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SeparationRemarks",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SeparationType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "SeparationDate",
                table: "separations",
                newName: "ResignDate");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "separations",
                newName: "HandoverNotes");

            migrationBuilder.RenameIndex(
                name: "IX_separations_SeparationDate",
                table: "separations",
                newName: "IX_separations_ResignDate");

            migrationBuilder.AlterColumn<string>(
                name: "ApprovedBy",
                table: "separations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "separations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWorkingDay",
                table: "separations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EmployeeStatus",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Regular");
        }
    }
}
