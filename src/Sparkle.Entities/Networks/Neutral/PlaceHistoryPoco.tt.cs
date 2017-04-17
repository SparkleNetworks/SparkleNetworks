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
    public partial class PlaceHistoryPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
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
    
        public int PlaceParentId
        {
            get;
            set;
        }
    
        public System.DateTime Day
        {
            get;
            set;
        }
    
        public System.TimeSpan Hour
        {
            get;
            set;
        }
    
        public Nullable<int> Activity
        {
            get;
            set;
        }
    
        public int UserId
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
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
    
        private void FixupPlace(PlacePoco previousValue)
        {
            if (previousValue != null && previousValue.PlaceHistories.Contains(this))
            {
                previousValue.PlaceHistories.Remove(this);
            }
    
            if (Place != null)
            {
                if (!Place.PlaceHistories.Contains(this))
                {
                    Place.PlaceHistories.Add(this);
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
