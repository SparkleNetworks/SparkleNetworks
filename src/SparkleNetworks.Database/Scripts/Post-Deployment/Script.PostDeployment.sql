--
-- UPGRADE SCRIPT
--     Script.PostDeployment.sql
--
-- DESCRIPTION
--     Performs database upgrades after structural upgrades.
--
-- PROGRAMMING NOTES
--     Be careful.
--

-- Add 6 rows to [dbo].[aspnet_SchemaVersions]
IF NOT EXISTS (SELECT * FROM [dbo].[aspnet_SchemaVersions] WHERE [Feature] = (N'common') AND [CompatibleSchemaVersion] = N'1')
BEGIN
    INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'common', N'1', 1)
END

--IF NOT EXISTS (SELECT * FROM [dbo].[aspnet_SchemaVersions] WHERE [Feature] = (N'health monitoring') AND [CompatibleSchemaVersion] = N'1')
--  BEGIN  
--    INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'health monitoring', N'1', 1)
--  END

IF NOT EXISTS (SELECT * FROM [dbo].[aspnet_SchemaVersions] WHERE [Feature] = (N'membership') AND [CompatibleSchemaVersion] = N'1')
BEGIN
    INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'membership', N'1', 1)
END

--IF NOT EXISTS (SELECT * FROM [dbo].[aspnet_SchemaVersions] WHERE [Feature] = (N'personalization') AND [CompatibleSchemaVersion] = N'1')
--  BEGIN 
--    INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'personalization', N'1', 1)
--  END

--IF NOT EXISTS (SELECT * FROM [dbo].[aspnet_SchemaVersions] WHERE [Feature] = (N'profile') AND [CompatibleSchemaVersion] = N'1')
--  BEGIN 
--    INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'profile', N'1', 1)
--  END

--IF NOT EXISTS (SELECT * FROM [dbo].[aspnet_SchemaVersions] WHERE [Feature] = (N'role manager') AND [CompatibleSchemaVersion] = N'1')
--  BEGIN 
--   INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion]) VALUES (N'role manager', N'1', 1)
--  END

GRANT EXECUTE ON [dbo].[aspnet_RegisterSchemaVersion] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_RegisterSchemaVersion] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_UnRegisterSchemaVersion] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_UnRegisterSchemaVersion] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_CheckSchemaVersion] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_CheckSchemaVersion] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT SELECT ON [dbo].[vw_aspnet_Applications] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT SELECT ON [dbo].[vw_aspnet_Users] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_UpdateUserInfo] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_UpdateUser] TO [aspnet_Membership_FullAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_UnlockUser] TO [aspnet_Membership_FullAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_SetPassword] TO [aspnet_Membership_FullAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_ResetPassword] TO [aspnet_Membership_FullAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByUserId] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByUserId] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByName] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByName] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByEmail] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetUserByEmail] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetPasswordWithFormat] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetPassword] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetNumberOfUsersOnline] TO [aspnet_Membership_BasicAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetNumberOfUsersOnline] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_GetAllUsers] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_FindUsersByName] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_FindUsersByEmail] TO [aspnet_Membership_ReportingAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_CreateUser] TO [aspnet_Membership_FullAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer] TO [aspnet_Membership_FullAccess] AS [dbo]
GRANT EXECUTE ON [dbo].[aspnet_Users_DeleteUser] TO [aspnet_Membership_FullAccess] AS [dbo]
GRANT SELECT ON [dbo].[vw_aspnet_MembershipUsers] TO [aspnet_Membership_ReportingAccess] AS [dbo]


-- Initialize dbo.ProfileFields & dbo.ProfileFieldsAvailiableValues
EXECUTE [dbo].[SetupProfileFields]
EXECUTE [dbo].[SetupProfileFieldsAvailableValues]

-- initialize default network type
IF NOT EXISTS (SELECT Id FROM dbo.NetworkTypes)
BEGIN
    INSERT INTO dbo.NetworkTypes
    (Id, Name)
    VALUES
    (1, 'Default')
END

-- generate a warning if dbo.ProfileFields is empty
IF (SELECT TOP 1 1 FROM dbo.ProfileFields) IS NULL
BEGIN
    print 'dbo.ProfileFields is empty, you should set it up.'
END

-- insert built-in tag categories
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TagCategories)
BEGIN
    INSERT INTO [dbo].[TagCategories]
    ([Id], [Name], [Alias], [CanUsersCreate])
    VALUES
    (1, 'Skill', 'Skill', 1),
    (2, 'Interest', 'Interest', 1),
    (3, 'Recreation', 'Recreation', 1),
    (4, 'City', 'City', 0)
END

-- insert built-in tag categories
IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[TagCategories] WHERE Id = 5)
BEGIN
    INSERT INTO [dbo].[TagCategories]
    ([Id], [Name], [Alias], [CanUsersCreate])
    VALUES
    (5, 'PartnerResource', 'PartnerResource', 1)
END

-- TODO: comment this upgrade
IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[CompanyCategories] WHERE NetworkId IS NOT NULL) AND EXISTS (SELECT TOP 1 1 FROM [dbo].[CompanyCategories] WHERE NetworkId IS NULL)
BEGIN
    EXECUTE [dbo].[Upgrade20150319_UpdateCompanyCategoriesStruct]
END

-- Ad.Alias needs to be not null for structural updates
IF (EXISTS(select top 1 1 from sys.columns where name='Alias' and object_id=object_id('dbo.Ads')) and object_id('dbo.Upgrade20160228_PopulateAdAlias') IS NOT NULL)
BEGIN
	PRINT 'Script.PreDeployment: Found Ads with Alias=NULL. Generating Alias where NULL...';
	EXECUTE dbo.Upgrade20160228_PopulateAdAlias
END


--
-- END OF -- UPGRADE SCRIPT
--     Script.PostDeployment.sql
--
