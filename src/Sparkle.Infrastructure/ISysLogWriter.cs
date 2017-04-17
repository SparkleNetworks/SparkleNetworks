
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using System;
    using System.Data;


    /// <summary>
    /// Data access facade to access the central logging store.
    /// </summary>
    public interface ISysLogRepository : IDisposable {
        /// <summary>
        /// Finds the application id.
        /// </summary>
        /// <param name="product">The product name.</param>
        /// <param name="host">The host name.</param>
        /// <param name="universe">The universe name.</param>
        /// <returns>The application ID</returns>
        /// <exception cref="UnknownApplicationException">if the specified application is not registered</exception>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        int FindApplicationId(string product, string host, string universe);

        /// <summary>
        /// Finds the application details with its id.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <returns>The application details</returns>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        Application FindApplication(string product, string host, string universe);

        /// <summary>
        /// Writes the specified log entry.
        /// </summary>
        /// <param name="logEntryType">Type of the log entry.</param>
        /// <param name="applicationId">The application id.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="path">The path.</param>
        /// <param name="remoteClient">The remote client.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="data">The data.</param>
        /// <exception cref="DataException">the datasource failed to fullfill the request</exception>
        void Write(short logEntryType, int applicationId, string applicationVersion, string path, string remoteClient, string identity, short errorLevel, string data);
    }
}
