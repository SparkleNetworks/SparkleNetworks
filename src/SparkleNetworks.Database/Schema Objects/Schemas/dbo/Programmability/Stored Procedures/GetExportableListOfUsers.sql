

CREATE PROCEDURE [dbo].[GetExportableListOfUsers]
    @networkId int
AS

declare @now datetime = SYSUTCDATETIME()

;WITH MaxLive
AS
(
    SELECT l.UserId, MAX(l.DateTime) LastActivity
    FROM dbo.Live l
    INNER JOIN dbo.Users u ON u.Id = l.UserId AND u.NetworkId = @networkId
    GROUP BY l.UserId
),
CurrentSubs
AS
(
    SELECT AppliesToUserId, COUNT(Id) CurrentActiveSubscriptions
    from dbo.Subscriptions
    where 
        (AppliesToUserId is not null and DateBeginUtc is not null and DateBeginUtc <= @now and DateEndUtc is null)
    or
        (AppliesToUserId is not null and DateBeginUtc is not null and DateBeginUtc <= @now and DateEndUtc is not null and @now <= DateEndUtc)
    group by AppliesToUserId
)

SELECT 
    u.[Id],u.[Login],u.[Email],
    u.[FirstName],u.[LastName],u.[Gender], upfPhone.Value [Phone],
    u.[CompanyID],c.Name CompanyName,c.Alias CompanyAlias, c.IsEnabled CompanyIsEnabled,
    u.[JobId],j.Libelle JobName,j.Alias JobAlias,
    u.[AccountClosed],u.[CompanyAccessLevel],u.[NetworkAccessLevel],u.[IsEmailConfirmed],
    m.CreateDate, m.LastLoginDate, l.LastActivity,
    COALESCE(cs.CurrentActiveSubscriptions, 0) CurrentActiveSubscriptions,
    u.PersonalDataUpdateDateUtc
FROM [dbo].[Users] u
LEFT JOIN dbo.Jobs j on j.Id = u.JobId
INNER JOIN dbo.Companies c on c.Id = u.CompanyId
INNER JOIN dbo.aspnet_Membership m on m.UserId = u.UserId
LEFT JOIN MaxLive l ON l.UserId = u.Id
LEFT JOIN dbo.UserProfileFields upfPhone ON upfPhone.UserId = u.Id and upfPhone.ProfileFieldId = 2
LEFT JOIN CurrentSubs cs on cs.AppliesToUserId = u.id
WHERE u.NetworkId = @networkId
ORDER BY FirstName, LastName
