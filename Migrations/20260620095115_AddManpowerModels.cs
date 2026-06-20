using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class AddManpowerModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "manpowerrequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    HeadcountNeeded = table.Column<int>(type: "int", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_manpowerrequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_manpowerrequirements_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_manpowerrequirements_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_manpowerrequirements_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "manpowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    TargetCapacity = table.Column<int>(type: "int", nullable: false),
                    CurrentHeadcount = table.Column<int>(type: "int", nullable: false),
                    Vacancies = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_manpowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_manpowers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_manpowers_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_manpowers_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "separations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: true),
                    SeparationType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ResignDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastWorkingDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitInterviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HandoverNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ClearanceProgress = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_separations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_separations_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_separations_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_separations_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_manpowerrequirements_DepartmentId",
                table: "manpowerrequirements",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_manpowerrequirements_DesignationId",
                table: "manpowerrequirements",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_manpowerrequirements_Priority",
                table: "manpowerrequirements",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_manpowerrequirements_SectionId",
                table: "manpowerrequirements",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_manpowerrequirements_Status",
                table: "manpowerrequirements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_manpowers_DepartmentId_SectionId_DesignationId",
                table: "manpowers",
                columns: new[] { "DepartmentId", "SectionId", "DesignationId" },
                unique: true,
                filter: "[SectionId] IS NOT NULL AND [DesignationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_manpowers_DesignationId",
                table: "manpowers",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_manpowers_SectionId",
                table: "manpowers",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_separations_DepartmentId",
                table: "separations",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_separations_DesignationId",
                table: "separations",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_separations_EmployeeId",
                table: "separations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_separations_ResignDate",
                table: "separations",
                column: "ResignDate");

            migrationBuilder.CreateIndex(
                name: "IX_separations_SectionId",
                table: "separations",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_separations_Status",
                table: "separations",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "manpowerrequirements");

            migrationBuilder.DropTable(
                name: "manpowers");

            migrationBuilder.DropTable(
                name: "separations");
        }
    }
}
