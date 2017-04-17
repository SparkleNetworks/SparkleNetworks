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
    public partial class TimelineItemCommentPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int TimelineItemId
        {
            get { return _timelineItemId; }
            set
            {
                if (this._timelineItemId != value)
                {
                    if (this.TimelineItem != null && this.TimelineItem.Id != value)
                    {
                        this.TimelineItem = null;
                    }
                    this._timelineItemId = value;
                }
            }
        }
        private int _timelineItemId;
    
        public System.DateTime CreateDate
        {
            get;
            set;
        }
    
        public string Text
        {
            get;
            set;
        }
    
        public int PostedByUserId
        {
            get { return _postedByUserId; }
            set
            {
                if (this._postedByUserId != value)
                {
                    if (this.PostedBy != null && this.PostedBy.Id != value)
                    {
                        this.PostedBy = null;
                    }
                    this._postedByUserId = value;
                }
            }
        }
        private int _postedByUserId;
    
        public string ImportedId
        {
            get;
            set;
        }
    
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
    
        public Nullable<int> InboundEmailId
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual TimelineItemPoco TimelineItem
        {
            get { return _timelineItem; }
            set
            {
                if (!ReferenceEquals(_timelineItem, value))
                {
                    var previousValue = _timelineItem;
                    _timelineItem = value;
                    FixupTimelineItem(previousValue);
                }
            }
        }
        private TimelineItemPoco _timelineItem;
    
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
    
        public ICollection<TimelineItemCommentLikePoco> Likes
        {
            get
            {
                if (_likes == null)
                {
                    var newCollection = new FixupCollection<TimelineItemCommentLikePoco>();
                    newCollection.CollectionChanged += FixupLikes;
                    _likes = newCollection;
                }
                return _likes;
            }
            set
            {
                if (!ReferenceEquals(_likes, value))
                {
                    var previousValue = _likes as FixupCollection<TimelineItemCommentLikePoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupLikes;
                    }
                    _likes = value;
                    var newValue = value as FixupCollection<TimelineItemCommentLikePoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupLikes;
                    }
                }
            }
        }
        private ICollection<TimelineItemCommentLikePoco> _likes;

        #endregion

        #region Association Fixup
    
        private void FixupTimelineItem(TimelineItemPoco previousValue)
        {
            if (previousValue != null && previousValue.Comments.Contains(this))
            {
                previousValue.Comments.Remove(this);
            }
    
            if (TimelineItem != null)
            {
                if (!TimelineItem.Comments.Contains(this))
                {
                    TimelineItem.Comments.Add(this);
                }
                if (TimelineItemId != TimelineItem.Id)
                {
                    TimelineItemId = TimelineItem.Id;
                }
            }
        }
    
        private void FixupPostedBy(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.PostedTimelineItemComments.Contains(this))
            {
                previousValue.PostedTimelineItemComments.Remove(this);
            }
    
            if (PostedBy != null)
            {
                if (!PostedBy.PostedTimelineItemComments.Contains(this))
                {
                    PostedBy.PostedTimelineItemComments.Add(this);
                }
                if (PostedByUserId != PostedBy.Id)
                {
                    PostedByUserId = PostedBy.Id;
                }
            }
        }
    
        private void FixupLikes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemCommentLikePoco item in e.NewItems)
                {
                    item.TimelineItemComment = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemCommentLikePoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.TimelineItemComment, this))
                    {
                        item.TimelineItemComment = null;
                    }
                }
            }
        }

        #endregion

    }
}