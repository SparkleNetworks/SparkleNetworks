
* [Index](0000-Index.md)
* Next: [Code layers](1001-Code-layers.md)

System deployment overview
===========================


The classical stack!
-----------------------

Sparkle Networks consists of a few websites and a SQL database.

### Database: Microsoft SQL Server 2008 R2 or later

The project started with 2008 R1. At a given time we started to use `Geography` types and had to move to 2008 R2.

The project has also been deployed with SQL 2012, SQL 2012 R2 and SQL 2014.

### Web server: Microsoft IIS 7

The application uses .NET Framework 4.5. A move to 4.6.1 is to be expected.

How to install IIS? Open the "Turn Windows Features on or off" in windows. You should the a "Internet Information Services" checkbox. Check it.

When installing IIS, check the following items. Don't un-check items that auto-selected themselves. 

* Web Management Tools
	* IIS Management Console
	* IIS Management Service (if you want to do [WebDeploy](https://www.iis.net/downloads/microsoft/web-deploy))
	* IIS Management Scripts and Tools
* World Wide Web Services
	* Application Development Features
		* .NET Extensibility 4.6
		*  ASP.NET 4.6
	*  Common HTTP Features
		*  Static content
	*  Health and Diagnostics
		*  HTTP Logging (optionnal)
		*  Logging tools
		*  Tracing (optionnal)
	*  Performance Features
		*  Dynamic Content Compression
		*  Static Content Compression

While you're in this Windows window, check .NET Framework 3.5 (it is required by SQL Server).


IIS Web farm
-------------------

If you plan on making a web farm (multiple web servers to server one website), make sure you configure the session provider to use a single service for all web servers.

[Configure a State Server to Maintain Session State](https://technet.microsoft.com/en-us/library/cc732412(v=ws.10).aspx)  
[Web farms in .NET and IIS part 5: Session state management](https://dotnetcodr.com/2013/07/01/web-farms-in-net-and-iis-part-5-session-state-management/)  
[PRB: Session State Is Lost in Web Farm If You Use SqlServer or StateServer Session Mode](https://support.microsoft.com/en-gb/kb/325056)  


Configuration database
----------------------------

We made a choice to store the application configuration in a specific database. It is called SparkleSystems. This is quite useful when you plan on hosting many networks. It might appear hard to tackle if you host just a single network.











