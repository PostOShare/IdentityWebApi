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
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    CREATE TABLE [Login] (
        [Username] nvarchar(10) NOT NULL,
        [Key] nvarchar(300) NOT NULL,
        [Salt] nvarchar(300) NOT NULL,
        [RegisteredDate] datetime NOT NULL,
        [LastLoginTime] datetime NOT NULL,
        [UserRole] nvarchar(10) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_IdentityPM_Login] PRIMARY KEY ([Username])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    CREATE TABLE [OTPValidate] (
        [Username] nvarchar(10) NOT NULL,
        [OTP] numeric(6,0) NULL,
        [RequestedTime] datetime NOT NULL,
        [RetryAttempt] numeric(2,0) NULL,
        CONSTRAINT [PK_IdentityPM_OTPValidate] PRIMARY KEY ([Username]),
        CONSTRAINT [FK_OTPValidate_Login] FOREIGN KEY ([Username]) REFERENCES [Login] ([Username]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    CREATE TABLE [User] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(10) NOT NULL,
        [Title] nvarchar(5) NULL,
        [FirstName] nvarchar(30) NOT NULL,
        [LastName] nvarchar(30) NOT NULL,
        [Suffix] nvarchar(10) NULL,
        [EmailAddress] nvarchar(30) NOT NULL,
        [Phone] nvarchar(10) NULL,
        CONSTRAINT [PK_IdentityPM_User] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Login_User] FOREIGN KEY ([Username]) REFERENCES [Login] ([Username]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    CREATE TABLE [UserAuth] (
        [Username] nvarchar(10) NOT NULL,
        [Token] nvarchar(max) NULL,
        [CreatedTime] datetime NOT NULL,
        [Enabled] bit NULL,
        CONSTRAINT [PK_IdentityPM_UserAuth] PRIMARY KEY ([Username]),
        CONSTRAINT [FK_UserAuth_Login] FOREIGN KEY ([Username]) REFERENCES [Login] ([Username]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    CREATE INDEX [IX_Login_Username] ON [Login] ([Username]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    CREATE INDEX [IX_OTPValidate_Username] ON [OTPValidate] ([Username]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    CREATE INDEX [IX_User_Username] ON [User] ([Username]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    CREATE INDEX [IX_UserAuth_Username] ON [UserAuth] ([Username]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260425072155_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260425072155_InitialCreate', N'6.0.36');
END;
GO

COMMIT;
GO

