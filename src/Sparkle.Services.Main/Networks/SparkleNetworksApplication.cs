
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web;
    using Sparkle.Data.Entity.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Data;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Internals;

    public class SparkleNetworksApplication
    {
        private bool isDisposed;
        private AppConfiguration config;
        private Func<IEmailTemplateProvider> emailTemplateProviderFactory;
        private int networkId;
        private HostingEnvironment hostingEnvironment = new HostingEnvironment();

        [DebuggerStepThrough]
        public SparkleNetworksApplication(
            AppConfiguration config,
            Func<IEmailTemplateProvider> emailTemplateProviderFactory)
        {
            this.config = config;
            this.emailTemplateProviderFactory = emailTemplateProviderFactory;
        }

        public AppConfiguration Config
        {
            get { return this.config.Clone(); }
        }

        public HostingEnvironment HostingEnvironment
        {
            get { return this.hostingEnvironment; }
            set { this.hostingEnvironment = value; }
        }

        public static SparkleNetworksApplication WebCreate(string domainName, Func<IEmailTemplateProvider> emailTemplateProviderFactory)
        {
            AppConfiguration config;
            try
            {
                config = AppConfiguration.CreateSingleFromWebConfiguration(domainName);
            }
            catch (Exception ex)
            {
                throw new HttpException(500, "System configuration error", ex);
            }

            if (config.Application.MainStatusValue == -2)
            {
                throw new HttpException(503, "This application is disabled.");
            }

            if (config.Application.MainStatusValue < 0)
            {
                throw new HttpException(503, "This application is disabled or under maintenance.");
            }

            var app = new SparkleNetworksApplication(config, emailTemplateProviderFactory);
            app.HostingEnvironment.ParallelismMode = ContextParallelismMode.ThreadPool;
            app.HostingEnvironment.ThreadPoolDelegate = new Action<IServiceFactory,Action<IServiceFactory>>((s,a) =>
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    System.Web.Hosting.HostingEnvironment.IncrementBusyCount();
                    IServiceFactory services = null;
                    try
                    {
                        services = s.CreateNewFactory();
                        services.HostingEnvironment = s.HostingEnvironment;
                        ////services.Logger.BasePath = s.Logger.BasePath;
                        ////services.Logger.RemoteClient = s.Logger.RemoteClient;
                        ////services.Identity = s.Identity != null ? s.Identity.Clone() : null;

                        try
                        {
                            a(services);
                        }
                        catch (Exception ex)
                        {
                            services.Logger.Critical("SparkleNetworksApplication.WebCreate.ThreadPoolDelegate", ErrorLevel.Critical, ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("SparkleNetworksApplication.WebCreate.ThreadPoolDelegate" + ex.ToString());
                    }
                    finally
                    {
                        if (services != null)
                        {
                            services.Dispose();
                        }

                        System.Web.Hosting.HostingEnvironment.DecrementBusyCount();
                    }
                });
            });
            
            return app;
        }

        public IServiceFactory GetNewServiceFactoryWithoutCache()
        {
            return this.GetNewServiceFactoryImpl(new BasicServiceCache());
        }

        public IServiceFactory GetNewServiceFactory(IServiceCache serviceCache)
        {
            return this.GetNewServiceFactoryImpl(serviceCache);
        }

        /// <summary>
        /// Returns a new instance of mase services.
        /// Don't forget to <see cref="IDisposable.Dispose"/>() it!
        /// </summary>
        /// <returns></returns>
        private IServiceFactory GetNewServiceFactoryImpl(IServiceCache serviceCache)
        {
            if (serviceCache == null)
                throw new ArgumentNullException("serviceCache");

            var sysloggerFactory = new Func<SysLogger>(() =>
            {
                var sysloggerRepository = new SqlSysLogRepository(this.config.Tree.ConnectionStrings.SysLogger);
                var syslogger = new SysLogger(config.Application.Id, AppVersion.Full, sysloggerRepository);
                return syslogger;
            });

            var factory = new MainServiceFactory(
                this.config.Tree.ConnectionStrings.NetworkApplicationServices,
                this.emailTemplateProviderFactory,
                sysloggerFactory,
                this.HostingEnvironment.ParallelismMode,
                serviceCache);

            var services = (IServiceFactory)factory;
            services.AppConfiguration = this.config.Clone();
            services.Application = services.AppConfiguration.Application;

            if (this.HostingEnvironment.ThreadPoolDelegate != null)
            {
                services.HostingEnvironment.ThreadPoolDelegate = this.HostingEnvironment.ThreadPoolDelegate;
            }

            if (this.networkId == 0)
            {
                var networkname = this.config.Tree.NetworkName ?? this.config.Application.UniverseName;
                var network = services.Networks.GetByName(networkname);
                if (network == null)
                {
                    throw new AppConfigurationException("Could not find network '" + networkname + "' in database.");
                }

                this.networkId = network.Id;
            }

            services.NetworkId = networkId;

            // this is commented because Initialize should not occur too many times
            // the web app calls that too often
            ////services.Initialize();

            return factory;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    ////if (this.services.IsValueCreated)
                    ////{
                    ////    this.services.Value.Dispose();
                    ////    this.services = null;
                    ////}
                }

                this.isDisposed = true;
            }
        }
    }
}
