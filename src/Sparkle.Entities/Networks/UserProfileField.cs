
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class UserProfileField : IEntityInt32Id, IProfileFieldValue
    {
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
            return string.Format(
                "UserProfileField {0} of user {1} and field {2} with value '{3}'",
                this.Id,
                this.UserId,
                this.ProfileFieldId,
                this.Value);
        }
    }

    public enum ProfileFieldSource : byte
    {
        None = 0,
        UserInput = 1,
        LinkedIn = 2,
    }
}
