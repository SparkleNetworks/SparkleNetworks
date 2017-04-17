CREATE PROCEDURE [dbo].[SetupProfileFieldsAvailableValues]
AS
SET NOCOUNT ON

print 'SetupProfileFieldsAvailableValues: updating ProfileFields table.'

DECLARE @ProfileFieldsAvailableValuesTemp TABLE (
		Id INT NOT NULL,
		ProfileFieldId INT NOT NULL,
		Value NVARCHAR(200) NOT NULL)
INSERT INTO @ProfileFieldsAvailableValuesTemp
([Id], [ProfileFieldId], [Value])
VALUES
(1,		12,	'Accounting'),
(2,		12,	'Airlines/Aviation'),
(3,		12,	'Alternative Dispute Resolution'),
(4,		12,	'Alternative Medicine'),
(5,		12,	'Animation'),
(6,		12,	'Apparel & Fashion'),
(7,		12,	'Architecture & Planning'),
(8,		12,	'Arts and Crafts'),
(9,		12,	'Automotive'),
(10,	12,	'Aviation & Aerospace'),
(11,	12,	'Banking'),
(12,	12,	'Biotechnology'),
(13,	12,	'Broadcast Media'),
(14,	12,	'Building Materials'),
(15,	12,	'Business Supplies and Equipment'),
(16,	12,	'Capital Markets'),
(17,	12,	'Chemicals'),
(18,	12,	'Civic & Social Organization'),
(19,	12,	'Civil Engineering'),
(20,	12,	'Commercial Real Estate'),
(21,	12,	'Computer & Network Security'),
(22,	12,	'Computer Games'),
(23,	12,	'Computer Hardware'),
(24,	12,	'Computer Networking'),
(25,	12,	'Computer Software'),
(26,	12,	'Construction'),
(27,	12,	'Consumer Electronics'),
(28,	12,	'Consumer Goods'),
(29,	12,	'Consumer Services'),
(30,	12,	'Cosmetics'),
(31,	12,	'Dairy'),
(32,	12,	'Defense & Space'),
(33,	12,	'Design'),
(34,	12,	'Education Management'),
(35,	12,	'E-Learning'),
(36,	12,	'Electrical/Electronic Manufacturing'),
(37,	12,	'Entertainment'),
(38,	12,	'Environmental Services'),
(39,	12,	'Events Services'),
(40,	12,	'Executive Office'),
(41,	12,	'Facilities Services'),
(42,	12,	'Farming'),
(43,	12,	'Financial Services'),
(44,	12,	'Fine Art'),
(45,	12,	'Fishery'),
(46,	12,	'Food & Beverages'),
(47,	12,	'Food Production'),
(48,	12,	'Fund-Raising'),
(49,	12,	'Furniture'),
(50,	12,	'Gambling & Casinos'),
(51,	12,	'Glass, Ceramics & Concrete'),
(52,	12,	'Government Administration'),
(53,	12,	'Government Relations'),
(54,	12,	'Graphic Design'),
(55,	12,	'Health, Wellness and Fitness'),
(56,	12,	'Higher Education'),
(57,	12,	'Hospital & Health Care'),
(58,	12,	'Hospitality'),
(59,	12,	'Human Resources'),
(60,	12,	'Import and Export'),
(61,	12,	'Individual & Family Services'),
(62,	12,	'Industrial Automation'),
(63,	12,	'Information Services'),
(64,	12,	'Information Technology and Services'),
(65,	12,	'Insurance'),
(66,	12,	'International Affairs'),
(67,	12,	'International Trade and Development'),
(68,	12,	'Internet'),
(69,	12,	'Investment Banking'),
(70,	12,	'Investment Management'),
(71,	12,	'Judiciary'),
(72,	12,	'Law Enforcement'),
(73,	12,	'Law Practice'),
(74,	12,	'Legal Services'),
(75,	12,	'Legislative Office'),
(76,	12,	'Leisure, Travel & Tourism'),
(77,	12,	'Libraries'),
(78,	12,	'Logistics and Supply Chain'),
(79,	12,	'Luxury Goods & Jewelry'),
(80,	12,	'Machinery'),
(81,	12,	'Management Consulting'),
(82,	12,	'Maritime'),
(83,	12,	'Market Research'),
(84,	12,	'Marketing and Advertising'),
(85,	12,	'Mechanical or Industrial Engineering'),
(86,	12,	'Media Production'),
(87,	12,	'Medical Devices'),
(88,	12,	'Medical Practice'),
(89,	12,	'Mental Health Care'),
(90,	12,	'Military'),
(91,	12,	'Mining & Metals'),
(92,	12,	'Motion Pictures and Film'),
(93,	12,	'Museums and Institutions'),
(94,	12,	'Music'),
(95,	12,	'Nanotechnology'),
(96,	12,	'Newspapers'),
(97,	12,	'Non-Profit Organization Management'),
(98,	12,	'Oil & Energy'),
(99,	12,	'Online Media'),
(100,	12,	'Outsourcing/Offshoring'),
(101,	12,	'Package/Freight Delivery'),
(102,	12,	'Packaging and Containers'),
(103,	12,	'Paper & Forest Products'),
(104,	12,	'Performing Arts'),
(105,	12,	'Pharmaceuticals'),
(106,	12,	'Philanthropy'),
(107,	12,	'Photography'),
(108,	12,	'Plastics'),
(109,	12,	'Political Organization'),
(110,	12,	'Primary/Secondary Education'),
(111,	12,	'Printing'),
(112,	12,	'Professional Training & Coaching'),
(113,	12,	'Program Development'),
(114,	12,	'Public Policy'),
(115,	12,	'Public Relations and Communications'),
(116,	12,	'Public Safety'),
(117,	12,	'Publishing'),
(118,	12,	'Railroad Manufacture'),
(119,	12,	'Ranching'),
(120,	12,	'Real Estate'),
(121,	12,	'Recreational Facilities and Services'),
(122,	12,	'Religious Institutions'),
(123,	12,	'Renewables & Environment'),
(124,	12,	'Research'),
(125,	12,	'Restaurants'),
(126,	12,	'Retail'),
(127,	12,	'Security and Investigations'),
(128,	12,	'Semiconductors'),
(129,	12,	'Shipbuilding'),
(130,	12,	'Sporting Goods'),
(131,	12,	'Sports'),
(132,	12,	'Staffing and Recruiting'),
(133,	12,	'Supermarkets'),
(134,	12,	'Telecommunications'),
(135,	12,	'Textiles'),
(136,	12,	'Think Tanks'),
(137,	12,	'Tobacco'),
(138,	12,	'Translation and Localization'),
(139,	12,	'Transportation/Trucking/Railroad'),
(140,	12,	'Utilities'),
(141,	12,	'Venture Capital & Private Equity'),
(142,	12,	'Veterinary'),
(143,	12,	'Warehousing'),
(144,	12,	'Wholesale'),
(145,	12,	'Wine and Spirits'),
(146,	12,	'Wireless'),
(147,	12,	'Writing and Editing')

BEGIN TRANSACTION
	DECLARE @Id INT
	DECLARE @ProfileFieldId INT
	DECLARE @Value NVARCHAR(200)
	DECLARE @RowCount INT = 0

	DECLARE TempCursor CURSOR
	FOR (SELECT [Id], [ProfileFieldId], [Value] FROM @ProfileFieldsAvailableValuesTemp)

	BEGIN TRY
		OPEN TempCursor
		FETCH NEXT FROM TempCursor INTO @Id, @ProfileFieldId, @Value
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[ProfileFieldsAvailiableValues] WHERE [Id] = @Id)
			BEGIN
				INSERT INTO [dbo].[ProfileFieldsAvailiableValues]
				([Id], [ProfileFieldId], [Value])
				VALUES
				(@Id, @ProfileFieldId, @Value)

				SET @RowCount = @RowCount + 1
			END
			ELSE
			BEGIN
				UPDATE [dbo].[ProfileFieldsAvailiableValues]
				SET [ProfileFieldId] = @ProfileFieldId,
					[Value] = @Value
				WHERE [Id] = @Id
			END

			FETCH NEXT FROM TempCursor INTO @Id, @ProfileFieldId, @Value
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
	print 'SetupProfileFieldsAvailableValues: ' + CAST(@RowCount AS VARCHAR) + ' new profile fields inserted successfuly.'
COMMIT TRANSACTION
RETURN 0
