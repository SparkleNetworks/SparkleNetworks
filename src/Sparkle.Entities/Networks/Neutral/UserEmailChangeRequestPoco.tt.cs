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
    public partial class UserEmailChangeRequestPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int NetworkId
        {
            get { return _networkId; }
            set
            {
                if (this._networkId != value)
                {
                    if (this.Network != null && this.Network.Id != value)
                    {
                        this.Network = null;
                    }
                    this._networkId = value;
                }
            }
        }
        private int _networkId;
    
        public string PreviousEmailAccountPart
        {
            get;
            set;
        }
    
        public string PreviousEmailTagPart
        {
            get;
            set;
        }
    
        public string PreviousEmailDomainPart
        {
            get;
            set;
        }
    
        public string NewEmailAccountPart
        {
            get;
            set;
        }
    
        public string NewEmailTagPart
        {
            get;
            set;
        }
    
        public string NewEmailDomainPart
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
    
        public int CreatedByUserId
        {
            get { return _createdByUserId; }
            set
            {
                if (this._createdByUserId != value)
                {
                    if (this.CreatedByUser != null && this.CreatedByUser.Id != value)
                    {
                        this.CreatedByUser = null;
                    }
                    this._createdByUserId = value;
                }
            }
        }
        private int _createdByUserId;
    
        public int Status
        {
            get;
            set;
        }
    
        public int PreviousEmailForbidden
        {
            get;
            set;
        }
    
        public string EmailChangeRemark
        {
            get;
            set;
        }
    
        public System.DateTime CreateDateUtc
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ValidateDateUtc
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual UserPoco CreatedByUser
        {
            get { return _createdByUser; }
            set
            {
                if (!ReferenceEquals(_createdByUser, value))
                {
                    var previousValue = _createdByUser;
                    _createdByUser = value;
                    FixupCreatedByUser(previousValue);
                }
            }
        }
        private UserPoco _createdByUser;
    
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
    
        public virtual NetworkPoco Network
        {
            get { return _network; }
            set
            {
                if (!ReferenceEquals(_network, value))
                {
                    var previousValue = _network;
                    _network = value;
                    FixupNetwork(previousValue);
                }
            }
        }
        private NetworkPoco _network;

        #endregion

        #region Association Fixup
    
        private void FixupCreatedByUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.UserEmailChangeRequests.Contains(this))
            {
                previousValue.UserEmailChangeRequests.Remove(this);
            }
    
            if (CreatedByUser != null)
            {
                if (!CreatedByUser.UserEmailChangeRequests.Contains(this))
                {
                    CreatedByUser.UserEmailChangeRequests.Add(this);
                }
                if (CreatedByUserId != CreatedByUser.Id)
                {
                    CreatedByUserId = CreatedByUser.Id;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.UserEmailChangeRequests1.Contains(this))
            {
                previousValue.UserEmailChangeRequests1.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.UserEmailChangeRequests1.Contains(this))
                {
                    User.UserEmailChangeRequests1.Add(this);
                }
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
        }
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.UserEmailChangeRequests.Contains(this))
            {
                previousValue.UserEmailChangeRequests.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.UserEmailChangeRequests.Contains(this))
                {
                    Network.UserEmailChangeRequests.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }

        #endregion

    }
}
