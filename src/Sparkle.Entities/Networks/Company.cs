
namespace Sparkle.Entities.Networks
{
    partial class Company : INetworkEntity
    {
        public Company()
        {
        }

        public override string ToString()
        {
            return this.ID + " " + this.Name;
        }
    }

    partial class CompanyRequest : INetworkEntity, IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + (this.Approved != null ? this.Approved.Value ? " approved: " : " rejected: " : " pending: ") + this.Name;
        }
    }

    partial class CompanyRequestMessage : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " R" + this.CompanyRequestId + (this.IsMessageFromCompany ? " from company" : " from staff");
        }
    }

    partial class CompanyPlace : IEntityInt32Id
    {
        public override string ToString()
        {
            return "CompanyPlace " + this.Id + " C:" + this.CompanyId + " P:" + this.PlaceId;
        }
    }
}
