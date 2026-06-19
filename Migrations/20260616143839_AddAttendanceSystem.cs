using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BreakMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DuplicateIntervalMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GraceInMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HalfDayThresholdMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinimumOvertimeMinutes",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "attendancerecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftId = table.Column<int>(type: "int", nullable: false),
                    ActualInPunch = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualOutPunch = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttendanceInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    AttendanceOutTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    LateMinutes = table.Column<int>(type: "int", nullable: false),
                    EarlyExitMinutes = table.Column<int>(type: "int", nullable: false),
                    WorkedMinutes = table.Column<int>(type: "int", nullable: false),
                    OvertimeMinutes = table.Column<int>(type: "int", nullable: false),
                    AttendanceStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendancerecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HolidayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "leaveapplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LeaveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leaveapplications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attendancerecords_AttendanceDate",
                table: "attendancerecords",
                column: "AttendanceDate");

            migrationBuilder.CreateIndex(
                name: "IX_attendancerecords_EmployeeId_AttendanceDate",
                table: "attendancerecords",
                columns: new[] { "EmployeeId", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_HolidayDate",
                table: "Holidays",
                column: "HolidayDate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendancerecords");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "leaveapplications");

            migrationBuilder.DropColumn(
                name: "BreakMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "DuplicateIntervalMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "GraceInMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "HalfDayThresholdMinutes",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "MinimumOvertimeMinutes",
                table: "Shifts");
        }
    }
}
