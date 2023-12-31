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
	  ,CONSTRAINT [PK_IdetityPM_Login] PRIMARY KEY 
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
	  ,CONSTRAINT [PK_IdetityPM_User] PRIMARY KEY CLUSTERED 
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