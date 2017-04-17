
// This file was automatically generated by the PetaPoco T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: `sqlserver`
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `Data Source=SparkleSqlDev;Initial Catalog=SparkleNetworksStatus;Integrated Security=True`
//     Schema:                 ``
//     Include Views:          `False`


// PetaPoco.Generator.ttinclude

namespace Sparkle.NetworksStatus.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using PetaPoco;
	using PetaPoco.Core;

#region Context

	public partial class PetaContext : Database
	{
		public PetaContext() 
			: base("sqlserver")
		{
			CommonConstruct();
		}

		public PetaContext(string connectionStringName) 
			: base(connectionStringName)
		{
			CommonConstruct();
		}
		
        public PetaContext(string connectionString, string providerName)
            : base(connectionString, providerName)
        {
            CommonConstruct();
        }

		partial void CommonConstruct();
		
		public interface IFactory
		{
			PetaContext GetInstance();
		}
		
		public static IFactory Factory { get; set; }
		public class Record<T> : IContextAwarePoco where T : new()
		{
			[Ignore]
            public PetaContext repo {
                get { return (PetaContext)this.Database; }
            }
			[Ignore]
            public Database Database { get; set; }
			public bool IsNew() { return repo.IsNew(this); }
			public object Insert() { return repo.Insert(this); }
			public void Save() { repo.Save(this); }
			public int Update() { return repo.Update(this); }
			public int Update(IEnumerable<string> columns) { return repo.Update(this, columns); }
			public int Delete() { return repo.Delete(this); }
		}
	}
	

#endregion

#region POCOs
    
	[TableName("Cache")]
	[PrimaryKey("Id")]
	[ExplicitColumns]
    public partial class Cache : PetaContext.Record<Cache>  
    {
		[Column] public int Id { get; set; }
		[Column] public byte Type { get; set; }
		[Column] public string Name { get; set; }
		[Column] public string Value { get; set; }
		[Column] public DateTime DateCreatedUtc { get; set; }
	}
    
	[TableName("EmailAddresses")]
	[PrimaryKey("Id")]
	[ExplicitColumns]
    public partial class EmailAddress : PetaContext.Record<EmailAddress>  
    {
		[Column] public int Id { get; set; }
		[Column] public string AccountPart { get; set; }
		[Column] public string TagPart { get; set; }
		[Column] public string DomainPart { get; set; }
		[Column] public DateTime DateCreatedUtc { get; set; }
		[Column] public DateTime? DateConfirmedUtc { get; set; }
		[Column] public bool IsClosed { get; set; }
	}
    
	[TableName("LinkedInRedirections")]
	[PrimaryKey("Id", autoIncrement=false)]
	[ExplicitColumns]
    public partial class LinkedInRedirection : PetaContext.Record<LinkedInRedirection>  
    {
		[Column] public Guid Id { get; set; }
		[Column] public int UserId { get; set; }
		[Column] public int Scope { get; set; }
		[Column] public string ApiKey { get; set; }
		[Column] public string State { get; set; }
		[Column] public string ReturnUrl { get; set; }
		[Column] public DateTime DateCreatedUtc { get; set; }
	}
    
	[TableName("NetworkRequests")]
	[PrimaryKey("Id")]
	[ExplicitColumns]
    public partial class NetworkRequest : PetaContext.Record<NetworkRequest>  
    {
		[Column] public int Id { get; set; }
		[Column] public DateTime DateCreatedUtc { get; set; }
		[Column] public string RemoteAddress { get; set; }
		[Column] public string Culture { get; set; }
		[Column] public Guid? AdminCode { get; set; }
		[Column] public Guid? WebId { get; set; }
		[Column] public string ContactFirstname { get; set; }
		[Column] public string ContactLastname { get; set; }
		[Column] public string ContactPhoneNumber { get; set; }
		[Column] public string ContactEmailAccount { get; set; }
		[Column] public string ContactEmailTag { get; set; }
		[Column] public string ContactEmailDomain { get; set; }
		[Column] public string NetworkName { get; set; }
		[Column] public long NetworkSize { get; set; }
		[Column] public string NetworkCity { get; set; }
		[Column] public string NetworkCountry { get; set; }
		[Column] public string NetworkSubdomain { get; set; }
	}
    
	[TableName("UserAuthenticationAttempts")]
	[PrimaryKey("Id")]
	[ExplicitColumns]
    public partial class UserAuthenticationAttempt : PetaContext.Record<UserAuthenticationAttempt>  
    {
		[Column] public int Id { get; set; }
		[Column] public int UserId { get; set; }
		[Column] public DateTime DateUtc { get; set; }
		[Column] public byte ErrorCode { get; set; }
		[Column] public byte[] RemoteAddressValue { get; set; }
		[Column] public string RemoteAddressLocation { get; set; }
		[Column] public string UserAgent { get; set; }
		[Column] public string Token { get; set; }
		[Column] public DateTime? LastTokenUsage { get; set; }
	}
    
	[TableName("UserPasswords")]
	[PrimaryKey("Id")]
	[ExplicitColumns]
    public partial class UserPassword : PetaContext.Record<UserPassword>  
    {
		[Column] public int Id { get; set; }
		[Column] public int UserId { get; set; }
		[Column] public DateTime DateCreatedUtc { get; set; }
		[Column] public string PasswordValue { get; set; }
		[Column] public string PasswordType { get; set; }
		[Column] public DateTime? DateLockedUtc { get; set; }
		[Column] public int VerifiedUsages { get; set; }
		[Column] public DateTime? LastVerifiedUsageDateUtc { get; set; }
		[Column] public DateTime? FirstFailedTentativeDateUtc { get; set; }
		[Column] public DateTime? LastFailedTentativeDateUtc { get; set; }
		[Column] public int FailedTentatives { get; set; }
	}
    
	[TableName("Users")]
	[PrimaryKey("Id")]
	[ExplicitColumns]
    public partial class User : PetaContext.Record<User>  
    {
		[Column] public int Id { get; set; }
		[Column] public string DisplayName { get; set; }
		[Column] public string Firstname { get; set; }
		[Column] public string Lastname { get; set; }
		[Column] public string Country { get; set; }
		[Column] public string Culture { get; set; }
		[Column] public string Timezone { get; set; }
		[Column] public DateTime DateCreatedUtc { get; set; }
		[Column] public short Status { get; set; }
		[Column] public int PrimaryEmailAddressId { get; set; }
	}

#endregion

}


// PetaPoco.Repos.ttinclude

namespace Sparkle.NetworksStatus.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using PetaPoco;
	using Sparkle.NetworksStatus.Data.Repositories;

    
    public partial interface ICachesRepository : IRepository<Cache, int>
    {
	}
    
    public partial interface IEmailAddresssRepository : IRepository<EmailAddress, int>
    {
	}
    
    public partial interface ILinkedInRedirectionsRepository : IRepository<LinkedInRedirection, Guid>
    {
	}
    
    public partial interface INetworkRequestsRepository : IRepository<NetworkRequest, int>
    {
	}
    
    public partial interface IUserAuthenticationAttemptsRepository : IRepository<UserAuthenticationAttempt, int>
    {
	}
    
    public partial interface IUserPasswordsRepository : IRepository<UserPassword, int>
    {
	}
    
    public partial interface IUsersRepository : IRepository<User, int>
    {
	}
	
}

namespace Sparkle.NetworksStatus.Data
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using PetaPoco;
	using Sparkle.NetworksStatus.Data.Repositories;

	public partial interface IRepositoryFactory
	{
    
		ICachesRepository Caches { get; }
    
		IEmailAddresssRepository EmailAddresss { get; }
    
		ILinkedInRedirectionsRepository LinkedInRedirections { get; }
    
		INetworkRequestsRepository NetworkRequests { get; }
    
		IUserAuthenticationAttemptsRepository UserAuthenticationAttempts { get; }
    
		IUserPasswordsRepository UserPasswords { get; }
    
		IUsersRepository Users { get; }
	}
}


// PetaPoco.SqlClient.ttinclude

namespace Sparkle.NetworksStatus.Data.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using PetaPoco;
	using Sparkle.NetworksStatus.Data.Repositories;
    using Sparkle.Infrastructure;

    
    public partial class SqlCachesRepository : BaseSqlRepository<Cache, int>, ICachesRepository
    {
		internal const string TableName = "dbo.Cache";

        public SqlCachesRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }
	}
    
    public partial class SqlEmailAddresssRepository : BaseSqlRepository<EmailAddress, int>, IEmailAddresssRepository
    {
		internal const string TableName = "dbo.EmailAddresses";

        public SqlEmailAddresssRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }
	}
    
    public partial class SqlLinkedInRedirectionsRepository : BaseSqlRepository<LinkedInRedirection, Guid>, ILinkedInRedirectionsRepository
    {
		internal const string TableName = "dbo.LinkedInRedirections";

        public SqlLinkedInRedirectionsRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }
	}
    
    public partial class SqlNetworkRequestsRepository : BaseSqlRepository<NetworkRequest, int>, INetworkRequestsRepository
    {
		internal const string TableName = "dbo.NetworkRequests";

        public SqlNetworkRequestsRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }
	}
    
    public partial class SqlUserAuthenticationAttemptsRepository : BaseSqlRepository<UserAuthenticationAttempt, int>, IUserAuthenticationAttemptsRepository
    {
		internal const string TableName = "dbo.UserAuthenticationAttempts";

        public SqlUserAuthenticationAttemptsRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }
	}
    
    public partial class SqlUserPasswordsRepository : BaseSqlRepository<UserPassword, int>, IUserPasswordsRepository
    {
		internal const string TableName = "dbo.UserPasswords";

        public SqlUserPasswordsRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }
	}
    
    public partial class SqlUsersRepository : BaseSqlRepository<User, int>, IUsersRepository
    {
		internal const string TableName = "dbo.Users";

        public SqlUsersRepository(SqlClientRepositoryFactory factory)
            : base(factory)
        {
        }
	}
	
	public partial class SqlClientRepositoryFactory : IRepositoryFactory
	{
        private readonly CompositeDisposable disposable = new CompositeDisposable();
    
		private ICachesRepository cachesRepository;
		public ICachesRepository Caches
		{
            [System.Diagnostics.DebuggerStepThrough]
			get { return this.cachesRepository ?? (this.cachesRepository = new SqlCachesRepository(this)); }
		}

		private IEmailAddresssRepository emailAddresssRepository;
		public IEmailAddresssRepository EmailAddresss
		{
            [System.Diagnostics.DebuggerStepThrough]
			get { return this.emailAddresssRepository ?? (this.emailAddresssRepository = new SqlEmailAddresssRepository(this)); }
		}

		private ILinkedInRedirectionsRepository linkedInRedirectionsRepository;
		public ILinkedInRedirectionsRepository LinkedInRedirections
		{
            [System.Diagnostics.DebuggerStepThrough]
			get { return this.linkedInRedirectionsRepository ?? (this.linkedInRedirectionsRepository = new SqlLinkedInRedirectionsRepository(this)); }
		}

		private INetworkRequestsRepository networkRequestsRepository;
		public INetworkRequestsRepository NetworkRequests
		{
            [System.Diagnostics.DebuggerStepThrough]
			get { return this.networkRequestsRepository ?? (this.networkRequestsRepository = new SqlNetworkRequestsRepository(this)); }
		}

		private IUserAuthenticationAttemptsRepository userAuthenticationAttemptsRepository;
		public IUserAuthenticationAttemptsRepository UserAuthenticationAttempts
		{
            [System.Diagnostics.DebuggerStepThrough]
			get { return this.userAuthenticationAttemptsRepository ?? (this.userAuthenticationAttemptsRepository = new SqlUserAuthenticationAttemptsRepository(this)); }
		}

		private IUserPasswordsRepository userPasswordsRepository;
		public IUserPasswordsRepository UserPasswords
		{
            [System.Diagnostics.DebuggerStepThrough]
			get { return this.userPasswordsRepository ?? (this.userPasswordsRepository = new SqlUserPasswordsRepository(this)); }
		}

		private IUsersRepository usersRepository;
		public IUsersRepository Users
		{
            [System.Diagnostics.DebuggerStepThrough]
			get { return this.usersRepository ?? (this.usersRepository = new SqlUsersRepository(this)); }
		}

	}
}


