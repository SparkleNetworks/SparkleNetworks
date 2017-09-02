
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerStepThrough]
    public class ServiceBase : IServiceBase
    {
        private readonly IRepositoryFactory repositoryFactory;
        private readonly IServiceFactory serviceFactory;
        private bool isDisposed;

        private IList<string> optionsList;

        public ServiceBase(IRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }

        internal ServiceBase(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
        {
            if (repositoryFactory == null)
                throw new ArgumentNullException("repositoryFactory");
            if (serviceFactory == null)
                throw new ArgumentNullException("serviceFactory");

            this.repositoryFactory = repositoryFactory;
            this.serviceFactory = serviceFactory;
        }

        public IList<string> OptionsList
        {
            get { return this.optionsList ?? (this.optionsList = new List<string>()); }
            set { this.optionsList = value; }
        }

        public ServiceIdentity Identity
        {
            get { return this.serviceFactory.HostingEnvironment.Identity; }
            set { this.serviceFactory.HostingEnvironment.Identity = value; }
        }

        protected IRepositoryFactory Repo
        {
            get { return this.repositoryFactory; }
        }

        protected IServiceFactory Services
        {
            get { return this.serviceFactory; }
        }

        protected AclEnforcer AclEnforcer { get { return new AclEnforcer(this.Identity); } }

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
                    if (this.serviceFactory != null)
                    {
                        this.serviceFactory.Dispose();
                    }

                    if (this.repositoryFactory != null)
                    {
                        this.repositoryFactory.Dispose();
                    }
                }

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Throws an exception if the entity belongs to another network to protect data integrity.
        /// </summary>
        /// <param name="networkEntity">The network entity.</param>
        /// <exception cref="System.InvalidOperationException">Entity must specify the current NetworkId</exception>
        protected void VerifyNetwork(INetworkEntity networkEntity)
        {
            if (this.Services.NetworkId != networkEntity.NetworkId || networkEntity.NetworkId == 0)
            {
                throw new InvalidOperationException("Entity of type " + networkEntity.GetType().Name + " must specify the NetworkId (expected:" + this.Services.NetworkId + ", actual: "+networkEntity.NetworkId+")");
            }
        }

        /// <summary>
        /// Sets the network ID within the specified entity.
        /// Throws an exception if the entity belongs to another network to protect data integrity.
        /// </summary>
        /// <param name="networkEntity"></param>
        protected void SetNetwork(INetworkEntity networkEntity)
        {
            if (this.Services.NetworkId != networkEntity.NetworkId && networkEntity.NetworkId != 0)
            {
                throw new InvalidOperationException("Entity of type " + networkEntity.GetType().Name + " has an invalid NetworkId (expected:" + this.Services.NetworkId + ", actual: " + networkEntity.NetworkId + ")");
            }

            if (0 == networkEntity.NetworkId)
                networkEntity.NetworkId = this.Services.NetworkId;
        }

        /// <summary>
        /// Throws an exception if the entity belongs to another network to protect data integrity.
        /// </summary>
        /// <param name="networkEntity">The network entity.</param>
        /// <exception cref="System.InvalidOperationException">Entity must specify the current NetworkId</exception>
        protected void VerifyNetwork(ICommonNetworkEntity networkEntity)
        {
            if (networkEntity.NetworkId == null) return;

            if (this.Services.NetworkId != networkEntity.NetworkId || networkEntity.NetworkId == 0)
            {
                throw new InvalidOperationException("Entity of type " + networkEntity.GetType().Name + " must specify the NetworkId (expected:" + this.Services.NetworkId + "/null, actual: " + networkEntity.NetworkId + ")");
            }
        }

        /// <summary>
        /// Logs the domain result.
        /// </summary>
        /// <param name="path">The recommended format is: `&lt;Class name&gt;.&lt;Method name&gt;`</param>
        protected T LogResult<T>(T result, string path)
            where T : IBaseResult
        {
            return this.LogResultImpl<T>(EventLogEntryType.Information, result, path, null);
        }

        /// <summary>
        /// Logs the domain result with a message.
        /// </summary>
        /// <param name="path">The recommended format is: `&lt;Class name&gt;.&lt;Method name&gt;`</param>
        protected T LogResult<T>(T result, string path, string message)
            where T : IBaseResult
        {
            return this.LogResultImpl<T>(EventLogEntryType.Information, result, path, message);
        }

        /// <summary>
        /// Logs the domain result as an error.
        /// </summary>
        /// <param name="path">The recommended format is: `&lt;Class name&gt;.&lt;Method name&gt;`</param>
        protected T LogErrorResult<T>(T result, string path)
            where T : IBaseResult
        {
            return this.LogResultImpl<T>(EventLogEntryType.Error, result, path, null);
        }

        /// <summary>
        /// Logs the domain result as an error with a message.
        /// </summary>
        /// <param name="path">The recommended format is: `&lt;Class name&gt;.&lt;Method name&gt;`</param>
        protected T LogErrorResult<T>(T result, string path, string message)
            where T : IBaseResult
        {
            return this.LogResultImpl<T>(EventLogEntryType.Error, result, path, message);
        }

        private T LogResultImpl<T>(EventLogEntryType type, T result, string path, string message)
            where T : IBaseResult
        {
            if (result == null)
                throw new ArgumentNullException("result");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("The value cannot be empty", "path");

            if (result.Succeed)
            {
                if (type == EventLogEntryType.Error)
                    this.Services.Logger.Error(path, ErrorLevel.Success, message ?? "Domain method result: success.");
                else
                    this.Services.Logger.Info(path, ErrorLevel.Success, message ?? "Domain method result: success.");
            }
            else
            {
                var errors = result.Errors
                    .Select<IResultError, string>(e => e.Code + " " + e.Detail)
                    .ToArray();
                if (type == EventLogEntryType.Error)
                    this.Services.Logger.Error(path, ErrorLevel.Business, message ?? "Domain method result: failed. " + string.Join("; ", errors));
                else
                    this.Services.Logger.Info(path, ErrorLevel.Business, message ?? "Domain method result: failed. " + string.Join("; ", errors));
            }

            return result;
        }
    }
}
