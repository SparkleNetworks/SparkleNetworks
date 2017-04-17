CREATE ROLE [ConfigurationReader]
GO

GRANT EXECUTE ON OBJECT::[dbo].FindApplication TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplication2 TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplications TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplications2 TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplicationId TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplicationById2 TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplicationById TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].FindApplicationIdByDomainName TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].GetConfigurationValuesForApplication TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].GetConfigurationKeys TO [ConfigurationReader]
GO
GRANT EXECUTE ON OBJECT::[dbo].GetUniversesDomainNames TO [ConfigurationReader]
GO
