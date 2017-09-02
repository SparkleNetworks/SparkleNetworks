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
    public partial class ResumeSkillPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int ResumeId
        {
            get { return _resumeId; }
            set
            {
                if (this._resumeId != value)
                {
                    if (this.Resume != null && this.Resume.Id != value)
                    {
                        this.Resume = null;
                    }
                    this._resumeId = value;
                }
            }
        }
        private int _resumeId;
    
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
    
        public System.DateTime DateUtc
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual ResumePoco Resume
        {
            get { return _resume; }
            set
            {
                if (!ReferenceEquals(_resume, value))
                {
                    var previousValue = _resume;
                    _resume = value;
                    FixupResume(previousValue);
                }
            }
        }
        private ResumePoco _resume;
    
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
    
        private void FixupResume(ResumePoco previousValue)
        {
            if (previousValue != null && previousValue.ResumeSkills.Contains(this))
            {
                previousValue.ResumeSkills.Remove(this);
            }
    
            if (Resume != null)
            {
                if (!Resume.ResumeSkills.Contains(this))
                {
                    Resume.ResumeSkills.Add(this);
                }
                if (ResumeId != Resume.Id)
                {
                    ResumeId = Resume.Id;
                }
            }
        }
    
        private void FixupSkill(SkillPoco previousValue)
        {
            if (previousValue != null && previousValue.ResumeSkills.Contains(this))
            {
                previousValue.ResumeSkills.Remove(this);
            }
    
            if (Skill != null)
            {
                if (!Skill.ResumeSkills.Contains(this))
                {
                    Skill.ResumeSkills.Add(this);
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
