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
    public partial class RecreationPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int ParentId
        {
            get;
            set;
        }
    
        public string TagName
        {
            get;
            set;
        }
    
        public System.DateTime Date
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
                    if (this.CreatedBy != null && this.CreatedBy.Id != value)
                    {
                        this.CreatedBy = null;
                    }
                    this._createdByUserId = value;
                }
            }
        }
        private int _createdByUserId;

        #endregion

        #region Navigation Properties
    
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
    
        // UserRecreations
        public ICollection<UserRecreationPoco> UserRecreations
        {
            get
            {
                if (_userRecreations == null)
                {
                    var newCollection = new FixupCollection<UserRecreationPoco>();
                    newCollection.CollectionChanged += FixupUserRecreations;
                    _userRecreations = newCollection;
                }
                return _userRecreations;
            }
            set
            {
                if (!ReferenceEquals(_userRecreations, value))
                {
                    var previousValue = _userRecreations as FixupCollection<UserRecreationPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUserRecreations;
                    }
                    _userRecreations = value;
                    var newValue = value as FixupCollection<UserRecreationPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUserRecreations;
                    }
                }
            }
        }
        private ICollection<UserRecreationPoco> _userRecreations;
    
        // GroupRecreation
        public ICollection<GroupRecreationPoco> Groups
        {
            get
            {
                if (_groups == null)
                {
                    var newCollection = new FixupCollection<GroupRecreationPoco>();
                    newCollection.CollectionChanged += FixupGroups;
                    _groups = newCollection;
                }
                return _groups;
            }
            set
            {
                if (!ReferenceEquals(_groups, value))
                {
                    var previousValue = _groups as FixupCollection<GroupRecreationPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGroups;
                    }
                    _groups = value;
                    var newValue = value as FixupCollection<GroupRecreationPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGroups;
                    }
                }
            }
        }
        private ICollection<GroupRecreationPoco> _groups;

        #endregion

        #region Association Fixup
    
        private void FixupCreatedBy(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.Recreations.Contains(this))
            {
                previousValue.Recreations.Remove(this);
            }
    
            if (CreatedBy != null)
            {
                if (!CreatedBy.Recreations.Contains(this))
                {
                    CreatedBy.Recreations.Add(this);
                }
                if (CreatedByUserId != CreatedBy.Id)
                {
                    CreatedByUserId = CreatedBy.Id;
                }
            }
        }
    
        private void FixupUserRecreations(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (UserRecreationPoco item in e.NewItems)
                {
                    item.Recreation = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserRecreationPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Recreation, this))
                    {
                        item.Recreation = null;
                    }
                }
            }
        }
    
        private void FixupGroups(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupRecreationPoco item in e.NewItems)
                {
                    item.Recreation = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupRecreationPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Recreation, this))
                    {
                        item.Recreation = null;
                    }
                }
            }
        }

        #endregion

    }
}
