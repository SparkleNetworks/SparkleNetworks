
namespace Sparkle.Services.Networks.Companies
{
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    public class CompanyRelationshipTypeModel
    {
        private void Set(CompanyRelationshipType item)
        {
            this.Id = item.Id;
            this.NetworkId = item.NetworkId;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.Verb = item.Verb;
            this.Rules = item.Rules;
            this.KnownType = item.KnownTypeValue;
        }

        public CompanyRelationshipTypeModel(CompanyRelationshipType item)
        {
            this.Set(item);
        }

        public CompanyRelationshipTypeModel(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }

        public int NetworkId { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public string Verb { get; set; }

        public string Rules { get; set; }

        public KnownCompanyRelationshipType KnownType { get; set; }

        public CompanyRelationshipTypeRulesModel RulesModel
        {
            get { return this.Rules != null ? JsonConvert.DeserializeObject<CompanyRelationshipTypeRulesModel>(this.Rules) : new CompanyRelationshipTypeRulesModel(); }
            set { this.Rules = JsonConvert.SerializeObject(value); }
        }
    }

    [DataContract]
    public class CompanyRelationshipTypeRulesModel
    {
        [DataMember]
        public bool IsReadOnly { get; set; }

        [DataMember]
        public int[] AllowedMasterCompanyCategoryIds { get; set; }

        [DataMember]
        public int[] AllowedSlaveCompanyCategoryIds { get; set; }
    }
}
