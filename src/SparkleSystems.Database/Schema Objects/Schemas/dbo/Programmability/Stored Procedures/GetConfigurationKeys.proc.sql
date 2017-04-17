CREATE PROCEDURE [dbo].[GetConfigurationKeys]
AS
-- 0: Key ID
-- 1: Key Name
-- 2: Key BlitableType
-- 3: Key Summary
-- 4: Key IsRequired
-- 5: Key IsCollection
-- 6: Key DefaultRawValue
	
--DECLARE @ApplicationId INT
--SET @ApplicationId = 4

SELECT K.Id, K.Name, K.BlitableType, K.Summary, K.IsRequired, K.IsCollection, K.DefaultValue
FROM dbo.ConfigKeys K
ORDER BY K.Name

RETURN 0