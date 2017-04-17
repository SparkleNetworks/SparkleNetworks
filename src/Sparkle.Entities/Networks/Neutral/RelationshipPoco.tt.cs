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
    public partial class RelationshipPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public string Name
        {
            get;
            set;
        }
    
        public Nullable<int> Gender
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        // Users
        public ICollection<UserPoco> Users
        {
            get
            {
                if (_users == null)
                {
                    var newCollection = new FixupCollection<UserPoco>();
                    newCollection.CollectionChanged += FixupUsers;
                    _users = newCollection;
                }
                return _users;
            }
            set
            {
                if (!ReferenceEquals(_users, value))
                {
                    var previousValue = _users as FixupCollection<UserPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUsers;
                    }
                    _users = value;
                    var newValue = value as FixupCollection<UserPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUsers;
                    }
                }
            }
        }
        private ICollection<UserPoco> _users;

        #endregion

        #region Association Fixup
    
        private void FixupUsers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (UserPoco item in e.NewItems)
                {
                    item.Relationship = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Relationship, this))
                    {
                        item.Relationship = null;
                    }
                }
            }
        }

        #endregion

    }
}
