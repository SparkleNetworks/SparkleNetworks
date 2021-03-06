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
    public partial class InterestPoco
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
                    if (this.User != null && this.User.Id != value)
                    {
                        this.User = null;
                    }
                    this._createdByUserId = value;
                }
            }
        }
        private int _createdByUserId;

        #endregion

        #region Navigation Properties
    
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
    
        // UserInterests
        public ICollection<UserInterestPoco> UserInterests
        {
            get
            {
                if (_userInterests == null)
                {
                    var newCollection = new FixupCollection<UserInterestPoco>();
                    newCollection.CollectionChanged += FixupUserInterests;
                    _userInterests = newCollection;
                }
                return _userInterests;
            }
            set
            {
                if (!ReferenceEquals(_userInterests, value))
                {
                    var previousValue = _userInterests as FixupCollection<UserInterestPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUserInterests;
                    }
                    _userInterests = value;
                    var newValue = value as FixupCollection<UserInterestPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUserInterests;
                    }
                }
            }
        }
        private ICollection<UserInterestPoco> _userInterests;
    
        // GroupInterest
        public ICollection<GroupInterestPoco> Groups
        {
            get
            {
                if (_groups == null)
                {
                    var newCollection = new FixupCollection<GroupInterestPoco>();
                    newCollection.CollectionChanged += FixupGroups;
                    _groups = newCollection;
                }
                return _groups;
            }
            set
            {
                if (!ReferenceEquals(_groups, value))
                {
                    var previousValue = _groups as FixupCollection<GroupInterestPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGroups;
                    }
                    _groups = value;
                    var newValue = value as FixupCollection<GroupInterestPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGroups;
                    }
                }
            }
        }
        private ICollection<GroupInterestPoco> _groups;

        #endregion

        #region Association Fixup
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.Interests.Contains(this))
            {
                previousValue.Interests.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.Interests.Contains(this))
                {
                    User.Interests.Add(this);
                }
                if (CreatedByUserId != User.Id)
                {
                    CreatedByUserId = User.Id;
                }
            }
        }
    
        private void FixupUserInterests(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (UserInterestPoco item in e.NewItems)
                {
                    item.Interest = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserInterestPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Interest, this))
                    {
                        item.Interest = null;
                    }
                }
            }
        }
    
        private void FixupGroups(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupInterestPoco item in e.NewItems)
                {
                    item.Interest = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupInterestPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Interest, this))
                    {
                        item.Interest = null;
                    }
                }
            }
        }

        #endregion

    }
}
