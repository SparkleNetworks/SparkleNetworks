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
    public partial class AspnetUsersPoco
    {
        #region Primitive Properties
    
        public System.Guid ApplicationId
        {
            get;
            set;
        }
    
        public System.Guid UserId
        {
            get;
            set;
        }
    
        public string UserName
        {
            get;
            set;
        }
    
        public string LoweredUserName
        {
            get;
            set;
        }
    
        public string MobileAlias
        {
            get;
            set;
        }
    
        public bool IsAnonymous
        {
            get;
            set;
        }
    
        public System.DateTime LastActivityDate
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        // User
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
    
        public virtual AspnetMembershipPoco AspnetMembership
        {
            get { return _aspnetMembership; }
            set
            {
                if (!ReferenceEquals(_aspnetMembership, value))
                {
                    var previousValue = _aspnetMembership;
                    _aspnetMembership = value;
                    FixupAspnetMembership(previousValue);
                }
            }
        }
        private AspnetMembershipPoco _aspnetMembership;

        #endregion

        #region Association Fixup
    
        private void FixupAspnetMembership(AspnetMembershipPoco previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.AspnetUser, this))
            {
                previousValue.AspnetUser = null;
            }
    
            if (AspnetMembership != null)
            {
                AspnetMembership.AspnetUser = this;
            }
        }
    
        private void FixupUsers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (UserPoco item in e.NewItems)
                {
                    item.AspnetUser = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.AspnetUser, this))
                    {
                        item.AspnetUser = null;
                    }
                }
            }
        }

        #endregion

    }
}
