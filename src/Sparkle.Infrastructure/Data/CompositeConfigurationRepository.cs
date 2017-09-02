
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
#if !SSC
    using Sparkle.Infrastructure.Data.Objects;
#endif

    /// <summary>
    /// Composite implementation of <see cref="IConfigurationRepository"/>.
    /// </summary>
    public class CompositeConfigurationRepository : IConfigurationRepository
    {
        /// <summary>
        /// Indicates the object was disposed.
        /// </summary>
        private bool disposed;

        private List<Tuple<Func<IConfigurationRepository>, IConfigurationRepository>> sources;

        public CompositeConfigurationRepository(IList<Func<IConfigurationRepository>> sources)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");
            if (sources.Count == 0)
                throw new ArgumentException("No sources passed", "sources");

            this.sources = sources.Select(s => new Tuple<Func<IConfigurationRepository>, IConfigurationRepository>(s, null)).ToList();
        }

        private IEnumerable<IConfigurationRepository> SourceIterator
        {
            get
            {
                this.Check();

                if (this.sources == null)
                    yield break;

                for (int i = 0; i < this.sources.Count; i++)
                {
                    var item = this.sources[i];
                    var obj = this.GetSourceInstance(ref item);
                    yield return obj;
                }
            }
        }

        public int FindApplicationId(string product, string host, string universe)
        {
            return this.IterateSources(s => s.FindApplicationId(product, host, universe));
        }

        public int FindApplicationIdByDomainName(string product, string host, string domainName)
        {
            return this.IterateSources(s => s.FindApplicationIdByDomainName(product, host, domainName));
        }

        public Application FindApplicationById(int applicationId)
        {
            return this.IterateSources(s => s.FindApplicationById(applicationId));
        }

        public IDictionary<string, AppConfigurationEntry> FetchValues(int applicationId)
        {
            return this.IterateSources(s => s.FetchValues(applicationId));
        }

        public IList<AppConfigurationEntry> FetchKeys()
        {
            return this.IterateSources(s => s.FetchKeys());
        }

        public IList<Application> FindApplications(string product, string host)
        {
            return this.IterateSources(s => s.FindApplications(product, host));
        }

        public Application FindApplication(string product, string host, string universe)
        {
            return this.IterateSources(s => s.FindApplication(product, host, universe));
        }

        public IList<Application> GetAllApplications()
        {
            return this.IterateSources(s => s.GetAllApplications());
        }

        public IList<Host> GetAllHosts()
        {
            return this.IterateSources(s => s.GetAllHosts());
        }

        public IList<Universe> GetAllUniverses()
        {
            return this.IterateSources(s => s.GetAllUniverses());
        }

        public IList<Product> GetAllProducts()
        {
            return this.IterateSources(s => s.GetAllProducts());
        }

        public Application CreateApplication(int product, int host, int universe, short status)
        {
            return this.IterateSources(s => s.CreateApplication(product, host, universe, status));
        }

        public int SetValue(int applicationId, int keyId, int index, string rawValue)
        {
            return this.IterateSources(s => s.SetValue(applicationId, keyId, index, rawValue));
        }

        public int AddKey(string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            return this.IterateSources(s => s.AddKey(name, blitableType, summary, isRequired, isCollection, defaultValue));
        }

        public void UpdateKey(int id, string name, string blitableType, string summary, bool isRequired, bool isCollection, string defaultValue)
        {
            this.IterateSources(s => s.UpdateKey(id, name, blitableType, summary, isRequired, isCollection, defaultValue));
        }

        public void DeleteValue(int applicationId, int keyId, int index)
        {
            this.IterateSources(s => s.DeleteValue(applicationId, keyId, index));
        }

        public int AddProduct(string name, string displayName)
        {
            return this.IterateSources(s => s.AddProduct(name, displayName));
        }

        public void UpdateProduct(int id, string name, string displayName)
        {
            this.IterateSources(s => s.UpdateProduct(id, name, displayName));
        }

        public int AddUniverse(string name, string displayName, short status)
        {
            return this.IterateSources(s => s.AddUniverse(name, displayName, status));
        }

        public void UpdateUniverse(int id, string name, string displayName, short status)
        {
            this.IterateSources(s => s.UpdateUniverse(id, name, displayName, status));
        }

        public void ChangeUniverseStatus(int id, short status)
        {
            this.IterateSources(s => s.ChangeUniverseStatus(id, status));
        }

        public int AddHost(string name)
        {
            return this.IterateSources(s => s.AddHost(name));
        }

        public void UpdateHost(int id, string name)
        {
            this.IterateSources(s => s.UpdateHost(id, name));
        }

        public void ChangeAppStatus(int id, short status)
        {
            this.IterateSources(s => s.ChangeAppStatus(id, status));
        }

        public IList<UniverseDomainName> GetUniversesDomainNames(int universeId)
        {
            return this.IterateSources(s => s.GetUniversesDomainNames(universeId));
        }

        public int AddUniverseDomainName(int universeId, string name, bool redirectToMain)
        {
            return this.IterateSources(s => s.AddUniverseDomainName(universeId, name, redirectToMain));
        }

        public void UpdateUniverseDomainName(int id, string name, bool redirectToMain)
        {
            this.IterateSources(s => s.UpdateUniverseDomainName(id, name, redirectToMain));
        }

        #region Internals

        private TResult IterateSources<TResult>(Func<IConfigurationRepository, TResult> action)
        {
            this.Check();

            foreach (var source in this.SourceIterator)
            {
                if (source != null)
                {
                    try
                    {
                        var result = action(source);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsFatal())
                            throw;
                    }
                }
            }

            throw new AppConfigurationException("No source in composite source could fulfill the request");
        }

        private void IterateSources(Action<IConfigurationRepository> action)
        {
            this.Check();

            foreach (var source in this.SourceIterator)
            {
                if (source != null)
                {
                    try
                    {
                        action(source);
                        return;
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsFatal())
                            throw;
                    }
                }
            }

            throw new AppConfigurationException("No source in composite source could fulfill the request");
        }

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
                    if (this.sources != null)
                    {
                        for (int i = 0; i < this.sources.Count; i++)
                        {
                            var facto = this.sources[i].Item1;
                            var obj = this.sources[i].Item2;
                            if (obj != null)
                            {
                                try
                                {
                                    obj.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    if (ex.IsFatal())
                                        throw;
                                }

                                this.sources[i] = new Tuple<Func<IConfigurationRepository>, IConfigurationRepository>(facto, null);
                            }
                        }
                    }
                }

                this.disposed = true;
            }
        }

        private IConfigurationRepository GetSourceInstance(ref Tuple<Func<IConfigurationRepository>, IConfigurationRepository> item)
        {
            this.Check();

            var facto = item.Item1;
            var obj = item.Item2;

            if (obj != null)
            {
                return obj;
            }

            obj = facto();

            item = new Tuple<Func<IConfigurationRepository>, IConfigurationRepository>(facto, obj);

            return obj;
        }

        private void Check()
        {
            if (this.disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        #endregion
    }
}
