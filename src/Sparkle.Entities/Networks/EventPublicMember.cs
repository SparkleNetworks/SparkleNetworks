
namespace Sparkle.Entities.Networks
{
    partial class EventPublicMember : IEntityInt32Id
    {
        public EventMemberState StateValue
        {
            get { return (EventMemberState)this.State; }
            set { this.State = (int)value; }
        }
    }
}
