using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "leaveapplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "leaveapplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "leaveapplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "leaveapplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DesignationId",
                table: "leaveapplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "leaveapplications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LeaveTypeId",
                table: "leaveapplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                table: "leaveapplications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalDays",
                table: "leaveapplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "leaveapplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "leavetypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaxDaysPerYear = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    AccrualType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RequiresMedicalCertificate = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leavetypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_leaveapplications_DepartmentId",
                table: "leaveapplications",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_leaveapplications_DesignationId",
                table: "leaveapplications",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_leaveapplications_EmployeeId",
                table: "leaveapplications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_leaveapplications_LeaveDate",
                table: "leaveapplications",
                column: "LeaveDate");

            migrationBuilder.CreateIndex(
                name: "IX_leaveapplications_LeaveTypeId",
                table: "leaveapplications",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_leaveapplications_Status",
                table: "leaveapplications",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_leaveapplications_Departments_DepartmentId",
                table: "leaveapplications",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_leaveapplications_Designations_DesignationId",
                table: "leaveapplications",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_leaveapplications_leavetypes_LeaveTypeId",
                table: "leaveapplications",
                column: "LeaveTypeId",
                principalTable: "leavetypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leaveapplications_Departments_DepartmentId",
                table: "leaveapplications");

            migrationBuilder.DropForeignKey(
                name: "FK_leaveapplications_Designations_DesignationId",
                table: "leaveapplications");

            migrationBuilder.DropForeignKey(
                name: "FK_leaveapplications_leavetypes_LeaveTypeId",
                table: "leaveapplications");

            migrationBuilder.DropTable(
                name: "leavetypes");

            migrationBuilder.DropIndex(
                name: "IX_leaveapplications_DepartmentId",
                table: "leaveapplications");

            migrationBuilder.DropIndex(
                name: "IX_leaveapplications_DesignationId",
                table: "leaveapplications");

            migrationBuilder.DropIndex(
                name: "IX_leaveapplications_EmployeeId",
                table: "leaveapplications");

            migrationBuilder.DropIndex(
                name: "IX_leaveapplications_LeaveDate",
                table: "leaveapplications");

            migrationBuilder.DropIndex(
                name: "IX_leaveapplications_LeaveTypeId",
                table: "leaveapplications");

            migrationBuilder.DropIndex(
                name: "IX_leaveapplications_Status",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "LeaveTypeId",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "RejectedReason",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "TotalDays",
                table: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "leaveapplications");
        }
    }
}
