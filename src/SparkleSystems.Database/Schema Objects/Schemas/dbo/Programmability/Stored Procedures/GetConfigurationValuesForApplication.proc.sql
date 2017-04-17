CREATE PROCEDURE [dbo].[GetConfigurationValuesForApplication]
	@ApplicationId int
AS

-- 0: Value ID
-- 1: Key ID
-- 2: Key Name
-- 3: Key BlitableType
-- 4: Key Summary
-- 5: Key IsRequired
-- 6: Key IsCollection
-- 7: Key DefaultRawValue
-- 8: Value RawValue
	
	--DECLARE @ApplicationId INT
	--SET @ApplicationId = 4
	
	SELECT V.Id, V.ConfigKeyId, K.Name, K.BlitableType, K.Summary, K.IsRequired, K.IsCollection, K.DefaultValue, V.Value
	FROM dbo.ApplicationConfigValues V
	INNER JOIN dbo.ConfigKeys K ON K.Id = V.ConfigKeyId
	WHERE V.ApplicationId = @ApplicationId
	
	UNION
	
	SELECT 0, K.Id, K.Name, K.BlitableType, K.Summary, K.IsRequired, K.IsCollection, K.DefaultValue, null as Value
	FROM dbo.ConfigKeys K
	WHERE K.Id NOT IN (SELECT ConfigKeyId FROM dbo.ApplicationConfigValues WHERE ApplicationId = @ApplicationId)

	ORDER BY K.Name

RETURN 0