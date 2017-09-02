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
    public partial class CompanyPlacePoco
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
    
        public int PlaceId
        {
            get { return _placeId; }
            set
            {
                if (this._placeId != value)
                {
                    if (this.Place != null && this.Place.Id != value)
                    {
                        this.Place = null;
                    }
                    this._placeId = value;
                }
            }
        }
        private int _placeId;
    
        public System.DateTime DateCreatedUtc
        {
            get;
            set;
        }
    
        public int CreatedByUserId
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> DateDeletedUtc
        {
            get;
            set;
        }
    
        public Nullable<int> DeletedByUserId
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
    
        public virtual PlacePoco Place
        {
            get { return _place; }
            set
            {
                if (!ReferenceEquals(_place, value))
                {
                    var previousValue = _place;
                    _place = value;
                    FixupPlace(previousValue);
                }
            }
        }
        private PlacePoco _place;

        #endregion

        #region Association Fixup
    
        private void FixupCompany(CompanyPoco previousValue)
        {
            if (previousValue != null && previousValue.CompanyPlaces.Contains(this))
            {
                previousValue.CompanyPlaces.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.CompanyPlaces.Contains(this))
                {
                    Company.CompanyPlaces.Add(this);
                }
                if (CompanyId != Company.ID)
                {
                    CompanyId = Company.ID;
                }
            }
        }
    
        private void FixupPlace(PlacePoco previousValue)
        {
            if (previousValue != null && previousValue.CompanyPlaces.Contains(this))
            {
                previousValue.CompanyPlaces.Remove(this);
            }
    
            if (Place != null)
            {
                if (!Place.CompanyPlaces.Contains(this))
                {
                    Place.CompanyPlaces.Add(this);
                }
                if (PlaceId != Place.Id)
                {
                    PlaceId = Place.Id;
                }
            }
        }

        #endregion

    }
}
