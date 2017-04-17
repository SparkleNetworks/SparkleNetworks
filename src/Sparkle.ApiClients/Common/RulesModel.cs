
namespace Sparkle.ApiClients.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class RulesModel
    {
        [DataMember]
        public IDictionary<string, TagCategoryRuleModel> Rules { get; set; }
    }
}
