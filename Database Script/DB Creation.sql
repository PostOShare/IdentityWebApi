IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'IdentityPM')
BEGIN
	CREATE DATABASE [IdentityPM];
END
GO
    USE [IdentityPM];
GO

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='Login' and xtype='U')
BEGIN
	CREATE TABLE [Login]
	(
	   [Username] NVARCHAR(10) NOT NULL
	  ,[Key] NVARCHAR(300) NOT NULL
	  ,[Salt] NVARCHAR(300) NOT NULL
      ,[RegisteredDate] DATETIME NOT NULL
      ,[LastLoginTime] DATETIME NOT NULL
      ,[UserRole] NVARCHAR(10)
      ,[IsActive] BIT NOT NULL
	  ,CONSTRAINT [PK_IdentityPM_Login] PRIMARY KEY 
	  (
		[Username] ASC
	  )
	);

	CREATE NONCLUSTERED INDEX IX_Login_Username   
    ON [Login] (Username);   
END

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='User' and xtype='U')
BEGIN
	CREATE TABLE [User]
	(
	   [Id] INT IDENTITY(1,1)
	  ,[Username] NVARCHAR(10) NOT NULL
	  ,[Title] NVARCHAR(5)
      ,[FirstName] NVARCHAR(30) NOT NULL
      ,[LastName] NVARCHAR(30) NOT NULL
      ,[Suffix] NVARCHAR(10)
      ,[EmailAddress] NVARCHAR(30) NOT NULL
      ,[Phone] NVARCHAR(10)
	  ,CONSTRAINT [PK_IdentityPM_User] PRIMARY KEY CLUSTERED 
	  (
		[Id] ASC
	  )
	  ,CONSTRAINT FK_Login_User FOREIGN KEY (Username)
        REFERENCES Login (Username)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	); 

	CREATE NONCLUSTERED INDEX IX_User_Username   
    ON [User] (Username);
END

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='OTPValidate' and xtype='U')
BEGIN
	CREATE TABLE [OTPValidate]
	(
	   [Username] NVARCHAR(10) NOT NULL
	  ,[OTP] NUMERIC(6,0)
      ,[RequestedTime] DATETIME NOT NULL
      ,[RetryAttempt] NUMERIC(2,0)
	  ,CONSTRAINT [PK_IdentityPM_OTPValidate] PRIMARY KEY CLUSTERED 
	  (
		[Username] ASC
	  )
	  ,CONSTRAINT FK_OTPValidate_Login FOREIGN KEY (Username)
        REFERENCES Login (Username)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	); 

	CREATE NONCLUSTERED INDEX IX_OTPValidate_Username   
    ON [OTPValidate] (Username);
END

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='UserAuth' and xtype='U')
BEGIN
	CREATE TABLE [UserAuth]
	(
	   [Username] NVARCHAR(10) NOT NULL
	  ,[Token] NVARCHAR(MAX)
      ,[CreatedTime] DATETIME NOT NULL
      ,[Enabled] BIT
	  ,CONSTRAINT [PK_IdentityPM_UserAuth] PRIMARY KEY CLUSTERED 
	  (
		[Username] ASC
	  )
	  ,CONSTRAINT FK_UserAuth_Login FOREIGN KEY (Username)
        REFERENCES Login (Username)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	); 

	CREATE NONCLUSTERED INDEX IX_UserAuth_Username   
    ON [UserAuth] (Username);
END