
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class PlaceProfileField : IEntityInt32Id, IProfileFieldValue
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
    }
}
