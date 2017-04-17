
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;
#if SSC
#else
    using Sparkle.Infrastructure.Contracts;
    using Sparkle.Infrastructure.Data.Objects;
#endif

    /// <summary>
    /// Data access facade to access the central configuration store.
    /// </summary>
    [ServiceContract(Namespace = Names.ServiceContractNamespace, Name = "Configuration", SessionMode = SessionMode.NotAllowed)]
    public interface IConfigurationRepository : IDisposable
    {
        /// <summary>
        /// Finds the application id from a product/universe/universe couple.
        /// </summary>
        /// <param name="product">The product name.</param>
        /// <param name="host">The host name.</param>
        /// <param name="universe">The universe name.</param>
        /// <returns>The application ID</returns>
        /// <exception cref="UnknownApplicationException">if the specified application is not registered</exception>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        [OperationContract]
        int FindApplicationId(string product, string host, string universe);

        /// <summary>
        /// Finds the application id from a product/universe/domain-name couple.
        /// </summary>
        /// <param name="product">The product name.</param>
        /// <param name="host">The host name.</param>
        /// <param name="universe">The universe's domain name.</param>
        /// <returns>The application ID</returns>
        /// <exception cref="UnknownApplicationException">if the specified application is not registered</exception>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        [OperationContract]
        int FindApplicationIdByDomainName(string product, string host, string domainName);

        /// <summary>
        /// Finds the application details with its id.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <returns>The application details</returns>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        [OperationContract]
        Application FindApplicationById(int applicationId);

        /// <summary>
        /// Fetches the configuration values for the specified application.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <returns>Configuration values associated to the specified application, including the non-specified ones.</returns>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        [OperationContract]
        IDictionary<string, AppConfigurationEntry> FetchValues(int applicationId);

        /// <summary>
        /// Fetches the configuration keys.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        [OperationContract]
        IList<AppConfigurationEntry> FetchKeys();

        /// <summary>
        /// Fetches all applications for a host and product.
        /// </summary>
        /// <param name="product">The product name.</param>
        /// <param name="host">The host name.</param>
        /// <returns></returns>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        [OperationContract]
        IList<Application> FindApplications(string product, string host);

        /// <summary>
        /// Fetches all applications for a host and product.
        /// </summary>
        /// <param name="product">The product name.</param>
        /// <param name="host">The host name.</param>
        /// <param name="universe">The universe.</param>
        /// <returns></returns>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        Application FindApplication(string product, string host, string universe);

        /// <summary>
        /// Gets all applications.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<Application> GetAllApplications();

        /// <summary>
        /// Gets all hosts.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<Host> GetAllHosts();

        /// <summary>
        /// Gets all universes.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<Universe> GetAllUniverses();

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<Product> GetAllProducts();

        /// <summary>
        /// Creates an application from existing product/host/universe.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="host">The host.</param>
        /// <param name="universe">The universe.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        [OperationContract]
        Application CreateApplication(int product, int host, int universe, short status);

        /// <summary>
        /// Sets a configuration value.
        /// </summary>
        /// <param name="applicationId">The application unique identifier.</param>
        /// <param name="keyId">The key unique identifier.</param>
        /// <param name="index">The index.</param>
        /// <param name="rawValue">The raw value.</param>
        /// <returns></returns>
        [OperationContract]
        int SetValue(int applicationId, int keyId, int index, string rawValue);

        /// <summary>
        /// Adds a configuration key.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="blitableType">Type of the blitable.</param>
        /// <param name="summary">The summary.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <param name="isCollection">if set to <c>true</c> [is collection].</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        [OperationContract]
        int AddKey(string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue);

        /// <summary>
        /// Updates a configuration key.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="blitableType">Type of the blitable.</param>
        /// <param name="summary">The summary.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <param name="isCollection">if set to <c>true</c> [is collection].</param>
        /// <param name="defaultValue">The default value.</param>
        [OperationContract]
        void UpdateKey(int id, string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue);

        /// <summary>
        /// Deletes a configuration value.
        /// </summary>
        /// <param name="applicationId">The application unique identifier.</param>
        /// <param name="keyId">The key unique identifier.</param>
        /// <param name="index">The index.</param>
        [OperationContract]
        void DeleteValue(int applicationId, int keyId, int index);

        /// <summary>
        /// Adds a product.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        [OperationContract]
        int AddProduct(string name, string displayName);

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        [OperationContract]
        void UpdateProduct(int id, string name, string displayName);

        /// <summary>
        /// Adds a universe.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        [OperationContract]
        int AddUniverse(string name, string displayName, short status);

        /// <summary>
        /// Updates a universe.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="status">The status.</param>
        [OperationContract]
        void UpdateUniverse(int id, string name, string displayName, short status);

        /// <summary>
        /// Changes the universe status.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="status">The status.</param>
        [OperationContract]
        void ChangeUniverseStatus(int id, short status);

        /// <summary>
        /// Adds a host.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        [OperationContract]
        int AddHost(string name);

        /// <summary>
        /// Updates a host.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="name">The name.</param>
        [OperationContract]
        void UpdateHost(int id, string name);

        /// <summary>
        /// Changes the application status.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="status">The status.</param>
        [OperationContract]
        void ChangeAppStatus(int id, short status);

        ////UniverseDomainName GetDomainNames(IList<int> universeIds);

        ////IList<AppConfigurationEntry> GetValue(IList<int> applicationIds, );

        IList<UniverseDomainName> GetUniversesDomainNames(int universeId);

        int AddUniverseDomainName(int universeId, string name, bool redirectToMain);

        void UpdateUniverseDomainName(int id, string name, bool redirectToMain);
    }
}
