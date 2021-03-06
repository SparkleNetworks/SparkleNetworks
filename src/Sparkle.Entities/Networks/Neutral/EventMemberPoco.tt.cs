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
    public partial class EventMemberPoco
    {
        #region Primitive Properties
    
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
    
        public Nullable<int> Notifications
        {
            get;
            set;
        }
    
        public Nullable<int> Rights
        {
            get;
            set;
        }
    
        public string Comment
        {
            get;
            set;
        }
    
        public int UserId
        {
            get { return _userId; }
            set
            {
                if (this._userId != value)
                {
                    if (this.User != null && this.User.Id != value)
                    {
                        this.User = null;
                    }
                    this._userId = value;
                }
            }
        }
        private int _userId;
    
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

        #endregion

        #region Navigation Properties
    
        public virtual UserPoco User
        {
            get { return _user; }
            set
            {
                if (!ReferenceEquals(_user, value))
                {
                    var previousValue = _user;
                    _user = value;
                    FixupUser(previousValue);
                }
            }
        }
        private UserPoco _user;
    
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
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.EventMembers.Contains(this))
            {
                previousValue.EventMembers.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.EventMembers.Contains(this))
                {
                    User.EventMembers.Add(this);
                }
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
        }
    
        private void FixupEvent(EventPoco previousValue)
        {
            if (previousValue != null && previousValue.EventMembers.Contains(this))
            {
                previousValue.EventMembers.Remove(this);
            }
    
            if (Event != null)
            {
                if (!Event.EventMembers.Contains(this))
                {
                    Event.EventMembers.Add(this);
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
