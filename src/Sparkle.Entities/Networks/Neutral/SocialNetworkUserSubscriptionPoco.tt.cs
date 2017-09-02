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
    public partial class SocialNetworkUserSubscriptionPoco
    {
        #region Primitive Properties
    
        public int Id
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
    
        public int SocialNetworkConnectionsId
        {
            get { return _socialNetworkConnectionsId; }
            set
            {
                if (this._socialNetworkConnectionsId != value)
                {
                    if (this.SocialNetworkConnection != null && this.SocialNetworkConnection.Id != value)
                    {
                        this.SocialNetworkConnection = null;
                    }
                    this._socialNetworkConnectionsId = value;
                }
            }
        }
        private int _socialNetworkConnectionsId;
    
        public bool AutoPublish
        {
            get;
            set;
        }
    
        public string ContentContainsFilter
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
    
        public virtual SocialNetworkConnectionPoco SocialNetworkConnection
        {
            get { return _socialNetworkConnection; }
            set
            {
                if (!ReferenceEquals(_socialNetworkConnection, value))
                {
                    var previousValue = _socialNetworkConnection;
                    _socialNetworkConnection = value;
                    FixupSocialNetworkConnection(previousValue);
                }
            }
        }
        private SocialNetworkConnectionPoco _socialNetworkConnection;

        #endregion

        #region Association Fixup
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.SocialNetworkUserSubscriptions.Contains(this))
            {
                previousValue.SocialNetworkUserSubscriptions.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.SocialNetworkUserSubscriptions.Contains(this))
                {
                    User.SocialNetworkUserSubscriptions.Add(this);
                }
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
        }
    
        private void FixupSocialNetworkConnection(SocialNetworkConnectionPoco previousValue)
        {
            if (previousValue != null && previousValue.SocialNetworkUserSubscriptions.Contains(this))
            {
                previousValue.SocialNetworkUserSubscriptions.Remove(this);
            }
    
            if (SocialNetworkConnection != null)
            {
                if (!SocialNetworkConnection.SocialNetworkUserSubscriptions.Contains(this))
                {
                    SocialNetworkConnection.SocialNetworkUserSubscriptions.Add(this);
                }
                if (SocialNetworkConnectionsId != SocialNetworkConnection.Id)
                {
                    SocialNetworkConnectionsId = SocialNetworkConnection.Id;
                }
            }
        }

        #endregion

    }
}
