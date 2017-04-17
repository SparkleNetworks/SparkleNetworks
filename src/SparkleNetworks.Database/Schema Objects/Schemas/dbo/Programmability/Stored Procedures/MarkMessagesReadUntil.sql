--
-- STORED PROCEDURE
--     [dbo].[MarkMessagesReadUntil]
--
-- DESCRIPTION
--     For a conversation between 2 users.
--     Marks all messages as read up to the specified message id.
--
-- PARAMETERS
--     myUserId
--     otherUserId
--     messageId
--
-- RETURN VALUE
--
-- PROGRAMMING NOTES
--     N/A.
--

CREATE PROCEDURE [dbo].[MarkMessagesReadUntil]
	@myUserId int,
	@otherUserId int,
	@messageId int
AS

UPDATE dbo.Messages
SET Displayed = 1
where
(
	FromUserId = @otherUserId and ToUserId = @myUserId
)
and
(
	Id <= @messageId
)
and
(
	Displayed = 0
)

