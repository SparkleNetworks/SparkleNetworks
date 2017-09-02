
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using Sparkle.Data.Entity.Networks;
    using Sparkle.Data.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.UI;
    using Sparkle.Services.Networks.Models;
    using System.Threading;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Authentication;
    using Sparkle.Services.Main.Internal;

    sealed partial class MainServiceFactory : IServiceFactory, IDisposable
    {
        private static readonly List<ServiceFactoryAssertion> verifyRules;

        private static readonly Dictionary<string, Dictionary<int, NetworkModel>> networksCache = new Dictionary<string, Dictionary<int, NetworkModel>>(); // <universeName, <networkId, networkModel>>
        private static readonly ReaderWriterLockSlim networksCacheLock = new ReaderWriterLockSlim();
        private static DateTime networksCacheDate = DateTime.UtcNow;
        private static readonly TimeSpan networksCacheDuration = TimeSpan.FromMinutes(10D);

        private readonly string connectionString;
        private readonly Func<SysLogger> sysloggerFactory;
        private readonly Func<IEmailTemplateProvider> emailTemplateProviderFactory;
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        private bool disposed;
        private IRepositoryFactory repositoryFactory;
        private IEmailTemplateProvider emailTemplateProvider;
        private ILogger logger;
        private SysLogger syslogger;
        private NetworksServiceContext context;
        private HostingEnvironment hostingEnvironment;
        private INetworksTransaction transaction;

        private int networkId;
        private NetworkModel network;
        private UI.Strings strings;
        private CultureInfo[] supportedCultures;
        private TimeZoneInfo defaultTimezone;
        private CultureInfo defaultCulture;
        private ContextParallelismMode contextParallelismMode;
        private IServiceCache cache;
        private CacheService cacheService;
        private ServiceIdentity identity;
        private IMembershipService membershipService;

        static MainServiceFactory()
        {
            // self verification rules
            // will check configuration values are ok
            // will check some environment elements (folders, files)
            verifyRules = new List<ServiceFactoryAssertion>()
            {
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Storage.SparkleLangDirectory Directory.Exists",
                    s => Directory.Exists(s.MySelf.AppConfiguration.Tree.Storage.SparkleLangDirectory),
                    "Configuration entry 'Storage.SparkleLangDirectory' is not valid"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Storage.UserContentsDirectory Directory.Exists",
                    s => Directory.Exists(s.MySelf.AppConfiguration.Tree.Storage.UserContentsDirectory),
                    "Configuration entry 'Storage.UserContentsDirectory' is not valid"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.ConnectionStrings.NetworkApplicationServices !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.ConnectionStrings.NetworkApplicationServices),
                    "Configuration entry 'ConnectionStrings.NetworkApplicationServices' is not valid"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.ConnectionStrings.SysLogger !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.ConnectionStrings.SysLogger),
                    "Configuration entry 'ConnectionStrings.SysLogger' is not valid"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.DefaultCulture !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.DefaultCulture),
                    "Configuration entry 'DefaultCulture' is not valid"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Site.MainDomainName !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.Site.MainDomainName),
                    "Configuration entry 'Site.MainDomainName' is not valid"),
                new ServiceFactoryAssertion(
                    "s.MySelf.DefaultTimezone != null",
                    s => s.MySelf.DefaultTimezone != null,
                    "Configuration entry 'Features.I18N.DefaultTimezone' is not valid"),
                new ServiceFactoryAssertion(
                    "SupportedCultures.Length > 0",
                    s => s.MySelf.SupportedCultures != null && s.MySelf.SupportedCultures.Length > 0,
                    "Configuration entry 'Features.I18N.AvailableCultures' is not valid"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Features.I18N.AvailableCultures !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.Features.I18N.AvailableCultures)),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Features.I18N.AvailableCultures VerifyCultureListFromConfiguration",
                    s => VerifyCultureListFromConfiguration(s.MySelf.AppConfiguration.Tree.Features.I18N.AvailableCultures)),
                new ServiceFactoryAssertion(
                    "Repositories.ProfileFields.Count() > 0",
                    s => s.MySelf.Repositories.ProfileFields.Count() > 0,
                    "ProfileFields table is empty."),
                new ServiceFactoryAssertion(
                    "Repositories.UserProfileFields.Count() > 0",
                    s => s.MySelf.Repositories.UserProfileFields.Count() > 0,
                    "UserProfileField table is empty."),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Externals.SparkleStatus.BaseUrl !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.Externals.SparkleStatus.BaseUrl)),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Externals.SparkleStatus.ApiKey != Guid.Empty",
                    s => s.MySelf.AppConfiguration.Tree.Externals.SparkleStatus.ApiKey != Guid.Empty),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Features.I18N.DefaultTimezone !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.Features.I18N.DefaultTimezone),
                    "Configuration entry 'Features.I18N.DefaultTimezone' is not valid"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.ConnectionStrings.NetworkApplicationServices Should not enable SQL MARS. ",
                    s => s.MySelf.AppConfiguration.Tree.ConnectionStrings.NetworkApplicationServices.IndexOf("MultipleActiveResultSets=True", StringComparison.InvariantCultureIgnoreCase) < 0,
                    "Configuration entry 'ConnectionStrings.NetworkApplicationServices' is not valid, MARS should be disabled"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.ConnectionStrings.NetworkApplicationServices Should not enable SQL MARS. ",
                    s => s.MySelf.AppConfiguration.Tree.ConnectionStrings.NetworkApplicationServices.IndexOf("MARS=True", StringComparison.InvariantCultureIgnoreCase) < 0,
                    "Configuration entry 'ConnectionStrings.NetworkApplicationServices' is not valid, MARS should be disabled"),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.ConnectionStrings.NetworkEntities string.IsNullOrEmpty",
                    s => string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.ConnectionStrings.NetworkEntities),
                    "Configuration entry 'ConnectionStrings.NetworkEntities' is obsolete and should be cleared."),
                new ServiceFactoryAssertion(
                    "EmailAddress.TryCreate(s.MySelf.AppConfiguration.Tree.Features.Emails.ClassA.SenderAddress)",
                    s => (SrkToolkit.Common.Validation.EmailAddress.TryCreate(s.MySelf.AppConfiguration.Tree.Features.Emails.ClassA.SenderAddress) != null),
                    "Configuration entry 'Features.Emails.ClassA.SenderAddress' should be a valid email address."),
                new ServiceFactoryAssertion(
                    "EmailAddress.TryCreate(s.MySelf.AppConfiguration.Tree.Features.Emails.ClassB.SenderAddress)",
                    s => (SrkToolkit.Common.Validation.EmailAddress.TryCreate(s.MySelf.AppConfiguration.Tree.Features.Emails.ClassB.SenderAddress) != null),
                    "Configuration entry 'Features.Emails.ClassB.SenderAddress' should be a valid email address."),
                new ServiceFactoryAssertion(
                    "EmailAddress.TryCreate(s.MySelf.AppConfiguration.Tree.Features.Emails.ClassC.SenderAddress)",
                    s => (SrkToolkit.Common.Validation.EmailAddress.TryCreate(s.MySelf.AppConfiguration.Tree.Features.Emails.ClassC.SenderAddress) != null),
                    "Configuration entry 'Features.Emails.ClassC.SenderAddress' should be a valid email address."),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Providers.Emails.ClassA !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassA),
                    "Configuration entry 'Providers.Emails.ClassA' should not be empty."),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Providers.Emails.ClassB !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassB),
                    "Configuration entry 'Providers.Emails.ClassB' should not be empty."),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Tree.Providers.Emails.ClassC !string.IsNullOrEmpty",
                    s => !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassC),
                    "Configuration entry 'Providers.Emails.ClassC' should not be empty."),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Values.ContainsKey(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassA)",
                    s => s.MySelf.AppConfiguration.Values.ContainsKey(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassA),
                    "Configuration entry 'Providers.Emails.ClassA' should contain a valid configuration key, 'AppConfiguration.Tree.Providers.Emails.ClassA' does not exists."),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Values.ContainsKey(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassB)",
                    s => s.MySelf.AppConfiguration.Values.ContainsKey(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassB),
                    "Configuration entry 'Providers.Emails.ClassB' should contain a valid configuration key, 'AppConfiguration.Tree.Providers.Emails.ClassB' does not exists."),
                new ServiceFactoryAssertion(
                    "AppConfiguration.Values.ContainsKey(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassC)",
                    s => s.MySelf.AppConfiguration.Values.ContainsKey(s.MySelf.AppConfiguration.Tree.Providers.Emails.ClassC),
                    "Configuration entry 'Providers.Emails.ClassC' should contain a valid configuration key, 'AppConfiguration.Tree.Providers.Emails.ClassC' does not exists."),
                new ServiceFactoryAssertion(
                    "Features.Users.RegisterInCompany is empty or integer",
                    s => 
                    {
                        if (s.MySelf.AppConfiguration.Values.ContainsKey("Features.Users.RegisterInCompany"))
                        {
                            var val = s.MySelf.AppConfiguration.Values["Features.Users.RegisterInCompany"];
                            int value;
                            return val == null || string.IsNullOrEmpty(val.RawValue) || int.TryParse(val.RawValue, out value);
                        }

                        return true;
                    },
                    "Configuration entry 'Features.Users.RegisterInCompany' should be empty or contain a valid company ID."),
                new ServiceFactoryAssertion(
                    "Security.Purposes." + KnownCryptoPurposes.PrivateCalendarUserToken + ".HashAlgorithmTypeName",
                    s => s.MySelf.AppConfiguration.Values.ContainsKey("Security.Purposes." + KnownCryptoPurposes.PrivateCalendarUserToken + ".HashAlgorithmTypeName"),
                    "Configuration entry 'Security.Purposes." + KnownCryptoPurposes.PrivateCalendarUserToken + ".HashAlgorithmTypeName' should contain a valid configuration value."),
                new ServiceFactoryAssertion(
                    "Security.Purposes." + KnownCryptoPurposes.PrivateCalendarUserToken + ".StaticNoise",
                    s => s.MySelf.AppConfiguration.Values.ContainsKey("Security.Purposes." + KnownCryptoPurposes.PrivateCalendarUserToken + ".StaticNoise") && !string.IsNullOrEmpty(s.MySelf.AppConfiguration.Values["Security.Purposes." + KnownCryptoPurposes.PrivateCalendarUserToken + ".StaticNoise"].RawValue),
                    "Configuration entry 'Security.Purposes." + KnownCryptoPurposes.PrivateCalendarUserToken + ".StaticNoise' should contain a valid configuration value."),
            };
        }

        public MainServiceFactory(
            string connectionString,
            Func<IEmailTemplateProvider> emailTemplateProvider,
            Func<SysLogger> syslogger,
            ContextParallelismMode parallelismMode,
            IServiceCache cache)
        {
            this.connectionString = connectionString;

            this.emailTemplateProviderFactory = emailTemplateProvider;
            this.emailTemplateProvider = emailTemplateProviderFactory();

            this.sysloggerFactory = syslogger;
            this.syslogger = sysloggerFactory();

            this.contextParallelismMode = parallelismMode;
            this.MySelf.HostingEnvironment.ParallelismMode = parallelismMode;

            this.cache = cache ?? new BasicServiceCache();
        }

        private MainServiceFactory(
            IRepositoryFactory repositories,
            Func<IEmailTemplateProvider> emailTemplateProvider,
            Func<SysLogger> syslogger,
            ContextParallelismMode parallelismMode,
            IServiceCache cache)
        {
            this.repositoryFactory = repositories;

            this.emailTemplateProviderFactory = emailTemplateProvider;
            this.emailTemplateProvider = emailTemplateProviderFactory();

            this.sysloggerFactory = syslogger;
            this.syslogger = sysloggerFactory();

            this.contextParallelismMode = parallelismMode;
            this.MySelf.HostingEnvironment.ParallelismMode = parallelismMode;

            this.cache = cache ?? new BasicServiceCache();
        }

        private IServiceFactory MySelf
        {
            get { return this; }
        }

        CultureInfo[] IServiceFactory.SupportedCultures
        {
            get
            {
                if (this.supportedCultures == null)
                {
                    var configValue = this.MySelf.AppConfiguration.Tree.Features.I18N.AvailableCultures;
                    if (string.IsNullOrEmpty(configValue))
                        configValue = "fr;fr-FR;en";

                    var cultures = configValue.Split(';');
                    this.supportedCultures = cultures
                        .Select(c =>
                        {
                            try
                            {
                                return new CultureInfo(c);
                            }
                            catch (CultureNotFoundException)
                            {
                                return null;
                            }
                        })
                        .Where(c => c != null)
                        .ToArray();
                }

                var copy = new CultureInfo[this.supportedCultures.Length];
                Array.Copy(this.supportedCultures, copy, this.supportedCultures.Length);
                return copy;
            }
        }

        CultureInfo IServiceFactory.DefaultCulture
        {
            get
            {
                if (this.defaultCulture == null)
                {
                    var configValue = this.MySelf.AppConfiguration.Tree.DefaultCulture;
                    if (string.IsNullOrEmpty(configValue))
                        configValue = this.MySelf.SupportedCultures.FirstOrDefault().Name;
                    if (string.IsNullOrEmpty(configValue))
                        configValue = "fr-FR";

                    this.defaultCulture = new CultureInfo(configValue);
                }

                return this.defaultCulture;
            }
        }

        TimeZoneInfo IServiceFactory.DefaultTimezone
        {
            get
            {
                if (this.defaultTimezone == null)
                {
                    var configValue = this.MySelf.AppConfiguration.Tree.Features.I18N.DefaultTimezone;
                    if (string.IsNullOrEmpty(configValue))
                        configValue = "Romance Standard Time";

                    this.defaultTimezone = TimeZoneInfo.FindSystemTimeZoneById(configValue);
                }

                return this.defaultTimezone;
            }
        }

        Strings IServiceFactory.Lang
        {
            get 
            { 
                return this.strings ?? (this.strings = SparkleLang.CreateStrings(
                    this.MySelf.AppConfiguration.Tree.Storage.SparkleLangDirectory,
                    this.MySelf.AppConfiguration.Application.UniverseName,
                    this.MySelf.Network.Type.Name));
            }
        }

        IRepositoryFactory IServiceFactory.Repositories
        {
            get
            {
                return this.repositoryFactory ??
                    (this.connectionString != null 
                     ? (this.repositoryFactory = new EntityRepositoryFactory(this.connectionString))
                     : null);
            }
            set { this.repositoryFactory = value; }
        }

        INetworksTransaction IServiceFactory.CurrentTransaction
        {
            get { return this.transaction; }
            set { this.transaction = value; }
        }

        IEmailTemplateProvider IServiceFactory.EmailTemplateProvider
        {
            get { return this.emailTemplateProvider; }
            set { this.emailTemplateProvider = value; }
        }

        ////ServiceIdentity IServiceFactory.Identity
        ////{
        ////    get { return this.identity ?? (this.identity = ServiceIdentity.Anonymous); }
        ////    set { this.identity = value; }
        ////}

        ILogger IServiceFactory.Logger
        {
            get { return this.logger ?? (this.logger = new Logger(this.repositoryFactory, this, this.syslogger, this.hostingEnvironment)); }
            set { this.logger = value; }
        }

        Application IServiceFactory.Application { get; set; }

        AppConfiguration IServiceFactory.AppConfiguration { get; set; }

        int IServiceFactory.NetworkId
        {
            get
            {
                this.VerifyNetworkIsSet();
                return this.networkId;
            }
            set { this.networkId = value; }
        }

        NetworkModel IServiceFactory.Network
        {
            get
            {
                if (this.network != null)
                {
                    if (this.network.Id != this.networkId)
                        this.network = null;
                }

                if (this.network == null && this.networkId != 0)
                {
                    networksCacheLock.EnterReadLock();
                    try
                    {
                        if ((networksCacheDate.Add(networksCacheDuration)) < DateTime.UtcNow)
                        {
                            // cache has expired
                        }
                        else
                        {
                            if (networksCache.ContainsKey(this.MySelf.Application.UniverseName))
                            {
                                if (networksCache[this.MySelf.Application.UniverseName].ContainsKey(this.networkId))
                                {
                                    var item = networksCache[this.MySelf.Application.UniverseName][this.networkId];
                                    if (item.Type != null)
                                    {
                                        this.network = item.Clone();
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        networksCacheLock.ExitReadLock();
                    }
                }

                if (this.network == null && this.networkId != 0)
                {
                    var item = this.MySelf.Repositories.Networks.GetById(this.networkId);
                    var itemType = this.MySelf.Repositories.NetworkTypes.GetById(item.NetworkTypeId);
                    this.network = new NetworkModel(item, itemType);

                    networksCacheLock.EnterWriteLock();
                    try
                    {
                        if ((networksCacheDate.Add(networksCacheDuration)) < DateTime.UtcNow)
                        {
                            networksCache.Clear();
                            networksCacheDate = DateTime.UtcNow;
                        }

                        if (!networksCache.ContainsKey(this.MySelf.Application.UniverseName))
                        {
                            networksCache.Add(this.MySelf.Application.UniverseName, new Dictionary<int, NetworkModel>());
                        }

                        if (!networksCache[this.MySelf.Application.UniverseName].ContainsKey(this.networkId))
                        {
                            networksCache[this.MySelf.Application.UniverseName].Add(this.networkId, this.network.Clone());
                        }
                    }
                    finally
                    {
                        networksCacheLock.ExitWriteLock();
                    }
                }

                return this.network;
            }
        }

        HostingEnvironment IServiceFactory.HostingEnvironment
        {
            get 
            {
                if (this.hostingEnvironment == null)
                {
                    this.hostingEnvironment = new HostingEnvironment();
                }

                return this.hostingEnvironment;
            }
            set { this.hostingEnvironment = value; }
        }

        public NetworksServiceContext Context
        {
            get
            {
                if (this.context == null)
                {
                    this.context = new NetworksServiceContext(this.contextParallelismMode);
                }

                return this.context;
            }
            set { this.context = value; }
        }

        public ICacheService Cache
        {
            get { return this.cacheService ?? (this.cacheService = new CacheService(this.repositoryFactory, this, this.cache)); }
        }

        IMembershipService IServiceFactory.MembershipService
        {
            get { return this.membershipService ?? (this.membershipService = this.GetAccountMembershipService()); }
        }

        IServiceFactory IServiceFactory.CreateNewFactory()
        {
            MainServiceFactory item = null;
            if (this.connectionString != null)
            {
                item = new MainServiceFactory(
                    this.connectionString,
                    this.emailTemplateProviderFactory,
                    this.sysloggerFactory,
                    this.contextParallelismMode,
                    this.cache);
            }
            else
            {
                item = new MainServiceFactory(
                    this.repositoryFactory.Clone(),
                    this.emailTemplateProviderFactory,
                    this.sysloggerFactory,
                    this.contextParallelismMode,
                    this.cache);
            }
            item.networkId = this.networkId;
            item.MySelf.AppConfiguration = this.MySelf.AppConfiguration.Clone();
            item.MySelf.Application = item.MySelf.AppConfiguration.Application;
            item.MySelf.HostingEnvironment = item.MySelf.HostingEnvironment;
            ////item.MySelf.Logger.BasePath = this.MySelf.Logger.BasePath;
            ////item.MySelf.Logger.RemoteClient = this.MySelf.Logger.RemoteClient;
            ////item.MySelf.Identity = this.MySelf.Identity != null ? this.MySelf.Identity.Clone() : null;

            return (IServiceFactory)item;
        }

        INetworksTransaction IServiceFactory.NewTransaction()
        {
            IDataTransaction transact;
            if (this.transaction != null)
            {
                transact = this.MySelf.Repositories.NewTransaction(this.transaction.DataTransaction);
            }
            else
            {
                transact = this.MySelf.Repositories.NewTransaction();
            }

            var services = (IServiceFactory)new MainServiceFactory(transact.Repositories, this.emailTemplateProviderFactory, this.sysloggerFactory, this.contextParallelismMode, this.cache);
            services.NetworkId = this.MySelf.NetworkId;
            services.AppConfiguration = this.MySelf.AppConfiguration;
            services.Application = this.MySelf.Application;
            services.HostingEnvironment = this.MySelf.HostingEnvironment;

            NetworksTransaction tr = null;
            if (this.MySelf.CurrentTransaction != null)
                tr = new NetworksTransaction(services, transact, this.MySelf.CurrentTransaction.PostSaveActions);
            else
                tr = new NetworksTransaction(services, transact);
            this.disposable.Add(services);
            this.disposable.Add(tr);
            services.CurrentTransaction = tr;
            return tr;
        }

        void IServiceFactory.Parallelize(Action<IServiceFactory> action)
        {
            var env = this.MySelf.HostingEnvironment;
            if (env.ParallelismMode == ContextParallelismMode.None)
            {
                action(this);
            }
            else if (env.ParallelismMode == ContextParallelismMode.ThreadPool)
            {
                if (env.ThreadPoolDelegate == null)
                    throw new InvalidOperationException("Instance is not correctly configured for ParallelismMode " + env.ParallelismMode + ": missing delegate");

                env.ThreadPoolDelegate(this, action);
            }
            else
            {
                throw new InvalidOperationException("ParallelismMode " + env.ParallelismMode + " is not supported");
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.repositoryFactory != null)
                    {
                        this.repositoryFactory.Dispose();
                        this.repositoryFactory = null;
                    }

                    if (this.membershipService != null)
                    {
                        ////this.membershipService.Dispose(); // does not seem disposable
                        this.membershipService = null;
                    }

                    // check currentTransaction because logger is the same everywhere
                    if (this.logger != null && this.transaction != null)
                    {
                        this.logger.Dispose();
                        this.logger = null;
                    }

                    var cache = this.cache;
                    if (cache != null)
                    {
                        cache.Dispose();
                        this.cache = null;
                    }

                    this.transaction = null;
                    this.emailTemplateProvider = null;

                    this.disposable.Dispose();
                }

                this.disposed = true;
            }
        }

        private void VerifyNetworkIsSet()
        {
            if (this.networkId <= 0)
            {
                throw new InvalidOperationException("Network ID is not set");
            }
        }

        void IServiceFactory.Initialize()
        {
            const string cacheKey = "ServiceFactory.Initialize";
            bool alreadyDone = this.cache.GetObject<bool>(cacheKey);
            if (alreadyDone)
            {
                return;
            }

            this.MySelf.EventsCategories.Initialize();
            this.MySelf.CompanyRelationships.Initialize();
            this.MySelf.Company.Initialize();
            this.MySelf.ProfileFields.Initialize();
            this.MySelf.PlacesCategories.Initialize();
            this.MySelf.Tags.Initialize();
            this.MySelf.Groups.Initialize();
            this.MySelf.Hints.Initialize();

            this.cache.Add(cacheKey, true, TimeSpan.FromMinutes(10D));
        }

        void IServiceFactory.Verify()
        {
            var errors = new List<Exception>();
            foreach (var item in verifyRules)
            {
                try
                {
                    item.Verify(this);
                }
                catch (Exception ex)
                {
                    if (ex.IsFatal())
                        throw;

                    errors.Add(ex);
                }
            }

            if (errors.Count > 0)
            {
                this.MySelf.Logger.Error("MainServiceFactory.Verify", ErrorLevel.Success, "Services verification failed with " + errors.Count + " errors." + Environment.NewLine + string.Join(Environment.NewLine, errors.Select(x => x.Message)));

                var agg = new AggregateException("Services verification failed with " + errors.Count + " errors", errors);
                throw agg;
            }
            else
            {
                this.MySelf.Logger.Info("MainServiceFactory.Verify", ErrorLevel.Success, "Services verification succeeded.");
            }
        }

        public string GetUrl(string path, IDictionary<string, string> query = null, string fragment = null)
        {
            var https = this.MySelf.AppConfiguration.Tree.Site.ForceSecureHttpGet
                     || this.MySelf.AppConfiguration.Tree.Site.ForceSecureHttpRequests;
            var domainName = this.MySelf.AppConfiguration.Tree.Site.MainDomainName;

            return string.Concat(
                https ? "https" : "http",
                "://",
                domainName,
                path.StartsWith("/") ? string.Empty : "/",
                Uri.EscapeUriString(path),
                query != null && query.Count > 0 ? "?" : string.Empty,
                query != null ? query.ToQueryString() : string.Empty,
                fragment != null ? "#" : string.Empty,
                fragment);
        }

        public string GetLocalUrl(string path, IDictionary<string, string> query = null, string fragment = null)
        {
            var https = this.MySelf.AppConfiguration.Tree.Site.ForceSecureHttpGet
                     || this.MySelf.AppConfiguration.Tree.Site.ForceSecureHttpRequests;
            var domainName = this.MySelf.AppConfiguration.Tree.Site.MainDomainName;

            return string.Concat(
                path.StartsWith("/") ? string.Empty : "/",
                Uri.EscapeUriString(path),
                query != null && query.Count > 0 ? "?" : string.Empty,
                query != null ? query.ToQueryString() : string.Empty,
                fragment != null ? "#" : string.Empty,
                fragment);
        }

        private static void VerifyCultureListFromConfiguration(string cultureList)
        {
            var cultures = cultureList.Split(';');
            cultures.Select(c => new CultureInfo(c)).ToArray();
        }

        private Sparkle.Services.Authentication.AccountMembershipService GetAccountMembershipService()
        {
            var mbsProvider = new System.Web.Security.SqlMembershipProvider();
            var membershipConfig = new System.Collections.Specialized.NameValueCollection();
            membershipConfig.Add("requiresQuestionAndAnswer", "False");
            membershipConfig.Add("minRequiredNonalphanumericCharacters", "0");
            membershipConfig.Add("passwordStrengthRegularExpression", null);
            membershipConfig.Add("connectionStringName", null);
            membershipConfig.Add("connectionString", this.MySelf.AppConfiguration.Tree.ConnectionStrings.NetworkApplicationServices);
            membershipConfig.Add("enablePasswordRetrieval", null);
            membershipConfig.Add("enablePasswordReset", null);
            membershipConfig.Add("applicationName", "/");
            membershipConfig.Add("requiresUniqueEmail", null);
            membershipConfig.Add("maxInvalidPasswordAttempts", null);
            membershipConfig.Add("passwordAttemptWindow", null);
            membershipConfig.Add("commandTimeout", null);
            membershipConfig.Add("passwordFormat", null);
            membershipConfig.Add("name", "AspNetSqlMembershipProvider");
            membershipConfig.Add("minRequiredPasswordLength", null);
            membershipConfig.Add("passwordCompatMode", null);
            mbsProvider.Initialize("AspNetSqlMembershipProvider", membershipConfig);
            var mbs = new Sparkle.Services.Authentication.AccountMembershipService(mbsProvider);
            return mbs;
        }
    }
}
