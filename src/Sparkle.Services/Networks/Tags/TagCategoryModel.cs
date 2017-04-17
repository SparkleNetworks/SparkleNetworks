
namespace Sparkle.Services.Networks.Tags
{
    using Newtonsoft.Json;
    using Sparkle.Services.Networks.Lang;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class TagCategoryModel
    {
        private RulesModel rulesModel;

        public TagCategoryModel()
        {
        }

        public TagCategoryModel(int id, string name, string alias)
        {
            this.Id = id;
            this.Name = name;
            this.Alias = alias;
        }

        public TagCategoryModel(Sparkle.Entities.Networks.TagCategory item)
        {
            this.Set(item);
        }

        [DataMember]
        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(NetworksLabels))]
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [Display(Name = "CanUsersCreate", ResourceType = typeof(NetworksLabels))]
        [DataMember]
        public bool CanUsersCreate { get; set; }

        [DataMember]
        public int? NetworkId { get; set; }

        [Display(Name = "ApplyToUsers", ResourceType = typeof(NetworksLabels))]
        [DataMember]
        public bool ApplyToUsers
        {
            get { return this.IsAppliedTo(RuleType.User); }
        }

        [Display(Name = "ApplyToCompanies", ResourceType = typeof(NetworksLabels))]
        [DataMember]
        public bool ApplyToCompanies
        {
            get { return this.IsAppliedTo(RuleType.Company); }
        }

        [Display(Name = "ApplyToGroups", ResourceType = typeof(NetworksLabels))]
        [DataMember]
        public bool ApplyToGroups
        {
            get { return this.IsAppliedTo(RuleType.Group); }
        }

        [Display(Name = "ApplyToTimelineItems", ResourceType = typeof(NetworksLabels))]
        [DataMember]
        public bool ApplyToTimelineItems
        {
            get { return this.IsAppliedTo(RuleType.TimelineItem); }
        }

        [IgnoreDataMember]
        public bool HasRules
        {
            get { return this.RulesModel != null && this.RulesModel.Rules != null; }
        }

        [IgnoreDataMember]
        public string Rules { get; set; }

        [Display(Name = "Rules", ResourceType = typeof(NetworksLabels))]
        [DataMember]
        public RulesModel RulesModel
        {
            get
            {
                if (this.rulesModel == null && this.Rules != null)
                    this.rulesModel = JsonConvert.DeserializeObject<RulesModel>(this.Rules);

                return this.rulesModel;
            }
            set
            {
                this.Rules = JsonConvert.SerializeObject(value, Formatting.None);
                this.rulesModel = null;
            }
        }

        public bool IsAppliedTo(string entityName)
        {
            RuleType type;
            if (Enum.TryParse<RuleType>(entityName, out type))
                return this.IsAppliedTo(type);

            return false;
        }

        public RuleModel GetRule(string entityName)
        {
            RuleType type;
            if (Enum.TryParse<RuleType>(entityName, out type) && this.IsAppliedTo(type))
                return this.RulesModel.Rules[type];

            return null;
        }

        private void Set(Entities.Networks.TagCategory item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.CanUsersCreate = item.CanUsersCreate;
            this.NetworkId = item.NetworkId;
            this.Rules = item.Rules;
        }

        private bool IsAppliedTo(RuleType type)
        {
            return this.HasRules && this.RulesModel.Rules.ContainsKey(type) && this.RulesModel.Rules[type].IsEnabled;
        }

        public override string ToString()
        {
            return "TagCategoryModel " + this.Id + " " + this.Name + " " + this.Alias + " " + this.NetworkId;
        }
    }

    [DataContract]
    public class RulesModel
    {
        [DataMember]
        public IDictionary<RuleType, RuleModel> Rules { get; set; }

        [DataMember]
        public GlobalRuleModel GlobalRules { get; set; }
    }

    [DataContract]
    public class RuleModel
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

    [DataContract]
    public class GlobalRuleModel
    {
        private bool displayInDirectory = true;

        [DataMember]
        public bool DisplayInDirectory
        {
            get { return this.displayInDirectory; }
            set { this.displayInDirectory = value; }
        }
    }

    public enum RuleType
    {
        None = 0,
        User = 1,
        Company = 2,
        Group = 3,
        TimelineItem = 4,
        Ad = 5,
    }
}
