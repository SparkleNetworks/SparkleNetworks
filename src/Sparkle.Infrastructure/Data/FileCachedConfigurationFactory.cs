
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
    /// File-cached implementation of <see cref="IConfigurationRepository"/> using another repository to get fresh data.
    /// </summary>
    public class FileCachedConfigurationFactory : IConfigurationRepository
    {
        /// <summary>
        /// Indicates the object was disposed.
        /// </summary>
        private bool disposed;

        private IConfigurationRepository source;
        private Func<IConfigurationRepository> sourceFactory;

        private CachedConfigurationContainer memory;
        private bool isInitialized;
        private string dataDirectory;
        private string dataFile;
        private DateTime? lastFileModified;

        public FileCachedConfigurationFactory(IConfigurationRepository source)
        {
            this.source = source;
        }

        private IConfigurationRepository Source
        {
            get
            {
                if (this.source == null)
                {
                    this.source = this.CreateSource();
                }

                return this.source;
            }
        }

        public int FindApplicationId(string product, string host, string universe)
        {
            return this.GetValue(
                "FindApplicationId",
                new object[] { product, host, universe, },
                s => s.FindApplicationId(product, host, universe));
        }

        public int FindApplicationIdByDomainName(string product, string host, string domainName)
        {
            return this.GetValue(
                "FindApplicationIdByDomainName",
                new object[] { product, host, domainName, },
                s => s.FindApplicationIdByDomainName(product, host, domainName));
        }

        public Application FindApplicationById(int applicationId)
        {
            return this.GetValue(
                "FindApplicationById",
                new object[] { applicationId, },
                s => s.FindApplicationById(applicationId));
        }

        public IDictionary<string, AppConfigurationEntry> FetchValues(int applicationId)
        {
            return this.GetValue(
                "FetchValues",
                new object[] { applicationId, },
                s => s.FetchValues(applicationId));
        }

        public IList<AppConfigurationEntry> FetchKeys()
        {
            return this.GetValue(
                "FetchKeys",
                new object[] { },
                s => s.FetchKeys());
        }

        public IList<Application> FindApplications(string product, string host)
        {
            return this.GetValue(
                "FindApplications",
                new object[] { product, host, },
                s => s.FindApplications(product, host));
        }

        public Application FindApplication(string product, string host, string universe)
        {
            return this.GetValue(
                "FindApplications",
                new object[] { product, host, universe, },
                s => s.FindApplication(product, host, universe));
        }

        public IList<Application> GetAllApplications()
        {
            return this.GetValue(
                "GetAllApplications",
                new object[0],
                s => s.GetAllApplications());
        }

        public IList<Host> GetAllHosts()
        {
            return this.GetValue(
                "GetAllHosts",
                new object[0],
                s => s.GetAllHosts());
        }

        public IList<Universe> GetAllUniverses()
        {
            return this.GetValue(
                "GetAllUniverses",
                new object[0],
                s => s.GetAllUniverses());
        }

        public IList<Product> GetAllProducts()
        {
            return this.GetValue(
                "GetAllProducts",
                new object[0],
                s => s.GetAllProducts());
        }

        public IList<UniverseDomainName> GetUniversesDomainNames(int universeId)
        {
            return this.GetValue(
                "GetUniversesDomainNames",
                new object[] { universeId, },
                s => s.GetUniversesDomainNames(universeId));
        }

        public Application CreateApplication(int product, int host, int universe, short status)
        {
            throw new NotSupportedException();
        }

        public int SetValue(int applicationId, int keyId, int index, string rawValue)
        {
            throw new NotSupportedException();
        }

        public int AddKey(string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            throw new NotSupportedException();
        }

        public void UpdateKey(int id, string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            throw new NotSupportedException();
        }

        public void DeleteValue(int applicationId, int keyId, int index)
        {
            throw new NotSupportedException();
        }

        public int AddProduct(string name, string displayName)
        {
            throw new NotSupportedException();
        }

        public void UpdateProduct(int id, string name, string displayName)
        {
            throw new NotSupportedException();
        }

        public int AddUniverse(string name, string displayName, short status)
        {
            throw new NotSupportedException();
        }

        public void UpdateUniverse(int id, string name, string displayName, short status)
        {
            throw new NotSupportedException();
        }

        public void ChangeUniverseStatus(int id, short status)
        {
            throw new NotSupportedException();
        }

        public int AddHost(string name)
        {
            throw new NotSupportedException();
        }

        public void UpdateHost(int id, string name)
        {
            throw new NotSupportedException();
        }

        public void ChangeAppStatus(int id, short status)
        {
            throw new NotSupportedException();
        }

        public int AddUniverseDomainName(int universeId, string name, bool redirectToMain)
        {
            throw new NotSupportedException();
        }

        public void UpdateUniverseDomainName(int id, string name, bool redirectToMain)
        {
            throw new NotSupportedException();
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
                    // TODO: dispose this object
                }

                this.disposed = true;
            }
        }

        #endregion

        #region Internals

        private IConfigurationRepository CreateSource()
        {
            return this.source;
        }

        private void Check()
        {
            if (this.disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            this.Initialize();
        }

        private void Initialize()
        {
            if (!this.isInitialized)
            {
                this.isInitialized = true;

                var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var sparkleDirectory = Path.Combine(appDataDir, "SparkleSystems");
                if (!Directory.Exists(sparkleDirectory))
                {
                    var acls = Directory.GetAccessControl(appDataDir);
                    acls.SetOwner(WindowsIdentity.GetCurrent().User);
                    SecurityIdentifier domain = WindowsIdentity.GetCurrent().User.AccountDomainSid;
                    ////SecurityIdentifier domain = null;
                    var admin = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, domain);
                    ////var iis = new WindowsIdentity("IIS_IUSRS");

                    acls.AddAccessRule(new FileSystemAccessRule(
                        admin,
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow));
                    acls.AddAccessRule(new FileSystemAccessRule(
                        "IIS_IUSRS",
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow));
                    acls.AddAccessRule(new FileSystemAccessRule(
                        WindowsIdentity.GetCurrent().User,
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow));
                    Directory.CreateDirectory(sparkleDirectory, acls);
                }

                this.dataDirectory = Path.Combine(sparkleDirectory, "FileCachedConfiguration");
                if (!Directory.Exists(this.dataDirectory))
                {
                    Directory.CreateDirectory(this.dataDirectory);
                }

                this.dataFile = Path.Combine(this.dataDirectory, "master.xml");
            }
        }

        private TResult GetValue<TResult>(string methodName, object[] parameters, Func<IConfigurationRepository, TResult> sourceAction)
        {
            this.Check();

            object result;

            // read from memory
            if (this.MayReadFromMemoryCache())
            {
                if (this.TryGetValue(this.memory, methodName, parameters, out result))
                    return (TResult)result;
            }
            
            // read from file
            var stream = File.Open(this.dataFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            var container = new FileCachedConfigurationContainer(stream);
            try
            {
                if (this.MayReadFromFileCache()
                 && this.TryGetValue(container, methodName, parameters, out result))
                {
                    // file hit
                }
                else
                {
                    // use source
                    result = sourceAction(this.Source);

                    // update file cache
                    this.SetValue(container, methodName, parameters, result);
                    container.Save();
                }

                // refresh memory from file
                this.CloneFileToMemory(container);

                return (TResult)result;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        private void CloneFileToMemory(CachedConfigurationContainer container)
        {
            this.memory = container.Clone();
            this.lastFileModified = File.GetLastWriteTimeUtc(this.dataFile);
        }

        private void SetValue(CachedConfigurationContainer container, string methodName, object[] parameters, object result)
        {
            container.SetValue(methodName, parameters, result);
        }

        private bool MayReadFromMemoryCache()
        {
            var fileModified = File.GetLastWriteTimeUtc(this.dataFile);
            if (this.lastFileModified != fileModified)
                return false;

            return true;
        }

        private bool MayReadFromFileCache()
        {
            if (this.dataFile == null)
                return false;

            if (!File.Exists(this.dataFile))
                return false;

            if (this.lastFileModified == null)
                this.lastFileModified = File.GetLastWriteTimeUtc(this.dataFile);

            return true;
        }

        private bool TryGetValue(CachedConfigurationContainer container, string methodName, object[] parameters, out object content)
        {
            object outResult;
            if (container.TryGetValue(methodName, parameters, out outResult))
            {
                content = outResult;
                return true;
            }
            else
            {
                content = null;
                return false;
            }
        }

        #endregion
    }
}
