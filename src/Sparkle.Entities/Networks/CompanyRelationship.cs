
namespace Sparkle.Entities.Networks
{
    using System;

    public partial class CompanyRelationship : IEntityInt32Id
    {
    }

    public partial class CompanyRelationshipType : IEntityInt32Id
    {
        public KnownCompanyRelationshipType KnownTypeValue
        {
            get { return (KnownCompanyRelationshipType)this.KnownType; }
            set { this.KnownType = (byte)value; }
        }

        public override string ToString()
        {
            return "CompanyRelationshipType " + this.Id + " " + this.KnownTypeValue + " N" + this.NetworkId;
        }
    }

    public enum KnownCompanyRelationshipType
    {
        Unknown = 0,
        Invited = 1,
        StartupAccelerator = 2,
    }
}
