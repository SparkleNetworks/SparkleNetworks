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
    public partial class GroupCategoryPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get { return _id; }
            set
            {
                if (this._id != value)
                {
                    if (this.GroupCategory1 != null && this.GroupCategory1.Id != value)
                    {
                        this.GroupCategory1 = null;
                    }
                    this._id = value;
                }
            }
        }
        private int _id;
    
        public int ParentId
        {
            get;
            set;
        }
    
        public string Name
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual GroupCategoryPoco GroupCategories1
        {
            get { return _groupCategories1; }
            set
            {
                if (!ReferenceEquals(_groupCategories1, value))
                {
                    var previousValue = _groupCategories1;
                    _groupCategories1 = value;
                    FixupGroupCategories1(previousValue);
                }
            }
        }
        private GroupCategoryPoco _groupCategories1;
    
        public virtual GroupCategoryPoco GroupCategory1
        {
            get { return _groupCategory1; }
            set
            {
                if (!ReferenceEquals(_groupCategory1, value))
                {
                    var previousValue = _groupCategory1;
                    _groupCategory1 = value;
                    FixupGroupCategory1(previousValue);
                }
            }
        }
        private GroupCategoryPoco _groupCategory1;
    
        // Groups
        public ICollection<GroupPoco> Group
        {
            get
            {
                if (_group == null)
                {
                    var newCollection = new FixupCollection<GroupPoco>();
                    newCollection.CollectionChanged += FixupGroup;
                    _group = newCollection;
                }
                return _group;
            }
            set
            {
                if (!ReferenceEquals(_group, value))
                {
                    var previousValue = _group as FixupCollection<GroupPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGroup;
                    }
                    _group = value;
                    var newValue = value as FixupCollection<GroupPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGroup;
                    }
                }
            }
        }
        private ICollection<GroupPoco> _group;

        #endregion

        #region Association Fixup
    
        private void FixupGroupCategories1(GroupCategoryPoco previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.GroupCategory1, this))
            {
                previousValue.GroupCategory1 = null;
            }
    
            if (GroupCategories1 != null)
            {
                GroupCategories1.GroupCategory1 = this;
            }
        }
    
        private void FixupGroupCategory1(GroupCategoryPoco previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.GroupCategories1, this))
            {
                previousValue.GroupCategories1 = null;
            }
    
            if (GroupCategory1 != null)
            {
                GroupCategory1.GroupCategories1 = this;
                if (Id != GroupCategory1.Id)
                {
                    Id = GroupCategory1.Id;
                }
            }
        }
    
        private void FixupGroup(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupPoco item in e.NewItems)
                {
                    item.GroupCategory = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.GroupCategory, this))
                    {
                        item.GroupCategory = null;
                    }
                }
            }
        }

        #endregion

    }
}
