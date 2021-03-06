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
    public partial class SkillPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int ParentId
        {
            get;
            set;
        }
    
        public string TagName
        {
            get;
            set;
        }
    
        public System.DateTime Date
        {
            get;
            set;
        }
    
        public int CreatedByUserId
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        // CompanySkills
        public ICollection<CompanySkillPoco> CompanySkills
        {
            get
            {
                if (_companySkills == null)
                {
                    var newCollection = new FixupCollection<CompanySkillPoco>();
                    newCollection.CollectionChanged += FixupCompanySkills;
                    _companySkills = newCollection;
                }
                return _companySkills;
            }
            set
            {
                if (!ReferenceEquals(_companySkills, value))
                {
                    var previousValue = _companySkills as FixupCollection<CompanySkillPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCompanySkills;
                    }
                    _companySkills = value;
                    var newValue = value as FixupCollection<CompanySkillPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCompanySkills;
                    }
                }
            }
        }
        private ICollection<CompanySkillPoco> _companySkills;
    
        // ExchangeSkills
        public ICollection<ExchangeSkillPoco> ExchangeSkills
        {
            get
            {
                if (_exchangeSkills == null)
                {
                    var newCollection = new FixupCollection<ExchangeSkillPoco>();
                    newCollection.CollectionChanged += FixupExchangeSkills;
                    _exchangeSkills = newCollection;
                }
                return _exchangeSkills;
            }
            set
            {
                if (!ReferenceEquals(_exchangeSkills, value))
                {
                    var previousValue = _exchangeSkills as FixupCollection<ExchangeSkillPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupExchangeSkills;
                    }
                    _exchangeSkills = value;
                    var newValue = value as FixupCollection<ExchangeSkillPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupExchangeSkills;
                    }
                }
            }
        }
        private ICollection<ExchangeSkillPoco> _exchangeSkills;
    
        // UserSkills
        public ICollection<UserSkillPoco> UserSkills
        {
            get
            {
                if (_userSkills == null)
                {
                    var newCollection = new FixupCollection<UserSkillPoco>();
                    newCollection.CollectionChanged += FixupUserSkills;
                    _userSkills = newCollection;
                }
                return _userSkills;
            }
            set
            {
                if (!ReferenceEquals(_userSkills, value))
                {
                    var previousValue = _userSkills as FixupCollection<UserSkillPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUserSkills;
                    }
                    _userSkills = value;
                    var newValue = value as FixupCollection<UserSkillPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUserSkills;
                    }
                }
            }
        }
        private ICollection<UserSkillPoco> _userSkills;
    
        // ResumeSkill
        public ICollection<ResumeSkillPoco> ResumeSkills
        {
            get
            {
                if (_resumeSkills == null)
                {
                    var newCollection = new FixupCollection<ResumeSkillPoco>();
                    newCollection.CollectionChanged += FixupResumeSkills;
                    _resumeSkills = newCollection;
                }
                return _resumeSkills;
            }
            set
            {
                if (!ReferenceEquals(_resumeSkills, value))
                {
                    var previousValue = _resumeSkills as FixupCollection<ResumeSkillPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupResumeSkills;
                    }
                    _resumeSkills = value;
                    var newValue = value as FixupCollection<ResumeSkillPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupResumeSkills;
                    }
                }
            }
        }
        private ICollection<ResumeSkillPoco> _resumeSkills;
    
        // TimelineItemSkill
        public ICollection<TimelineItemSkillPoco> TimelineItems
        {
            get
            {
                if (_timelineItems == null)
                {
                    var newCollection = new FixupCollection<TimelineItemSkillPoco>();
                    newCollection.CollectionChanged += FixupTimelineItems;
                    _timelineItems = newCollection;
                }
                return _timelineItems;
            }
            set
            {
                if (!ReferenceEquals(_timelineItems, value))
                {
                    var previousValue = _timelineItems as FixupCollection<TimelineItemSkillPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTimelineItems;
                    }
                    _timelineItems = value;
                    var newValue = value as FixupCollection<TimelineItemSkillPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTimelineItems;
                    }
                }
            }
        }
        private ICollection<TimelineItemSkillPoco> _timelineItems;
    
        // GroupSkill
        public ICollection<GroupSkillPoco> Groups
        {
            get
            {
                if (_groups == null)
                {
                    var newCollection = new FixupCollection<GroupSkillPoco>();
                    newCollection.CollectionChanged += FixupGroups;
                    _groups = newCollection;
                }
                return _groups;
            }
            set
            {
                if (!ReferenceEquals(_groups, value))
                {
                    var previousValue = _groups as FixupCollection<GroupSkillPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGroups;
                    }
                    _groups = value;
                    var newValue = value as FixupCollection<GroupSkillPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGroups;
                    }
                }
            }
        }
        private ICollection<GroupSkillPoco> _groups;

        #endregion

        #region Association Fixup
    
        private void FixupCompanySkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CompanySkillPoco item in e.NewItems)
                {
                    item.Skill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CompanySkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Skill, this))
                    {
                        item.Skill = null;
                    }
                }
            }
        }
    
        private void FixupExchangeSkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ExchangeSkillPoco item in e.NewItems)
                {
                    item.Skill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ExchangeSkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Skill, this))
                    {
                        item.Skill = null;
                    }
                }
            }
        }
    
        private void FixupUserSkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (UserSkillPoco item in e.NewItems)
                {
                    item.Skill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserSkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Skill, this))
                    {
                        item.Skill = null;
                    }
                }
            }
        }
    
        private void FixupResumeSkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ResumeSkillPoco item in e.NewItems)
                {
                    item.Skill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ResumeSkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Skill, this))
                    {
                        item.Skill = null;
                    }
                }
            }
        }
    
        private void FixupTimelineItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TimelineItemSkillPoco item in e.NewItems)
                {
                    item.Skill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TimelineItemSkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Skill, this))
                    {
                        item.Skill = null;
                    }
                }
            }
        }
    
        private void FixupGroups(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GroupSkillPoco item in e.NewItems)
                {
                    item.Skill = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GroupSkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Skill, this))
                    {
                        item.Skill = null;
                    }
                }
            }
        }

        #endregion

    }
}
