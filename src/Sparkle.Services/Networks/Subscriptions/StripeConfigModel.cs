
namespace Sparkle.Services.Networks.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class StripeConfigModel
    {
        [DataMember(Name = "Src")]
        public string ScriptUrl { get; set; }

        [DataMember(Name = "Class")]
        public string CssClass { get; set; }

        [DataMember(Name = "PublicApiKey")]
        public string PublicKey { get; set; }

        [DataMember(Name = "SecretApiKey")]
        public string SecretKey { get; set; }
    }
}
