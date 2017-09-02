
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Low-level exceptions are wrapped into this one to provide a friendly error message on top of the error.
    /// </summary>
    [Serializable]
    public class SparkleServicesException : InvalidOperationException
    {
        public SparkleServicesException()
        {
        }

        // uncomment this overload if necessary
        ////public SparkleServicesException(string message)
        ////    : base(message)
        ////{
        ////}

        // uncomment this overload if necessary
        ////public SparkleServicesException(string message, Exception inner)
        ////    : base(message, inner)
        ////{
        ////}

        public SparkleServicesException(string message, string displayMessage, Exception inner)
            : base(message, inner)
        {
            this.DisplayMessage = displayMessage;
        }

        protected SparkleServicesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public string DisplayMessage { get; set; }
    }
}
