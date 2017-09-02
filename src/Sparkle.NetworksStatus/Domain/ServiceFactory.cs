
namespace Sparkle.NetworksStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.NetworksStatus.Data;
    using Sparkle.NetworksStatus.Data.Repositories;
    using Sparkle.NetworksStatus.Domain.Cache;
    using Sparkle.NetworksStatus.Domain.Internals;
    using Sparkle.NetworksStatus.Domain.Services;

    partial class ServiceFactory : IServiceFactoryEx
    {
        private SqlClientRepositoryFactory repositories;
        private EmailService emailService;
        private CacheService cacheService;
        private IDomainCacheProvider cache;

        public ServiceFactory(ServicesConfiguration configuration, IDomainCacheProvider cache)
        {
            this.configuration = configuration;
            this.cache = cache ?? new BasicDomainCacheProvider();
            this.initialize();
        }

        public IRepositoryFactory Repositories
        {
            get
            {
                this.CheckDisposed();
                if (this.repositories == null)
                {
                    SqlClientRepositoryFactory item = this.AddDisposable(this.GetNewRepositoryFactory());
                    this.repositories = this.AddDisposable<SqlClientRepositoryFactory>(this.GetNewRepositoryFactory());
                }

                return repositories;
            }
        }

        public IRepositoryFactory NewTransaction
        {
            get
            {
                this.CheckDisposed();
                return this.AddDisposable((SqlClientRepositoryFactory)this.GetNewRepositoryFactory().BeginTransaction());
            }
        }

        public IEmailService EmailService
        {
            get { return this.emailService ?? (this.emailService = new EmailService(this)); }
        }

        public ICacheService Cache
        {
            get { return this.cacheService ?? (this.cacheService = new CacheService(this, this.cache)); }
        }

        ServicesConfiguration IServiceFactoryEx.Configuration
        {
            get { return this.configuration; }
        }

        private SqlClientRepositoryFactory GetNewRepositoryFactory()
        {
            return new SqlClientRepositoryFactory(this.configuration.DatabaseConnectionString);
        }

        partial void initialize()
        {
        }

        public void Verify()
        {
            var errors = new List<Tuple<string, Exception>>();

            if (string.IsNullOrEmpty(this.configuration.SmtpHost))
            {
                errors.Add(new Tuple<string, Exception>("Configuration does not have a SMTP configuration", null));
            }

            if (string.IsNullOrEmpty(this.configuration.AppUrl))
                errors.Add(new Tuple<string, Exception>("Configuration does not specify AppUrl", null));

            if (string.IsNullOrEmpty(this.configuration.DatabaseConnectionString))
                errors.Add(new Tuple<string, Exception>("Configuration does not specify DatabaseConnectionString", null));

            try
            {
                this.NetworkRequests.Count();
            }
            catch (Exception ex)
            {
                errors.Add(new Tuple<string, Exception>("Fails to access database", ex));
            }

            if (errors.Count > 0)
            {
                var agg = new InvalidOperationException("NetworksStatusServices verification failed with " + errors.Count + " errors" + Environment.NewLine + string.Join(Environment.NewLine, errors.Select(e => e.Item1 + (e.Item2 != null ? (": " + e.Item2.Message) : string.Empty))));
                agg.Data["Errors"] = errors;
                throw agg;
            }
        }
    }
}
