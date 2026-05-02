IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'IdentitySD')
BEGIN
	CREATE DATABASE [IdentitySD];
END
GO
    USE [IdentitySD];
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
	  ,CONSTRAINT [PK_IdentitySD_Login] PRIMARY KEY 
	  (
		[Username] ASC
	  )
	);

	CREATE NONCLUSTERED INDEX IX_Login_Username   
    ON [dbo].[Login] (Username);   
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
	  ,CONSTRAINT [PK_IdentitySD_User] PRIMARY KEY CLUSTERED 
	  (
		[Id] ASC
	  )
	  ,CONSTRAINT FK_Login_User FOREIGN KEY (Username)
        REFERENCES Login (Username)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	); 

	CREATE NONCLUSTERED INDEX IX_User_Username   
    ON [dbo].[User] (Username);
END

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='OTPValidate' and xtype='U')
BEGIN
	CREATE TABLE [OTPValidate]
	(
	   [Username] NVARCHAR(10) NOT NULL
	  ,[OTP] NUMERIC(6,0)
      ,[RequestedTime] DATETIME NOT NULL
      ,[RetryAttempt] NUMERIC(2,0)
	  ,CONSTRAINT [PK_IdentitySD_OTPValidate] PRIMARY KEY CLUSTERED 
	  (
		[Username] ASC
	  )
	  ,CONSTRAINT FK_OTPValidate_Login FOREIGN KEY (Username)
        REFERENCES Login (Username)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	); 

	CREATE NONCLUSTERED INDEX IX_OTPValidate_Username   
    ON [dbo].[OTPValidate] (Username);
END

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='UserAuth' and xtype='U')
BEGIN
	CREATE TABLE [UserAuth]
	(
	   [Username] NVARCHAR(10) NOT NULL
	  ,[Token] NVARCHAR(MAX)
      ,[CreatedTime] DATETIME NOT NULL
      ,[Enabled] BIT
	  ,CONSTRAINT [PK_IdentitySD_UserAuth] PRIMARY KEY CLUSTERED 
	  (
		[Username] ASC
	  )
	  ,CONSTRAINT FK_UserAuth_Login FOREIGN KEY (Username)
        REFERENCES Login (Username)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	); 

	CREATE NONCLUSTERED INDEX IX_UserAuth_Username   
    ON [dbo].[UserAuth] (Username);
END

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='User_PersonalDetails' and xtype='U')
BEGIN
	CREATE TABLE [User_PersonalDetails]
	(
	   [Id] INT IDENTITY(1,1)
	  ,[UserId] INT NOT NULL
	  ,[Location] NVARCHAR(300)
      ,[BirthDate] DATETIME
      ,[Status] NVARCHAR(10)
      ,[Gender] NVARCHAR(10)
	  ,[LanguageOne] NVARCHAR(10)
	  ,[LanguageTwo] NVARCHAR(10)
	  ,CONSTRAINT [PK_IdentitySD_User_PersonalDetails] PRIMARY KEY CLUSTERED 
	  (
		[Id] ASC
	  )
	  ,CONSTRAINT FK_UserPersonalDetails_User FOREIGN KEY (UserId)
        REFERENCES [User] (Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	);

	CREATE NONCLUSTERED INDEX IX_User_PersonalDetails_UserId   
    ON [dbo].[User_PersonalDetails] (UserId);   
END

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='User_EmploymentDetails' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[User_EmploymentDetails]
	(
	   [Id] INT IDENTITY(1,1)
	  ,[UserId] INT NOT NULL
	  ,[EmployerName] NVARCHAR(300) NOT NULL
      ,[EmployerCity] NVARCHAR(50)
      ,[Role] NVARCHAR(20) NOT NULL
      ,[Responsibilities] NVARCHAR(MAX)
	  ,[StartDate] DATETIME2  
	  ,[EndDate] DATETIME2
	  ,[IsCurrentEmployer] BIT DEFAULT 0
	  ,CONSTRAINT [SD_IdentityPM_User_EmploymentDetails] PRIMARY KEY CLUSTERED 
	  (
		[Id] ASC
	  )
	  ,CONSTRAINT FK_UserEmploymentDetails_User FOREIGN KEY (UserId)
        REFERENCES [dbo].[User] (Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	);

	CREATE NONCLUSTERED INDEX IX_User_EmploymentDetails_UserId   
    ON [dbo].[User_EmploymentDetails] (UserId);   
END