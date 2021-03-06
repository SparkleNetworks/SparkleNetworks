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
    public partial class CompanyRequestMessagePoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int CompanyRequestId
        {
            get { return _companyRequestId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._companyRequestId != value)
                    {
                        if (this.CompanyRequest != null && this.CompanyRequest.Id != value)
                        {
                            this.CompanyRequest = null;
                        }
                        this._companyRequestId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _companyRequestId;
    
        public System.DateTime DateUtc
        {
            get;
            set;
        }
    
        public bool IsMessageFromCompany
        {
            get;
            set;
        }
    
        public string NewReplyEmail
        {
            get;
            set;
        }
    
        public Nullable<int> FromUserId
        {
            get { return _fromUserId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._fromUserId != value)
                    {
                        if (this.FromUser != null && this.FromUser.Id != value)
                        {
                            this.FromUser = null;
                        }
                        this._fromUserId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _fromUserId;
    
        public string FromUserName
        {
            get;
            set;
        }
    
        public string ToEmailAddress
        {
            get;
            set;
        }
    
        public string Content
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DisplayDateUtc
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual CompanyRequestPoco CompanyRequest
        {
            get { return _companyRequest; }
            set
            {
                if (!ReferenceEquals(_companyRequest, value))
                {
                    var previousValue = _companyRequest;
                    _companyRequest = value;
                    FixupCompanyRequest(previousValue);
                }
            }
        }
        private CompanyRequestPoco _companyRequest;
    
        public virtual UserPoco FromUser
        {
            get { return _fromUser; }
            set
            {
                if (!ReferenceEquals(_fromUser, value))
                {
                    var previousValue = _fromUser;
                    _fromUser = value;
                    FixupFromUser(previousValue);
                }
            }
        }
        private UserPoco _fromUser;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupCompanyRequest(CompanyRequestPoco previousValue)
        {
            if (previousValue != null && previousValue.Messages.Contains(this))
            {
                previousValue.Messages.Remove(this);
            }
    
            if (CompanyRequest != null)
            {
                if (!CompanyRequest.Messages.Contains(this))
                {
                    CompanyRequest.Messages.Add(this);
                }
                if (CompanyRequestId != CompanyRequest.Id)
                {
                    CompanyRequestId = CompanyRequest.Id;
                }
            }
        }
    
        private void FixupFromUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.CompanyRequestMessages.Contains(this))
            {
                previousValue.CompanyRequestMessages.Remove(this);
            }
    
            if (FromUser != null)
            {
                if (!FromUser.CompanyRequestMessages.Contains(this))
                {
                    FromUser.CompanyRequestMessages.Add(this);
                }
                if (FromUserId != FromUser.Id)
                {
                    FromUserId = FromUser.Id;
                }
            }
            else if (!_settingFK)
            {
                FromUserId = null;
            }
        }

        #endregion

    }
}
