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
    public partial class SocialNetworkStatePoco
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
    
        public byte SocialNetworkType
        {
            get;
            set;
        }
    
        public Nullable<long> LastItemId
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> LastStartDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> LastEndDate
        {
            get;
            set;
        }
    
        public bool IsProcessing
        {
            get;
            set;
        }
    
        public string Username
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> OAuthDateUtc
        {
            get;
            set;
        }
    
        public string OAuthRequestToken
        {
            get;
            set;
        }
    
        public string OAuthRequestVerifier
        {
            get;
            set;
        }
    
        public string OAuthAccessToken
        {
            get;
            set;
        }
    
        public string OAuthAccessSecret
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
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
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.SocialNetworkStates.Contains(this))
            {
                previousValue.SocialNetworkStates.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.SocialNetworkStates.Contains(this))
                {
                    Network.SocialNetworkStates.Add(this);
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
