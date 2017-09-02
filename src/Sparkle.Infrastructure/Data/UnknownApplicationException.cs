
#if SSC
namespace SparkleSystems.Configuration.Data
#else
namespace Sparkle.Infrastructure.Data
#endif
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Used when the specified application cannot be found.
    /// </summary>
    [Serializable]
    public class UnknownApplicationException : Exception
    {
        private readonly string product;
        private readonly string host;
        private readonly string universe;

        /// <summary>
        /// Used when the specified application cannot be found.
        /// </summary>
        public UnknownApplicationException()
            : base("Application is not defined")
        {
        }

        /// <summary>
        /// Used when the specified application cannot be found.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="host">The host.</param>
        /// <param name="universe">The universe.</param>
        public UnknownApplicationException(string product, string host, string universe)
            : base(string.Format(CultureInfo.InvariantCulture, "Application {0}/{1}/{2} is not defined", product, host, universe))
        {
            this.product = product;
            this.host = host;
            this.universe = universe;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownApplicationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected UnknownApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Gets the product.
        /// </summary>
        public string Product
        {
            get { return this.product; }
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        public string Host
        {
            get { return this.host; }
        }

        /// <summary>
        /// Gets the universe.
        /// </summary>
        public string Universe
        {
            get { return this.universe; }
        }
    }
}
