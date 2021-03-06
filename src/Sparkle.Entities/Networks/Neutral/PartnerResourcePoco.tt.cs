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
    public partial class PartnerResourcePoco
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
    
        public string Name
        {
            get;
            set;
        }
    
        public bool Available
        {
            get;
            set;
        }
    
        public int CreatedByUserId
        {
            get { return _createdByUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._createdByUserId != value)
                    {
                        if (this.CreatedByUser != null && this.CreatedByUser.Id != value)
                        {
                            this.CreatedByUser = null;
                        }
                        this._createdByUserId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _createdByUserId;
    
        public Nullable<int> DeletedByUserId
        {
            get { return _deletedByUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._deletedByUserId != value)
                    {
                        if (this.DeletedByUser != null && this.DeletedByUser.Id != value)
                        {
                            this.DeletedByUser = null;
                        }
                        this._deletedByUserId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _deletedByUserId;
    
        public System.DateTime DateCreatedUtc
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateDeletedUtc
        {
            get;
            set;
        }
    
        public string Alias
        {
            get;
            set;
        }
    
        public string PictureName
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateLastPictureUpdateUtc
        {
            get;
            set;
        }
    
        public bool IsApproved
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateApprovedUtc
        {
            get;
            set;
        }
    
        public Nullable<int> ApprovedByUserId
        {
            get { return _approvedByUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._approvedByUserId != value)
                    {
                        if (this.ApprovedByUser != null && this.ApprovedByUser.Id != value)
                        {
                            this.ApprovedByUser = null;
                        }
                        this._approvedByUserId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _approvedByUserId;

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
    
        public virtual UserPoco DeletedByUser
        {
            get { return _deletedByUser; }
            set
            {
                if (!ReferenceEquals(_deletedByUser, value))
                {
                    var previousValue = _deletedByUser;
                    _deletedByUser = value;
                    FixupDeletedByUser(previousValue);
                }
            }
        }
        private UserPoco _deletedByUser;
    
        // PartnerResourceProfileField
        public ICollection<PartnerResourceProfileFieldPoco> PartnerResourceProfileFields
        {
            get
            {
                if (_partnerResourceProfileFields == null)
                {
                    var newCollection = new FixupCollection<PartnerResourceProfileFieldPoco>();
                    newCollection.CollectionChanged += FixupPartnerResourceProfileFields;
                    _partnerResourceProfileFields = newCollection;
                }
                return _partnerResourceProfileFields;
            }
            set
            {
                if (!ReferenceEquals(_partnerResourceProfileFields, value))
                {
                    var previousValue = _partnerResourceProfileFields as FixupCollection<PartnerResourceProfileFieldPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPartnerResourceProfileFields;
                    }
                    _partnerResourceProfileFields = value;
                    var newValue = value as FixupCollection<PartnerResourceProfileFieldPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPartnerResourceProfileFields;
                    }
                }
            }
        }
        private ICollection<PartnerResourceProfileFieldPoco> _partnerResourceProfileFields;
    
        // PartnerResourceTag
        public ICollection<PartnerResourceTagPoco> Tags
        {
            get
            {
                if (_tags == null)
                {
                    var newCollection = new FixupCollection<PartnerResourceTagPoco>();
                    newCollection.CollectionChanged += FixupTags;
                    _tags = newCollection;
                }
                return _tags;
            }
            set
            {
                if (!ReferenceEquals(_tags, value))
                {
                    var previousValue = _tags as FixupCollection<PartnerResourceTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTags;
                    }
                    _tags = value;
                    var newValue = value as FixupCollection<PartnerResourceTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTags;
                    }
                }
            }
        }
        private ICollection<PartnerResourceTagPoco> _tags;
    
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
    
        public virtual UserPoco ApprovedByUser
        {
            get { return _approvedByUser; }
            set
            {
                if (!ReferenceEquals(_approvedByUser, value))
                {
                    var previousValue = _approvedByUser;
                    _approvedByUser = value;
                    FixupApprovedByUser(previousValue);
                }
            }
        }
        private UserPoco _approvedByUser;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.PartnerResources.Contains(this))
            {
                previousValue.PartnerResources.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.PartnerResources.Contains(this))
                {
                    Network.PartnerResources.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupCreatedByUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.CreatedPartnerResources.Contains(this))
            {
                previousValue.CreatedPartnerResources.Remove(this);
            }
    
            if (CreatedByUser != null)
            {
                if (!CreatedByUser.CreatedPartnerResources.Contains(this))
                {
                    CreatedByUser.CreatedPartnerResources.Add(this);
                }
                if (CreatedByUserId != CreatedByUser.Id)
                {
                    CreatedByUserId = CreatedByUser.Id;
                }
            }
        }
    
        private void FixupDeletedByUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.DeletedPartnerResources.Contains(this))
            {
                previousValue.DeletedPartnerResources.Remove(this);
            }
    
            if (DeletedByUser != null)
            {
                if (!DeletedByUser.DeletedPartnerResources.Contains(this))
                {
                    DeletedByUser.DeletedPartnerResources.Add(this);
                }
                if (DeletedByUserId != DeletedByUser.Id)
                {
                    DeletedByUserId = DeletedByUser.Id;
                }
            }
            else if (!_settingFK)
            {
                DeletedByUserId = null;
            }
        }
    
        private void FixupApprovedByUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.PartnerResources.Contains(this))
            {
                previousValue.PartnerResources.Remove(this);
            }
    
            if (ApprovedByUser != null)
            {
                if (!ApprovedByUser.PartnerResources.Contains(this))
                {
                    ApprovedByUser.PartnerResources.Add(this);
                }
                if (ApprovedByUserId != ApprovedByUser.Id)
                {
                    ApprovedByUserId = ApprovedByUser.Id;
                }
            }
            else if (!_settingFK)
            {
                ApprovedByUserId = null;
            }
        }
    
        private void FixupPartnerResourceProfileFields(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PartnerResourceProfileFieldPoco item in e.NewItems)
                {
                    item.PartnerResource = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PartnerResourceProfileFieldPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.PartnerResource, this))
                    {
                        item.PartnerResource = null;
                    }
                }
            }
        }
    
        private void FixupTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PartnerResourceTagPoco item in e.NewItems)
                {
                    item.PartnerResource = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PartnerResourceTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.PartnerResource, this))
                    {
                        item.PartnerResource = null;
                    }
                }
            }
        }
    
        private void FixupTimelineItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemPoco item in e.NewItems)
                {
                    item.PartnerResource = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.PartnerResource, this))
                    {
                        item.PartnerResource = null;
                    }
                }
            }
        }

        #endregion

    }
}
