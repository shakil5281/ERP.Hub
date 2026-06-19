using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPHub.Migrations
{
    public partial class SimplifyPunchRecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @ NVARCHAR(128) = (SELECT name FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('punchrecords'));
                IF @ IS NOT NULL EXEC('ALTER TABLE punchrecords DROP CONSTRAINT [' + @ + ']');
            ");

            migrationBuilder.Sql(@"
                DECLARE @ NVARCHAR(128) = (SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('punchrecords') AND name LIKE 'IX_punchrecords_EmployeeId%' AND is_unique = 1);
                IF @ IS NOT NULL EXEC('DROP INDEX [' + @ + '] ON punchrecords');
            ");

            migrationBuilder.Sql(@"
                DECLARE @ NVARCHAR(128) = (SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('punchrecords') AND name = 'IX_punchrecords_LogDateTime');
                IF @ IS NOT NULL EXEC('DROP INDEX [' + @ + '] ON punchrecords');
            ");

            var cols = new[] { "EmployeeName", "LogType", "DeviceTerminal", "VerificationMode", "CreatedAt", "IsProcessed" };
            foreach (var col in cols)
            {
                migrationBuilder.Sql($@"
                    IF COL_LENGTH('punchrecords', '{col}') IS NOT NULL
                        ALTER TABLE punchrecords DROP COLUMN [{col}];
                ");
            }

            migrationBuilder.Sql(@"
                IF COL_LENGTH('punchrecords', 'DeviceId') IS NULL
                    ALTER TABLE punchrecords ADD [DeviceId] NVARCHAR(50) NOT NULL DEFAULT '';
            ");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_punchrecords_EmployeeId_LogDateTime
                ON punchrecords (EmployeeId, LogDateTime);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IX_punchrecords_LogDateTime
                ON punchrecords (LogDateTime);
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IX_punchrecords_DeviceId
                ON punchrecords (DeviceId);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_punchrecords_EmployeeId_LogDateTime ON punchrecords");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_punchrecords_LogDateTime ON punchrecords");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_punchrecords_DeviceId ON punchrecords");

            migrationBuilder.Sql("ALTER TABLE punchrecords ADD [EmployeeName] NVARCHAR(100) NOT NULL DEFAULT ''");
            migrationBuilder.Sql("ALTER TABLE punchrecords ADD [LogType] NVARCHAR(20) NOT NULL DEFAULT 'I'");
            migrationBuilder.Sql("ALTER TABLE punchrecords ADD [DeviceTerminal] NVARCHAR(50) NOT NULL DEFAULT ''");
            migrationBuilder.Sql("ALTER TABLE punchrecords ADD [VerificationMode] NVARCHAR(50) NOT NULL DEFAULT ''");
            migrationBuilder.Sql("ALTER TABLE punchrecords ADD [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()");
            migrationBuilder.Sql("ALTER TABLE punchrecords ADD [IsProcessed] BIT NOT NULL DEFAULT 0");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_punchrecords_EmployeeId_LogDateTime
                ON punchrecords (EmployeeId, LogDateTime)
                WHERE [IsProcessed] = 0;
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IX_punchrecords_LogDateTime
                ON punchrecords (LogDateTime);
            ");
        }
    }
}
