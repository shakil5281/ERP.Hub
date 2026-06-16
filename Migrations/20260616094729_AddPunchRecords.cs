using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class AddPunchRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "punchrecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LogDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DeviceTerminal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VerificationMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_punchrecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_punchrecords_EmployeeId_LogDateTime",
                table: "punchrecords",
                columns: new[] { "EmployeeId", "LogDateTime" },
                unique: true,
                filter: "[IsProcessed] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_punchrecords_LogDateTime",
                table: "punchrecords",
                column: "LogDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "punchrecords");
        }
    }
}
