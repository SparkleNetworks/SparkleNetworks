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
    public partial class TagDefinitionPoco
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
    
        public int CategoryId
        {
            get { return _categoryId; }
            set
            {
                if (this._categoryId != value)
                {
                    if (this.Category != null && this.Category.Id != value)
                    {
                        this.Category = null;
                    }
                    this._categoryId = value;
                }
            }
        }
        private int _categoryId;
    
        public string Name
        {
            get;
            set;
        }
    
        public string Description
        {
            get;
            set;
        }
    
        public System.DateTime CreatedDateUtc
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
    
        public string Alias
        {
            get;
            set;
        }
    
        public string Data
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
    
        public virtual TagCategoryPoco Category
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
        private TagCategoryPoco _category;
    
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
    
        // PartnerResourceTag
        public ICollection<PartnerResourceTagPoco> PartnerResourceTags
        {
            get
            {
                if (_partnerResourceTags == null)
                {
                    var newCollection = new FixupCollection<PartnerResourceTagPoco>();
                    newCollection.CollectionChanged += FixupPartnerResourceTags;
                    _partnerResourceTags = newCollection;
                }
                return _partnerResourceTags;
            }
            set
            {
                if (!ReferenceEquals(_partnerResourceTags, value))
                {
                    var previousValue = _partnerResourceTags as FixupCollection<PartnerResourceTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPartnerResourceTags;
                    }
                    _partnerResourceTags = value;
                    var newValue = value as FixupCollection<PartnerResourceTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPartnerResourceTags;
                    }
                }
            }
        }
        private ICollection<PartnerResourceTagPoco> _partnerResourceTags;
    
        // CompanyTag
        public ICollection<CompanyTagPoco> CompanyTags
        {
            get
            {
                if (_companyTags == null)
                {
                    var newCollection = new FixupCollection<CompanyTagPoco>();
                    newCollection.CollectionChanged += FixupCompanyTags;
                    _companyTags = newCollection;
                }
                return _companyTags;
            }
            set
            {
                if (!ReferenceEquals(_companyTags, value))
                {
                    var previousValue = _companyTags as FixupCollection<CompanyTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCompanyTags;
                    }
                    _companyTags = value;
                    var newValue = value as FixupCollection<CompanyTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCompanyTags;
                    }
                }
            }
        }
        private ICollection<CompanyTagPoco> _companyTags;
    
        // UserTag
        public ICollection<UserTagPoco> UserTags
        {
            get
            {
                if (_userTags == null)
                {
                    var newCollection = new FixupCollection<UserTagPoco>();
                    newCollection.CollectionChanged += FixupUserTags;
                    _userTags = newCollection;
                }
                return _userTags;
            }
            set
            {
                if (!ReferenceEquals(_userTags, value))
                {
                    var previousValue = _userTags as FixupCollection<UserTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUserTags;
                    }
                    _userTags = value;
                    var newValue = value as FixupCollection<UserTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUserTags;
                    }
                }
            }
        }
        private ICollection<UserTagPoco> _userTags;
    
        // GroupTag
        public ICollection<GroupTagPoco> GroupTags
        {
            get
            {
                if (_groupTags == null)
                {
                    var newCollection = new FixupCollection<GroupTagPoco>();
                    newCollection.CollectionChanged += FixupGroupTags;
                    _groupTags = newCollection;
                }
                return _groupTags;
            }
            set
            {
                if (!ReferenceEquals(_groupTags, value))
                {
                    var previousValue = _groupTags as FixupCollection<GroupTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGroupTags;
                    }
                    _groupTags = value;
                    var newValue = value as FixupCollection<GroupTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGroupTags;
                    }
                }
            }
        }
        private ICollection<GroupTagPoco> _groupTags;
    
        // TimelineItemTag
        public ICollection<TimelineItemTagPoco> TimelineItemTags
        {
            get
            {
                if (_timelineItemTags == null)
                {
                    var newCollection = new FixupCollection<TimelineItemTagPoco>();
                    newCollection.CollectionChanged += FixupTimelineItemTags;
                    _timelineItemTags = newCollection;
                }
                return _timelineItemTags;
            }
            set
            {
                if (!ReferenceEquals(_timelineItemTags, value))
                {
                    var previousValue = _timelineItemTags as FixupCollection<TimelineItemTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTimelineItemTags;
                    }
                    _timelineItemTags = value;
                    var newValue = value as FixupCollection<TimelineItemTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTimelineItemTags;
                    }
                }
            }
        }
        private ICollection<TimelineItemTagPoco> _timelineItemTags;
    
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

        #endregion

        #region Association Fixup
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.TagDefinitions.Contains(this))
            {
                previousValue.TagDefinitions.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.TagDefinitions.Contains(this))
                {
                    Network.TagDefinitions.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupCategory(TagCategoryPoco previousValue)
        {
            if (previousValue != null && previousValue.TagDefinitions.Contains(this))
            {
                previousValue.TagDefinitions.Remove(this);
            }
    
            if (Category != null)
            {
                if (!Category.TagDefinitions.Contains(this))
                {
                    Category.TagDefinitions.Add(this);
                }
                if (CategoryId != Category.Id)
                {
                    CategoryId = Category.Id;
                }
            }
        }
    
        private void FixupCreatedByUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.CreatedTagDefinitions.Contains(this))
            {
                previousValue.CreatedTagDefinitions.Remove(this);
            }
    
            if (CreatedByUser != null)
            {
                if (!CreatedByUser.CreatedTagDefinitions.Contains(this))
                {
                    CreatedByUser.CreatedTagDefinitions.Add(this);
                }
                if (CreatedByUserId != CreatedByUser.Id)
                {
                    CreatedByUserId = CreatedByUser.Id;
                }
            }
        }
    
        private void FixupPartnerResourceTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PartnerResourceTagPoco item in e.NewItems)
                {
                    item.TagDefinition = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PartnerResourceTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TagDefinition, this))
                    {
                        item.TagDefinition = null;
                    }
                }
            }
        }
    
        private void FixupCompanyTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CompanyTagPoco item in e.NewItems)
                {
                    item.TagDefinition = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CompanyTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TagDefinition, this))
                    {
                        item.TagDefinition = null;
                    }
                }
            }
        }
    
        private void FixupUserTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (UserTagPoco item in e.NewItems)
                {
                    item.TagDefinition = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TagDefinition, this))
                    {
                        item.TagDefinition = null;
                    }
                }
            }
        }
    
        private void FixupGroupTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupTagPoco item in e.NewItems)
                {
                    item.TagDefinition = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TagDefinition, this))
                    {
                        item.TagDefinition = null;
                    }
                }
            }
        }
    
        private void FixupTimelineItemTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemTagPoco item in e.NewItems)
                {
                    item.TagDefinition = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TagDefinition, this))
                    {
                        item.TagDefinition = null;
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
                    item.TagDefinition = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AdTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TagDefinition, this))
                    {
                        item.TagDefinition = null;
                    }
                }
            }
        }

        #endregion

    }
}
