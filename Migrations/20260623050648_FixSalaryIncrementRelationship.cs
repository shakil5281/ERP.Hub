using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class FixSalaryIncrementRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salary_increments_Employees_EmployeeId1",
                table: "salary_increments");

            migrationBuilder.DropIndex(
                name: "IX_salary_increments_EmployeeId1",
                table: "salary_increments");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "salary_increments");

            migrationBuilder.AddForeignKey(
                name: "FK_salary_increments_Employees_EmployeeRefId",
                table: "salary_increments",
                column: "EmployeeRefId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salary_increments_Employees_EmployeeRefId",
                table: "salary_increments");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                table: "salary_increments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_salary_increments_EmployeeId1",
                table: "salary_increments",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_salary_increments_Employees_EmployeeId1",
                table: "salary_increments",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
