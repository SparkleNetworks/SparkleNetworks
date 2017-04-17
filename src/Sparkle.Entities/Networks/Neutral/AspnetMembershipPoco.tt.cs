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
    public partial class AspnetMembershipPoco
    {
        #region Primitive Properties
    
        public System.Guid ApplicationId
        {
            get;
            set;
        }
    
        public System.Guid UserId
        {
            get { return _userId; }
            set
            {
                if (this._userId != value)
                {
                    if (this.AspnetUser != null && this.AspnetUser.UserId != value)
                    {
                        this.AspnetUser = null;
                    }
                    this._userId = value;
                }
            }
        }
        private System.Guid _userId;
    
        public string Password
        {
            get;
            set;
        }
    
        public int PasswordFormat
        {
            get;
            set;
        }
    
        public string PasswordSalt
        {
            get;
            set;
        }
    
        public string MobilePIN
        {
            get;
            set;
        }
    
        public string Email
        {
            get;
            set;
        }
    
        public string LoweredEmail
        {
            get;
            set;
        }
    
        public string PasswordQuestion
        {
            get;
            set;
        }
    
        public string PasswordAnswer
        {
            get;
            set;
        }
    
        public bool IsApproved
        {
            get;
            set;
        }
    
        public bool IsLockedOut
        {
            get;
            set;
        }
    
        public System.DateTime CreateDate
        {
            get;
            set;
        }
    
        public System.DateTime LastLoginDate
        {
            get;
            set;
        }
    
        public System.DateTime LastPasswordChangedDate
        {
            get;
            set;
        }
    
        public System.DateTime LastLockoutDate
        {
            get;
            set;
        }
    
        public int FailedPasswordAttemptCount
        {
            get;
            set;
        }
    
        public System.DateTime FailedPasswordAttemptWindowStart
        {
            get;
            set;
        }
    
        public int FailedPasswordAnswerAttemptCount
        {
            get;
            set;
        }
    
        public System.DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get;
            set;
        }
    
        public string Comment
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual AspnetUsersPoco AspnetUser
        {
            get { return _aspnetUser; }
            set
            {
                if (!ReferenceEquals(_aspnetUser, value))
                {
                    var previousValue = _aspnetUser;
                    _aspnetUser = value;
                    FixupAspnetUser(previousValue);
                }
            }
        }
        private AspnetUsersPoco _aspnetUser;

        #endregion

        #region Association Fixup
    
        private void FixupAspnetUser(AspnetUsersPoco previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.AspnetMembership, this))
            {
                previousValue.AspnetMembership = null;
            }
    
            if (AspnetUser != null)
            {
                AspnetUser.AspnetMembership = this;
                if (UserId != AspnetUser.UserId)
                {
                    UserId = AspnetUser.UserId;
                }
            }
        }

        #endregion

    }
}
