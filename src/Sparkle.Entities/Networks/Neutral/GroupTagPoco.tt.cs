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
    public partial class GroupTagPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int RelationId
        {
            get { return _relationId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._relationId != value)
                    {
                        if (this.Group != null && this.Group.Id != value)
                        {
                            this.Group = null;
                        }
                        this._relationId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _relationId;
    
        public int TagId
        {
            get { return _tagId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._tagId != value)
                    {
                        if (this.TagDefinition != null && this.TagDefinition.Id != value)
                        {
                            this.TagDefinition = null;
                        }
                        this._tagId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _tagId;
    
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
    
        public Nullable<System.DateTime> DateDeletedUtc
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
    
        public Nullable<byte> DeleteReason
        {
            get;
            set;
        }
    
        public int SortOrder
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
    
        public virtual TagDefinitionPoco TagDefinition
        {
            get { return _tagDefinition; }
            set
            {
                if (!ReferenceEquals(_tagDefinition, value))
                {
                    var previousValue = _tagDefinition;
                    _tagDefinition = value;
                    FixupTagDefinition(previousValue);
                }
            }
        }
        private TagDefinitionPoco _tagDefinition;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupGroup(GroupPoco previousValue)
        {
            if (previousValue != null && previousValue.Tags.Contains(this))
            {
                previousValue.Tags.Remove(this);
            }
    
            if (Group != null)
            {
                if (!Group.Tags.Contains(this))
                {
                    Group.Tags.Add(this);
                }
                if (RelationId != Group.Id)
                {
                    RelationId = Group.Id;
                }
            }
        }
    
        private void FixupCreatedByUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.GroupTagsCreated.Contains(this))
            {
                previousValue.GroupTagsCreated.Remove(this);
            }
    
            if (CreatedByUser != null)
            {
                if (!CreatedByUser.GroupTagsCreated.Contains(this))
                {
                    CreatedByUser.GroupTagsCreated.Add(this);
                }
                if (CreatedByUserId != CreatedByUser.Id)
                {
                    CreatedByUserId = CreatedByUser.Id;
                }
            }
        }
    
        private void FixupDeletedByUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.GroupTagsDeleted.Contains(this))
            {
                previousValue.GroupTagsDeleted.Remove(this);
            }
    
            if (DeletedByUser != null)
            {
                if (!DeletedByUser.GroupTagsDeleted.Contains(this))
                {
                    DeletedByUser.GroupTagsDeleted.Add(this);
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
    
        private void FixupTagDefinition(TagDefinitionPoco previousValue)
        {
            if (previousValue != null && previousValue.GroupTags.Contains(this))
            {
                previousValue.GroupTags.Remove(this);
            }
    
            if (TagDefinition != null)
            {
                if (!TagDefinition.GroupTags.Contains(this))
                {
                    TagDefinition.GroupTags.Add(this);
                }
                if (TagId != TagDefinition.Id)
                {
                    TagId = TagDefinition.Id;
                }
            }
        }

        #endregion

    }
}
