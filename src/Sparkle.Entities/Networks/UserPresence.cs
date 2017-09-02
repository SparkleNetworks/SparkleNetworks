
namespace Sparkle.Entities.Networks
{
    partial class UserPresence : IEntityInt32Id
    {
        public override string ToString()
        {
            return "UserPresence " + this.UserId + " on " + this.Day;
        }
    }
}
