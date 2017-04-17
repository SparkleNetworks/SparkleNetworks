
namespace Sparkle.Entities.Networks
{
    partial class Achievement : IEntityInt32Id, ICommonNetworkEntity
    {
        public Achievement()
        {
        }

        public override string ToString()
        {
            return this.Id + " " + this.Title;
        }
    }
}
