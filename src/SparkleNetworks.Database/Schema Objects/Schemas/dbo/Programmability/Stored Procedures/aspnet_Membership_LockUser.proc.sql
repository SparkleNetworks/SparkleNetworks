
CREATE PROCEDURE dbo.aspnet_Membership_LockUser
    @ApplicationName   nvarchar(256),
    @UserName          nvarchar(256),
    @CurrentTimeUtc    datetime
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF ( @UserId IS NULL )
        RETURN 1

    UPDATE dbo.aspnet_Membership
    SET IsLockedOut     = 1,
        LastLockoutDate = @CurrentTimeUtc
    WHERE @UserId = UserId

    RETURN 0
END
