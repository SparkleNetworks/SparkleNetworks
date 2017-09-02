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
    public partial class UserRecreationPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int RecreationId
        {
            get { return _recreationId; }
            set
            {
                if (this._recreationId != value)
                {
                    if (this.Recreation != null && this.Recreation.Id != value)
                    {
                        this.Recreation = null;
                    }
                    this._recreationId = value;
                }
            }
        }
        private int _recreationId;
    
        public System.DateTime Date
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
                    if (this.User != null && this.User.Id != value)
                    {
                        this.User = null;
                    }
                    this._userId = value;
                }
            }
        }
        private int _userId;

        #endregion

        #region Navigation Properties
    
        public virtual RecreationPoco Recreation
        {
            get { return _recreation; }
            set
            {
                if (!ReferenceEquals(_recreation, value))
                {
                    var previousValue = _recreation;
                    _recreation = value;
                    FixupRecreation(previousValue);
                }
            }
        }
        private RecreationPoco _recreation;
    
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

        #endregion

        #region Association Fixup
    
        private void FixupRecreation(RecreationPoco previousValue)
        {
            if (previousValue != null && previousValue.UserRecreations.Contains(this))
            {
                previousValue.UserRecreations.Remove(this);
            }
    
            if (Recreation != null)
            {
                if (!Recreation.UserRecreations.Contains(this))
                {
                    Recreation.UserRecreations.Add(this);
                }
                if (RecreationId != Recreation.Id)
                {
                    RecreationId = Recreation.Id;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.UserRecreations.Contains(this))
            {
                previousValue.UserRecreations.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.UserRecreations.Contains(this))
                {
                    User.UserRecreations.Add(this);
                }
                if (UserId != User.Id)
                {
                    UserId = User.Id;
                }
            }
        }

        #endregion

    }
}
