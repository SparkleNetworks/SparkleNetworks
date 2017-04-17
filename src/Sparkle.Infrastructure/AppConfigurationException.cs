
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Occurs when an error occurs when loading an application's configuration.
    /// </summary>
    [Serializable]
    public class AppConfigurationException : Exception
    {
        /// <summary>
        /// Occurs when an error occurs when loading an application's configuration.
        /// </summary>
        public AppConfigurationException()
        {
        }

        /// <summary>
        /// Occurs when an error occurs when loading an application's configuration.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AppConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Occurs when an error occurs when loading an application's configuration.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public AppConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Occurs when an error occurs when loading an application's configuration.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="product">The product.</param>
        /// <param name="universe">The universe.</param>
        /// <param name="host">The host.</param>
        public AppConfigurationException(string message, string product, string universe, string host)
            : base(message + string.Format(CultureInfo.InvariantCulture, "product:{0}, host:{1}, universe:{2}", product ?? "NULL", host ?? "NULL", universe ?? "NULL"))
        {
        }

        /// <summary>
        /// Occurs when an error occurs when loading an application's configuration.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="product">The product.</param>
        /// <param name="universe">The universe.</param>
        /// <param name="host">The host.</param>
        /// <param name="inner">The inner exception.</param>
        public AppConfigurationException(string message, string product, string universe, string host, Exception inner)
            : base(message + string.Format(CultureInfo.InvariantCulture, "product:{0}, host:{1}, universe:{2}", product ?? "NULL", host ?? "NULL", universe ?? "NULL"), inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected AppConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
