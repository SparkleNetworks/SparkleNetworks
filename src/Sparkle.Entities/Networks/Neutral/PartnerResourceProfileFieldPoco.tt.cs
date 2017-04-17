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
    public partial class PartnerResourceProfileFieldPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int PartnerResourceId
        {
            get { return _partnerResourceId; }
            set
            {
                if (this._partnerResourceId != value)
                {
                    if (this.PartnerResource != null && this.PartnerResource.Id != value)
                    {
                        this.PartnerResource = null;
                    }
                    this._partnerResourceId = value;
                }
            }
        }
        private int _partnerResourceId;
    
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
    
        public string Data
        {
            get;
            set;
        }
    
        public byte Source
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual PartnerResourcePoco PartnerResource
        {
            get { return _partnerResource; }
            set
            {
                if (!ReferenceEquals(_partnerResource, value))
                {
                    var previousValue = _partnerResource;
                    _partnerResource = value;
                    FixupPartnerResource(previousValue);
                }
            }
        }
        private PartnerResourcePoco _partnerResource;
    
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

        #endregion

        #region Association Fixup
    
        private void FixupPartnerResource(PartnerResourcePoco previousValue)
        {
            if (previousValue != null && previousValue.PartnerResourceProfileFields.Contains(this))
            {
                previousValue.PartnerResourceProfileFields.Remove(this);
            }
    
            if (PartnerResource != null)
            {
                if (!PartnerResource.PartnerResourceProfileFields.Contains(this))
                {
                    PartnerResource.PartnerResourceProfileFields.Add(this);
                }
                if (PartnerResourceId != PartnerResource.Id)
                {
                    PartnerResourceId = PartnerResource.Id;
                }
            }
        }
    
        private void FixupProfileField(ProfileFieldPoco previousValue)
        {
            if (previousValue != null && previousValue.PartnerResourceProfileFields.Contains(this))
            {
                previousValue.PartnerResourceProfileFields.Remove(this);
            }
    
            if (ProfileField != null)
            {
                if (!ProfileField.PartnerResourceProfileFields.Contains(this))
                {
                    ProfileField.PartnerResourceProfileFields.Add(this);
                }
                if (ProfileFieldId != ProfileField.Id)
                {
                    ProfileFieldId = ProfileField.Id;
                }
            }
        }

        #endregion

    }
}
