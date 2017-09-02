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
    public partial class ResumePoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public string FirstName
        {
            get;
            set;
        }
    
        public string LastName
        {
            get;
            set;
        }
    
        public int Gender
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> Birthday
        {
            get;
            set;
        }
    
        public string Objective
        {
            get;
            set;
        }
    
        public string About
        {
            get;
            set;
        }
    
        public string Email
        {
            get;
            set;
        }
    
        public string Phone
        {
            get;
            set;
        }
    
        public string Pin
        {
            get;
            set;
        }
    
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
    
        public bool IsApproved
        {
            get;
            set;
        }
    
        public string ResumeUrl
        {
            get;
            set;
        }
    
        public byte RequestType
        {
            get;
            set;
        }
    
        public System.DateTime CreatedDateUtc
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> EditedDateUtc
        {
            get;
            set;
        }
    
        public string LinkedInUrl
        {
            get;
            set;
        }
    
        public string ViadeoUrl
        {
            get;
            set;
        }
    
        public string Picture
        {
            get;
            set;
        }
    
        public bool IsActive
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
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

        #endregion

        #region Association Fixup
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.Resumes.Contains(this))
            {
                previousValue.Resumes.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.Resumes.Contains(this))
                {
                    Network.Resumes.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupResumeSkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ResumeSkillPoco item in e.NewItems)
                {
                    item.Resume = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ResumeSkillPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Resume, this))
                    {
                        item.Resume = null;
                    }
                }
            }
        }

        #endregion

    }
}
