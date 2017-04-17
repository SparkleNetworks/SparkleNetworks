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
    public partial class UsersVisitPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int UserId
        {
            get { return _userId; }
            set
            {
                if (this._userId != value)
                {
                    if (this.User1 != null && this.User1.Id != value)
                    {
                        this.User1 = null;
                    }
                    this._userId = value;
                }
            }
        }
        private int _userId;
    
        public System.DateTime Date
        {
            get;
            set;
        }
    
        public int ProfileId
        {
            get { return _profileId; }
            set
            {
                if (this._profileId != value)
                {
                    if (this.User != null && this.User.Id != value)
                    {
                        this.User = null;
                    }
                    this._profileId = value;
                }
            }
        }
        private int _profileId;
    
        public byte ViewCount
        {
            get;
            set;
        }

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
    
        public virtual UserPoco User1
        {
            get { return _user1; }
            set
            {
                if (!ReferenceEquals(_user1, value))
                {
                    var previousValue = _user1;
                    _user1 = value;
                    FixupUser1(previousValue);
                }
            }
        }
        private UserPoco _user1;

        #endregion

        #region Association Fixup
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.OtherUsersVisits.Contains(this))
            {
                previousValue.OtherUsersVisits.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.OtherUsersVisits.Contains(this))
                {
                    User.OtherUsersVisits.Add(this);
                }
                if (ProfileId != User.Id)
                {
                    ProfileId = User.Id;
                }
            }
        }
    
        private void FixupUser1(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.MyUsersVisits.Contains(this))
            {
                previousValue.MyUsersVisits.Remove(this);
            }
    
            if (User1 != null)
            {
                if (!User1.MyUsersVisits.Contains(this))
                {
                    User1.MyUsersVisits.Add(this);
                }
                if (UserId != User1.Id)
                {
                    UserId = User1.Id;
                }
            }
        }

        #endregion

    }
}