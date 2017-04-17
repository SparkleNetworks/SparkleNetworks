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
    public partial class ExchangeSkillPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int CompanyId
        {
            get { return _companyId; }
            set
            {
                if (this._companyId != value)
                {
                    if (this.Company != null && this.Company.ID != value)
                    {
                        this.Company = null;
                    }
                    this._companyId = value;
                }
            }
        }
        private int _companyId;
    
        public System.DateTime Date
        {
            get;
            set;
        }
    
        public int SkillId
        {
            get { return _skillId; }
            set
            {
                if (this._skillId != value)
                {
                    if (this.Skill != null && this.Skill.Id != value)
                    {
                        this.Skill = null;
                    }
                    this._skillId = value;
                }
            }
        }
        private int _skillId;
    
        public byte Type
        {
            get;
            set;
        }
    
        public string Title
        {
            get;
            set;
        }
    
        public string Description
        {
            get;
            set;
        }
    
        public byte Status
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
    
        public int NetworkId
        {
            get { return _networkId; }
            set
            {
                if (this._networkId != value)
                {
                    if (this.Network != null && this.Network.Id != value)
                    {
                        this.Network = null;
                    }
                    this._networkId = value;
                }
            }
        }
        private int _networkId;

        #endregion

        #region Navigation Properties
    
        public virtual CompanyPoco Company
        {
            get { return _company; }
            set
            {
                if (!ReferenceEquals(_company, value))
                {
                    var previousValue = _company;
                    _company = value;
                    FixupCompany(previousValue);
                }
            }
        }
        private CompanyPoco _company;
    
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
    
        public virtual SkillPoco Skill
        {
            get { return _skill; }
            set
            {
                if (!ReferenceEquals(_skill, value))
                {
                    var previousValue = _skill;
                    _skill = value;
                    FixupSkill(previousValue);
                }
            }
        }
        private SkillPoco _skill;
    
        public virtual NetworkPoco Network
        {
            get { return _network; }
            set
            {
                if (!ReferenceEquals(_network, value))
                {
                    var previousValue = _network;
                    _network = value;
                    FixupNetwork(previousValue);
                }
            }
        }
        private NetworkPoco _network;

        #endregion

        #region Association Fixup
    
        private void FixupCompany(CompanyPoco previousValue)
        {
            if (previousValue != null && previousValue.ExchangeSkills.Contains(this))
            {
                previousValue.ExchangeSkills.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.ExchangeSkills.Contains(this))
                {
                    Company.ExchangeSkills.Add(this);
                }
                if (CompanyId != Company.ID)
                {
                    CompanyId = Company.ID;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.ExchangeSkills.Contains(this))
            {
                previousValue.ExchangeSkills.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.ExchangeSkills.Contains(this))
                {
                    User.ExchangeSkills.Add(this);
                }
                if (CreatedByUserId != User.Id)
                {
                    CreatedByUserId = User.Id;
                }
            }
        }
    
        private void FixupSkill(SkillPoco previousValue)
        {
            if (previousValue != null && previousValue.ExchangeSkills.Contains(this))
            {
                previousValue.ExchangeSkills.Remove(this);
            }
    
            if (Skill != null)
            {
                if (!Skill.ExchangeSkills.Contains(this))
                {
                    Skill.ExchangeSkills.Add(this);
                }
                if (SkillId != Skill.Id)
                {
                    SkillId = Skill.Id;
                }
            }
        }
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.ExchangeSkills.Contains(this))
            {
                previousValue.ExchangeSkills.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.ExchangeSkills.Contains(this))
                {
                    Network.ExchangeSkills.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }

        #endregion

    }
}