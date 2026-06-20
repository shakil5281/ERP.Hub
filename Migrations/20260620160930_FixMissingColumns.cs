using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the previously recorded but failed migration
            migrationBuilder.Sql("DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260620160145_AddMissingEmployeeFields'");

            // Rename columns that were renamed in the previous attempt
            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='IsActive') EXEC sp_rename N'[Employees].[IsActive]', N'OverTimeStatus', 'COLUMN'");
            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='Address') EXEC sp_rename N'[Employees].[Address]', N'SpouseName', 'COLUMN'");

            // Add all missing columns
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='DateOfBirth') ALTER TABLE [Employees] ADD [DateOfBirth] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='Gender') ALTER TABLE [Employees] ADD [Gender] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='ChildrenCount') ALTER TABLE [Employees] ADD [ChildrenCount] int NOT NULL DEFAULT 0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='AccountType') ALTER TABLE [Employees] ADD [AccountType] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='AccountNumber') ALTER TABLE [Employees] ADD [AccountNumber] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='GrossSalary') ALTER TABLE [Employees] ADD [GrossSalary] decimal(18,2) NOT NULL DEFAULT 0.0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PhotoBase64') ALTER TABLE [Employees] ADD [PhotoBase64] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='SignatureBase64') ALTER TABLE [Employees] ADD [SignatureBase64] nvarchar(max) NOT NULL DEFAULT N''");

            // Present Address columns
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentVillage') ALTER TABLE [Employees] ADD [PresentVillage] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentPostOffice') ALTER TABLE [Employees] ADD [PresentPostOffice] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentDivisionId') ALTER TABLE [Employees] ADD [PresentDivisionId] int NOT NULL DEFAULT 0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentDistrictId') ALTER TABLE [Employees] ADD [PresentDistrictId] int NOT NULL DEFAULT 0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentUpazilaId') ALTER TABLE [Employees] ADD [PresentUpazilaId] int NOT NULL DEFAULT 0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentPostalCode') ALTER TABLE [Employees] ADD [PresentPostalCode] nvarchar(max) NOT NULL DEFAULT N''");

            // Permanent Address columns
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentVillage') ALTER TABLE [Employees] ADD [PermanentVillage] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentPostOffice') ALTER TABLE [Employees] ADD [PermanentPostOffice] nvarchar(max) NOT NULL DEFAULT N''");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentDivisionId') ALTER TABLE [Employees] ADD [PermanentDivisionId] int NOT NULL DEFAULT 0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentDistrictId') ALTER TABLE [Employees] ADD [PermanentDistrictId] int NOT NULL DEFAULT 0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentUpazilaId') ALTER TABLE [Employees] ADD [PermanentUpazilaId] int NOT NULL DEFAULT 0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentPostalCode') ALTER TABLE [Employees] ADD [PermanentPostalCode] nvarchar(max) NOT NULL DEFAULT N''");

            // Employee info columns
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='EmployeeStatus') ALTER TABLE [Employees] ADD [EmployeeStatus] nvarchar(max) NOT NULL DEFAULT N'Regular'");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='EmployeeType') ALTER TABLE [Employees] ADD [EmployeeType] nvarchar(max) NOT NULL DEFAULT N''");

            // CompanyId and OverTimeStatus (OverTimeStatus may already exist from partial rename)
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='CompanyId') ALTER TABLE [Employees] ADD [CompanyId] int NOT NULL DEFAULT 0");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='OverTimeStatus') ALTER TABLE [Employees] ADD [OverTimeStatus] bit NOT NULL DEFAULT 0");

            // Add Company FK constraint if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Employees_Companies_CompanyId')
                BEGIN
                    CREATE INDEX [IX_Employees_CompanyId] ON [Employees] ([CompanyId]);
                    ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
