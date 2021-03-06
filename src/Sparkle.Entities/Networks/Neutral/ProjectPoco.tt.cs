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
    public partial class ProjectPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public Nullable<bool> IsPrivate
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
    
        public int OwnerType
        {
            get;
            set;
        }
    
        public int OwnerValue
        {
            get;
            set;
        }
    
        public int CreatedByUserId
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        // ProjectMembers
        public ICollection<ProjectMemberPoco> ProjectMembers
        {
            get
            {
                if (_projectMembers == null)
                {
                    var newCollection = new FixupCollection<ProjectMemberPoco>();
                    newCollection.CollectionChanged += FixupProjectMembers;
                    _projectMembers = newCollection;
                }
                return _projectMembers;
            }
            set
            {
                if (!ReferenceEquals(_projectMembers, value))
                {
                    var previousValue = _projectMembers as FixupCollection<ProjectMemberPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupProjectMembers;
                    }
                    _projectMembers = value;
                    var newValue = value as FixupCollection<ProjectMemberPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupProjectMembers;
                    }
                }
            }
        }
        private ICollection<ProjectMemberPoco> _projectMembers;
    
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

        #endregion

        #region Association Fixup
    
        private void FixupProjectMembers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProjectMemberPoco item in e.NewItems)
                {
                    item.Project = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ProjectMemberPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Project, this))
                    {
                        item.Project = null;
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
                    item.Project = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Project, this))
                    {
                        item.Project = null;
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
                    item.Project = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EventPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Project, this))
                    {
                        item.Project = null;
                    }
                }
            }
        }

        #endregion

    }
}
