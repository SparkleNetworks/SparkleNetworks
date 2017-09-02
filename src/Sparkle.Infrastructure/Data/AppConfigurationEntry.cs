
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure.Data
#endif
{
#if SSC
#else
    using Sparkle.Infrastructure.Contracts;
    using Sparkle.Infrastructure.Data;
#endif
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A configuration entry key with value(s).
    /// </summary>
    [DataContract(Namespace = Names.ServiceContractNamespace)]
    public class AppConfigurationEntry
    {
        public AppConfigurationEntry()
        {
        }

        internal AppConfigurationEntry(AppConfigurationEntry item)
        {
            this.BlittableType = item.BlittableType;
            this.DefaultRawValue = item.DefaultRawValue;
            this.Id = item.Id;
            this.IsCollection = item.IsCollection;
            this.IsRequired = item.IsRequired;
            this.Key = item.Key;
            this.KeyId = item.KeyId;
            this.RawValue = item.RawValue;
            if (item.RawValues != null)
            this.RawValues = new List<string>(item.RawValues);
            this.Summary = item.Summary;
        }

        /// <summary>
        /// Gets or sets the value id (app+key+index).
        /// </summary>
        /// <value>
        /// The id (app+key+index).
        /// </value>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the key (Site.Name, ConnectionStrings.Logging).
        /// </summary>
        /// <value>
        /// The key (Site.Name, ConnectionStrings.Logging.
        /// </value>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the raw value.
        /// </summary>
        /// <value>
        /// The raw value.
        /// </value>
        [DataMember]
        public string RawValue { get; set; }

        /// <summary>
        /// Gets or sets the raw values.
        /// </summary>
        /// <value>
        /// The raw values.
        /// </value>
        [DataMember]
        public IList<string> RawValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this entry is required; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the type of the blittable.
        /// </summary>
        /// <value>
        /// The type of the blittable.
        /// </value>
        [DataMember]
        public string BlittableType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is collection.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this entry is collection; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsCollection { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        [DataMember]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the default raw value.
        /// </summary>
        /// <value>
        /// The default raw value.
        /// </value>
        [DataMember]
        public string DefaultRawValue { get; set; }

        /// <summary>
        /// Gets or sets the key id.
        /// </summary>
        /// <value>
        /// The key id.
        /// </value>
        [DataMember]
        public int KeyId { get; set; }
    }
}
