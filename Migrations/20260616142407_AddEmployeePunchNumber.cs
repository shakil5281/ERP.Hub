using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePunchNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "punchrecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PunchNumber",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PunchNumber",
                table: "Employees",
                column: "PunchNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_PunchNumber",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PunchNumber",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "punchrecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
