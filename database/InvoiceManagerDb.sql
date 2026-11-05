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
    WHERE [MigrationId] = N'20261104210108_InitialCreate'
)
BEGIN
    CREATE TABLE [Suppliers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [NormalizedName] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104210108_InitialCreate'
)
BEGIN
    CREATE TABLE [Invoices] (
        [Id] int NOT NULL IDENTITY,
        [SupplierId] int NOT NULL,
        [InvoiceNumber] nvarchar(100) NOT NULL,
        [NormalizedInvoiceNumber] nvarchar(100) NOT NULL,
        [IssueDate] date NOT NULL,
        [DueDate] date NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [Currency] char(3) NOT NULL,
        [CustomerOrContactNumber] nvarchar(100) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Invoices_TotalAmount_Positive] CHECK ([TotalAmount] > 0),
        CONSTRAINT [FK_Invoices_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104210108_InitialCreate'
)
BEGIN
    CREATE TABLE [InvoiceDocuments] (
        [Id] int NOT NULL IDENTITY,
        [InvoiceId] int NOT NULL,
        [OriginalFileName] nvarchar(255) NOT NULL,
        [ContentType] nvarchar(100) NOT NULL,
        [SizeBytes] bigint NOT NULL,
        [UploadedAtUtc] datetime2 NOT NULL,
        [StoragePath] nvarchar(500) NOT NULL,
        [OriginalLlmResponseJson] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_InvoiceDocuments] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_InvoiceDocuments_SizeBytes_Positive] CHECK ([SizeBytes] > 0),
        CONSTRAINT [FK_InvoiceDocuments_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104210108_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InvoiceDocuments_InvoiceId] ON [InvoiceDocuments] ([InvoiceId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104210108_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UX_Invoices_Supplier_InvoiceNumber] ON [Invoices] ([SupplierId], [NormalizedInvoiceNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104210108_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UX_Suppliers_NormalizedName] ON [Suppliers] ([NormalizedName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104210108_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104210108_InitialCreate', N'10.0.14');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104212756_ActualizacionModelo'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[InvoiceDocuments]') AND [c].[name] = N'ContentType');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [InvoiceDocuments] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [InvoiceDocuments] ALTER COLUMN [ContentType] nvarchar(max) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261104212756_ActualizacionModelo'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261104212756_ActualizacionModelo', N'10.0.14');
END;

COMMIT;
GO

