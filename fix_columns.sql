IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Companies] (
        [Id] int NOT NULL IDENTITY,
        [CompanyNameEn] nvarchar(100) NOT NULL,
        [CompanyNameBn] nvarchar(100) NOT NULL,
        [AddressEn] nvarchar(200) NOT NULL,
        [AddressBn] nvarchar(200) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [Signature] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Departments] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameBn] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Invoices] (
        [Id] int NOT NULL IDENTITY,
        [InvoiceNumber] nvarchar(max) NOT NULL,
        [ClientName] nvarchar(max) NOT NULL,
        [ClientEmail] nvarchar(max) NOT NULL,
        [IssueDate] datetime2 NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [TaxRate] decimal(18,4) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Sku] nvarchar(max) NOT NULL,
        [Category] nvarchar(max) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [Stock] int NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [LastUpdated] datetime2 NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [ProjectTasks] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(100) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [Priority] int NOT NULL,
        [Assignee] nvarchar(max) NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_ProjectTasks] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Shifts] (
        [Id] int NOT NULL IDENTITY,
        [ShiftName] nvarchar(100) NOT NULL,
        [InTime] time NOT NULL,
        [OutTime] time NOT NULL,
        [LateTime] time NOT NULL,
        [OffDay] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Shifts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [Role] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Sections] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameBn] nvarchar(100) NOT NULL,
        [DepartmentId] int NOT NULL,
        [DepartmentNameEn] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Sections] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sections_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [InvoiceItems] (
        [Id] int NOT NULL IDENTITY,
        [Description] nvarchar(max) NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [InvoiceId] int NULL,
        CONSTRAINT [PK_InvoiceItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InvoiceItems_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Designations] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameBn] nvarchar(100) NOT NULL,
        [SectionId] int NOT NULL,
        [SectionNameEn] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Designations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Designations_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Lines] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameBn] nvarchar(100) NOT NULL,
        [SectionId] int NOT NULL,
        [SectionNameEn] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Lines] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Lines_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(max) NOT NULL,
        [EmployeeName] nvarchar(max) NOT NULL,
        [FatherName] nvarchar(max) NOT NULL,
        [MotherName] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [NID] nvarchar(max) NOT NULL,
        [MobileNo] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [DepartmentId] int NOT NULL,
        [SectionId] int NOT NULL,
        [DesignationId] int NOT NULL,
        [LineId] int NOT NULL,
        [ShiftId] int NOT NULL,
        [JoiningDate] datetime2 NOT NULL,
        [BasicSalary] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employees_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [Designations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employees_Lines_LineId] FOREIGN KEY ([LineId]) REFERENCES [Lines] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employees_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employees_Shifts_ShiftId] FOREIGN KEY ([ShiftId]) REFERENCES [Shifts] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Designations_SectionId] ON [Designations] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Employees_DepartmentId] ON [Employees] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Employees_DesignationId] ON [Employees] ([DesignationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Employees_LineId] ON [Employees] ([LineId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Employees_SectionId] ON [Employees] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Employees_ShiftId] ON [Employees] ([ShiftId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InvoiceItems_InvoiceId] ON [InvoiceItems] ([InvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Lines_SectionId] ON [Lines] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Sections_DepartmentId] ON [Sections] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616090533_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616090533_InitialCreate', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616094729_AddPunchRecords'
)
BEGIN
    CREATE TABLE [punchrecords] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(50) NOT NULL,
        [EmployeeName] nvarchar(100) NOT NULL,
        [LogDateTime] datetime2 NOT NULL,
        [LogType] nvarchar(20) NOT NULL,
        [DeviceTerminal] nvarchar(50) NOT NULL,
        [VerificationMode] nvarchar(50) NOT NULL,
        [DeviceId] nvarchar(50) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [IsProcessed] bit NOT NULL,
        CONSTRAINT [PK_punchrecords] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616094729_AddPunchRecords'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_punchrecords_EmployeeId_LogDateTime] ON [punchrecords] ([EmployeeId], [LogDateTime]) WHERE [IsProcessed] = 0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616094729_AddPunchRecords'
)
BEGIN
    CREATE INDEX [IX_punchrecords_LogDateTime] ON [punchrecords] ([LogDateTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616094729_AddPunchRecords'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616094729_AddPunchRecords', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                    DECLARE @ NVARCHAR(128) = (SELECT name FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('punchrecords'));
                    IF @ IS NOT NULL EXEC('ALTER TABLE punchrecords DROP CONSTRAINT [' + @ + ']');
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                    DECLARE @ NVARCHAR(128) = (SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('punchrecords') AND name LIKE 'IX_punchrecords_EmployeeId%' AND is_unique = 1);
                    IF @ IS NOT NULL EXEC('DROP INDEX [' + @ + '] ON punchrecords');
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                    DECLARE @ NVARCHAR(128) = (SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('punchrecords') AND name = 'IX_punchrecords_LogDateTime');
                    IF @ IS NOT NULL EXEC('DROP INDEX [' + @ + '] ON punchrecords');
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                        IF COL_LENGTH('punchrecords', 'EmployeeName') IS NOT NULL
                            ALTER TABLE punchrecords DROP COLUMN [EmployeeName];
                    
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                        IF COL_LENGTH('punchrecords', 'LogType') IS NOT NULL
                            ALTER TABLE punchrecords DROP COLUMN [LogType];
                    
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                        IF COL_LENGTH('punchrecords', 'DeviceTerminal') IS NOT NULL
                            ALTER TABLE punchrecords DROP COLUMN [DeviceTerminal];
                    
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                        IF COL_LENGTH('punchrecords', 'VerificationMode') IS NOT NULL
                            ALTER TABLE punchrecords DROP COLUMN [VerificationMode];
                    
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                        IF COL_LENGTH('punchrecords', 'CreatedAt') IS NOT NULL
                            ALTER TABLE punchrecords DROP COLUMN [CreatedAt];
                    
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                        IF COL_LENGTH('punchrecords', 'IsProcessed') IS NOT NULL
                            ALTER TABLE punchrecords DROP COLUMN [IsProcessed];
                    
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                    IF COL_LENGTH('punchrecords', 'DeviceId') IS NULL
                        ALTER TABLE punchrecords ADD [DeviceId] NVARCHAR(50) NOT NULL DEFAULT '';
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                    CREATE UNIQUE INDEX IX_punchrecords_EmployeeId_LogDateTime
                    ON punchrecords (EmployeeId, LogDateTime);
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                    CREATE INDEX IX_punchrecords_LogDateTime
                    ON punchrecords (LogDateTime);
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN

                    CREATE INDEX IX_punchrecords_DeviceId
                    ON punchrecords (DeviceId);
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616134050_SimplifyPunchRecords'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616134050_SimplifyPunchRecords', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616142407_AddEmployeePunchNumber'
)
BEGIN
    DROP INDEX [IX_punchrecords_DeviceId] ON [punchrecords];
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[punchrecords]') AND [c].[name] = N'DeviceId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [punchrecords] DROP CONSTRAINT ' + @var + ';');
    EXEC(N'UPDATE [punchrecords] SET [DeviceId] = N'''' WHERE [DeviceId] IS NULL');
    ALTER TABLE [punchrecords] ALTER COLUMN [DeviceId] nvarchar(50) NOT NULL;
    ALTER TABLE [punchrecords] ADD DEFAULT N'' FOR [DeviceId];
    CREATE INDEX [IX_punchrecords_DeviceId] ON [punchrecords] ([DeviceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616142407_AddEmployeePunchNumber'
)
BEGIN
    ALTER TABLE [Employees] ADD [PunchNumber] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616142407_AddEmployeePunchNumber'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Employees_PunchNumber] ON [Employees] ([PunchNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616142407_AddEmployeePunchNumber'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616142407_AddEmployeePunchNumber', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    ALTER TABLE [Shifts] ADD [BreakMinutes] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    ALTER TABLE [Shifts] ADD [DuplicateIntervalMinutes] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    ALTER TABLE [Shifts] ADD [GraceInMinutes] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    ALTER TABLE [Shifts] ADD [HalfDayThresholdMinutes] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    ALTER TABLE [Shifts] ADD [MinimumOvertimeMinutes] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    CREATE TABLE [attendancerecords] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(50) NOT NULL,
        [AttendanceDate] datetime2 NOT NULL,
        [ShiftId] int NOT NULL,
        [ActualInPunch] datetime2 NULL,
        [ActualOutPunch] datetime2 NULL,
        [AttendanceInTime] time NULL,
        [AttendanceOutTime] time NULL,
        [LateMinutes] int NOT NULL,
        [EarlyExitMinutes] int NOT NULL,
        [WorkedMinutes] int NOT NULL,
        [OvertimeMinutes] int NOT NULL,
        [AttendanceStatus] nvarchar(50) NOT NULL,
        [Remarks] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_attendancerecords] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    CREATE TABLE [Holidays] (
        [Id] int NOT NULL IDENTITY,
        [HolidayName] nvarchar(200) NOT NULL,
        [HolidayDate] datetime2 NOT NULL,
        [IsRecurring] bit NOT NULL,
        CONSTRAINT [PK_Holidays] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    CREATE TABLE [leaveapplications] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(50) NOT NULL,
        [EmployeeName] nvarchar(100) NOT NULL,
        [LeaveDate] datetime2 NOT NULL,
        [LeaveType] nvarchar(50) NOT NULL,
        [Reason] nvarchar(500) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_leaveapplications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    CREATE INDEX [IX_attendancerecords_AttendanceDate] ON [attendancerecords] ([AttendanceDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    CREATE UNIQUE INDEX [IX_attendancerecords_EmployeeId_AttendanceDate] ON [attendancerecords] ([EmployeeId], [AttendanceDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Holidays_HolidayDate] ON [Holidays] ([HolidayDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260616143839_AddAttendanceSystem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260616143839_AddAttendanceSystem', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620080812_AddAttendanceSystemExtensions'
)
BEGIN
    CREATE TABLE [dailysalaryrecords] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(50) NOT NULL,
        [SalaryDate] datetime2 NOT NULL,
        [DailyBasic] decimal(18,2) NOT NULL,
        [OtHours] float NOT NULL,
        [OtPay] decimal(18,2) NOT NULL,
        [Allowances] decimal(18,2) NOT NULL,
        [Deductions] decimal(18,2) NOT NULL,
        [NetPay] decimal(18,2) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_dailysalaryrecords] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620080812_AddAttendanceSystemExtensions'
)
BEGIN
    CREATE TABLE [jobcards] (
        [Id] int NOT NULL IDENTITY,
        [CardId] nvarchar(50) NOT NULL,
        [EmployeeId] nvarchar(50) NOT NULL,
        [CardType] nvarchar(100) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [AuthorizedBy] nvarchar(100) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_jobcards] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620080812_AddAttendanceSystemExtensions'
)
BEGIN
    CREATE TABLE [manualpunchlogs] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(50) NOT NULL,
        [PunchDate] datetime2 NOT NULL,
        [PunchTime] time NOT NULL,
        [PunchType] nvarchar(50) NOT NULL,
        [Reason] nvarchar(255) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_manualpunchlogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620080812_AddAttendanceSystemExtensions'
)
BEGIN
    CREATE TABLE [overtimedeductions] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(50) NOT NULL,
        [DeductionDate] datetime2 NOT NULL,
        [BaseOtHours] float NOT NULL,
        [DeductedHours] float NOT NULL,
        [Reason] nvarchar(255) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_overtimedeductions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620080812_AddAttendanceSystemExtensions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620080812_AddAttendanceSystemExtensions', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620084058_AddBusinessGroup'
)
BEGIN
    CREATE TABLE [BusinessGroups] (
        [Id] int NOT NULL IDENTITY,
        [GroupName] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_BusinessGroups] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620084058_AddBusinessGroup'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620084058_AddBusinessGroup', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620090434_UpdatePunchRecordSchema'
)
BEGIN
    DROP INDEX [IX_punchrecords_EmployeeId_LogDateTime] ON [punchrecords];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620090434_UpdatePunchRecordSchema'
)
BEGIN
    ALTER TABLE [punchrecords] ADD [PunchNumber] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620090434_UpdatePunchRecordSchema'
)
BEGIN
    ALTER TABLE [punchrecords] ADD [UserPunchId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620090434_UpdatePunchRecordSchema'
)
BEGIN
    UPDATE punchrecords
    SET UserPunchId = TRY_CAST(EmployeeId AS int)
    WHERE TRY_CAST(EmployeeId AS int) IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620090434_UpdatePunchRecordSchema'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[punchrecords]') AND [c].[name] = N'EmployeeId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [punchrecords] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [punchrecords] DROP COLUMN [EmployeeId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620090434_UpdatePunchRecordSchema'
)
BEGIN
    CREATE INDEX [IX_punchrecords_PunchNumber] ON [punchrecords] ([PunchNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620090434_UpdatePunchRecordSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_punchrecords_UserPunchId_LogDateTime] ON [punchrecords] ([UserPunchId], [LogDateTime]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620090434_UpdatePunchRecordSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620090434_UpdatePunchRecordSchema', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE TABLE [manpowerrequirements] (
        [Id] int NOT NULL IDENTITY,
        [RoleTitle] nvarchar(100) NOT NULL,
        [DepartmentId] int NOT NULL,
        [SectionId] int NULL,
        [DesignationId] int NULL,
        [HeadcountNeeded] int NOT NULL,
        [TargetDate] datetime2 NOT NULL,
        [Priority] nvarchar(20) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [Requirements] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_manpowerrequirements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_manpowerrequirements_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_manpowerrequirements_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [Designations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_manpowerrequirements_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE TABLE [manpowers] (
        [Id] int NOT NULL IDENTITY,
        [DepartmentId] int NOT NULL,
        [SectionId] int NULL,
        [DesignationId] int NULL,
        [TargetCapacity] int NOT NULL,
        [CurrentHeadcount] int NOT NULL,
        [Vacancies] int NOT NULL,
        [Remarks] nvarchar(max) NOT NULL,
        [LastUpdated] datetime2 NOT NULL,
        CONSTRAINT [PK_manpowers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_manpowers_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_manpowers_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [Designations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_manpowers_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE TABLE [separations] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeId] nvarchar(50) NOT NULL,
        [EmployeeName] nvarchar(100) NOT NULL,
        [DepartmentId] int NOT NULL,
        [SectionId] int NULL,
        [DesignationId] int NULL,
        [SeparationType] nvarchar(30) NOT NULL,
        [ResignDate] datetime2 NOT NULL,
        [LastWorkingDay] datetime2 NOT NULL,
        [ExitInterviewDate] datetime2 NULL,
        [Reason] nvarchar(1000) NOT NULL,
        [HandoverNotes] nvarchar(500) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [ClearanceProgress] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [ApprovedBy] nvarchar(max) NOT NULL,
        [ApprovedDate] datetime2 NULL,
        CONSTRAINT [PK_separations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_separations_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_separations_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [Designations] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_separations_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_manpowerrequirements_DepartmentId] ON [manpowerrequirements] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_manpowerrequirements_DesignationId] ON [manpowerrequirements] ([DesignationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_manpowerrequirements_Priority] ON [manpowerrequirements] ([Priority]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_manpowerrequirements_SectionId] ON [manpowerrequirements] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_manpowerrequirements_Status] ON [manpowerrequirements] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_manpowers_DepartmentId_SectionId_DesignationId] ON [manpowers] ([DepartmentId], [SectionId], [DesignationId]) WHERE [SectionId] IS NOT NULL AND [DesignationId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_manpowers_DesignationId] ON [manpowers] ([DesignationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_manpowers_SectionId] ON [manpowers] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_separations_DepartmentId] ON [separations] ([DepartmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_separations_DesignationId] ON [separations] ([DesignationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_separations_EmployeeId] ON [separations] ([EmployeeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_separations_ResignDate] ON [separations] ([ResignDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_separations_SectionId] ON [separations] ([SectionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    CREATE INDEX [IX_separations_Status] ON [separations] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620095115_AddManpowerModels'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620095115_AddManpowerModels', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160417_AddMissingEmployeeFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620160417_AddMissingEmployeeFields', N'10.0.9');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260620160145_AddMissingEmployeeFields'
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='IsActive') EXEC sp_rename N'[Employees].[IsActive]', N'OverTimeStatus', 'COLUMN'
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='Address') EXEC sp_rename N'[Employees].[Address]', N'SpouseName', 'COLUMN'
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='DateOfBirth') ALTER TABLE [Employees] ADD [DateOfBirth] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='Gender') ALTER TABLE [Employees] ADD [Gender] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='ChildrenCount') ALTER TABLE [Employees] ADD [ChildrenCount] int NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='AccountType') ALTER TABLE [Employees] ADD [AccountType] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='AccountNumber') ALTER TABLE [Employees] ADD [AccountNumber] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='GrossSalary') ALTER TABLE [Employees] ADD [GrossSalary] decimal(18,2) NOT NULL DEFAULT 0.0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PhotoBase64') ALTER TABLE [Employees] ADD [PhotoBase64] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='SignatureBase64') ALTER TABLE [Employees] ADD [SignatureBase64] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentVillage') ALTER TABLE [Employees] ADD [PresentVillage] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentPostOffice') ALTER TABLE [Employees] ADD [PresentPostOffice] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentDivisionId') ALTER TABLE [Employees] ADD [PresentDivisionId] int NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentDistrictId') ALTER TABLE [Employees] ADD [PresentDistrictId] int NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentUpazilaId') ALTER TABLE [Employees] ADD [PresentUpazilaId] int NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PresentPostalCode') ALTER TABLE [Employees] ADD [PresentPostalCode] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentVillage') ALTER TABLE [Employees] ADD [PermanentVillage] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentPostOffice') ALTER TABLE [Employees] ADD [PermanentPostOffice] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentDivisionId') ALTER TABLE [Employees] ADD [PermanentDivisionId] int NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentDistrictId') ALTER TABLE [Employees] ADD [PermanentDistrictId] int NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentUpazilaId') ALTER TABLE [Employees] ADD [PermanentUpazilaId] int NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='PermanentPostalCode') ALTER TABLE [Employees] ADD [PermanentPostalCode] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='EmployeeStatus') ALTER TABLE [Employees] ADD [EmployeeStatus] nvarchar(max) NOT NULL DEFAULT N'Regular'
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='EmployeeType') ALTER TABLE [Employees] ADD [EmployeeType] nvarchar(max) NOT NULL DEFAULT N''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='CompanyId') ALTER TABLE [Employees] ADD [CompanyId] int NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Employees' AND COLUMN_NAME='OverTimeStatus') ALTER TABLE [Employees] ADD [OverTimeStatus] bit NOT NULL DEFAULT 0
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN

                    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Employees_Companies_CompanyId')
                    BEGIN
                        CREATE INDEX [IX_Employees_CompanyId] ON [Employees] ([CompanyId]);
                        ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION;
                    END
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260620160930_FixMissingColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260620160930_FixMissingColumns', N'10.0.9');
END;

COMMIT;
GO

