CREATE PROCEDURE [dbo].[SetupProfileFields]
AS
SET NOCOUNT ON

print 'SetupProfileFields: updating ProfileFields table.'

DECLARE @ProfileFieldsTemp TABLE (
		Id INT NOT NULL,
		Name NVARCHAR(60) NOT NULL,
		ApplyToUsers BIT NOT NULL,
		ApplyToCompanies BIT NOT NULL)
INSERT INTO @ProfileFieldsTemp
([Id], [Name], [ApplyToUsers], [ApplyToCompanies])
VALUES
(1,		'Site',						1,	1), -- 0..1
(2,		'Phone',					1,	1), -- 0..1
(3,		'About',					1,	1), -- 0..1
(4,		'City',						1,	1), -- 0..1
(5,		'ZipCode',					1,	1), -- 0..1
(6,		'FavoriteQuotes',			1,	0), -- 0..1
(7,		'CurrentTarget',			1,	0), -- 0..1
(8,		'Contribution',				1,	0), -- 0..1
(9,		'Country',					1,	1), -- 0..1
(10,	'Headline',					1,	0), -- 0..1
(11,	'ContactGuideline',			1,	0), -- 0..1
(12,	'Industry',					1,	1), -- 0..1
(13,	'LinkedInPublicUrl',		1,	1), -- 0..1
(14,	'Language',					1,	0), -- 0..∞
(15,	'Education',				1,	0), -- 0..∞
(16,	'Twitter',					1,	1), -- 0..∞
(17,	'GTalk',					1,	0), -- 0..∞
(18,	'Msn',						1,	0), -- 0..∞
(19,	'Skype',					1,	0), -- 0..∞
(20,	'Yahoo',					1,	0), -- 0..∞
(21,	'Volunteer',				1,	0), -- 0..∞
(22,	'Certification',			1,	0), -- 0..∞
(23,	'Patents',					1,	0), -- 0..∞
(24,	'Location',					1,	1), -- 0..∞
(25,	'Contact',					0,	0), -- 0..∞
(26,	'Recommendation',			1,	0), -- 0..∞
(27,	'Email',					0,	1), -- 0..1
(28,	'Facebook',					0,	1), -- 0..1
(29,	'AngelList',				0,	1), -- 0..1
(30,	'NetworkTeamRole',			1,	0), -- 0..1
(31,	'NetworkTeamDescription',	1,	0), -- 0..1
(32,	'NetworkTeamGroup',			1,	0), -- 0..1
(33,	'NetworkTeamOrder',			1,	0), -- 0..1
(69,	'Position',					1,	0)  -- 0..∞

BEGIN TRANSACTION
	DECLARE @Id INT
	DECLARE @Name NVARCHAR(60)
	DECLARE @ApplyToUsers BIT
	DECLARE @ApplyToCompanies BIT
	DECLARE @RowCount INT = 0

	DECLARE TempCursor CURSOR
	FOR (SELECT [Id], [Name], [ApplyToUsers], [ApplyToCompanies] FROM @ProfileFieldsTemp)

	BEGIN TRY
		OPEN TempCursor
		FETCH NEXT FROM TempCursor INTO @Id, @Name, @ApplyToUsers, @ApplyToCompanies
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[ProfileFields] WHERE [Id] = @Id)
			BEGIN
				INSERT INTO [dbo].[ProfileFields]
				([Id], [Name], [ApplyToUsers], [ApplyToCompanies])
				VALUES
				(@Id, @Name, @ApplyToUsers, @ApplyToCompanies)

				SET @RowCount = @RowCount + 1
			END
			ELSE
			BEGIN
				UPDATE [dbo].[ProfileFields]
				SET [Name] = @Name,
					[ApplyToUsers] = @ApplyToUsers,
					[ApplyToCompanies] = @ApplyToCompanies
				WHERE [Id] = @Id
			END

			FETCH NEXT FROM TempCursor INTO @Id, @Name, @ApplyToUsers, @ApplyToCompanies
		END
	END TRY

	BEGIN CATCH
		EXECUTE [dbo].[GetErrorInfo]
		CLOSE TempCursor
		DEALLOCATE TempCursor
		ROLLBACK TRANSACTION
		RETURN 1
	END CATCH

	CLOSE TempCursor
	DEALLOCATE TempCursor
	print 'SetupProfileFields: ' + CAST(@RowCount AS VARCHAR) + ' new profile fields inserted successfuly.'
COMMIT TRANSACTION
RETURN 0
