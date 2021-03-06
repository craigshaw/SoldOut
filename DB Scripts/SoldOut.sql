USE [master]
GO
/****** Object:  Database [SoldOut]    Script Date: 02/04/2016 10:51:45 ******/
CREATE DATABASE [SoldOut]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SoldOut', FILENAME = N'D:\RDSDBDATA\DATA\SoldOut.mdf' , SIZE = 8384KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SoldOut_log', FILENAME = N'D:\RDSDBDATA\DATA\SoldOut_log.ldf' , SIZE = 4288KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SoldOut] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SoldOut].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SoldOut] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SoldOut] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SoldOut] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SoldOut] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SoldOut] SET ARITHABORT OFF 
GO
ALTER DATABASE [SoldOut] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SoldOut] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SoldOut] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SoldOut] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SoldOut] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SoldOut] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SoldOut] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SoldOut] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SoldOut] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SoldOut] SET  ENABLE_BROKER 
GO
ALTER DATABASE [SoldOut] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SoldOut] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SoldOut] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SoldOut] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SoldOut] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SoldOut] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [SoldOut] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SoldOut] SET RECOVERY FULL 
GO
ALTER DATABASE [SoldOut] SET  MULTI_USER 
GO
ALTER DATABASE [SoldOut] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SoldOut] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SoldOut] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SoldOut] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [SoldOut] SET DELAYED_DURABILITY = DISABLED 
GO
USE [SoldOut]
GO
/****** Object:  User [SoldOut]    Script Date: 02/04/2016 10:51:46 ******/
CREATE USER [SoldOut] FOR LOGIN [SoldOut] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [SoldOut]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[IncludeInKeywordSearch] [bit] NOT NULL,
	[ParentCategoryId] [int] NULL,
 CONSTRAINT [PK_dbo.Category] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Condition]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Condition](
	[ConditionId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[eBayConditionId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Condition] PRIMARY KEY CLUSTERED 
(
	[ConditionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Currency]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currency](
	[CurrencyCode] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[CurrencyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Price]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Price](
	[PriceID] [int] IDENTITY(1,1) NOT NULL,
	[Amount] [float] NOT NULL,
	[IsRRP] [bit] NOT NULL,
	[CurrencyCode] [nvarchar](128) NOT NULL,
	[ProductId] [int] NOT NULL,
 CONSTRAINT [PK_Price] PRIMARY KEY CLUSTERED 
(
	[PriceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Product]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[YearOfRelease] [nvarchar](max) NULL,
	[YearOfRetirement] [nvarchar](max) NULL,
	[ManufacturerCode] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[ProductId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ProductCategory] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductSubProduct]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductSubProduct](
	[ParentProductId] [int] NOT NULL,
	[SubProductId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ProductProduct] PRIMARY KEY CLUSTERED 
(
	[ParentProductId] ASC,
	[SubProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Search]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Search](
	[SearchId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[LastRun] [datetime] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Link] [nvarchar](max) NULL,
	[LastCleansed] [datetime] NOT NULL,
	[OriginalRRP] [float] NULL,
	[ProductId] [int] NULL,
 CONSTRAINT [PK_dbo.Search] PRIMARY KEY CLUSTERED 
(
	[SearchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchCriteria]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchCriteria](
	[SearchCriteriaID] [bigint] IDENTITY(1,1) NOT NULL,
	[SearchID] [bigint] NOT NULL,
	[Criteria] [nvarchar](max) NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.SearchCriteria] PRIMARY KEY CLUSTERED 
(
	[SearchCriteriaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchResult]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchResult](
	[SearchResultID] [int] IDENTITY(1,1) NOT NULL,
	[DateOfMatch] [datetime] NOT NULL,
	[Link] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[Price] [float] NULL,
	[ItemNumber] [nvarchar](max) NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[NumberOfBidders] [int] NULL,
	[ImageURL] [nvarchar](max) NULL,
	[SearchID] [bigint] NOT NULL,
	[Currency] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[SiteID] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NULL,
	[ShippingCost] [float] NULL,
	[Suspicious] [bit] NOT NULL CONSTRAINT [DF_SearchResult_Suspicious]  DEFAULT ((0)),
	[ConditionId] [int] NOT NULL,
	[ProductId] [int] NULL,
 CONSTRAINT [PK_dbo.SearchResult] PRIMARY KEY CLUSTERED 
(
	[SearchResultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SearchSuspiciousPhrase]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SearchSuspiciousPhrase](
	[Phrase] [nvarchar](128) NOT NULL,
	[SearchId] [bigint] NOT NULL,
 CONSTRAINT [PK_SearchSuspiciousPhrase] PRIMARY KEY CLUSTERED 
(
	[Phrase] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SuspiciousPhrase]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuspiciousPhrase](
	[Phrase] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_SuspiciousPhrase] PRIMARY KEY CLUSTERED 
(
	[Phrase] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Index [IX_SearchID]    Script Date: 02/04/2016 10:51:46 ******/
CREATE NONCLUSTERED INDEX [IX_SearchID] ON [dbo].[SearchCriteria]
(
	[SearchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SearchID]    Script Date: 02/04/2016 10:51:46 ******/
CREATE NONCLUSTERED INDEX [IX_SearchID] ON [dbo].[SearchResult]
(
	[SearchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Category]  WITH CHECK ADD  CONSTRAINT [FK_Category_Category] FOREIGN KEY([ParentCategoryId])
REFERENCES [dbo].[Category] ([CategoryID])
GO
ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_Category]
GO
ALTER TABLE [dbo].[Price]  WITH CHECK ADD  CONSTRAINT [FK_Price_Currency] FOREIGN KEY([CurrencyCode])
REFERENCES [dbo].[Currency] ([CurrencyCode])
GO
ALTER TABLE [dbo].[Price] CHECK CONSTRAINT [FK_Price_Currency]
GO
ALTER TABLE [dbo].[Price]  WITH CHECK ADD  CONSTRAINT [FK_Price_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[Price] CHECK CONSTRAINT [FK_Price_Product]
GO
ALTER TABLE [dbo].[ProductCategory]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProductCategory_dbo.Category_Category_CategoryID] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductCategory] CHECK CONSTRAINT [FK_dbo.ProductCategory_dbo.Category_Category_CategoryID]
GO
ALTER TABLE [dbo].[ProductCategory]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProductCategory_dbo.Product_Product_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductCategory] CHECK CONSTRAINT [FK_dbo.ProductCategory_dbo.Product_Product_ProductId]
GO
ALTER TABLE [dbo].[ProductSubProduct]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProductProduct_dbo.Product_Product_ProductId] FOREIGN KEY([ParentProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ProductSubProduct] CHECK CONSTRAINT [FK_dbo.ProductProduct_dbo.Product_Product_ProductId]
GO
ALTER TABLE [dbo].[ProductSubProduct]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProductProduct_dbo.Product_Product_ProductId1] FOREIGN KEY([SubProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[ProductSubProduct] CHECK CONSTRAINT [FK_dbo.ProductProduct_dbo.Product_Product_ProductId1]
GO
ALTER TABLE [dbo].[Search]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[SearchCriteria]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchCriteria_dbo.Search_SearchID] FOREIGN KEY([SearchID])
REFERENCES [dbo].[Search] ([SearchId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SearchCriteria] CHECK CONSTRAINT [FK_dbo.SearchCriteria_dbo.Search_SearchID]
GO
ALTER TABLE [dbo].[SearchResult]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[SearchResult]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SearchResult_dbo.Search_SearchID] FOREIGN KEY([SearchID])
REFERENCES [dbo].[Search] ([SearchId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SearchResult] CHECK CONSTRAINT [FK_dbo.SearchResult_dbo.Search_SearchID]
GO
ALTER TABLE [dbo].[SearchResult]  WITH CHECK ADD  CONSTRAINT [FK_SearchResult_Condition] FOREIGN KEY([ConditionId])
REFERENCES [dbo].[Condition] ([ConditionId])
GO
ALTER TABLE [dbo].[SearchResult] CHECK CONSTRAINT [FK_SearchResult_Condition]
GO
ALTER TABLE [dbo].[SearchSuspiciousPhrase]  WITH CHECK ADD  CONSTRAINT [FK_SearchSuspiciousPhrase_Search] FOREIGN KEY([SearchId])
REFERENCES [dbo].[Search] ([SearchId])
GO
ALTER TABLE [dbo].[SearchSuspiciousPhrase] CHECK CONSTRAINT [FK_SearchSuspiciousPhrase_Search]
GO
/****** Object:  StoredProcedure [dbo].[GetPriceStatsForSearch]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetPriceStatsForSearch] 
	@SearchId BIGINT,
	@ConditionId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Get basic price stats for a registered search
	SELECT
		IsNull(AVG(sr.price), 0) AS 'AverageSalePrice', 
		IsNull(STDEV(sr.price), 0) AS 'StandardDeviation',
		COUNT(sr.SearchResultId) AS 'NumberOfResults'
	FROM 
		searchresult AS sr
	WHERE 
		sr.searchid = @SearchId AND sr.ConditionId = @ConditionId
END

GO
/****** Object:  DdlTrigger [rds_deny_backups_trigger]    Script Date: 02/04/2016 10:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [rds_deny_backups_trigger] ON DATABASE WITH EXECUTE AS 'dbo' FOR
 ADD_ROLE_MEMBER, GRANT_DATABASE AS BEGIN
   SET NOCOUNT ON;
   SET ANSI_PADDING ON;
 
   DECLARE @data xml;
   DECLARE @user sysname;
   DECLARE @role sysname;
   DECLARE @type sysname;
   DECLARE @sql NVARCHAR(MAX);
   DECLARE @permissions TABLE(name sysname PRIMARY KEY);
   
   SELECT @data = EVENTDATA();
   SELECT @type = @data.value('(/EVENT_INSTANCE/EventType)[1]', 'sysname');
    
   IF @type = 'ADD_ROLE_MEMBER' BEGIN
      SELECT @user = @data.value('(/EVENT_INSTANCE/ObjectName)[1]', 'sysname'),
       @role = @data.value('(/EVENT_INSTANCE/RoleName)[1]', 'sysname');

      IF @role IN ('db_owner', 'db_backupoperator') BEGIN
         SELECT @sql = 'DENY BACKUP DATABASE, BACKUP LOG TO ' + QUOTENAME(@user);
         EXEC(@sql);
      END
   END ELSE IF @type = 'GRANT_DATABASE' BEGIN
      INSERT INTO @permissions(name)
      SELECT Permission.value('(text())[1]', 'sysname') FROM
       @data.nodes('/EVENT_INSTANCE/Permissions/Permission')
      AS DatabasePermissions(Permission);
      
      IF EXISTS (SELECT * FROM @permissions WHERE name IN ('BACKUP DATABASE',
       'BACKUP LOG'))
         RAISERROR('Cannot grant backup database or backup log', 15, 1) WITH LOG;       
   END
END


GO
ENABLE TRIGGER [rds_deny_backups_trigger] ON DATABASE
GO
USE [master]
GO
ALTER DATABASE [SoldOut] SET  READ_WRITE 
GO
