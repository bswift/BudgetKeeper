SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

-- Version 1
IF NOT EXISTS(SELECT * FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Users]') AND TYPE in (N'U'))
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](100) NULL,
	[Password] [varchar] (100) NULL,
	[UserType] [int] NULL,
	[Name] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[Phone] [varchar](10) NULL,
	[CreatedDate] datetime NOT NULL DEFAULT(('2000-01-01 00:00:00.000')),
	[LastLogin] datetime NOT NULL DEFAULT(('2000-01-01 00:00:00.000')),
	[Image] image NULL,
	[Status] [int] NOT NULL DEFAULT (1),
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  NOT EXISTS (SELECT * FROM Users where [Username]='BK_Admin') 
BEGIN
	   Insert Into Users (Username, [Password], UserType, Name, CreatedDate, LastLogin, [Status]) VALUES ('BK_Admin', '0LjZlR19Fl4Z+kisWEFQew==', 1, 'Administrator', CURRENT_TIMESTAMP, '2000-01-01 00:00:00.000', 1);
END 
GO


IF NOT EXISTS(SELECT * FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Entries]') AND TYPE in (N'U'))
CREATE TABLE [dbo].[Entries](
	[EntryID] [bigint] IDENTITY(1,1) NOT NULL,
	[Amount] [int] NULL,
	[EntryType] [int] NULL,
	[UserID] [int] NULL,
	[UserType] [int] NULL,
	[Description] [varchar](200) NULL,
	[Notes] [varchar](2000) NULL,
	[LocationID] [int] NULL,
	[CategoryID] [int] NULL,
	[Image] image NULL,
	[CreatedDate] datetime NOT NULL DEFAULT(('2000-01-01 00:00:00.000')),
	[Status] [int] NOT NULL DEFAULT (1),
 CONSTRAINT [PK_Entry] PRIMARY KEY CLUSTERED 
(
	[EntryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Categories]') AND TYPE in (N'U'))
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar] (100) NULL,
	[Description] [varchar](2000) NULL,
	[Status] [int] NOT NULL DEFAULT (1),
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM sys.objects WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[Locations]') AND TYPE in (N'U'))
CREATE TABLE [dbo].[Locations](
	[LocationID] [int] IDENTITY(1,1) NOT NULL,
	[LocationType] [int] NULL,
	[Name] [varchar] (100) NULL,
	[Description] [varchar](2000) NULL,
	[URL] [varchar] (200) NULL,
	[Image] image NULL,
	[Status] [int] NOT NULL DEFAULT (1),
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[LocationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
