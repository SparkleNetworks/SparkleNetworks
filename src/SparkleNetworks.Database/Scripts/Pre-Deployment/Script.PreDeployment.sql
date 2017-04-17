

--
-- UPGRADE SCRIPT
--     Script.PreDeployment.sql
--
-- DESCRIPTION
--     Performs database upgrades before structural upgrades.
--
-- PROGRAMMING NOTES
--     Be careful.
--

-- Place.CategoryId needs to be not null for structural updates
IF (EXISTS(select top 1 1 from sys.columns where name='Id' and object_id=object_id('dbo.Places')) and EXISTS (SELECT Id FROM dbo.Places WHERE CategoryId IS NULL) and object_id('[dbo].Upgrade_EnsurePlaceCategoryId') IS NOT NULL)
BEGIN
    EXEC [dbo].Upgrade_EnsurePlaceCategoryId
END

-- Ad.Alias needs to be not null for structural updates
IF (EXISTS(select top 1 1 from sys.columns where name='Alias' and object_id=object_id('dbo.Ads')) and object_id('dbo.Upgrade20160228_PopulateAdAlias') IS NOT NULL)
BEGIN
	PRINT 'Script.PreDeployment: Found Ads with Alias=NULL. Generating Alias where NULL...';
	EXECUTE dbo.Upgrade20160228_PopulateAdAlias
END

--
-- END OF -- UPGRADE SCRIPT
--     Script.PreDeployment.sql
--
