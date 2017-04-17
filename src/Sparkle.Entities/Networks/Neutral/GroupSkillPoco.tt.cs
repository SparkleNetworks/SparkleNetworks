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
    public partial class GroupSkillPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int GroupId
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
        private int _groupId;
    
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
    
        public System.DateTime DateCreatedUtc
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
    
        private void FixupGroup(GroupPoco previousValue)
        {
            if (previousValue != null && previousValue.Skills.Contains(this))
            {
                previousValue.Skills.Remove(this);
            }
    
            if (Group != null)
            {
                if (!Group.Skills.Contains(this))
                {
                    Group.Skills.Add(this);
                }
                if (GroupId != Group.Id)
                {
                    GroupId = Group.Id;
                }
            }
        }
    
        private void FixupSkill(SkillPoco previousValue)
        {
            if (previousValue != null && previousValue.Groups.Contains(this))
            {
                previousValue.Groups.Remove(this);
            }
    
            if (Skill != null)
            {
                if (!Skill.Groups.Contains(this))
                {
                    Skill.Groups.Add(this);
                }
                if (SkillId != Skill.Id)
                {
                    SkillId = Skill.Id;
                }
            }
        }
    
        private void FixupCreatedBy(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.GroupSkillsCreated.Contains(this))
            {
                previousValue.GroupSkillsCreated.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.GroupSkillsCreated.Contains(this))
                {
                    CreatedBy.GroupSkillsCreated.Add(this);
                }
                if (CreatedByUserId != CreatedBy.Id)
                {
                    CreatedByUserId = CreatedBy.Id;
                }
            }
        }
    
        private void FixupDeletedBy(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.GroupSkillsDeleted.Contains(this))
            {
                previousValue.GroupSkillsDeleted.Remove(this);
            }
    
            if (DeletedBy != null)
            {
                if (!DeletedBy.GroupSkillsDeleted.Contains(this))
                {
                    DeletedBy.GroupSkillsDeleted.Add(this);
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
