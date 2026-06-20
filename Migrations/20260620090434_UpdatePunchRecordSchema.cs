using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePunchRecordSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_punchrecords_EmployeeId_LogDateTime",
                table: "punchrecords");

            migrationBuilder.AddColumn<int>(
                name: "PunchNumber",
                table: "punchrecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserPunchId",
                table: "punchrecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE punchrecords
                SET UserPunchId = TRY_CAST(EmployeeId AS int)
                WHERE TRY_CAST(EmployeeId AS int) IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "punchrecords");

            migrationBuilder.CreateIndex(
                name: "IX_punchrecords_PunchNumber",
                table: "punchrecords",
                column: "PunchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_punchrecords_UserPunchId_LogDateTime",
                table: "punchrecords",
                columns: new[] { "UserPunchId", "LogDateTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_punchrecords_PunchNumber",
                table: "punchrecords");

            migrationBuilder.DropIndex(
                name: "IX_punchrecords_UserPunchId_LogDateTime",
                table: "punchrecords");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "punchrecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE punchrecords
                SET EmployeeId = CAST(UserPunchId AS nvarchar(50))
                WHERE UserPunchId > 0;
                """);

            migrationBuilder.DropColumn(
                name: "PunchNumber",
                table: "punchrecords");

            migrationBuilder.DropColumn(
                name: "UserPunchId",
                table: "punchrecords");

            migrationBuilder.CreateIndex(
                name: "IX_punchrecords_EmployeeId_LogDateTime",
                table: "punchrecords",
                columns: new[] { "EmployeeId", "LogDateTime" },
                unique: true);
        }
    }
}
