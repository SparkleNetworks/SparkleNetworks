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
    public partial class EventPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int EventCategoryId
        {
            get { return _eventCategoryId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._eventCategoryId != value)
                    {
                        if (this.EventCategory != null && this.EventCategory.Id != value)
                        {
                            this.EventCategory = null;
                        }
                        this._eventCategoryId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _eventCategoryId;
    
        public int Visibility
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
    
        public string Picture
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> CreateDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateEvent
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateEndEvent
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> NeedAnswerBefore
        {
            get;
            set;
        }
    
        public string Website
        {
            get;
            set;
        }
    
        public Nullable<int> PlaceId
        {
            get { return _placeId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._placeId != value)
                    {
                        if (this.Place != null && this.Place.Id != value)
                        {
                            this.Place = null;
                        }
                        this._placeId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _placeId;
    
        public Nullable<int> RecurrenceId
        {
            get;
            set;
        }
    
        public string Room
        {
            get;
            set;
        }
    
        public Nullable<int> CompanyId
        {
            get { return _companyId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._companyId != value)
                    {
                        if (this.Company != null && this.Company.ID != value)
                        {
                            this.Company = null;
                        }
                        this._companyId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _companyId;
    
        public Nullable<int> GroupId
        {
            get { return _groupId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._groupId != value)
                    {
                        if (this.Group != null && this.Group.Id != value)
                        {
                            this.Group = null;
                        }
                        this._groupId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _groupId;
    
        public Nullable<int> TeamId
        {
            get { return _teamId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._teamId != value)
                    {
                        if (this.Team != null && this.Team.Id != value)
                        {
                            this.Team = null;
                        }
                        this._teamId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _teamId;
    
        public Nullable<int> ProjectId
        {
            get { return _projectId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._projectId != value)
                    {
                        if (this.Project != null && this.Project.Id != value)
                        {
                            this.Project = null;
                        }
                        this._projectId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _projectId;
    
        public string Report
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> ReportDate
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
                        if (this.User != null && this.User.Id != value)
                        {
                            this.User = null;
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
    
        public string TicketsWebsite
        {
            get;
            set;
        }
    
        public Nullable<byte> DeleteReason
        {
            get;
            set;
        }
    
        public Nullable<int> DeletedByUserId
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DeleteDateUtc
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual EventCategoryPoco EventCategory
        {
            get { return _eventCategory; }
            set
            {
                if (!ReferenceEquals(_eventCategory, value))
                {
                    var previousValue = _eventCategory;
                    _eventCategory = value;
                    FixupEventCategory(previousValue);
                }
            }
        }
        private EventCategoryPoco _eventCategory;
    
        // EventMembers
        public ICollection<EventMemberPoco> EventMembers
        {
            get
            {
                if (_eventMembers == null)
                {
                    var newCollection = new FixupCollection<EventMemberPoco>();
                    newCollection.CollectionChanged += FixupEventMembers;
                    _eventMembers = newCollection;
                }
                return _eventMembers;
            }
            set
            {
                if (!ReferenceEquals(_eventMembers, value))
                {
                    var previousValue = _eventMembers as FixupCollection<EventMemberPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupEventMembers;
                    }
                    _eventMembers = value;
                    var newValue = value as FixupCollection<EventMemberPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupEventMembers;
                    }
                }
            }
        }
        private ICollection<EventMemberPoco> _eventMembers;
    
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
    
        public virtual CompanyPoco Company
        {
            get { return _company; }
            set
            {
                if (!ReferenceEquals(_company, value))
                {
                    var previousValue = _company;
                    _company = value;
                    FixupCompany(previousValue);
                }
            }
        }
        private CompanyPoco _company;
    
        public virtual GroupPoco Group
        {
            get { return _group; }
            set
            {
                if (!ReferenceEquals(_group, value))
                {
                    var previousValue = _group;
                    _group = value;
                    FixupGroup(previousValue);
                }
            }
        }
        private GroupPoco _group;
    
        public virtual PlacePoco Place
        {
            get { return _place; }
            set
            {
                if (!ReferenceEquals(_place, value))
                {
                    var previousValue = _place;
                    _place = value;
                    FixupPlace(previousValue);
                }
            }
        }
        private PlacePoco _place;
    
        public virtual ProjectPoco Project
        {
            get { return _project; }
            set
            {
                if (!ReferenceEquals(_project, value))
                {
                    var previousValue = _project;
                    _project = value;
                    FixupProject(previousValue);
                }
            }
        }
        private ProjectPoco _project;
    
        public virtual TeamPoco Team
        {
            get { return _team; }
            set
            {
                if (!ReferenceEquals(_team, value))
                {
                    var previousValue = _team;
                    _team = value;
                    FixupTeam(previousValue);
                }
            }
        }
        private TeamPoco _team;
    
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
    
        // EventPublicMember
        public ICollection<EventPublicMemberPoco> PublicMembers
        {
            get
            {
                if (_publicMembers == null)
                {
                    var newCollection = new FixupCollection<EventPublicMemberPoco>();
                    newCollection.CollectionChanged += FixupPublicMembers;
                    _publicMembers = newCollection;
                }
                return _publicMembers;
            }
            set
            {
                if (!ReferenceEquals(_publicMembers, value))
                {
                    var previousValue = _publicMembers as FixupCollection<EventPublicMemberPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPublicMembers;
                    }
                    _publicMembers = value;
                    var newValue = value as FixupCollection<EventPublicMemberPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPublicMembers;
                    }
                }
            }
        }
        private ICollection<EventPublicMemberPoco> _publicMembers;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupEventCategory(EventCategoryPoco previousValue)
        {
            if (previousValue != null && previousValue.Events.Contains(this))
            {
                previousValue.Events.Remove(this);
            }
    
            if (EventCategory != null)
            {
                if (!EventCategory.Events.Contains(this))
                {
                    EventCategory.Events.Add(this);
                }
                if (EventCategoryId != EventCategory.Id)
                {
                    EventCategoryId = EventCategory.Id;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.Events.Contains(this))
            {
                previousValue.Events.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.Events.Contains(this))
                {
                    User.Events.Add(this);
                }
                if (CreatedByUserId != User.Id)
                {
                    CreatedByUserId = User.Id;
                }
            }
        }
    
        private void FixupCompany(CompanyPoco previousValue)
        {
            if (previousValue != null && previousValue.Events.Contains(this))
            {
                previousValue.Events.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.Events.Contains(this))
                {
                    Company.Events.Add(this);
                }
                if (CompanyId != Company.ID)
                {
                    CompanyId = Company.ID;
                }
            }
            else if (!_settingFK)
            {
                CompanyId = null;
            }
        }
    
        private void FixupGroup(GroupPoco previousValue)
        {
            if (previousValue != null && previousValue.Events.Contains(this))
            {
                previousValue.Events.Remove(this);
            }
    
            if (Group != null)
            {
                if (!Group.Events.Contains(this))
                {
                    Group.Events.Add(this);
                }
                if (GroupId != Group.Id)
                {
                    GroupId = Group.Id;
                }
            }
            else if (!_settingFK)
            {
                GroupId = null;
            }
        }
    
        private void FixupPlace(PlacePoco previousValue)
        {
            if (previousValue != null && previousValue.Events.Contains(this))
            {
                previousValue.Events.Remove(this);
            }
    
            if (Place != null)
            {
                if (!Place.Events.Contains(this))
                {
                    Place.Events.Add(this);
                }
                if (PlaceId != Place.Id)
                {
                    PlaceId = Place.Id;
                }
            }
            else if (!_settingFK)
            {
                PlaceId = null;
            }
        }
    
        private void FixupProject(ProjectPoco previousValue)
        {
            if (previousValue != null && previousValue.Events.Contains(this))
            {
                previousValue.Events.Remove(this);
            }
    
            if (Project != null)
            {
                if (!Project.Events.Contains(this))
                {
                    Project.Events.Add(this);
                }
                if (ProjectId != Project.Id)
                {
                    ProjectId = Project.Id;
                }
            }
            else if (!_settingFK)
            {
                ProjectId = null;
            }
        }
    
        private void FixupTeam(TeamPoco previousValue)
        {
            if (previousValue != null && previousValue.Events.Contains(this))
            {
                previousValue.Events.Remove(this);
            }
    
            if (Team != null)
            {
                if (!Team.Events.Contains(this))
                {
                    Team.Events.Add(this);
                }
                if (TeamId != Team.Id)
                {
                    TeamId = Team.Id;
                }
            }
            else if (!_settingFK)
            {
                TeamId = null;
            }
        }
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.Events.Contains(this))
            {
                previousValue.Events.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.Events.Contains(this))
                {
                    Network.Events.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupEventMembers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EventMemberPoco item in e.NewItems)
                {
                    item.Event = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EventMemberPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Event, this))
                    {
                        item.Event = null;
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
                    item.Event = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Event, this))
                    {
                        item.Event = null;
                    }
                }
            }
        }
    
        private void FixupPublicMembers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EventPublicMemberPoco item in e.NewItems)
                {
                    item.Event = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EventPublicMemberPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Event, this))
                    {
                        item.Event = null;
                    }
                }
            }
        }

        #endregion

    }
}
