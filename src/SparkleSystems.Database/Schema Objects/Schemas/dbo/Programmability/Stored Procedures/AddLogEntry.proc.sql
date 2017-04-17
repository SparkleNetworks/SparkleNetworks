CREATE PROCEDURE [dbo].[AddLogEntry]
	@ApplicationId int,
	@ApplicationVersion nvarchar(20),
	@UtcDateTime datetime,
	@Type tinyint,
	@Path nvarchar(255),
	@RemoteClient nvarchar(50),
	@Identity nvarchar(50),
	@Error tinyint,
	@Data text = NULL
AS
	INSERT INTO [dbo].Logs
		([UtcDateTime], [Type], [ApplicationId], [ApplicationVersion], [Path], [RemoteClient], [Identity], [Error], [Data])
	VALUES
		(@UtcDateTime, @Type, @ApplicationId, @ApplicationVersion, @Path, @RemoteClient, @Identity, @Error, @Data)
		
RETURN 0