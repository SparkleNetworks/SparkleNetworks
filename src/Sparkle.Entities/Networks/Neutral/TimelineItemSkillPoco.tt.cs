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
    public partial class TimelineItemSkillPoco
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
                try
                {
                    _settingFK = true;
                    if (this._timelineItemId != value)
                    {
                        if (this.TimelineItem != null && this.TimelineItem.Id != value)
                        {
                            this.TimelineItem = null;
                        }
                        this._timelineItemId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _timelineItemId;
    
        public int SkillId
        {
            get { return _skillId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._skillId != value)
                    {
                        if (this.Skill != null && this.Skill.Id != value)
                        {
                            this.Skill = null;
                        }
                        this._skillId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _skillId;
    
        public System.DateTime DateUtc
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
                        if (this.CreatedBy != null && this.CreatedBy.Id != value)
                        {
                            this.CreatedBy = null;
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
    
        public Nullable<System.DateTime> DeletedDateUtc
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
    
        public Nullable<byte> DeleteReason
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual SkillPoco Skill
        {
            get { return _skill; }
            set
            {
                if (!ReferenceEquals(_skill, value))
                {
                    var previousValue = _skill;
                    _skill = value;
                    FixupSkill(previousValue);
                }
            }
        }
        private SkillPoco _skill;
    
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
    
        public virtual UserPoco CreatedBy
        {
            get { return _createdBy; }
            set
            {
                if (!ReferenceEquals(_createdBy, value))
                {
                    var previousValue = _createdBy;
                    _createdBy = value;
                    FixupCreatedBy(previousValue);
                }
            }
        }
        private UserPoco _createdBy;
    
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

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupSkill(SkillPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItems.Contains(this))
            {
                previousValue.TimelineItems.Remove(this);
            }
    
            if (Skill != null)
            {
                if (!Skill.TimelineItems.Contains(this))
                {
                    Skill.TimelineItems.Add(this);
                }
                if (SkillId != Skill.Id)
                {
                    SkillId = Skill.Id;
                }
            }
        }
    
        private void FixupTimelineItem(TimelineItemPoco previousValue)
        {
            if (previousValue != null && previousValue.Skills.Contains(this))
            {
                previousValue.Skills.Remove(this);
            }
    
            if (TimelineItem != null)
            {
                if (!TimelineItem.Skills.Contains(this))
                {
                    TimelineItem.Skills.Add(this);
                }
                if (TimelineItemId != TimelineItem.Id)
                {
                    TimelineItemId = TimelineItem.Id;
                }
            }
        }
    
        private void FixupCreatedBy(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItemSkillsCreated.Contains(this))
            {
                previousValue.TimelineItemSkillsCreated.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.TimelineItemSkillsCreated.Contains(this))
                {
                    CreatedBy.TimelineItemSkillsCreated.Add(this);
                }
                if (CreatedByUserId != CreatedBy.Id)
                {
                    CreatedByUserId = CreatedBy.Id;
                }
            }
        }
    
        private void FixupDeletedBy(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.TimelineItemSkillsDeleted.Contains(this))
            {
                previousValue.TimelineItemSkillsDeleted.Remove(this);
            }
    
            if (DeletedBy != null)
            {
                if (!DeletedBy.TimelineItemSkillsDeleted.Contains(this))
                {
                    DeletedBy.TimelineItemSkillsDeleted.Add(this);
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

        #endregion

    }
}
