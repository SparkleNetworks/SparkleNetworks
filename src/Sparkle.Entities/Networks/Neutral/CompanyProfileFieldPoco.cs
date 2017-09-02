
namespace Sparkle.Entities.Networks.Neutral
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    partial class CompanyProfileFieldPoco : ICompanyProfileFieldValue
    {
        public CompanyProfileFieldPoco()
        {
        }

        public CompanyProfileFieldPoco(ProfileFieldType type, ProfileFieldSource source)
        {
            this.ProfileFieldType = type;
            this.SourceType = source;
        }

        public CompanyProfileFieldPoco(string value, ProfileFieldType type, ProfileFieldSource source)
            : this(type, source)
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
            return string.Format("CompanyField {0} of type {1} with value {2}", this.Id, this.ProfileFieldType.ToString(), this.Value);
        }
    }
}
