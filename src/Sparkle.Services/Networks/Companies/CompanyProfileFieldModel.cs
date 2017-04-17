
namespace Sparkle.Services.Networks.Companies
{
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    public class CompanyProfileFieldModel : IProfileFieldValueModel
    {
        // TODO: MERGE WITH UserProfileFieldModel
        // TODO: MERGE WITH PartnerResourceProfileFieldModel

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
        };

        private LocationProfileFieldModel locationModel;

        public CompanyProfileFieldModel(ProfileFieldType type)
        {
            this.Type = type;
            this.TypeId = (int)type;
        }

        public CompanyProfileFieldModel(ICompanyProfileFieldValue item)
        {
            this.Id = item.Id;
            this.CompanyId = item.CompanyId;
            this.Type = item.ProfileFieldType;
            this.TypeId = (int)item.ProfileFieldType;
            this.Value = item.Value;
            this.Source = item.SourceType;
            this.Data = item.Data;
        }

        public CompanyProfileFieldModel(int companyId, ProfileFieldType fieldType, string distantValue, ProfileFieldSource source)
        {
            this.CompanyId = companyId;
            this.Type = fieldType;
            this.TypeId = (int)fieldType;
            this.Value = distantValue;
            this.Source = source;
        }

        private void SetValue(string value)
        {
            this.Value = value;

            if (this.Type == ProfileFieldType.Twitter)
            {
                string username = null;
                if (SrkToolkit.DataAnnotations.TwitterUsernameAttribute.GetUsername(this.Value, out username))
                    this.Value = username;
            }
        }

        [DataMember]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        [DataMember]
        int IProfileFieldValueModel.EntityId
        {
            get { return this.CompanyId; }
            set { this.CompanyId = value; }
        }

        [DataMember]
        public ProfileFieldType Type { get; set; }

        [DataMember]
        public int TypeId { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public ProfileFieldSource Source { get; set; }

        [DataMember]
        public string Data { get; set; }

        public LocationProfileFieldModel LocationModel
        {
            get { return this.GetModel(ref this.locationModel, ProfileFieldType.Location); }
            set
            {
                this.SetModel(ref this.locationModel, ProfileFieldType.Location, value);
                if (this.LocationModel != null)
                {
                    if (!string.IsNullOrEmpty(this.LocationModel.Street1))
                    {
                        this.Value = this.LocationModel.Street1;
                        if (!string.IsNullOrEmpty(this.LocationModel.Street2))
                            this.Value += ", " + this.LocationModel.Street2;
                        if (!string.IsNullOrEmpty(this.LocationModel.City))
                        {
                            if (!string.IsNullOrEmpty(this.LocationModel.State))
                                this.Value += " @ " + this.LocationModel.City + ", " + this.LocationModel.State;
                            else
                                this.Value += " @ " + this.LocationModel.City;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(this.LocationModel.City))
                        {
                            this.Value = this.LocationModel.City;
                            if (!string.IsNullOrEmpty(this.LocationModel.State))
                                this.Value += ", " + this.LocationModel.State;
                        }
                    }
                }
            }
        }

        private T GetModel<T>(ref T field, ProfileFieldType type)
        {
            if (field == null)
            {
                if (this.Data != null && this.Type == type)
                {
                    field = JsonConvert.DeserializeObject<T>(this.Data);
                }
            }

            return field;
        }

        private void SetModel<T>(ref T field, ProfileFieldType type, T value)
        {
            if (this.Type != type)
                throw new InvalidOperationException("Cannot set a " + typeof(T) + " into a field of type " + type);

            field = value;

            if (value != null)
            {
                this.Data = JsonConvert.SerializeObject(value, Formatting.None, JsonSettings);
            }
            else
            {
                this.Data = null;
            }
        }
    }
}
