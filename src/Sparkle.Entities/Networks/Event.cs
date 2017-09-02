
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class Event : IEntityInt32Id, INetworkEntity
    {
        public EventVisibility VisibilityValue
        {
            get { return (EventVisibility)this.Visibility; }
            set { this.Visibility = (int)value; }
        }

        public EventVisibility Scope
        {
            get { return (EventVisibility)this.Visibility; }
            set { this.Visibility = (int)value; }
        }

        public WallItemDeleteReason? DeleteReasonValue
        {
            get { return (WallItemDeleteReason)this.DeleteReason; }
            set { this.DeleteReason = value != null ? new byte?((byte)value.Value) : null; }
        }

        public override string ToString()
        {
            return this.Id + " " + this.Name + " on " + this.DateEvent;
        }
    }

    partial class EventCategory : IEntityInt32Id, ICommonNetworkEntity
    {
        public override string ToString()
        {
            return "EventCategory " + this.Id + " " + this.Name + " N" + (this.NetworkId != null ? this.NetworkId.Value.ToString() : "0");
        }
    }

    public enum EventVisibility : short
    {
        /// <summary>
        /// -2 Visible outside network
        /// </summary>
        Devices = -2,

        /// <summary>
        /// -1: visible from anonymous users
        /// -1 Visible outside network
        /// </summary>
        External = -1,

        /// <summary>
        /// 0: visible to members
        /// 0 Visible to everybody.
        /// </summary>
        Public = 0,

        /// <summary>
        /// 1: company events visible to members
        /// 1 Company event, visible to employees.
        /// </summary>
        Company = 1,

        /// <summary>
        /// 2: private event
        /// 2 Personal event, visible to owner and invited.
        /// </summary>
        Personal = 2,

        /// <summary>
        /// 3: private company event
        /// 3 Company event, visible to owner and invited.
        /// </summary>
        CompanyPrivate = 3,
    }

    public enum EventType
    {
        /// <summary>
        /// The event in posted for the network (internal or public).
        /// </summary>
        Network,

        /// <summary>
        /// The event is posted in/by a company (internal or public).
        /// </summary>
        Company,

        /// <summary>
        /// The event is posted in/by a group (internal or public);
        /// </summary>
        Group,
    }
}
