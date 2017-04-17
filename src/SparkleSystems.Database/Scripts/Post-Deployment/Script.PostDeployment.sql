--
-- POST-DEPLOYMENT SCRIPT
--     Script.PostDeployment.sql
--
-- DESCRIPTION
--     Creates base data for an empty database.
--
-- PROGRAMMING NOTES
--

IF NOT EXISTS (SELECT * FROM [dbo].[LogEntryTypes] )
BEGIN
	print 'Inserting default data into [LogEntryTypes]... '
	-- Log entry types -- PKs must be exactly those
	INSERT [dbo].[LogEntryTypes] ([Id], [Name], [Description]) VALUES
	 (1, N'Critical', NULL)
	,(2, N'Error', NULL)
	,(3, N'Warning', NULL)
	,(4, N'Info', NULL)
	,(5, N'Start', NULL)
	,(6, N'Stop', NULL)
	,(7, N'Verbose', NULL)
END

IF NOT EXISTS (SELECT * FROM [dbo].[Errors] )
BEGIN
	print 'Inserting default data into [Errors]... '
	-- Error status codes -- PKs must be exactly those
	INSERT [dbo].[Errors] ([Id], [Name], [Description]) VALUES
	 (0, N'Success', N'Operation complete successfully')
	,(1, N'Critical', N'Unrecoverable error. Requires immediate technical intervention.')
	,(2, N'Integrity', N'Data or business integrity issue (bad user or bad developper). Requires immediate intervention.')
	,(3, N'Internal', N'Internal error (bad developer)')
	,(4, N'Data', N'Data access error (bad database)')
	,(5, N'Authn', N'Session or credentials error (bad user)')
	,(6, N'Authz', N'Authorization error (bad user)')
	,(7, N'Business', N'Business rule violated (bad user)')
	,(8, N'Input', N'Wrong input (bad user)')
	,(9, N'ThirdParty', N'A third party component failed')
END

GO
