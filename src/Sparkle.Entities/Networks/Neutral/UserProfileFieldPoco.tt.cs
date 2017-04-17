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
    public partial class UserProfileFieldPoco
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
                    if (this.User != null && this.User.Id != value)
                    {
                        this.User = null;
                    }
                    this._userId = value;
                }
            }
        }
        private int _userId;
    
        public int ProfileFieldId
        {
            get { return _profileFieldId; }
            set
            {
                if (this._profileFieldId != value)
                {
                    if (this.ProfileField != null && this.ProfileField.Id != value)
                    {
                        this.ProfileField = null;
                    }
                    this._profileFieldId = value;
                }
            }
        }
        private int _profileFieldId;
    
        public string Value
        {
            get;
            set;
        }
    
        public System.DateTime DateCreatedUtc
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateUpdatedUtc
        {
            get;
            set;
        }
    
        public short UpdateCount
        {
            get;
            set;
        }
    
        public byte Source
        {
            get;
            set;
        }
    
        public string Data
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ProfileFieldPoco ProfileField
        {
            get { return _profileField; }
            set
            {
                if (!ReferenceEquals(_profileField, value))
                {
                    var previousValue = _profileField;
                    _profileField = value;
                    FixupProfileField(previousValue);
                }
            }
        }
        private ProfileFieldPoco _profileField;
    
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
    
        private void FixupProfileField(ProfileFieldPoco previousValue)
        {
            if (previousValue != null && previousValue.UserProfileFields.Contains(this))
            {
                previousValue.UserProfileFields.Remove(this);
            }
    
            if (ProfileField != null)
            {
                if (!ProfileField.UserProfileFields.Contains(this))
                {
                    ProfileField.UserProfileFields.Add(this);
                }
                if (ProfileFieldId != ProfileField.Id)
                {
                    ProfileFieldId = ProfileField.Id;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.UserProfileFields.Contains(this))
            {
                previousValue.UserProfileFields.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.UserProfileFields.Contains(this))
                {
                    User.UserProfileFields.Add(this);
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