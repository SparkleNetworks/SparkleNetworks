
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProfileFieldValueModel
    {
        public ProfileFieldValueModel(ProfileFieldModel field, string value)
        {
            this.FieldName = field.Name;
            this.ProfileFieldId = field.Id;
            this.Value = value;
        }

        public ProfileFieldValueModel(IProfileFieldValue item, int? itemId = null, ProfileField field = null)
        {
            this.Set(item, field);
        }

        private void Set(IProfileFieldValue item, ProfileField field)
        {
            this.Data = item.Data;
            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateUpdatedUtc = item.DateUpdatedUtc.AsUtc();
            this.ProfileFieldId = item.ProfileFieldId;
            this.ProfileFieldValueId = item.Id;
            this.FieldType = item.ProfileFieldType;
            this.SourceType = item.SourceType;
            this.UpdateCount = item.UpdateCount;
            this.Value = item.Value;

            if (field != null)
            {
                this.FieldName = field.Name;
            }
        }

        public int ProfileFieldId { get; set; }

        public int ProfileFieldValueId { get; set; }

        public ProfileFieldType FieldType { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime? DateUpdatedUtc { get; set; }

        public string Value { get; set; }

        public string Data { get; set; }

        public ProfileFieldSource SourceType { get; set; }

        public short UpdateCount { get; set; }

        public string FieldName { get; set; }

        public ProfileFieldValueAction? Action { get; set; }
    }

    public enum ProfileFieldValueAction
    {
        None,
        Delete,
    }
}
