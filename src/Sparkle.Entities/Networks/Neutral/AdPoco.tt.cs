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
    public partial class AdPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int CategoryId
        {
            get { return _categoryId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._categoryId != value)
                    {
                        if (this.Category != null && this.Category.Id != value)
                        {
                            this.Category = null;
                        }
                        this._categoryId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _categoryId;
    
        public System.DateTime Date
        {
            get;
            set;
        }
    
        public string Title
        {
            get;
            set;
        }
    
        public string Message
        {
            get;
            set;
        }
    
        public bool Visibility
        {
            get;
            set;
        }
    
        public int UserId
        {
            get { return _userId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._userId != value)
                    {
                        if (this.Owner != null && this.Owner.Id != value)
                        {
                            this.Owner = null;
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
        private int _userId;
    
        public string Alias
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> UpdateDateUtc
        {
            get;
            set;
        }
    
        public Nullable<bool> IsValidated
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ValidationDateUtc
        {
            get;
            set;
        }
    
        public Nullable<int> ValidationUserId
        {
            get { return _validationUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._validationUserId != value)
                    {
                        if (this.ValidationUser != null && this.ValidationUser.Id != value)
                        {
                            this.ValidationUser = null;
                        }
                        this._validationUserId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _validationUserId;
    
        public bool IsOpen
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> CloseDateUtc
        {
            get;
            set;
        }
    
        public Nullable<int> CloseUserId
        {
            get { return _closeUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._closeUserId != value)
                    {
                        if (this.CloseUser != null && this.CloseUser.Id != value)
                        {
                            this.CloseUser = null;
                        }
                        this._closeUserId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _closeUserId;
    
        public string PendingEditTitle
        {
            get;
            set;
        }
    
        public string PendingEditMessage
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> PendingEditDate
        {
            get;
            set;
        }
    
        public Nullable<int> NetworkId
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
        private Nullable<int> _networkId;

        #endregion

        #region Navigation Properties
    
        public virtual AdCategoryPoco Category
        {
            get { return _category; }
            set
            {
                if (!ReferenceEquals(_category, value))
                {
                    var previousValue = _category;
                    _category = value;
                    FixupCategory(previousValue);
                }
            }
        }
        private AdCategoryPoco _category;
    
        public virtual UserPoco Owner
        {
            get { return _owner; }
            set
            {
                if (!ReferenceEquals(_owner, value))
                {
                    var previousValue = _owner;
                    _owner = value;
                    FixupOwner(previousValue);
                }
            }
        }
        private UserPoco _owner;
    
        // TimelineItems
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
    
        public virtual UserPoco CloseUser
        {
            get { return _closeUser; }
            set
            {
                if (!ReferenceEquals(_closeUser, value))
                {
                    var previousValue = _closeUser;
                    _closeUser = value;
                    FixupCloseUser(previousValue);
                }
            }
        }
        private UserPoco _closeUser;
    
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
    
        public virtual UserPoco ValidationUser
        {
            get { return _validationUser; }
            set
            {
                if (!ReferenceEquals(_validationUser, value))
                {
                    var previousValue = _validationUser;
                    _validationUser = value;
                    FixupValidationUser(previousValue);
                }
            }
        }
        private UserPoco _validationUser;
    
        // AdTag
        public ICollection<AdTagPoco> AdTags
        {
            get
            {
                if (_adTags == null)
                {
                    var newCollection = new FixupCollection<AdTagPoco>();
                    newCollection.CollectionChanged += FixupAdTags;
                    _adTags = newCollection;
                }
                return _adTags;
            }
            set
            {
                if (!ReferenceEquals(_adTags, value))
                {
                    var previousValue = _adTags as FixupCollection<AdTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupAdTags;
                    }
                    _adTags = value;
                    var newValue = value as FixupCollection<AdTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupAdTags;
                    }
                }
            }
        }
        private ICollection<AdTagPoco> _adTags;
    
        // Activity
        public ICollection<ActivityPoco> Activities
        {
            get
            {
                if (_activities == null)
                {
                    var newCollection = new FixupCollection<ActivityPoco>();
                    newCollection.CollectionChanged += FixupActivities;
                    _activities = newCollection;
                }
                return _activities;
            }
            set
            {
                if (!ReferenceEquals(_activities, value))
                {
                    var previousValue = _activities as FixupCollection<ActivityPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupActivities;
                    }
                    _activities = value;
                    var newValue = value as FixupCollection<ActivityPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupActivities;
                    }
                }
            }
        }
        private ICollection<ActivityPoco> _activities;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupCategory(AdCategoryPoco previousValue)
        {
            if (previousValue != null && previousValue.Ads.Contains(this))
            {
                previousValue.Ads.Remove(this);
            }
    
            if (Category != null)
            {
                if (!Category.Ads.Contains(this))
                {
                    Category.Ads.Add(this);
                }
                if (CategoryId != Category.Id)
                {
                    CategoryId = Category.Id;
                }
            }
        }
    
        private void FixupOwner(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.Ads.Contains(this))
            {
                previousValue.Ads.Remove(this);
            }
    
            if (Owner != null)
            {
                if (!Owner.Ads.Contains(this))
                {
                    Owner.Ads.Add(this);
                }
                if (UserId != Owner.Id)
                {
                    UserId = Owner.Id;
                }
            }
        }
    
        private void FixupCloseUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.AdsClosed.Contains(this))
            {
                previousValue.AdsClosed.Remove(this);
            }
    
            if (CloseUser != null)
            {
                if (!CloseUser.AdsClosed.Contains(this))
                {
                    CloseUser.AdsClosed.Add(this);
                }
                if (CloseUserId != CloseUser.Id)
                {
                    CloseUserId = CloseUser.Id;
                }
            }
            else if (!_settingFK)
            {
                CloseUserId = null;
            }
        }
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.Ads.Contains(this))
            {
                previousValue.Ads.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.Ads.Contains(this))
                {
                    Network.Ads.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
            else if (!_settingFK)
            {
                NetworkId = null;
            }
        }
    
        private void FixupValidationUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.AdsValidated.Contains(this))
            {
                previousValue.AdsValidated.Remove(this);
            }
    
            if (ValidationUser != null)
            {
                if (!ValidationUser.AdsValidated.Contains(this))
                {
                    ValidationUser.AdsValidated.Add(this);
                }
                if (ValidationUserId != ValidationUser.Id)
                {
                    ValidationUserId = ValidationUser.Id;
                }
            }
            else if (!_settingFK)
            {
                ValidationUserId = null;
            }
        }
    
        private void FixupTimelineItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemPoco item in e.NewItems)
                {
                    item.Ad = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Ad, this))
                    {
                        item.Ad = null;
                    }
                }
            }
        }
    
        private void FixupAdTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AdTagPoco item in e.NewItems)
                {
                    item.Ad = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AdTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Ad, this))
                    {
                        item.Ad = null;
                    }
                }
            }
        }
    
        private void FixupActivities(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ActivityPoco item in e.NewItems)
                {
                    item.Ad = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActivityPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Ad, this))
                    {
                        item.Ad = null;
                    }
                }
            }
        }

        #endregion

    }
}
