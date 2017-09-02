
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
#if SSC
    using SparkleSystems.Configuration.Data;
#else
    using System.IO;
    using Sparkle.Infrastructure.Contracts;
    using Sparkle.Infrastructure.Data;
    using Sparkle.Infrastructure.Data.Objects;
#endif

    /// <summary>
    /// Represents an application configuration.
    /// </summary>
    [DataContract(Namespace = Names.ServiceContractNamespace)]
    public class AppConfiguration : IDisposable
    {
        private readonly Application application;
        private bool disposed;
        private readonly IDictionary<string, AppConfigurationEntry> values;
#if SSC
#else
        private ConfigTree tree;
#endif
        private IList<UniverseDomainName> domainNames;

        /// <summary>
        /// Initializes a default datasource.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AppConfiguration()
        {
            RepositoryFactory = () =>
            {
                var configFactoName = ConfigurationManager.AppSettings["SparkleSystems.ConfigurationFactory"];
                if (!string.IsNullOrWhiteSpace(configFactoName))
                {
                    return Activator.CreateInstance(Type.GetType(configFactoName)) as IConfigurationRepository;
                }
                else
                {
                    return new SqlConfigurationRepository();
                }
            };
        }

        private AppConfiguration()
        {
            this.values = new Dictionary<string, AppConfigurationEntry>();
        }

        public AppConfiguration(Application application, IDictionary<string, AppConfigurationEntry> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (application == null)
                throw new ArgumentNullException("application");

            this.application = application;
            this.values = values;
        }

        internal AppConfiguration(AppConfiguration appConfiguration)
        {
            this.values = new Dictionary<string, AppConfigurationEntry>();
            if (appConfiguration.values != null)
            {
                foreach (var item in appConfiguration.values)
                {
                    this.values.Add(item.Key, new AppConfigurationEntry(item.Value));
                }
            }

            if (appConfiguration.Application != null)
            {
                this.application = new Application(appConfiguration.Application);
            }

            if (appConfiguration.domainNames != null)
            {
                this.domainNames = appConfiguration.domainNames
                    .Select(d => new UniverseDomainName(d))
                    .ToList();
            }
        }

        /// <summary>
        /// The datasource factory (default to <see cref="SqlConfigurationRepository"/>).
        /// </summary>
        public static Func<IConfigurationRepository> RepositoryFactory { get; set; }

        public static IConfigurationRepository NewRepository
        {
            get
            {
                if (RepositoryFactory == null)
                    throw new InvalidOperationException("AppConfiguration repository factory not configured");
                return RepositoryFactory();
            }
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        [DataMember]
        public Application Application
        {
            get { return this.application; }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        [DataMember]
        public IDictionary<string, AppConfigurationEntry> Values
        {
            get { return this.values; }
        }

#if SSC
#else
        /// <summary>
        /// Gets the values as a tree.
        /// </summary>
        public ConfigTree Tree
        {
            get { return this.tree ?? (this.tree = new ConfigTree(this.values)); }
        }
#endif

        #region Initializers

        /// <summary>
        /// Returns a configuration from the SparkleSystems appSettings and a domain name.
        /// </summary>
        /// <remarks>
        /// If SparkleSystems.Universe is empty or Auto, the domain name is used; otherwise
        /// the specified universe will be used.
        /// </remarks>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns>An application configuration</returns>
        /// <exception cref="UnknownApplicationException">if the specified application is not registered</exception>
        /// <exception cref="AppConfigurationException">both the domainname and the universe name are empty</exception>
        /// <exception cref="DataException">the datasource could not fullfill the request</exception>
        public static AppConfiguration CreateSingleFromWebConfiguration(string domainName)
        {
            string product = ConfigurationManager.AppSettings["SparkleSystems.Product"];
            string host = ConfigurationManager.AppSettings["SparkleSystems.Host"];
            string universe = ConfigurationManager.AppSettings["SparkleSystems.Universe"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(host))
                throw new AppConfigurationException("Application misconfigured, check the AppSettings. ", product, universe, host);

            universe = universe.Trim();

            int applicationId = 0;
            IDictionary<string, AppConfigurationEntry> values;
            Application app;

            using (var repository = RepositoryFactory())
            {
                if (string.IsNullOrEmpty(universe) || universe.ToUpperInvariant() == "AUTO")
                {
                    if (string.IsNullOrWhiteSpace(domainName))
                        throw new AppConfigurationException("AppConfiguration misconfigured, check the AppSettings. Universe is set to Auto and no domain name was specified.");

                    // use domain binding to choose universe
                    applicationId = repository.FindApplicationIdByDomainName(product, host, domainName);
                }
                else
                {
                    // use configuration to choose universe
                    applicationId = repository.FindApplicationId(product, host, universe);
                }

                app = repository.FindApplicationById(applicationId);
                values = repository.FetchValues(applicationId);
                var domains = repository.GetUniversesDomainNames(app.UniverseId);

                var appConfig = new AppConfiguration(app, values)
                {
                    DomainNames = domains,
                };
                
                return appConfig;
            }
        }

        /// <summary>
        /// Returns a configuration from the SparkleSystems appSettings with the specified universe.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <returns>An application configuration</returns>
        /// <exception cref="UnknownApplicationException">if the specified application is not registered</exception>
        /// <exception cref="AppConfigurationException">appSettings are missing</exception>
        /// <exception cref="DataException">the datasource could not fullfill the request</exception>
        public static AppConfiguration CreateSingleFromConfiguration(string universe)
        {
            string product = ConfigurationManager.AppSettings["SparkleSystems.Product"];
            string host = ConfigurationManager.AppSettings["SparkleSystems.Host"];

            if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(universe))
                throw new AppConfigurationException("Application misconfigured, check the AppSettings. ", product, universe, host);

            int applicationId = 0;
            IDictionary<string, AppConfigurationEntry> values;
            Application app;

            using (var repository = RepositoryFactory())
            {
                applicationId = repository.FindApplicationId(product, host, universe);
                app = repository.FindApplicationById(applicationId);
                values = repository.FetchValues(applicationId);
            }

            return new AppConfiguration(app, values);
        }

        /// <summary>
        /// Returns a configuration from the SparkleSystems appSettings.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <returns>An application configuration</returns>
        /// <exception cref="UnknownApplicationException">if the specified application is not registered</exception>
        /// <exception cref="AppConfigurationException">appSettings are missing</exception>
        /// <exception cref="DataException">the datasource could not fullfill the request</exception>
        public static AppConfiguration CreateSingleFromConfiguration()
        {
            string product = ConfigurationManager.AppSettings["SparkleSystems.Product"];
            string host = ConfigurationManager.AppSettings["SparkleSystems.Host"];
            string universe = ConfigurationManager.AppSettings["SparkleSystems.Universe"];

            if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(universe))
                throw new AppConfigurationException("Application misconfigured, check the AppSettings. ", product, universe, host);
            if (string.IsNullOrEmpty(universe) || universe.ToUpperInvariant() == "AUTO")
                throw new AppConfigurationException("Application misconfigured, check the AppSettings. ", product, universe, host);

            int applicationId = 0;
            IDictionary<string, AppConfigurationEntry> values;
            Application app;

            using (var repository = RepositoryFactory())
            {
                applicationId = repository.FindApplicationId(product, host, universe);
                app = repository.FindApplicationById(applicationId);
                values = repository.FetchValues(applicationId);
            }

            return new AppConfiguration(app, values);
        }

        public static IList<AppConfiguration> CreateManyFromConfiguration(IEnumerable<string> universes)
        {
            if (universes == null)
                throw new ArgumentNullException("universes");

            string product = ConfigurationManager.AppSettings["SparkleSystems.Product"];
            string host = ConfigurationManager.AppSettings["SparkleSystems.Host"];
            var list = new List<AppConfiguration>();

            if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(host))
                throw new AppConfigurationException("Application misconfigured, check the AppSettings. ", product, "-", host);


            using (var repository = RepositoryFactory())
            {
                foreach (var universe in universes)
                {
                    int applicationId = 0;
                    IDictionary<string, AppConfigurationEntry> values;
                    Application app;

                    applicationId = repository.FindApplicationId(product, host, universe);
                    app = repository.FindApplicationById(applicationId);
                    values = repository.FetchValues(applicationId);

                    list.Add(new AppConfiguration(app, values));
                }
            }

            return list;
        }

        /// <summary>
        /// Returns a list of configurations from the SparkleSystems appSettings with all universes supported by the host.
        /// </summary>
        /// <returns>An application configuration list</returns>
        /// <exception cref="AppConfigurationException">appSettings are missing</exception>
        /// <exception cref="DataException">the datasource could not fullfill the request</exception>
        public static IList<AppConfiguration> CreateManyFromConfiguration()
        {
            string product = ConfigurationManager.AppSettings["SparkleSystems.Product"];
            string host = ConfigurationManager.AppSettings["SparkleSystems.Host"];
            var list = new List<AppConfiguration>();

            if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(host))
                throw new AppConfigurationException("Application misconfigured, check the AppSettings. ", product, "-", host);

            using (var repository = RepositoryFactory())
            {
                IList<Application> apps = repository.FindApplications(product, host);

                foreach (Application app in apps)
                {
                    int applicationId = app.Id;
                    IDictionary<string, AppConfigurationEntry> values = repository.FetchValues(applicationId);

                    list.Add(new AppConfiguration(app, values));
                }
            }

            return list;
        }

        /// <summary>
        /// Returns a list of configurations from the SparkleSystems appSettings with all universes supported by the host.
        /// </summary>
        /// <returns>An application configuration list</returns>
        /// <exception cref="AppConfigurationException">appSettings are missing</exception>
        /// <exception cref="DataException">the datasource could not fullfill the request</exception>
        public static IList<Application> GetUniversesFromConfiguration()
        {
            string product = ConfigurationManager.AppSettings["SparkleSystems.Product"];
            string host = ConfigurationManager.AppSettings["SparkleSystems.Host"];
            IList<Application> list;

            if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(host))
                throw new AppConfigurationException("Application misconfigured, check the AppSettings. ", product, "-", host);

            using (var repository = RepositoryFactory())
            {
                list = repository.FindApplications(product, host);
            }

            return list;
        }

        /// <summary>
        /// Returns a configuration for the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>An application configuration</returns>
        /// <exception cref="DataException">the datasource could not fullfill the request</exception>
        public static AppConfiguration CreateFromApplication(Application application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            IDictionary<string, AppConfigurationEntry> values;

            using (var repository = RepositoryFactory())
            {
                values = repository.FetchValues(application.Id);
            }

            return new AppConfiguration(application, values);
        }

        #endregion

        public AppConfiguration Clone()
        {
            return new AppConfiguration(this);
        }

        #region IDisposable members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.values.Clear();
#if SSC
#else
                    this.tree = null;
#endif
                }

                this.disposed = true;
            }
        }

        #endregion

        public IList<UniverseDomainName> DomainNames
        {
            get { return this.domainNames; }
            set { this.domainNames = value; }
        }
    }
}
