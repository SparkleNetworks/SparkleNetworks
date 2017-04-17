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
    public partial class CompanySkillPoco
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
    
        public System.DateTime Date
        {
            get;
            set;
        }

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

        #endregion

        #region Association Fixup
    
        private void FixupCompany(CompanyPoco previousValue)
        {
            if (previousValue != null && previousValue.CompanySkills.Contains(this))
            {
                previousValue.CompanySkills.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.CompanySkills.Contains(this))
                {
                    Company.CompanySkills.Add(this);
                }
                if (CompanyId != Company.ID)
                {
                    CompanyId = Company.ID;
                }
            }
        }
    
        private void FixupSkill(SkillPoco previousValue)
        {
            if (previousValue != null && previousValue.CompanySkills.Contains(this))
            {
                previousValue.CompanySkills.Remove(this);
            }
    
            if (Skill != null)
            {
                if (!Skill.CompanySkills.Contains(this))
                {
                    Skill.CompanySkills.Add(this);
                }
                if (SkillId != Skill.Id)
                {
                    SkillId = Skill.Id;
                }
            }
        }

        #endregion

    }
}
