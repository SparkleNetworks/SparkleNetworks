
* [Index](0000-Index.md)
* Previous: [System deployment overview](1000-System-deployment-overview.md)
* Next: ???


Code layers
================











Introduction
---------------

This project is built using a multi-layer code architecture. You find the classic elements: database, DAL, domain layer (called Services), different UIs and library projects.

![Code layers](1001-Code-layers\CodeLayers.png)











Databases
---------------

### Visual Studio Database Projects

We are using Visual Studio database projects (VS 2013 and later). Don't forget to update SQL Server Data Tools (SSDT) if you have a fresh install.

This kind of project allows you to:

* define the desired schema using T-SQL files (+ a intuitive UI)
* one-click deploy of database changes on developer machines
* easy deploy of database on production servers

More info about Visual Studio database projects:

* [MSDN: Working with Database Projects](https://msdn.microsoft.com/en-us/library/xee70aty.aspx)
* [blog post: Simplifying Development with Visual Studio Database Projects](https://visualstudiomagazine.com/articles/2014/12/01/visual-studio-database-projects.aspx), [Leveraging a Visual Studio Database Project](https://visualstudiomagazine.com/articles/2015/01/01/visual-studio-database-project.aspx)
* [blog post: Using Visual Studio database projects in real life](http://weblogs.asp.net/gunnarpeipman/using-visual-studio-database-projects-in-real-life)
* [blog post: Creating a SQL Server Database Project in Visual Studio 2012](https://candordeveloper.com/2013/01/08/creating-a-sql-server-database-project-in-visual-studio-2012/)

Downloads:

* [Install SQL Server Data Tools](https://msdn.microsoft.com/en-us/hh500335(v=vs.103).aspx)
* [Download SQL Server Data Tools (SSDT)](https://msdn.microsoft.com/en-us/library/mt204009.aspx?f=255&MSPPError=-2147217396)
* [Download SQL Server Data Tools in Visual Studio 2013](https://www.microsoft.com/en-us/download/details.aspx?id=42313)
* [Download SQL Server Data Tools in Visual Studio 2012](https://msdn.microsoft.com/en-us/jj650015)

Historically, we were using the VS 2010 tools (sqlpackage and stuff). The projects have been migrated to SSDT.

You may find a few SQL objects prefixed with `eura_` (mostly indexes and constraints). This is a remnant of the very first version of the project. Please ignore the prefix as it is an artifact of a past naming convention.


### The three databases

There are 3 database projects. You have to deploy all of them in 3 databases.

* SparkleSystems.Database: network configuration and domain logging
* SparkleNetworks.Database: the Main database (which stores network data)
* Sparkle.NetworksStatus.Database: see details below


### SparkleSystems.Database

We chose to use a database to store [configuration for Sparkle Networks networks](0000-Missing-documentation.md). This was the key to deploy many networks with a centralized configuration system. This is a little bit inspired from the configuration database that you find in Microsoft TFS and Microsoft Commerce Server.

We plan to provide an alternative way of providing configuration to the applications to avoid using an extra database when putting in production a single Sparkle Networks network. You can make a pull request though.

If you deploy this database from scratch, it will contain the definition for a demo network.

It is easier to use the [Sparkle Systems WPF app](http://sparklenetworks.com/internals/systems/Default.htm) to change configuration values.

### SparkleNetworks.Database

This is the big Main database. It can store data for multiple network (multi-tenant). This is why almost every table has a `NetworkId` column.

We already created important indexes in the database to make it perform fast. It is not an easy job to create useful indexes, so please share the indexes that you create and find relevant to performance.

If you deploy this database from scratch, some tables will be initialized with content. Some other tables needs initialization via the Setup controller.

### Sparkle.NetworksStatus.Database

This tiny database is used for tiny centrilized operations.

* the status website serve as a bridge for linkedin authentication
* the status website serve as a cache for geocoding requests (the cache is in the database)












Data Access Layer
------------------------

![Code layers](1001-Code-layers\VS-projects-data.png)

How this layer is built:

* there is Entity Framework 5 code at many places
    * The repositories implementation and the EF context are in the `Sparkle.Data.Entity` project
    * The EDMX file is located: src\Internal\Sparkle.Data.Entity\Networks\Model\NetworksModel.edmx
    * We are using the Database first approach 
    * It generates the `ObjectContext` class using a T4 file: src\Internal\Sparkle.Data.Entity\Networks\Model\NetworksModel.tt
    * The Entities are in the `Sparkle.Entities` project
    * Entities are generated by a T4 file: src\Public\Sparkle.Entities\Networks\NetworksModelEntities.tt
    * Extensions to entities are generates by a T4 file: src\Public\Sparkle.Entities\Networks\NetworksModelEntitiesExtensions.tt 
* The repository pattern is used to organize data operations on a by-table basis
    * The repositories expose the entities or other custom objects that represent stored data
    * One repository per table
    * A RepositoryFactory serves as en entry point to the layer and allows easy access to repository instances

What you can find in each project

* Sparkle.Entities
    * Entity Framework generated Entity classes
    * Non-Entity Framework classes that represent data
* Sparkle.Data
    * The `IRepositoryFactory` interface that is in charge of instantiating repositories for you (generated code)
    * One repository interface per SQL table
    * Non-Entity Framework classes that represent data
    * Extension methods relating to LINQ (EF) queries
* Sparkle.Data.Entity
    * The EDMX file and the `ObjectContext` generated code
    * The implementation of all the interfaces you find in `Sparkle.Data`

Resources:

* [MSDN: Microsoft Application Architecture Guide, 2nd Edition - Chapter 8: Data Layer Guidelines](https://msdn.microsoft.com/en-us/library/ee658127.aspx)
* [MSDN: Entity Framework > Get Started > Database First](https://msdn.microsoft.com/en-us/data/jj206878.aspx?f=255&MSPPError=-2147217396)
* [MSDN: Entity Framework > Documentation](https://msdn.microsoft.com/en-us/data/ee712907.aspx)
* [MSDN: Entity Framework > Get Started > Querying/Finding Entities](https://msdn.microsoft.com/en-us/data/jj573936.aspx)
* [MSDN: Stored Procedures in the Entity Framework](https://msdn.microsoft.com/en-us/data/gg699321.aspx)
* [MSDN: Entity Framework > Get Started > EF Designer Code Generation Templates](https://msdn.microsoft.com/en-us/data/jj613116.aspx)
* [MSDN: The Repository Pattern](https://msdn.microsoft.com/en-us/library/ff649690.aspx?f=255&MSPPError=-2147217396)

Related documentation: 

* [Creating a new repository and service](0000-Missing-documentation.md)
* [Mapping a stored procedure](0000-Missing-documentation.md)
* [Update the DAL after changing the database structure](0000-Missing-documentation.md)














The Domain layer (aka the Services layer)
----------------------------------------------

This is the most important layer. This is where all the operations are defined. 

### The `IServiceFactory` class / The `services` object

The guidelines behind this structure:

* Allow developers of any level to easily access the important stuff
* Organize the big number of operations in logical containers
* Have a consistent way to do the authorization checks
* Have a consistent way to receive an operation request and receive an operation result (the Request-Result pattern)

All the executable projects that make use of this layer should ensure developers can always access a fresh `services` object to work with.

* In the ASP MVC projects
    * controllers gives you a `this.Services` property
    * there is a unique services instance for each http query, accessible with `httpContext.GetNetworkServices()`
* In the CLI projects
    * a context object has a `context.Services` property

This object is strongly related to ONE of the networks in the database. All the operations relate to the current network. Some operations relate to all networks, as indicated in the name of the method.

#### The entry point for all operations

GET operations:

    var groupCategories = this.Services.GroupsCategories.SelectAll();
    var request = this.Services.People.GetApplyRequestRequest(Key, null, this.UserId, CompanyCategory);
    var eventWithPlaceAndCategory = this.Services.Events.GetById(id, EventOptions.Category | EventOptions.Place);
    this.Services.PrivateMessage.CountUnread(user.Id);
    this.Services.Ads.CountNewAdsForUser(user.Id);
    this.Services.CompaniesVisits.GetByCompanyIdAndUserIdAndDay(company.ID, this.UserId.Value, DateTime.Now.Date);
    var isGroupAdmin = this.Services.GroupsMembers.IsAdmin(this.UserId.Value, groupId)
    var userPicture = this.Services.People.GetProfilePictureUrl(item, UserProfilePictureSize.Medium, UriKind.Relative);

Sample Request-Result operation:

    var createAdRequest = new EditAdRequest();
    createAdRequest.Title = "...";
    createAdRequest.ActingUserId = 12;
    var result = this.Services.Ads.Edit(request);
    if (result.Succeed)
    {
        // show success message
    }
    else
    {
        // show error messages from result.Errors
    }

You can also bypass the layer and directly access the DAL. This is not recommended but you may feel the need to do it in specific cases (data import, data refresh).

    var group42 = this.Services.Repositories.Groups.GetById(42);

You have access to the network configuration tree.

    this.Services.AppConfiguration.Tree.Features.Ads.IsEnabled
    this.Services.AppConfiguration.Tree.Externals.MailChimp.ApiKey
    this.Services.AppConfiguration.Tree.Features.Users.DefaultGender


#### The services

Every concept has a service to provide operations. Groups, Timelines, Events, People, Companies, Ads...

### Instantiating the services object

It's not super-easy to create it from scratch (form a new project for exemple). This is why we created a class to facilitate that: `SparkleNetworksApplication`. All you have to do is create this object from the desired configuration. Many methods are available.

From a web app related to a single network, the method of choice is to create the object based on the current domain name (of the HTTP request).

    // find the current domain name (say mynetwork.com)
    string domainName = context.Request.Url.Host;

    // create the app object with the status WebCreate method (you have to specify the source for email templates)
    var app = SparkleNetworksApplication.WebCreate(domainName, () => new DefaultEmailTemplateProvider());
    
    // you need a store for the cache. ASP.NET has one.
    var cache = new AspnetServiceCache(httpContext.Cache);

    // then you can get an many services instances as you want from this object
    var services = app.GetNewServiceFactory(cache);

There is also a standard way.

    // ask sparkle system for the configuration object (multiple methods are available)
    var config = AppConfiguration.CreateSingleFromConfiguration("network 42")
    var config = AppConfiguration.CreateSingleFromWebConfiguration("mynetwork.com")
    
    // create theapp object with the status WebCreate method (you have to specify the source for email templates)
    var app = new SparkleNetworksApplication(config, () => new DefaultEmailTemplateProvider());
    
    // you need a store for the cache. we provide a basic in-memory implementation.
    var cache = new BasicServiceCache();

    // then you can get an many services instances as you want from this object
    var services = app.GetNewServiceFactory(cache);

There is also a low-level way, which is hard to get right. Look at the `SparkleNetworksApplication.GetNewServiceFactoryImpl` method. You observe that a many dependencies need to be instantiated right and be given to the `MainServiceFactory` class constructor. Then the current network must be selected and verified.

### Code Inventory

The `Sparkle.Services` project contains all the abstractions (interfaces, domain entities).

* Sparkle.Services\Networks
    *  All service interfaces are declared here. This is where you find the Domain Methods.
* Sparkle.Services\Networks\Models, Sparkle.Services\Networks\Ads, Sparkle.Services\Networks\Events, Sparkle.Services\Networks\Groups...
    * Those folders contains Model objects that represent the concepts in Sparkle Networks
    * They also contain the classes that implement the Request-Result pattern
* Sparkle.Services\Authentication
    * Contains a abstract proxy to ASP.NET Membership that helps authenticate users

The `Sparkle.Services.Main` project contains all the implementations.

* Sparkle.Services\Networks
    *  All service implementations are declared here. This is where you find the Domain Methods.
* Sparkle.Services\Authentication
    * Contains a the proxy to ASP.NET Membership that helps authenticate users
* Sparkle.Services\EmailModels
    * For each kind of email that can be sent, there is a data class that contains the email data structure
* Sparkle.Services\Providers
    * Contains implementation of popular external services (email providers for example)

The `Sparkle.NetworksStatus` project contains a mini domain layer and DAL for the WebStatus website.



Resources:

* [MSDN: Microsoft Application Architecture Guide, 2nd Edition - Chapter 7: Business Layer Guidelines](https://msdn.microsoft.com/en-us/library/ee658103.aspx)
* [MSDN: Microsoft Application Architecture Guide, 2nd Edition - Chapter 13: Designing Business Entities](https://msdn.microsoft.com/en-us/library/ee658106.aspx)
* [Introduction to Membership](https://msdn.microsoft.com/en-us/library/yh26yfzy.aspx?f=255&MSPPError=-2147217396)


Related documentation: 

* [Creating a new repository and service](0000-Missing-documentation.md)
* [Analysis of a Request-Result method](0000-Missing-documentation.md)









The UI layer
-------------------------


TODO: describe the Sparkle.Web project

TODO: describe the Sparkle.Commands project

TODO: describe the Sparkle.WebStatus project






















Extras
---------------------


This diagram shows you the layers and the dependencies of the entire project.

![Code layers](1001-Code-layers\CodeLayers-Commented.png)





![Code layers](1001-Code-layers\VS-projects.png)





