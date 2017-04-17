
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
#if SSC
#else
    using Sparkle.Infrastructure.Contracts;
#endif
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Represents an application.
    /// </summary>
    [DataContract(Namespace = Names.ServiceContractNamespace)]
    public class Application
    {
        private int productId;
        private int id;
        private int hostId;
        private int universeId;
        private string hostName;
        private string universeName;
        private string productName;
        private short status;

        public Application()
        {
        }

        public Application(int applicationId, int productId, int universeId, int hostId)
        {
            this.id = applicationId;
            this.productId = productId;
            this.universeId = universeId;
            this.hostId = hostId;
        }

        public Application(Application application)
        {
            this.id = application.Id;
            this.productId = application.productId;
            this.productName = application.productName;
            this.universeId = application.universeId;
            this.universeName = application.universeName;
            this.hostId = application.hostId;
            this.hostName = application.hostName;
            this.status = application.status;
            this.UniverseStatus = application.UniverseStatus;
        }

        /// <summary>
        /// Gets or sets the universe id.
        /// </summary>
        /// <value>
        /// The universe id.
        /// </value>
        [DataMember]
        public int UniverseId
        {
            get { return this.universeId; }
            set { this.universeId = value; }
        }
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        /// <value>
        /// The application id.
        /// </value>
        [DataMember]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets the host id.
        /// </summary>
        /// <value>
        /// The host id.
        /// </value>
        [DataMember]
        public int HostId
        {
            get { return this.hostId; }
            set { this.hostId = value; }
        }

        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        [DataMember]
        public int ProductId
        {
            get { return this.productId; }
            set { this.productId = value; }
        }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        [DataMember]
        public string ProductName
        {
            get { return this.productName; }
            set { this.productName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the universe.
        /// </summary>
        /// <value>
        /// The name of the universe.
        /// </value>
        [DataMember]
        public string UniverseName
        {
            get { return this.universeName; }
            set { this.universeName = value; }
        }
        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        [DataMember]
        public string HostName
        {
            get { return this.hostName; }
            set { this.hostName = value; }
        }

        [DataMember]
        public short Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        [DataMember]
        public short UniverseStatus { get; set; }

        /// <summary>
        /// Get the combination of both the application status and the universe status.
        /// </summary>
        public short MainStatusValue
        {
            get { return Math.Min(this.Status, this.UniverseStatus); }
        }

        public ApplicationStatus MainStatus
        {
            get { return (ApplicationStatus)this.MainStatusValue; }
        }
    }
}
