
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class EventMember
    {
        public EventMemberState StateValue
        {
            get { return (EventMemberState)this.State; }
            set { this.State = (int)value; }
        }

        public EventMemberRight RightsValue
        {
            get { return (EventMemberRight)this.State; }
            set { this.State = (int)value; }
		}

        public override string ToString()
        {
            return "Event:" + this._EventId + " User:" + this._UserId + " Status:" + this.StateValue;
        }
    }

    public enum EventMemberRight
    {
        None = 0,
        Admin = 1,
    }

    public enum EventMemberOptions
    {
        None = 0,
        Event = 0x0001,
        User = 0x0002,
        CompanyUser = 0x0004,
        ApplyingUserSubscriptions = 0x0008,
    }
}
