CREATE PROCEDURE [dbo].[SetTextValue]
	@textId INT,
	@culture NVARCHAR(5),
	@title NVARCHAR(140),
	@value NVARCHAR(MAX),
	@date DATETIME,
	@userId INT
AS

BEGIN TRANSACTION

	BEGIN TRY
		IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Text] WHERE [Id] = @textId)
		BEGIN
			INSERT INTO [dbo].[Text] DEFAULT VALUES
			SET @textId = SCOPE_IDENTITY()
		END
		
		IF EXISTS (SELECT TOP 1 1 FROM [dbo].[TextValue] WHERE [TextId] = @textId AND [CultureName] = @culture)
		BEGIN
			DECLARE @OldTitle NVARCHAR(140)
			DECLARE @OldValue NVARCHAR(MAX)
			DECLARE @OldDateUpdatedUtc DATETIME
			DECLARE @OldUpdatedByUserId INT

			SELECT TOP 1
				@OldTitle = [Title],
				@OldValue = [Value],
				@OldDateUpdatedUtc = [DateUpdatedUtc],
				@OldUpdatedByUserId = [UpdatedByUserId]
			FROM [dbo].[TextValue] WHERE [TextId] = @textId AND [CultureName] = @culture

			INSERT INTO [dbo].[TextValueArchive] (TextId, CultureName, Title, Value, DateUpdatedUtc, UpdatedByUserId)
			VALUES (@textId, @culture, @OldTitle, @OldValue, @OldDateUpdatedUtc, @OldUpdatedByUserId)

			UPDATE [dbo].[TextValue] SET
				Title = @title,
				Value = @value,
				DateUpdatedUtc = @date,
				UpdatedByUserId = @userId
			WHERE [TextId] = @textId AND [CultureName] = @culture
		END

		ELSE
		BEGIN
			INSERT INTO [dbo].[TextValue] (TextId, CultureName, Title, Value, DateUpdatedUtc, UpdatedByUserId)
			VALUES (@textId, @culture, @title, @value, @date, @userId)
		END

	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION
		SET @textId = NULL
	END CATCH

COMMIT TRANSACTION
SELECT @textId
