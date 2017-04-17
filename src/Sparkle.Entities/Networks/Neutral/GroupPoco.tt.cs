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
    public partial class GroupPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int GroupCategoryId
        {
            get { return _groupCategoryId; }
            set
            {
                if (this._groupCategoryId != value)
                {
                    if (this.GroupCategory != null && this.GroupCategory.Id != value)
                    {
                        this.GroupCategory = null;
                    }
                    this._groupCategoryId = value;
                }
            }
        }
        private int _groupCategoryId;
    
        public bool IsPrivate
        {
            get;
            set;
        }
    
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
    
        public System.DateTime Date
        {
            get;
            set;
        }
    
        public byte NotificationFrequency
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
                    if (this.User != null && this.User.Id != value)
                    {
                        this.User = null;
                    }
                    this._createdByUserId = value;
                }
            }
        }
        private int _createdByUserId;
    
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
    
        public string ImportedId
        {
            get;
            set;
        }
    
        public bool IsDeleted
        {
            get;
            set;
        }
    
        public string Alias
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual GroupCategoryPoco GroupCategory
        {
            get { return _groupCategory; }
            set
            {
                if (!ReferenceEquals(_groupCategory, value))
                {
                    var previousValue = _groupCategory;
                    _groupCategory = value;
                    FixupGroupCategory(previousValue);
                }
            }
        }
        private GroupCategoryPoco _groupCategory;
    
        // GroupMembers
        public ICollection<GroupMemberPoco> Members
        {
            get
            {
                if (_members == null)
                {
                    var newCollection = new FixupCollection<GroupMemberPoco>();
                    newCollection.CollectionChanged += FixupMembers;
                    _members = newCollection;
                }
                return _members;
            }
            set
            {
                if (!ReferenceEquals(_members, value))
                {
                    var previousValue = _members as FixupCollection<GroupMemberPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupMembers;
                    }
                    _members = value;
                    var newValue = value as FixupCollection<GroupMemberPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupMembers;
                    }
                }
            }
        }
        private ICollection<GroupMemberPoco> _members;
    
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
    
        // Event
        public ICollection<EventPoco> Events
        {
            get
            {
                if (_events == null)
                {
                    var newCollection = new FixupCollection<EventPoco>();
                    newCollection.CollectionChanged += FixupEvents;
                    _events = newCollection;
                }
                return _events;
            }
            set
            {
                if (!ReferenceEquals(_events, value))
                {
                    var previousValue = _events as FixupCollection<EventPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupEvents;
                    }
                    _events = value;
                    var newValue = value as FixupCollection<EventPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupEvents;
                    }
                }
            }
        }
        private ICollection<EventPoco> _events;
    
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
    
        // GroupSkill
        public ICollection<GroupSkillPoco> Skills
        {
            get
            {
                if (_skills == null)
                {
                    var newCollection = new FixupCollection<GroupSkillPoco>();
                    newCollection.CollectionChanged += FixupSkills;
                    _skills = newCollection;
                }
                return _skills;
            }
            set
            {
                if (!ReferenceEquals(_skills, value))
                {
                    var previousValue = _skills as FixupCollection<GroupSkillPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSkills;
                    }
                    _skills = value;
                    var newValue = value as FixupCollection<GroupSkillPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSkills;
                    }
                }
            }
        }
        private ICollection<GroupSkillPoco> _skills;
    
        // GroupInterest
        public ICollection<GroupInterestPoco> Interests
        {
            get
            {
                if (_interests == null)
                {
                    var newCollection = new FixupCollection<GroupInterestPoco>();
                    newCollection.CollectionChanged += FixupInterests;
                    _interests = newCollection;
                }
                return _interests;
            }
            set
            {
                if (!ReferenceEquals(_interests, value))
                {
                    var previousValue = _interests as FixupCollection<GroupInterestPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupInterests;
                    }
                    _interests = value;
                    var newValue = value as FixupCollection<GroupInterestPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupInterests;
                    }
                }
            }
        }
        private ICollection<GroupInterestPoco> _interests;
    
        // GroupRecreation
        public ICollection<GroupRecreationPoco> Recreations
        {
            get
            {
                if (_recreations == null)
                {
                    var newCollection = new FixupCollection<GroupRecreationPoco>();
                    newCollection.CollectionChanged += FixupRecreations;
                    _recreations = newCollection;
                }
                return _recreations;
            }
            set
            {
                if (!ReferenceEquals(_recreations, value))
                {
                    var previousValue = _recreations as FixupCollection<GroupRecreationPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupRecreations;
                    }
                    _recreations = value;
                    var newValue = value as FixupCollection<GroupRecreationPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupRecreations;
                    }
                }
            }
        }
        private ICollection<GroupRecreationPoco> _recreations;
    
        // GroupTag
        public ICollection<GroupTagPoco> Tags
        {
            get
            {
                if (_tags == null)
                {
                    var newCollection = new FixupCollection<GroupTagPoco>();
                    newCollection.CollectionChanged += FixupTags;
                    _tags = newCollection;
                }
                return _tags;
            }
            set
            {
                if (!ReferenceEquals(_tags, value))
                {
                    var previousValue = _tags as FixupCollection<GroupTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTags;
                    }
                    _tags = value;
                    var newValue = value as FixupCollection<GroupTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTags;
                    }
                }
            }
        }
        private ICollection<GroupTagPoco> _tags;

        #endregion

        #region Association Fixup
    
        private void FixupGroupCategory(GroupCategoryPoco previousValue)
        {
            if (previousValue != null && previousValue.Group.Contains(this))
            {
                previousValue.Group.Remove(this);
            }
    
            if (GroupCategory != null)
            {
                if (!GroupCategory.Group.Contains(this))
                {
                    GroupCategory.Group.Add(this);
                }
                if (GroupCategoryId != GroupCategory.Id)
                {
                    GroupCategoryId = GroupCategory.Id;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.Groups.Contains(this))
            {
                previousValue.Groups.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.Groups.Contains(this))
                {
                    User.Groups.Add(this);
                }
                if (CreatedByUserId != User.Id)
                {
                    CreatedByUserId = User.Id;
                }
            }
        }
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.Groups.Contains(this))
            {
                previousValue.Groups.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.Groups.Contains(this))
                {
                    Network.Groups.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupMembers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupMemberPoco item in e.NewItems)
                {
                    item.Group = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupMemberPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Group, this))
                    {
                        item.Group = null;
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
                    item.Group = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Group, this))
                    {
                        item.Group = null;
                    }
                }
            }
        }
    
        private void FixupEvents(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EventPoco item in e.NewItems)
                {
                    item.Group = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EventPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Group, this))
                    {
                        item.Group = null;
                    }
                }
            }
        }
    
        private void FixupSkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupSkillPoco item in e.NewItems)
                {
                    item.Group = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupSkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Group, this))
                    {
                        item.Group = null;
                    }
                }
            }
        }
    
        private void FixupInterests(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupInterestPoco item in e.NewItems)
                {
                    item.Group = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupInterestPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Group, this))
                    {
                        item.Group = null;
                    }
                }
            }
        }
    
        private void FixupRecreations(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupRecreationPoco item in e.NewItems)
                {
                    item.Group = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupRecreationPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Group, this))
                    {
                        item.Group = null;
                    }
                }
            }
        }
    
        private void FixupTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupTagPoco item in e.NewItems)
                {
                    item.Group = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Group, this))
                    {
                        item.Group = null;
                    }
                }
            }
        }

        #endregion

    }
}