
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using Sparkle.Infrastructure.Data;

    public class SparkleNetworksApplicationConfiguration
    {
        private readonly Dictionary<string, AppConfigurationEntry> values;
        private SparkleNetworksApplicationConfigurationTree tree;

        private SparkleNetworksApplicationConfiguration()
        {
            this.values = new Dictionary<string, AppConfigurationEntry>();
        }

        public SparkleNetworksApplicationConfiguration(IEnumerable<AppConfigurationEntry> entries)
        {
            this.values = new Dictionary<string, AppConfigurationEntry>();
            foreach (var entry in entries)
            {
                this.values.Add(entry.Key, entry);
            }
        }
        
        public SparkleNetworksApplicationConfigurationTree Tree
        {
            get
            {
                return this.tree ?? (this.tree = new SparkleNetworksApplicationConfigurationTree(this.values));
            }
        }

        public static SparkleNetworksApplicationConfiguration FromWebConfiguration(string prefix)
        {
            var keys = SparkleNetworksApplicationConfigurationTree.ConfigKeys;
            var types = SparkleNetworksApplicationConfigurationTree.ConfigKeyTypes;
            var defaultValues = SparkleNetworksApplicationConfigurationTree.ConfigKeyValues;
            var entries = new List<AppConfigurationEntry>(keys.Length);
            for (int i = 0; i < keys.Length; i++)
            {
                var key = prefix + keys[i];
                var value = ConfigurationManager.AppSettings[key];
                var entry = new AppConfigurationEntry
                {
                    BlittableType = types[i],
                    Key = keys[i],
                    RawValue = value,
                    DefaultRawValue = defaultValues[i],
                };
                entries.Add(entry);
            }

            return new SparkleNetworksApplicationConfiguration(entries);
        }

        public Infrastructure.AppConfiguration AppConfiguration { get; set; }
    }
}
