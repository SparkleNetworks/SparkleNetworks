
namespace Sparkle.Entities.Networks
{
    partial class CreateNetworkRequest : IEntityInt32Id, INetworkEntity
    {
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }
}
