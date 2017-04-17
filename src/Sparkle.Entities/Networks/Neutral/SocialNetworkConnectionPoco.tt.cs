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
    public partial class SocialNetworkConnectionPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
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
    
        public byte Type
        {
            get;
            set;
        }
    
        public string Username
        {
            get;
            set;
        }
    
        public string OAuthToken
        {
            get;
            set;
        }
    
        public string OAuthVerifier
        {
            get;
            set;
        }
    
        public bool IsActive
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> OAuthTokenDateUtc
        {
            get;
            set;
        }
    
        public Nullable<int> OAuthTokenDurationMinutes
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
    
        // SocialNetworkCompanySubscription
        public ICollection<SocialNetworkCompanySubscriptionPoco> SocialNetworkCompanySubscriptions
        {
            get
            {
                if (_socialNetworkCompanySubscriptions == null)
                {
                    var newCollection = new FixupCollection<SocialNetworkCompanySubscriptionPoco>();
                    newCollection.CollectionChanged += FixupSocialNetworkCompanySubscriptions;
                    _socialNetworkCompanySubscriptions = newCollection;
                }
                return _socialNetworkCompanySubscriptions;
            }
            set
            {
                if (!ReferenceEquals(_socialNetworkCompanySubscriptions, value))
                {
                    var previousValue = _socialNetworkCompanySubscriptions as FixupCollection<SocialNetworkCompanySubscriptionPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSocialNetworkCompanySubscriptions;
                    }
                    _socialNetworkCompanySubscriptions = value;
                    var newValue = value as FixupCollection<SocialNetworkCompanySubscriptionPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSocialNetworkCompanySubscriptions;
                    }
                }
            }
        }
        private ICollection<SocialNetworkCompanySubscriptionPoco> _socialNetworkCompanySubscriptions;
    
        // SocialNetworkUserSubscription
        public ICollection<SocialNetworkUserSubscriptionPoco> SocialNetworkUserSubscriptions
        {
            get
            {
                if (_socialNetworkUserSubscriptions == null)
                {
                    var newCollection = new FixupCollection<SocialNetworkUserSubscriptionPoco>();
                    newCollection.CollectionChanged += FixupSocialNetworkUserSubscriptions;
                    _socialNetworkUserSubscriptions = newCollection;
                }
                return _socialNetworkUserSubscriptions;
            }
            set
            {
                if (!ReferenceEquals(_socialNetworkUserSubscriptions, value))
                {
                    var previousValue = _socialNetworkUserSubscriptions as FixupCollection<SocialNetworkUserSubscriptionPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSocialNetworkUserSubscriptions;
                    }
                    _socialNetworkUserSubscriptions = value;
                    var newValue = value as FixupCollection<SocialNetworkUserSubscriptionPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSocialNetworkUserSubscriptions;
                    }
                }
            }
        }
        private ICollection<SocialNetworkUserSubscriptionPoco> _socialNetworkUserSubscriptions;

        #endregion

        #region Association Fixup
    
        private void FixupCreatedByUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.SocialNetworkConnections.Contains(this))
            {
                previousValue.SocialNetworkConnections.Remove(this);
            }
    
            if (CreatedByUser != null)
            {
                if (!CreatedByUser.SocialNetworkConnections.Contains(this))
                {
                    CreatedByUser.SocialNetworkConnections.Add(this);
                }
                if (CreatedByUserId != CreatedByUser.Id)
                {
                    CreatedByUserId = CreatedByUser.Id;
                }
            }
        }
    
        private void FixupSocialNetworkCompanySubscriptions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SocialNetworkCompanySubscriptionPoco item in e.NewItems)
                {
                    item.SocialNetworkConnection = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SocialNetworkCompanySubscriptionPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.SocialNetworkConnection, this))
                    {
                        item.SocialNetworkConnection = null;
                    }
                }
            }
        }
    
        private void FixupSocialNetworkUserSubscriptions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SocialNetworkUserSubscriptionPoco item in e.NewItems)
                {
                    item.SocialNetworkConnection = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (SocialNetworkUserSubscriptionPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.SocialNetworkConnection, this))
                    {
                        item.SocialNetworkConnection = null;
                    }
                }
            }
        }

        #endregion

    }
}
