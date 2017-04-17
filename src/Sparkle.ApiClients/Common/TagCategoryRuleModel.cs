
namespace Sparkle.ApiClients.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class TagCategoryRuleModel
    {
        [DataMember]
        public bool DisplayInApplyProcess { get; set; }

        [DataMember]
        public short Minimum { get; set; }

        [DataMember]
        public short Maximum { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }
    }
}
