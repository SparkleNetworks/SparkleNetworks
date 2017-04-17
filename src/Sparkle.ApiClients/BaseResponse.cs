
namespace Sparkle.ApiClients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public class BaseResponse<T>
    {
        #region Sparkle custom response

        /// <summary>
        /// The default place to look for the expected data.
        /// </summary>
        [DataMember]
        public T Data { get; set; }

        /// <summary>
        /// The default place to look when an error occurs.
        /// </summary>
        [DataMember]
        public string ErrorCode { get; set; }

        /// <summary>
        /// This may contain the exception message.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// This may contain a help message in case of error.
        /// </summary>
        [DataMember]
        public string ErrorHelp { get; set; }

        /// <summary>
        /// This may contain the exception stack trace.
        /// </summary>
        [DataMember]
        public string Exception { get; set; }

        /// <summary>
        /// This may contain request validation errors.
        /// </summary>
        [DataMember]
        public Dictionary<string, string[]> ModelState { get; set; }

        #endregion

        #region Default ASP Web API items

        /// <summary>
        /// The default place to look when an error occurs.
        /// </summary>
        [Obsolete]
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// This may contain the exception message.
        /// </summary>
        [Obsolete]
        [DataMember]
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// This may contain the exception type.
        /// </summary>
        [Obsolete]
        [DataMember]
        public string ExceptionType { get; set; }

        /// <summary>
        /// This may contain the exception stack trace.
        /// </summary>
        [Obsolete]
        [DataMember]
        public string StackTrace { get; set; }

        #endregion
    }
}
