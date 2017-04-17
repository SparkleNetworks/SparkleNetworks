
namespace Sparkle.Services.Networks.Models
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Extra properties for the <see cref="BaseRequest"/>.
    /// </summary>
    [DataContract(Namespace = Names.PublicNamespace)]
    public class LocalBaseRequest : BaseRequest
    {
        [DataMember]
        public int? ActingUserId { get; set; }
    }
}
