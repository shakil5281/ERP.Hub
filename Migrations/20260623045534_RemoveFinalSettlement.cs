using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFinalSettlement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "final_settlements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "final_settlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeparationId = table.Column<int>(type: "int", nullable: false),
                    BasicSalaryDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BonusDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LeaveEncashment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPayable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NightBillDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OvertimeDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
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
                name: "IX_final_settlements_SeparationId",
                table: "final_settlements",
                column: "SeparationId");
        }
    }
}
