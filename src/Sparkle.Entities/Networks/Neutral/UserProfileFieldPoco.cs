
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class UserProfileFieldPoco
    {
        public UserProfileFieldPoco()
        {
        }

        public UserProfileFieldPoco(ProfileFieldType type)
        {
            this.ProfileFieldType = type;
        }

        public UserProfileFieldPoco(ProfileFieldType type, string value)
            : this(type)
        {
            this.Value = value;
        }

        public ProfileFieldType ProfileFieldType
        {
            get { return (ProfileFieldType)this.ProfileFieldId; }
            set { this.ProfileFieldId = (int)value; }
        }

        public ProfileFieldSource SourceType
        {
            get { return (ProfileFieldSource)this.Source; }
            set { this.Source = (byte)value; }
        }

        public override string ToString()
        {
            return string.Format("UserField {0} of type {1} with value {2}", this.Id, this.ProfileFieldType.ToString(), this.Value);
        }
    }
}
