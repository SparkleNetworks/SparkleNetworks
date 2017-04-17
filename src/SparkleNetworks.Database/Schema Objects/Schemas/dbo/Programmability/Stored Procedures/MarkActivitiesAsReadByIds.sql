
CREATE PROCEDURE [dbo].[MarkActivitiesAsReadByIds]
	@ids varchar(MAX) = ''
AS

SET NOCOUNT ON

-- Convert string to table of ints 
DECLARE @table TABLE (Id int)
insert into @table
select Id = Item from dbo.SplitInts(@ids)

-- now update activities
update dbo.Activities
set Displayed = 1
where Id in (select Id from @table) AND Displayed = 0
	
RETURN @@ROWCOUNT
