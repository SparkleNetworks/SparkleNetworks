
namespace Sparkle.Entities.Networks
{
    partial class GroupCategory : IEntityInt32Id
    {
        public override string ToString()
        {
            return this.Id + " " + this.Name;
        }
    }
}
