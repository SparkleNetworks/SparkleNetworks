CREATE PROCEDURE [dbo].[GetTimelineListIdByContent]
	@networkId int,
	@dateMax DateTime,
	@content nvarchar(2048)
AS

DECLARE @SQLQuery AS nvarchar(4000)
DECLARE @SplitLength int
DECLARE @Delimiter varchar(1)
DECLARE @Split varchar(50)

SET @SQLQuery = 'SELECT TOP 40 T.Id FROM TimelineItems T WHERE T.NetworkId = ' + CONVERT(NVARCHAR(5), @networkId) + ' AND T.CreateDate < ' + @dateMax + ' AND T.DeleteReason IS NULL AND T.ItemType != 2 AND ((1=0) '
SET @Delimiter = ' '

WHILE LEN(@content) > 0
BEGIN
	SELECT @SplitLength = (CASE CHARINDEX(@Delimiter, @content) WHEN 0 THEN
            LEN(@content) ELSE CHARINDEX(@Delimiter, @content) -1 END)

	SELECT @Split = '%' + SUBSTRING(@content, 1, @SplitLength) + '%'
    SET @SQLQuery = @SQLQuery + ' OR T.Text LIKE ' + @Split

    SELECT @content = (CASE (LEN(@content) - @SplitLength) WHEN 0 THEN  ''
            ELSE RIGHT(@content, LEN(@content) - @SplitLength - 1) END)
END

SET @SQLQuery = @SQLQuery + ') ORDER BY T.CreateDate DESC, T.Id ASC'

EXECUTE(@SQLQuery)

----SELECT TOP 40
----	T.Id
----FROM dbo.TimelineItems T
----WHERE T.NetworkId = @networkId AND T.CreateDate < @dateMax AND T.DeleteReason IS NULL AND T.ItemType != 2
----ORDER BY T.CreateDate DESC, T.Id ASC