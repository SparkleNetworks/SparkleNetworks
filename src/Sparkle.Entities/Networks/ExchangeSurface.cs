
namespace Sparkle.Entities.Networks
{
    partial class ExchangeSurface : IEntityInt32Id, INetworkEntity
    {
        public override string ToString()
        {
            return this.Id + " " + this.Title;
        }
    }

    partial class Device : IEntityInt32Id, INetworkEntity
    {
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }

    partial class DeviceConfiguration : IEntityInt32Id, INetworkEntity
    {
        public override string ToString()
        {
            return this.Id + " " + this.Key;
        }
    }

    partial class DevicePlanning : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id.ToString();
        }
    }

    partial class CompaniesVisit : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " Company " + this.CompanyId + " was visited by " + this.UserId + " on " + this.Date;
        }
    }

    partial class Building : IEntityInt32Id, INetworkEntity
    {
        public Building()
        {
        }

        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }
}
