//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Sparkle.Entities.Networks.Neutral
{
    public partial class EventPublicMemberPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int EventId
        {
            get { return _eventId; }
            set
            {
                if (this._eventId != value)
                {
                    if (this.Event != null && this.Event.Id != value)
                    {
                        this.Event = null;
                    }
                    this._eventId = value;
                }
            }
        }
        private int _eventId;
    
        public int State
        {
            get;
            set;
        }
    
        public string FirstName
        {
            get;
            set;
        }
    
        public string LastName
        {
            get;
            set;
        }
    
        public string Email
        {
            get;
            set;
        }
    
        public string Company
        {
            get;
            set;
        }
    
        public string Job
        {
            get;
            set;
        }
    
        public string Phone
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateCreatedUtc
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateUpdatedUtc
        {
            get;
            set;
        }
    
        public string RemoteAddress
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual EventPoco Event
        {
            get { return _event; }
            set
            {
                if (!ReferenceEquals(_event, value))
                {
                    var previousValue = _event;
                    _event = value;
                    FixupEvent(previousValue);
                }
            }
        }
        private EventPoco _event;

        #endregion

        #region Association Fixup
    
        private void FixupEvent(EventPoco previousValue)
        {
            if (previousValue != null && previousValue.PublicMembers.Contains(this))
            {
                previousValue.PublicMembers.Remove(this);
            }
    
            if (Event != null)
            {
                if (!Event.PublicMembers.Contains(this))
                {
                    Event.PublicMembers.Add(this);
                }
                if (EventId != Event.Id)
                {
                    EventId = Event.Id;
                }
            }
        }

        #endregion

    }
}
