
namespace Sparkle.ApiClients.Common
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class TagCategoryModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public bool CanUsersCreate { get; set; }

        [DataMember]
        public int? NetworkId { get; set; }

        [DataMember]
        public bool ApplyToUsers { get; set; }

        [DataMember]
        public bool ApplyToCompanies { get; set; }

        [DataMember]
        public bool ApplyToGroups { get; set; }

        [DataMember]
        public bool ApplyToTimelineItems { get; set; }

        [DataMember]
        public RulesModel RulesModel { get; set; }
    }
}
