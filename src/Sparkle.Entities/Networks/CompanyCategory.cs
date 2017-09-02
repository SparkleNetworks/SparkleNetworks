
namespace Sparkle.Entities.Networks
{
    public enum CompanyCategoryEnum
    {
        Incubator = 1,
        Startup = 2,
        Company = 3,
        Association = 4,
    }

    partial class CompanyCategory
    {
        public KnownCompanyCategory KnownCategoryValue
        {
            get { return (KnownCompanyCategory)this.KnownCategory; }
            set { this.KnownCategory = (byte)value; }
        }

        public override string ToString()
        {
            return "CompanyCategory " + this.Id + " " + this.Name + " " + this.KnownCategoryValue + " N" + this.NetworkId;
        }
    }

    public enum KnownCompanyCategory
    {
        Unknown = 0,
        CompanyAccelerator = 1,
        Startup = 2,
    }
}
