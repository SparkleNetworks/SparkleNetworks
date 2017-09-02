
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ProfileFieldModel
    {
        private string rulesValue;
        private Type valueType;
        private JObject rules;

        public ProfileFieldModel()
        {
        }

        public ProfileFieldModel(ProfileField item)
        {
            this.Set(item);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool ApplyToUsers { get; set; }

        public bool ApplyToCompanies { get; set; }

        public int? AvailableValuesCount { get; set; }

        public IList<ProfileFieldAvailableValueModel> AvailableValues { get; set; }

        public ProfileFieldType KnownFieldType
        {
            get
            {
                ProfileFieldType value;
                if (!Enum.TryParse<ProfileFieldType>(this.Id.ToString(), out value))
                    value = ProfileFieldType.Unknown;
                return value;
            }
        }

        public Dictionary<string, object> Counts { get; set; }

        public string RulesValue
        {
            get { return this.rulesValue; }
            set
            {
                this.rulesValue = value;
                this.rules = null;
            }
        }

        public JObject Rules
        {
            get
            {
                if (this.rules == null)
                {
                    if (this.RulesValue != null)
                    {
                        this.rules = (JObject)JsonConvert.DeserializeObject(this.RulesValue);
                    }
                    else
                    {
                        this.rules = new JObject();
                    }
                }

                return this.rules;
            }
        }

        public override string ToString()
        {
            return "ProfileFieldModel " + this.Id + " " + this.Name
                + "[" + (this.ApplyToUsers ? "Users " : null) + (this.ApplyToCompanies ? "Companies" : null) + "]" 
                + (this.AvailableValuesCount.GetValueOrDefault() > 0 ? "[AvailableValues:"+this.AvailableValuesCount.Value+"]" : null);
        }

        public Type GetValueType()
        {
            if (this.valueType != null)
            {
                return this.valueType;
            }

            var type = this.Rules != null && this.Rules["Value"] != null && this.Rules["Value"]["Type"] != null
                ? (string)((JValue)this.Rules["Value"]["Type"]).Value : "string";
            Type netType;
            if (string.IsNullOrEmpty(type) || "string".Equals(type, StringComparison.OrdinalIgnoreCase) || "system.string".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                netType = typeof(string);
            }
            else if ("bool".Equals(type, StringComparison.OrdinalIgnoreCase) || "System.Boolean".Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                netType = typeof(bool);
            }
            else
            {
                throw new InvalidOperationException("ProfileFieldModel type " + type + " is not supported.");
            }

            return this.valueType = netType;
        }

        public bool? GetBooleanValue(string value)
        {
            var obj = this.ParseValue(value);
            return obj != null ? (bool)obj : default(bool?);
        }

        public object ParseValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var netType = this.GetValueType();

            var result = Convert.ChangeType(value, netType);
            return result;
        }

        private void Set(ProfileField item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.ApplyToUsers = item.ApplyToUsers;
            this.ApplyToCompanies = item.ApplyToCompanies;
            this.RulesValue = item.Rules;
        }
    }
}
