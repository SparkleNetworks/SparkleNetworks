
namespace Sparkle.Services.Networks.Mocks
{
    using System.Collections.Generic;

    /// <summary>
    /// Basic implementation of <see cref="IServiceFactory"/> for unit tests.
    /// </summary>
    [DebuggerStepThrough]
    public partial class BasicServiceFactory : IServiceFactory, IDisposable
    {
        private IRepositoryFactory repositoryFactory;
        private NetworksServiceContext context;

        public BasicServiceFactory(IRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        public bool Disposed { get; set; }

        public int NetworkId { get; set; }

        public NetworkModel Network { get; set; }

        public NetworksServiceContext Context
        {
            get
            {
                if (this.context == null)
                {
                    this.context = new NetworksServiceContext();
                }

                return this.context;
            }
            set { this.context = value; }
        }

        public System.Globalization.CultureInfo[] SupportedCultures
        {
            get { throw new NotImplementedException(); }
        }

        public TimeZoneInfo DefaultTimezone
        {
            get { throw new NotImplementedException(); }
        }

        public System.Globalization.CultureInfo DefaultCulture
        {
            get { throw new NotImplementedException(); }
        }

        public INetworksTransaction CurrentTransaction
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ICacheService Cache
        {
            get { throw new NotImplementedException(); }
        }

        public Authentication.IMembershipService MembershipService
        {
            get { throw new NotImplementedException(); }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Verify()
        {
            throw new NotImplementedException();
        }

        public IServiceFactory CreateNewFactory()
        {
            throw new NotImplementedException();
        }

        public INetworksTransaction NewTransaction()
        {
            throw new NotImplementedException();
        }

        public void Parallelize(Action<IServiceFactory> action)
        {
            throw new NotImplementedException();
        }

        public string GetUrl(string path, IDictionary<string, string> query = null, string fragment = null)
        {
            throw new NotImplementedException();
        }

        public string GetLocalUrl(string path, IDictionary<string, string> query = null, string fragment = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    if (this.repositoryFactory != null)
                    {
                        this.repositoryFactory.Dispose();
                        this.repositoryFactory = null;
                    }
                }
                this.Disposed = true;
            }
        }
    }
}
