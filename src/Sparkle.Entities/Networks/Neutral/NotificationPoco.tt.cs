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
    public partial class NotificationPoco
    {
        #region Primitive Properties
    
        public bool ContactRequest
        {
            get;
            set;
        }
    
        public bool Publication
        {
            get;
            set;
        }
    
        public bool Comment
        {
            get;
            set;
        }
    
        public bool EventInvitation
        {
            get;
            set;
        }
    
        public bool PrivateMessage
        {
            get;
            set;
        }
    
        public Nullable<int> StartPage
        {
            get;
            set;
        }
    
        public Nullable<bool> Newsletter
        {
            get;
            set;
        }
    
        public Nullable<bool> Lang
        {
            get;
            set;
        }
    
        public Nullable<bool> NotifyOnPersonalEmailAddress
        {
            get;
            set;
        }
    
        public string Culture
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
    
        public Nullable<bool> DailyNewsletter
        {
            get;
            set;
        }
    
        public bool PrivateGroupJoinRequest
        {
            get;
            set;
        }
    
        public bool MailChimp
        {
            get;
            set;
        }
    
        public string MailChimpStatus
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> MailChimpStatusDateUtc
        {
            get;
            set;
        }
    
        public Nullable<bool> MainTimelineItems
        {
            get;
            set;
        }
    
        public Nullable<bool> MainTimelineComments
        {
            get;
            set;
        }
    
        public Nullable<bool> CompanyTimelineItems
        {
            get;
            set;
        }
    
        public Nullable<bool> CompanyTimelineComments
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

        #endregion

        #region Association Fixup
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.Notification, this))
            {
                previousValue.Notification = null;
            }
    
            if (User != null)
            {
                User.Notification = this;
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
        }

        #endregion

    }
}