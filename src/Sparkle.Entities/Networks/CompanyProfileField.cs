
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class CompanyProfileField : IEntityInt32Id, ICompanyProfileFieldValue
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
                "CompanyProfileField {0} of company {1} and field {2} with value '{3}'",
                this.Id,
                this.CompanyId,
                this.ProfileFieldId,
                this.Value);
        }
    }
}
