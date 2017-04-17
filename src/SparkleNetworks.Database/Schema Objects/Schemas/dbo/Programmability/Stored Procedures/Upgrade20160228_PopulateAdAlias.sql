
CREATE PROCEDURE [dbo].Upgrade20160228_PopulateAdAlias
AS

IF ((select top 1 1 from dbo.Ads where Alias is null) is not null)
BEGIN
	PRINT 'Upgrade20160228_PopulateAdAlias: Found Ads with Alias=NULL. Generating Alias where NULL...';
	update dbo.Ads set Alias = ('Ad-' + CONVERT(NVARCHAR(100),  [Id])) Where Alias is null
END
ELSE
BEGIN
	PRINT 'Upgrade20160228_PopulateAdAlias: Found zero Ads with Alias=NULL. Nothing to do.';
END
