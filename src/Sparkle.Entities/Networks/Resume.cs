
namespace Sparkle.Entities.Networks
{
    partial class Resume : IEntityInt32Id, INetworkEntity
    {
        public override string ToString()
        {
            return this.Id + " " + this.FirstName + " " + this.LastName;
        }
    }
}
