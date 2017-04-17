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
    public partial class EmailMessagePoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public Nullable<int> UserId
        {
            get { return _userId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._userId != value)
                    {
                        if (this.User != null && this.User.Id != value)
                        {
                            this.User = null;
                        }
                        this._userId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _userId;
    
        public System.DateTime DateSentUtc
        {
            get;
            set;
        }
    
        public string ProviderName
        {
            get;
            set;
        }
    
        public string ProviderId
        {
            get;
            set;
        }
    
        public Nullable<bool> ProviderDeliveryConfirmation
        {
            get;
            set;
        }
    
        public short SendErrors
        {
            get;
            set;
        }
    
        public bool SendSucceed
        {
            get;
            set;
        }
    
        public string LastSendError
        {
            get;
            set;
        }
    
        public Nullable<int> FirstBounceCode
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> FirstBounceDateUtc
        {
            get;
            set;
        }
    
        public Nullable<int> LastBounceCode
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> LastBounceDateUtc
        {
            get;
            set;
        }
    
        public string Tags
        {
            get;
            set;
        }
    
        public string EmailSubject
        {
            get;
            set;
        }
    
        public string EmailRecipient
        {
            get;
            set;
        }
    
        public string EmailSender
        {
            get;
            set;
        }
    
        public int NetworkId
        {
            get { return _networkId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._networkId != value)
                    {
                        if (this.Network != null && this.Network.Id != value)
                        {
                            this.Network = null;
                        }
                        this._networkId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _networkId;

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
    
        private bool _settingFK = false;
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.EmailMessages.Contains(this))
            {
                previousValue.EmailMessages.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.EmailMessages.Contains(this))
                {
                    User.EmailMessages.Add(this);
                }
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
            else if (!_settingFK)
            {
                UserId = null;
            }
        }
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.EmailMessages.Contains(this))
            {
                previousValue.EmailMessages.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.EmailMessages.Contains(this))
                {
                    Network.EmailMessages.Add(this);
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