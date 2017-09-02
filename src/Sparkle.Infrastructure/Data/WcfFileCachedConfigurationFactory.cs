
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
#if !SSC
    using Sparkle.Infrastructure.Data.Objects;
#endif

    /// <summary>
    /// WCF File-cached implementation of <see cref="IConfigurationRepository"/>.
    /// </summary>
    public class WcfFileCachedConfigurationFactory : IConfigurationRepository
    {
        private FileCachedConfigurationFactory source;
        private WcfConfigurationFactory wcf;

        /// <summary>
        /// Indicates the object was disposed.
        /// </summary>
        private bool disposed;

        public WcfFileCachedConfigurationFactory()
        {
        }

        private FileCachedConfigurationFactory Source
        {
            get
            {
                if (this.source == null)
                {
                    this.wcf = new WcfConfigurationFactory();
                    this.source = new FileCachedConfigurationFactory(this.wcf);
                }

                return this.source;
            }
        }

        public int FindApplicationId(string product, string host, string universe)
        {
            return this.Source.FindApplicationId(product, host, universe);
        }

        public int FindApplicationIdByDomainName(string product, string host, string domainName)
        {
            return this.Source.FindApplicationIdByDomainName(product, host, domainName);
        }

        public Application FindApplicationById(int applicationId)
        {
            return this.Source.FindApplicationById(applicationId);
        }

        public IDictionary<string, AppConfigurationEntry> FetchValues(int applicationId)
        {
            return this.Source.FetchValues(applicationId);
        }

        public IList<AppConfigurationEntry> FetchKeys()
        {
            return this.Source.FetchKeys();
        }

        public IList<Application> FindApplications(string product, string host)
        {
            return this.Source.FindApplications(product, host);
        }

        public Application FindApplication(string product, string host, string universe)
        {
            return this.Source.FindApplication(product, host, universe);
        }

        public IList<Application> GetAllApplications()
        {
            return this.Source.GetAllApplications();
        }

        public IList<Host> GetAllHosts()
        {
            return this.Source.GetAllHosts();
        }

        public IList<Universe> GetAllUniverses()
        {
            return this.Source.GetAllUniverses();
        }

        public IList<Product> GetAllProducts()
        {
            return this.Source.GetAllProducts();
        }

        public Application CreateApplication(int product, int host, int universe, short status)
        {
            return this.Source.CreateApplication(product, host, universe, status);
        }

        public IList<UniverseDomainName> GetUniversesDomainNames(int universeId)
        {
            return this.Source.GetUniversesDomainNames(universeId);
        }

        public int SetValue(int applicationId, int keyId, int index, string rawValue)
        {
            return this.Source.SetValue(applicationId, keyId, index, rawValue);
        }

        public int AddKey(string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            return this.Source.AddKey(name, blitableType, summary, isRequired, isCollection, defaultValue);
        }

        public void UpdateKey(int id, string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            this.Source.UpdateKey(id, name, blitableType, summary, isRequired, isCollection, defaultValue);
        }

        public void DeleteValue(int applicationId, int keyId, int index)
        {
            this.Source.DeleteValue(applicationId, keyId, index);
        }

        public int AddProduct(string name, string displayName)
        {
            return this.Source.AddProduct(name, displayName);
        }

        public void UpdateProduct(int id, string name, string displayName)
        {
            this.Source.UpdateProduct(id, name, displayName);
        }

        public int AddUniverse(string name, string displayName, short status)
        {
            return this.Source.AddUniverse(name, displayName, status);
        }

        public void UpdateUniverse(int id, string name, string displayName, short status)
        {
            this.Source.UpdateUniverse(id, name, displayName, status);
        }

        public void ChangeUniverseStatus(int id, short status)
        {
            this.Source.ChangeUniverseStatus(id, status);
        }

        public int AddHost(string name)
        {
            return this.Source.AddHost(name);
        }

        public void UpdateHost(int id, string name)
        {
            this.Source.UpdateHost(id, name);
        }

        public void ChangeAppStatus(int id, short status)
        {
            this.Source.ChangeAppStatus(id, status);
        }

        public int AddUniverseDomainName(int universeId, string name, bool redirectToMain)
        {
            return this.Source.AddUniverseDomainName(universeId, name, redirectToMain);
        }

        public void UpdateUniverseDomainName(int id, string name, bool redirectToMain)
        {
            this.Source.UpdateUniverseDomainName(id, name, redirectToMain);
        }

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases managed and - optionally - unmanaged resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.wcf != null)
                    {
                        this.wcf.Dispose();
                        this.wcf = null;
                    }

                    if (this.source == null)
                    {
                        this.source.Dispose();
                        this.source = null;
                    }
                }

                this.disposed = true;
            }
        }

        #endregion
    }
}
