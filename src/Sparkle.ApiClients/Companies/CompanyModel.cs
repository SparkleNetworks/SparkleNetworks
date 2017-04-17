
namespace Sparkle.ApiClients.Companies
{
    using Sparkle.ApiClients.Common;
    using Sparkle.ApiClients.Places;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class CompanyModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int NetworkId { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsApproved { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        public List<TagModel> Skills { get; set; }

        [DataMember]
        public short CategoryId { get; set; }

        [DataMember]
        public string PictureUrl { get; set; }

        [DataMember]
        public string SmallProfilePictureUrl { get; set; }

        [DataMember]
        public string MediumProfilePictureUrl { get; set; }

        [DataMember]
        public string LargeProfilePictureUrl { get; set; }

        [DataMember]
        public string ProfileUrl { get; set; }

        [DataMember]
        public string Baseline { get; set; }

        [DataMember]
        public string About { get; set; }

        [DataMember]
        public IList<ProfileFieldModel> Fields { get; set; }

        [DataMember]
        public IDictionary<string, object> Data { get; set; }

        [DataMember]
        public IList<PlaceModel> Places { get; set; }

        [DataMember]
        public IList<Tag2Model> Tags { get; set; }
    }
}
