
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class ProfileFieldsAvailiableValue : IEntityInt32Id
    {
        public ProfileFieldType ProfileFieldType
        {
            get { return (ProfileFieldType)this.ProfileFieldId; }
            set { this.ProfileFieldId = (int)value; }
        }

        public override string ToString()
        {
            return string.Format(
                "ProfileFieldsAvailiableValue {0} T {1} V {2}",
                this.Id,
                this.ProfileFieldType,
                this.Value);
        }
    }
}
