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
    public partial class InboundEmailMessagePoco
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
    
        public string SenderEmailAccount
        {
            get;
            set;
        }
    
        public string SenderEmailTag
        {
            get;
            set;
        }
    
        public string SenderEmailDomain
        {
            get;
            set;
        }
    
        public string ReceiverEmailAccount
        {
            get;
            set;
        }
    
        public string ReceiverEmailTag
        {
            get;
            set;
        }
    
        public string ReceiverEmailDomain
        {
            get;
            set;
        }
    
        public int Provider
        {
            get;
            set;
        }
    
        public System.DateTime DateReceivedUtc
        {
            get;
            set;
        }
    
        public string SourceEmailFileName
        {
            get;
            set;
        }
    
        public Nullable<double> ProviderSpamScore
        {
            get;
            set;
        }
    
        public Nullable<bool> DkimSigned
        {
            get;
            set;
        }
    
        public Nullable<bool> DkimValid
        {
            get;
            set;
        }
    
        public string SpfTestResult
        {
            get;
            set;
        }
    
        public string SpfTestDetail
        {
            get;
            set;
        }
    
        public bool Success
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
    
        // TimelineItem
        public ICollection<TimelineItemPoco> TimelineItems
        {
            get
            {
                if (_timelineItems == null)
                {
                    var newCollection = new FixupCollection<TimelineItemPoco>();
                    newCollection.CollectionChanged += FixupTimelineItems;
                    _timelineItems = newCollection;
                }
                return _timelineItems;
            }
            set
            {
                if (!ReferenceEquals(_timelineItems, value))
                {
                    var previousValue = _timelineItems as FixupCollection<TimelineItemPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTimelineItems;
                    }
                    _timelineItems = value;
                    var newValue = value as FixupCollection<TimelineItemPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTimelineItems;
                    }
                }
            }
        }
        private ICollection<TimelineItemPoco> _timelineItems;

        #endregion

        #region Association Fixup
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.InboundEmailMessages.Contains(this))
            {
                previousValue.InboundEmailMessages.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.InboundEmailMessages.Contains(this))
                {
                    Network.InboundEmailMessages.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupTimelineItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemPoco item in e.NewItems)
                {
                    item.InboundEmailMessage = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.InboundEmailMessage, this))
                    {
                        item.InboundEmailMessage = null;
                    }
                }
            }
        }

        #endregion

    }
}
