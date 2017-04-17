CREATE ROLE [LoggingRole]
GO
GRANT EXECUTE ON OBJECT::[dbo].AddLogEntry TO [LoggingRole]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplication TO [LoggingRole]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplicationId TO [LoggingRole]
GO