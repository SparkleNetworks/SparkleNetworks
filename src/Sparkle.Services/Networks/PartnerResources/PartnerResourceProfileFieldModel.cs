
namespace Sparkle.Services.Networks.PartnerResources
{
    using Newtonsoft.Json;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PartnerResourceProfileFieldModel
    {
        // TODO: MERGE WITH CompanyProfileFieldModel
        // TODO: MERGE WITH UserProfileFieldModel

        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
        };

        public PartnerResourceProfileFieldModel(ProfileFieldType type)
        {
            this.Type = type;
        }

        public PartnerResourceProfileFieldModel(PartnerResourceProfileField item)
        {
            this.Id = item.Id;
            this.PartnerResourceId = item.PartnerResourceId;
            this.Type = item.ProfileFieldType;
            this.Value = item.Value;
            this.Data = item.Data;
        }

        public int Id { get; set; }

        public int PartnerResourceId { get; set; }

        public ProfileFieldType Type { get; set; }

        public string Value { get; set; }

        public string Data { get; set; }

        private ContactProfileFieldModel contactModel;

        public ContactProfileFieldModel ContactModel
        {
            get { return this.GetModel(ref this.contactModel, ProfileFieldType.Contact); }
            set
            {
                this.SetModel(ref this.contactModel, ProfileFieldType.Contact, value);
                if (this.ContactModel != null)
                {
                    if (!string.IsNullOrEmpty(this.ContactModel.FirstName))
                    {
                        this.Value = this.ContactModel.FirstName;
                        if (!string.IsNullOrEmpty(this.ContactModel.LastName))
                            this.Value += " " + this.ContactModel.LastName;
                        if (!string.IsNullOrEmpty(this.ContactModel.Job))
                            this.Value += ", " + this.ContactModel.Job;
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
                this.Data = JsonConvert.SerializeObject(value, Formatting.None, jsonSettings);
            }
            else
            {
                this.Data = null;
            }
        }
    }
}
