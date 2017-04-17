CREATE PROCEDURE [dbo].[GetTextValueByTextId]
	@textId     INT
AS

SELECT V.*
FROM dbo.[Text] T
INNER JOIN dbo.TextValue V on V.TextId = T.Id
WHERE T.Id = @textId
