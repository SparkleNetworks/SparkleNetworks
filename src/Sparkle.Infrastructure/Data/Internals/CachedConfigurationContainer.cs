
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
#if SSC
    using SparkleSystems.Configuration.Internals;
#else
    using Sparkle.Infrastructure.Contracts;
#endif
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    internal abstract class CachedConfigurationContainer
    {
        private bool isLoaded;
        private CachedConfigurationEntryCollection data;

        protected CachedConfigurationContainer()
        {
        }

        protected CachedConfigurationEntryCollection Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public void SetValue(string methodName, object[] parameters, object result)
        {
            this.LoadIfNotLoaded();

            CachedConfigurationEntry entry = this.FindEntry(methodName, parameters);
            if (entry != null)
            {
                entry.Result = result;
            }
            else
            {
                this.AddEntry(new CachedConfigurationEntry
                {
                    MethodName = methodName,
                    Parameters = parameters.Select(p => p.ToString()).ToArray(),
                    Result = result,
                    ResultType = result != null ? result.GetType().FullName : "null",
                });
            }
        }

        public bool TryGetValue(string methodName, object[] parameters, out object result)
        {
            this.LoadIfNotLoaded();

            CachedConfigurationEntry entry = this.FindEntry(methodName, parameters);
            if (entry != null)
            {
                result = entry.Result;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        protected abstract void Load();
        public abstract void Save();

        protected virtual void AddEntry(CachedConfigurationEntry entry)
        {
            if (this.Data == null)
                this.Data = new CachedConfigurationEntryCollection();

            if (this.Data.Entries == null)
                this.Data.Entries = new List<CachedConfigurationEntry>();

            this.Data.Entries.Add(entry);
        }

        protected CachedConfigurationEntry FindEntry(string methodName, object[] parameters)
        {
            if (this.Data == null)
                this.Data = new CachedConfigurationEntryCollection();

            if (this.Data.Entries == null)
                this.Data.Entries = new List<CachedConfigurationEntry>();

            return this.Data.Entries.SingleOrDefault(e => e.MethodName == methodName && e.Parameters.ArraySequenceEquals(parameters));
        }

        public CachedConfigurationContainer Clone()
        {
            var obj = new MemoryCachedConfigurationContainer
            {
                Data = this.data.Clone(),
            };
            return obj;
        }

        private void LoadIfNotLoaded()
        {
            if (!this.isLoaded)
            {
                this.isLoaded = true;
                this.Load();
            }
        }
    }

    internal class FileCachedConfigurationContainer : CachedConfigurationContainer
    {
        private FileStream file;
        private DataContractSerializer serializer;

        public FileCachedConfigurationContainer(FileStream file)
            : base()
        {
            this.file = file;
        }

        protected override void Load()
        {
            if (this.file.Length == 0)
                return;

            ////this.serializer = this.serializer ?? (this.serializer = new XmlSerializer(typeof(CachedConfigurationEntryCollection)));
            this.serializer = this.serializer ?? (this.serializer = GetSerializer());
            this.file.Seek(0L, SeekOrigin.Begin);
            this.Data = this.serializer.ReadObject(this.file) as CachedConfigurationEntryCollection;
        }

        public override void Save()
        {
            this.serializer = this.serializer ?? (this.serializer = GetSerializer());
            this.file.Seek(0L, SeekOrigin.Begin);
            this.serializer.WriteObject(this.file, this.Data);
        }

        private static DataContractSerializer GetSerializer()
        {
            var ser = new DataContractSerializer(typeof(CachedConfigurationEntryCollection));
            return ser;
        }
    }

    internal class MemoryCachedConfigurationContainer : CachedConfigurationContainer
    {
        public MemoryCachedConfigurationContainer()
            : base()
        {
        }

        protected override void Load()
        {
        }

        public override void Save()
        {
        }
    }

    [KnownType(typeof(AppConfigurationEntry)), KnownType(typeof(Application))]
    [KnownType(typeof(AppConfigurationEntry[])), KnownType(typeof(Application[]))]
    [KnownType(typeof(Dictionary<string, AppConfigurationEntry>))]
    ////[KnownType(typeof(IList<AppConfigurationEntry>)), KnownType(typeof(IList<Application>))]
    [DataContract(Namespace = Names.ServiceContractNamespace)]
    public class CachedConfigurationEntry
    {
        [DataMember]
        public string MethodName { get; set; }

        [DataMember]
        public string[] Parameters { get; set; }

        [DataMember]
        public string ResultType { get; set; }

        [DataMember]
        public string ResultValue { get; set; }

        [DataMember]
        public object Result { get; set; }

        internal CachedConfigurationEntry Clone()
        {
            return new CachedConfigurationEntry
            {
                MethodName = this.MethodName,
                Parameters = this.Parameters.ToArray(),
                ResultType = this.ResultType,
                Result = this.Result,
                ResultValue = this.ResultValue,
            };
        }
    }

    [Serializable, KnownType(typeof(AppConfigurationEntry)), KnownType(typeof(Application))]
    [DataContract(Namespace = Names.ServiceContractNamespace)]
    public class CachedConfigurationEntryCollection
    {
        [DataMember]
        public List<CachedConfigurationEntry> Entries { get; set; }

        internal CachedConfigurationEntryCollection Clone()
        {
            return new CachedConfigurationEntryCollection
            {
                Entries = this.Entries.Select(e => e.Clone()).ToList(),
            };
        }
    }
}
