
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
#if !SSC
    using Sparkle.Infrastructure.Data.Objects;
#endif

    /// <summary>
    /// WCF implementation of <see cref="IConfigurationRepository"/>.
    /// Use the appSetting SparkleSystems.WcfConfigurationFactory.EndpointConfigurationName to determine the endpoint to load from configuration or loads the endpoint named SparkleSystems.WcfConfigurationFactory.DefaultEndpoint.
    /// </summary>
    public class WcfConfigurationFactory : IConfigurationRepository
    {
        private string endpointConfigurationName;
        private ChannelFactory<IConfigurationRepository> factory;
        private IConfigurationRepository channel;

        /// <summary>
        /// WCF implementation of <see cref="IConfigurationRepository"/>.
        /// </summary>
        public WcfConfigurationFactory()
        {
            var nameFromAppSettings = ConfigurationManager.AppSettings["SparkleSystems.WcfConfigurationFactory.EndpointConfigurationName"];
            if (nameFromAppSettings != null)
            {
                this.endpointConfigurationName = nameFromAppSettings;
            }
            else
            {
                this.endpointConfigurationName = "SparkleSystems.WcfConfigurationFactory.DefaultEndpoint";
            }
        }

        /// <summary>
        /// WCF implementation of <see cref="IConfigurationRepository"/>.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public WcfConfigurationFactory(string endpointConfigurationName)
        {
            this.endpointConfigurationName = endpointConfigurationName;
        }

        private ChannelFactory<IConfigurationRepository> Factory
        {
            get { return this.factory ?? (this.factory = this.CreateFactory()); }
        }

        private IConfigurationRepository Channel
        {
            get { return this.channel ?? (this.channel = this.CreateChannel(this.Factory)); }
        }

        public int FindApplicationId(string product, string host, string universe)
        {
            return this.Channel.FindApplicationId(product, host, universe);
        }

        public int FindApplicationIdByDomainName(string product, string host, string domainName)
        {
            return this.Channel.FindApplicationIdByDomainName(product, host, domainName);
        }

        public Application FindApplicationById(int applicationId)
        {
            return this.Channel.FindApplicationById(applicationId);
        }

        public IDictionary<string, AppConfigurationEntry> FetchValues(int applicationId)
        {
            return this.Channel.FetchValues(applicationId);
        }

        public IList<AppConfigurationEntry> FetchKeys()
        {
            return this.Channel.FetchKeys();
        }

        public IList<Application> FindApplications(string product, string host)
        {
            return this.Channel.FindApplications(product, host);
        }

        public Application FindApplication(string product, string host, string universe)
        {
            return this.Channel.FindApplication(product, host, universe);
        }

        public IList<Application> GetAllApplications()
        {
            return this.Channel.GetAllApplications();
        }

        public IList<Host> GetAllHosts()
        {
            return this.Channel.GetAllHosts();
        }

        public IList<Universe> GetAllUniverses()
        {
            return this.Channel.GetAllUniverses();
        }

        public IList<Product> GetAllProducts()
        {
            return this.Channel.GetAllProducts();
        }

        public Application CreateApplication(int product, int host, int universe, short status)
        {
            return this.Channel.CreateApplication(product, host, universe, status);
        }

        public IList<UniverseDomainName> GetUniversesDomainNames(int universeId)
        {
            return this.Channel.GetUniversesDomainNames(universeId);
        }

        public int SetValue(int applicationId, int keyId, int index, string rawValue)
        {
            return this.Channel.SetValue(applicationId, keyId, index, rawValue);
        }

        public int AddKey(string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            return this.Channel.AddKey(name, blitableType, summary, isRequired, isCollection, defaultValue);
        }

        public void UpdateKey(int id, string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            this.Channel.UpdateKey(id, name, blitableType, summary, isRequired, isCollection, defaultValue);
        }

        public void DeleteValue(int applicationId, int keyId, int index)
        {
            this.Channel.DeleteValue(applicationId, keyId, index);
        }

        public int AddProduct(string name, string displayName)
        {
            return this.Channel.AddProduct(name, displayName);
        }

        public void UpdateProduct(int id, string name, string displayName)
        {
            this.Channel.UpdateProduct(id, name, displayName);
        }

        public int AddUniverse(string name, string displayName, short status)
        {
            return this.Channel.AddUniverse(name, displayName, status);
        }

        public void UpdateUniverse(int id, string name, string displayName, short status)
        {
            this.Channel.UpdateUniverse(id, name, displayName, status);
        }

        public void ChangeUniverseStatus(int id, short status)
        {
            this.Channel.ChangeUniverseStatus(id, status);
        }

        public int AddHost(string name)
        {
            return this.Channel.AddHost(name);
        }

        public void UpdateHost(int id, string name)
        {
            this.Channel.UpdateHost(id, name);
        }

        public void ChangeAppStatus(int id, short name)
        {
            this.Channel.ChangeAppStatus(id, name);
        }

        public int AddUniverseDomainName(int universeId, string name, bool redirectToMain)
        {
            return this.Channel.AddUniverseDomainName(universeId, name, redirectToMain);
        }

        public void UpdateUniverseDomainName(int id, string name, bool redirectToMain)
        {
            this.Channel.UpdateUniverseDomainName(id, name, redirectToMain);
        }

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            ////GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases managed and - optionally - unmanaged resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.CloseChannel();
            }
        }

        #endregion

        #region Internals

        private ChannelFactory<IConfigurationRepository> CreateFactory()
        {
            return new ChannelFactory<IConfigurationRepository>(this.endpointConfigurationName);
        }

        private IConfigurationRepository CreateChannel(ChannelFactory<IConfigurationRepository> factory)
        {
            return factory.CreateChannel();
        }

        private void CloseChannel()
        {
            var factory = this.factory;
            if (factory != null)
            {
                this.factory = null;
                this.channel = null;
                try
                {
                    factory.Close();
                }
                catch (CommunicationObjectFaultedException ex)
                {
                    factory.Abort();
                }
            }
        }

        #endregion
    }
}
