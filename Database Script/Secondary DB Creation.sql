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
	  ,[PasswordHash] VARCHAR(128) NOT NULL
      ,[RegisteredDate] DATETIME NOT NULL
      ,[LastLoginTime] DATETIME NOT NULL
      ,[UserRole] NVARCHAR(10)
      ,[IsActive] BIT NOT NULL
	  ,CONSTRAINT [PK_IdentitySD_Login] PRIMARY KEY CLUSTERED 
	  (
		[Username] ASC
	  )
	);

	CREATE CLUSTERED INDEX IX_IdentitySD_Login_Username   
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
	  ,CONSTRAINT [PK_IdentitySD_User] PRIMARY KEY CLUSTERED 
	  (
		[Id] ASC
	  )
	  ,CONSTRAINT FK__IdentitySD_Login_User FOREIGN KEY (Username)
        REFERENCES [Login] (Username)
        ON DELETE CASCADE
        ON UPDATE CASCADE
	); 

	CREATE CLUSTERED INDEX IX_IdentitySD_User_Username   
    ON [User] (Username);
END