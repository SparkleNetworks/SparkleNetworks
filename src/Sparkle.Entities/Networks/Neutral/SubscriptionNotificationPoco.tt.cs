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
    public partial class SubscriptionNotificationPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int SubscriptionId
        {
            get { return _subscriptionId; }
            set
            {
                if (this._subscriptionId != value)
                {
                    if (this.Subscription != null && this.Subscription.Id != value)
                    {
                        this.Subscription = null;
                    }
                    this._subscriptionId = value;
                }
            }
        }
        private int _subscriptionId;
    
        public System.DateTime DateSendUtc
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateSentUtc
        {
            get;
            set;
        }
    
        public byte Status
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual SubscriptionPoco Subscription
        {
            get { return _subscription; }
            set
            {
                if (!ReferenceEquals(_subscription, value))
                {
                    var previousValue = _subscription;
                    _subscription = value;
                    FixupSubscription(previousValue);
                }
            }
        }
        private SubscriptionPoco _subscription;

        #endregion

        #region Association Fixup
    
        private void FixupSubscription(SubscriptionPoco previousValue)
        {
            if (previousValue != null && previousValue.SubscriptionNotifications.Contains(this))
            {
                previousValue.SubscriptionNotifications.Remove(this);
            }
    
            if (Subscription != null)
            {
                if (!Subscription.SubscriptionNotifications.Contains(this))
                {
                    Subscription.SubscriptionNotifications.Add(this);
                }
                if (SubscriptionId != Subscription.Id)
                {
                    SubscriptionId = Subscription.Id;
                }
            }
        }

        #endregion

    }
}
