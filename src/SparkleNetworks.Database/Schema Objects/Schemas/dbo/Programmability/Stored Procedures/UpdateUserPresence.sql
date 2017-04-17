
CREATE PROCEDURE [dbo].[UpdateUserPresence]
    @UserId int,
    @Day date,
    @Time datetime
AS

DECLARE @Id int
DECLARE @TimeTo datetime
SELECT @Id = Id, @TimeTo = TimeTo FROM dbo.UserPresences P WHERE P.UserId = @UserId AND P.Day = @Day

IF (@Id IS NOT NULL)
BEGIN

    IF (@TimeTo < @Time)
    UPDATE dbo.UserPresences
    SET TimeTo = @Time
    WHERE UserId = @UserId AND TimeTo = @TimeTo 

END
ELSE
BEGIN

    INSERT INTO dbo.UserPresences
    ( UserId, [Day], TimeFrom, TimeTo )
    VALUES
    ( @UserId, @Day, @Time, @Time )

END

