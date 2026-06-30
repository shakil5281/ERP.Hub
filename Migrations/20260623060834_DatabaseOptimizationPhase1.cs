using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseOptimizationPhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_BusinessGroups_BusinessGroupId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_BusinessGroups_BusinessGroupId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_leaveapplications_leavetypes_LeaveTypeId",
                table: "leaveapplications");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "salary_grades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "RolePermission");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Permission");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermission",
                newName: "IX_RolePermission_PermissionId");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermission",
                table: "RolePermission",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permission",
                table: "Permission",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_separations_CompanyId_SeparationDate",
                table: "separations",
                columns: new[] { "CompanyId", "SeparationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_punchrecords_PunchNumber_LogDateTime",
                table: "punchrecords",
                columns: new[] { "PunchNumber", "LogDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_leaveapplications_EmployeeId_LeaveDate_Status",
                table: "leaveapplications",
                columns: new[] { "EmployeeId", "LeaveDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeId",
                table: "Employees",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Status_JoiningDate_SeparationDate",
                table: "Employees",
                columns: new[] { "Status", "JoiningDate", "SeparationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_dailysalaryrecords_EmployeeId_SalaryDate",
                table: "dailysalaryrecords",
                columns: new[] { "EmployeeId", "SalaryDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_attendancerecords_AttendanceDate_EmployeeId",
                table: "attendancerecords",
                columns: new[] { "AttendanceDate", "EmployeeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_BusinessGroups_BusinessGroupId",
                table: "Companies",
                column: "BusinessGroupId",
                principalTable: "BusinessGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_BusinessGroups_BusinessGroupId",
                table: "Employees",
                column: "BusinessGroupId",
                principalTable: "BusinessGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_leaveapplications_leavetypes_LeaveTypeId",
                table: "leaveapplications",
                column: "LeaveTypeId",
                principalTable: "leavetypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Roles_RoleId",
                table: "RolePermission",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_BusinessGroups_BusinessGroupId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_BusinessGroups_BusinessGroupId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_leaveapplications_leavetypes_LeaveTypeId",
                table: "leaveapplications");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Roles_RoleId",
                table: "RolePermission");

            migrationBuilder.DropIndex(
                name: "IX_separations_CompanyId_SeparationDate",
                table: "separations");

            migrationBuilder.DropIndex(
                name: "IX_punchrecords_PunchNumber_LogDateTime",
                table: "punchrecords");

            migrationBuilder.DropIndex(
                name: "IX_leaveapplications_EmployeeId_LeaveDate_Status",
                table: "leaveapplications");

            migrationBuilder.DropIndex(
                name: "IX_Employees_EmployeeId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Status_JoiningDate_SeparationDate",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_dailysalaryrecords_EmployeeId_SalaryDate",
                table: "dailysalaryrecords");

            migrationBuilder.DropIndex(
                name: "IX_attendancerecords_AttendanceDate_EmployeeId",
                table: "attendancerecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermission",
                table: "RolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permission",
                table: "Permission");

            migrationBuilder.RenameTable(
                name: "RolePermission",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "Permission",
                newName: "Permissions");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_PermissionId");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salary_grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    GradeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GradeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_grades", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_BusinessGroups_BusinessGroupId",
                table: "Companies",
                column: "BusinessGroupId",
                principalTable: "BusinessGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_BusinessGroups_BusinessGroupId",
                table: "Employees",
                column: "BusinessGroupId",
                principalTable: "BusinessGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_leaveapplications_leavetypes_LeaveTypeId",
                table: "leaveapplications",
                column: "LeaveTypeId",
                principalTable: "leavetypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
