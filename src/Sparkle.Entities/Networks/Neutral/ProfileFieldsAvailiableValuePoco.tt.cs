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
    public partial class ProfileFieldsAvailiableValuePoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
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

        #endregion

        #region Association Fixup
    
        private void FixupProfileField(ProfileFieldPoco previousValue)
        {
            if (previousValue != null && previousValue.AvailiableValues.Contains(this))
            {
                previousValue.AvailiableValues.Remove(this);
            }
    
            if (ProfileField != null)
            {
                if (!ProfileField.AvailiableValues.Contains(this))
                {
                    ProfileField.AvailiableValues.Add(this);
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