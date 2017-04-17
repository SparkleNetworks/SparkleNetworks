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
    public partial class TimelineItemPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int ItemType
        {
            get;
            set;
        }
    
        public string Text
        {
            get;
            set;
        }
    
        public System.DateTime CreateDate
        {
            get;
            set;
        }
    
        public int PrivateMode
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
    
        public Nullable<int> EventId
        {
            get { return _eventId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._eventId != value)
                    {
                        if (this.Event != null && this.Event.Id != value)
                        {
                            this.Event = null;
                        }
                        this._eventId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _eventId;
    
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
    
        public Nullable<int> PlaceId
        {
            get;
            set;
        }
    
        public Nullable<int> AdId
        {
            get { return _adId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._adId != value)
                    {
                        if (this.Ad != null && this.Ad.Id != value)
                        {
                            this.Ad = null;
                        }
                        this._adId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _adId;
    
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
    
        public string Extra
        {
            get;
            set;
        }
    
        public Nullable<int> ExtraType
        {
            get;
            set;
        }
    
        public int PostedByUserId
        {
            get { return _postedByUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._postedByUserId != value)
                    {
                        if (this.PostedBy != null && this.PostedBy.Id != value)
                        {
                            this.PostedBy = null;
                        }
                        this._postedByUserId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _postedByUserId;
    
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
    
        public string ImportedId
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
            get { return _deletedByUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._deletedByUserId != value)
                    {
                        if (this.DeletedBy != null && this.DeletedBy.Id != value)
                        {
                            this.DeletedBy = null;
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
    
        public Nullable<System.DateTime> DeleteDateUtc
        {
            get;
            set;
        }
    
        public Nullable<int> InboundEmailId
        {
            get { return _inboundEmailId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._inboundEmailId != value)
                    {
                        if (this.InboundEmailMessage != null && this.InboundEmailMessage.Id != value)
                        {
                            this.InboundEmailMessage = null;
                        }
                        this._inboundEmailId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _inboundEmailId;
    
        public Nullable<int> PartnerResourceId
        {
            get { return _partnerResourceId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._partnerResourceId != value)
                    {
                        if (this.PartnerResource != null && this.PartnerResource.Id != value)
                        {
                            this.PartnerResource = null;
                        }
                        this._partnerResourceId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _partnerResourceId;

        #endregion

        #region Navigation Properties
    
        public virtual AdPoco Ad
        {
            get { return _ad; }
            set
            {
                if (!ReferenceEquals(_ad, value))
                {
                    var previousValue = _ad;
                    _ad = value;
                    FixupAd(previousValue);
                }
            }
        }
        private AdPoco _ad;
    
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
    
        public virtual EventPoco Event
        {
            get { return _event; }
            set
            {
                if (!ReferenceEquals(_event, value))
                {
                    var previousValue = _event;
                    _event = value;
                    FixupEvent(previousValue);
                }
            }
        }
        private EventPoco _event;
    
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
    
        public ICollection<TimelineItemCommentPoco> Comments
        {
            get
            {
                if (_comments == null)
                {
                    var newCollection = new FixupCollection<TimelineItemCommentPoco>();
                    newCollection.CollectionChanged += FixupComments;
                    _comments = newCollection;
                }
                return _comments;
            }
            set
            {
                if (!ReferenceEquals(_comments, value))
                {
                    var previousValue = _comments as FixupCollection<TimelineItemCommentPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupComments;
                    }
                    _comments = value;
                    var newValue = value as FixupCollection<TimelineItemCommentPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupComments;
                    }
                }
            }
        }
        private ICollection<TimelineItemCommentPoco> _comments;
    
        public virtual UserPoco PostedBy
        {
            get { return _postedBy; }
            set
            {
                if (!ReferenceEquals(_postedBy, value))
                {
                    var previousValue = _postedBy;
                    _postedBy = value;
                    FixupPostedBy(previousValue);
                }
            }
        }
        private UserPoco _postedBy;
    
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
    
        public ICollection<TimelineItemSkillPoco> Skills
        {
            get
            {
                if (_skills == null)
                {
                    var newCollection = new FixupCollection<TimelineItemSkillPoco>();
                    newCollection.CollectionChanged += FixupSkills;
                    _skills = newCollection;
                }
                return _skills;
            }
            set
            {
                if (!ReferenceEquals(_skills, value))
                {
                    var previousValue = _skills as FixupCollection<TimelineItemSkillPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupSkills;
                    }
                    _skills = value;
                    var newValue = value as FixupCollection<TimelineItemSkillPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupSkills;
                    }
                }
            }
        }
        private ICollection<TimelineItemSkillPoco> _skills;
    
        public virtual UserPoco DeletedBy
        {
            get { return _deletedBy; }
            set
            {
                if (!ReferenceEquals(_deletedBy, value))
                {
                    var previousValue = _deletedBy;
                    _deletedBy = value;
                    FixupDeletedBy(previousValue);
                }
            }
        }
        private UserPoco _deletedBy;
    
        public virtual InboundEmailMessagePoco InboundEmailMessage
        {
            get { return _inboundEmailMessage; }
            set
            {
                if (!ReferenceEquals(_inboundEmailMessage, value))
                {
                    var previousValue = _inboundEmailMessage;
                    _inboundEmailMessage = value;
                    FixupInboundEmailMessage(previousValue);
                }
            }
        }
        private InboundEmailMessagePoco _inboundEmailMessage;
    
        public ICollection<TimelineItemLikePoco> Likes
        {
            get
            {
                if (_likes == null)
                {
                    var newCollection = new FixupCollection<TimelineItemLikePoco>();
                    newCollection.CollectionChanged += FixupLikes;
                    _likes = newCollection;
                }
                return _likes;
            }
            set
            {
                if (!ReferenceEquals(_likes, value))
                {
                    var previousValue = _likes as FixupCollection<TimelineItemLikePoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupLikes;
                    }
                    _likes = value;
                    var newValue = value as FixupCollection<TimelineItemLikePoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupLikes;
                    }
                }
            }
        }
        private ICollection<TimelineItemLikePoco> _likes;
    
        public virtual PartnerResourcePoco PartnerResource
        {
            get { return _partnerResource; }
            set
            {
                if (!ReferenceEquals(_partnerResource, value))
                {
                    var previousValue = _partnerResource;
                    _partnerResource = value;
                    FixupPartnerResource(previousValue);
                }
            }
        }
        private PartnerResourcePoco _partnerResource;
    
        public ICollection<TimelineItemTagPoco> Tags
        {
            get
            {
                if (_tags == null)
                {
                    var newCollection = new FixupCollection<TimelineItemTagPoco>();
                    newCollection.CollectionChanged += FixupTags;
                    _tags = newCollection;
                }
                return _tags;
            }
            set
            {
                if (!ReferenceEquals(_tags, value))
                {
                    var previousValue = _tags as FixupCollection<TimelineItemTagPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTags;
                    }
                    _tags = value;
                    var newValue = value as FixupCollection<TimelineItemTagPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTags;
                    }
                }
            }
        }
        private ICollection<TimelineItemTagPoco> _tags;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupAd(AdPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (Ad != null)
            {
                if (!Ad.TimelineItems.Contains(this))
                {
                    Ad.TimelineItems.Add(this);
                }
                if (AdId != Ad.Id)
                {
                    AdId = Ad.Id;
                }
            }
            else if (!_settingFK)
            {
                AdId = null;
            }
        }
    
        private void FixupCompany(CompanyPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.TimelineItems.Contains(this))
                {
                    Company.TimelineItems.Add(this);
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
    
        private void FixupEvent(EventPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (Event != null)
            {
                if (!Event.TimelineItems.Contains(this))
                {
                    Event.TimelineItems.Add(this);
                }
                if (EventId != Event.Id)
                {
                    EventId = Event.Id;
                }
            }
            else if (!_settingFK)
            {
                EventId = null;
            }
        }
    
        private void FixupGroup(GroupPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (Group != null)
            {
                if (!Group.TimelineItems.Contains(this))
                {
                    Group.TimelineItems.Add(this);
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
    
        private void FixupProject(ProjectPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (Project != null)
            {
                if (!Project.TimelineItems.Contains(this))
                {
                    Project.TimelineItems.Add(this);
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
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (Team != null)
            {
                if (!Team.TimelineItems.Contains(this))
                {
                    Team.TimelineItems.Add(this);
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
    
        private void FixupPostedBy(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (PostedBy != null)
            {
                if (!PostedBy.TimelineItems.Contains(this))
                {
                    PostedBy.TimelineItems.Add(this);
                }
                if (PostedByUserId != PostedBy.Id)
                {
                    PostedByUserId = PostedBy.Id;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.MyTimelineItems.Contains(this))
            {
                previousValue.MyTimelineItems.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.MyTimelineItems.Contains(this))
                {
                    User.MyTimelineItems.Add(this);
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
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.TimelineItems.Contains(this))
                {
                    Network.TimelineItems.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupDeletedBy(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.DeletedTimelineItems.Contains(this))
            {
                previousValue.DeletedTimelineItems.Remove(this);
            }
    
            if (DeletedBy != null)
            {
                if (!DeletedBy.DeletedTimelineItems.Contains(this))
                {
                    DeletedBy.DeletedTimelineItems.Add(this);
                }
                if (DeletedByUserId != DeletedBy.Id)
                {
                    DeletedByUserId = DeletedBy.Id;
                }
            }
            else if (!_settingFK)
            {
                DeletedByUserId = null;
            }
        }
    
        private void FixupInboundEmailMessage(InboundEmailMessagePoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (InboundEmailMessage != null)
            {
                if (!InboundEmailMessage.TimelineItems.Contains(this))
                {
                    InboundEmailMessage.TimelineItems.Add(this);
                }
                if (InboundEmailId != InboundEmailMessage.Id)
                {
                    InboundEmailId = InboundEmailMessage.Id;
                }
            }
            else if (!_settingFK)
            {
                InboundEmailId = null;
            }
        }
    
        private void FixupPartnerResource(PartnerResourcePoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (PartnerResource != null)
            {
                if (!PartnerResource.TimelineItems.Contains(this))
                {
                    PartnerResource.TimelineItems.Add(this);
                }
                if (PartnerResourceId != PartnerResource.Id)
                {
                    PartnerResourceId = PartnerResource.Id;
                }
            }
            else if (!_settingFK)
            {
                PartnerResourceId = null;
            }
        }
    
        private void FixupComments(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemCommentPoco item in e.NewItems)
                {
                    item.TimelineItem = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemCommentPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TimelineItem, this))
                    {
                        item.TimelineItem = null;
                    }
                }
            }
        }
    
        private void FixupSkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemSkillPoco item in e.NewItems)
                {
                    item.TimelineItem = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemSkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TimelineItem, this))
                    {
                        item.TimelineItem = null;
                    }
                }
            }
        }
    
        private void FixupLikes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemLikePoco item in e.NewItems)
                {
                    item.TimelineItem = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemLikePoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TimelineItem, this))
                    {
                        item.TimelineItem = null;
                    }
                }
            }
        }
    
        private void FixupTags(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemTagPoco item in e.NewItems)
                {
                    item.TimelineItem = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemTagPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TimelineItem, this))
                    {
                        item.TimelineItem = null;
                    }
                }
            }
        }

        #endregion

    }
}
