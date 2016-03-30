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