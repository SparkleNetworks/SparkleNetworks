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
    public partial class PlacePoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int CategoryId
        {
            get { return _categoryId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._categoryId != value)
                    {
                        if (this.PlaceCategory != null && this.PlaceCategory.Id != value)
                        {
                            this.PlaceCategory = null;
                        }
                        this._categoryId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _categoryId;
    
        public string Name
        {
            get;
            set;
        }
    
        public string Alias
        {
            get;
            set;
        }
    
        public string Description
        {
            get;
            set;
        }
    
        public string Address
        {
            get;
            set;
        }
    
        public string ZipCode
        {
            get;
            set;
        }
    
        public string City
        {
            get;
            set;
        }
    
        public string Country
        {
            get;
            set;
        }
    
        public string Building
        {
            get;
            set;
        }
    
        public Nullable<int> Floor
        {
            get;
            set;
        }
    
        public string Access
        {
            get;
            set;
        }
    
        public string Door
        {
            get;
            set;
        }
    
        public Nullable<int> ParentId
        {
            get { return _parentId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._parentId != value)
                    {
                        if (this.Parent != null && this.Parent.Id != value)
                        {
                            this.Parent = null;
                        }
                        this._parentId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _parentId;
    
        public Nullable<System.Guid> PeopleOwner
        {
            get;
            set;
        }
    
        public Nullable<int> CompanyOwner
        {
            get { return _companyOwner; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._companyOwner != value)
                    {
                        if (this.Company != null && this.Company.ID != value)
                        {
                            this.Company = null;
                        }
                        this._companyOwner = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _companyOwner;
    
        public string Phone
        {
            get;
            set;
        }
    
        public int CreatedByUserId
        {
            get;
            set;
        }
    
        public Nullable<double> lat
        {
            get;
            set;
        }
    
        public Nullable<double> lon
        {
            get;
            set;
        }
    
        public string FoursquareId
        {
            get;
            set;
        }
    
        public int NetworkId
        {
            get { return _networkId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (this._networkId != value)
                    {
                        if (this.Network != null && this.Network.Id != value)
                        {
                            this.Network = null;
                        }
                        this._networkId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _networkId;
    
        public bool Main
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
    
        public virtual PlaceCategoryPoco PlaceCategory
        {
            get { return _placeCategory; }
            set
            {
                if (!ReferenceEquals(_placeCategory, value))
                {
                    var previousValue = _placeCategory;
                    _placeCategory = value;
                    FixupPlaceCategory(previousValue);
                }
            }
        }
        private PlaceCategoryPoco _placeCategory;
    
        // PlaceHistory
        public ICollection<PlaceHistoryPoco> PlaceHistories
        {
            get
            {
                if (_placeHistories == null)
                {
                    var newCollection = new FixupCollection<PlaceHistoryPoco>();
                    newCollection.CollectionChanged += FixupPlaceHistories;
                    _placeHistories = newCollection;
                }
                return _placeHistories;
            }
            set
            {
                if (!ReferenceEquals(_placeHistories, value))
                {
                    var previousValue = _placeHistories as FixupCollection<PlaceHistoryPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPlaceHistories;
                    }
                    _placeHistories = value;
                    var newValue = value as FixupCollection<PlaceHistoryPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPlaceHistories;
                    }
                }
            }
        }
        private ICollection<PlaceHistoryPoco> _placeHistories;
    
        // Event
        public ICollection<EventPoco> Events
        {
            get
            {
                if (_events == null)
                {
                    var newCollection = new FixupCollection<EventPoco>();
                    newCollection.CollectionChanged += FixupEvents;
                    _events = newCollection;
                }
                return _events;
            }
            set
            {
                if (!ReferenceEquals(_events, value))
                {
                    var previousValue = _events as FixupCollection<EventPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupEvents;
                    }
                    _events = value;
                    var newValue = value as FixupCollection<EventPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupEvents;
                    }
                }
            }
        }
        private ICollection<EventPoco> _events;
    
        // Place1
        public ICollection<PlacePoco> Children
        {
            get
            {
                if (_children == null)
                {
                    var newCollection = new FixupCollection<PlacePoco>();
                    newCollection.CollectionChanged += FixupChildren;
                    _children = newCollection;
                }
                return _children;
            }
            set
            {
                if (!ReferenceEquals(_children, value))
                {
                    var previousValue = _children as FixupCollection<PlacePoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupChildren;
                    }
                    _children = value;
                    var newValue = value as FixupCollection<PlacePoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupChildren;
                    }
                }
            }
        }
        private ICollection<PlacePoco> _children;
    
        public virtual PlacePoco Parent
        {
            get { return _parent; }
            set
            {
                if (!ReferenceEquals(_parent, value))
                {
                    var previousValue = _parent;
                    _parent = value;
                    FixupParent(previousValue);
                }
            }
        }
        private PlacePoco _parent;
    
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
    
        // CompanyPlace
        public ICollection<CompanyPlacePoco> CompanyPlaces
        {
            get
            {
                if (_companyPlaces == null)
                {
                    var newCollection = new FixupCollection<CompanyPlacePoco>();
                    newCollection.CollectionChanged += FixupCompanyPlaces;
                    _companyPlaces = newCollection;
                }
                return _companyPlaces;
            }
            set
            {
                if (!ReferenceEquals(_companyPlaces, value))
                {
                    var previousValue = _companyPlaces as FixupCollection<CompanyPlacePoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCompanyPlaces;
                    }
                    _companyPlaces = value;
                    var newValue = value as FixupCollection<CompanyPlacePoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCompanyPlaces;
                    }
                }
            }
        }
        private ICollection<CompanyPlacePoco> _companyPlaces;
    
        // PlaceProfileField
        public ICollection<PlaceProfileFieldPoco> PlaceProfileFields
        {
            get
            {
                if (_placeProfileFields == null)
                {
                    var newCollection = new FixupCollection<PlaceProfileFieldPoco>();
                    newCollection.CollectionChanged += FixupPlaceProfileFields;
                    _placeProfileFields = newCollection;
                }
                return _placeProfileFields;
            }
            set
            {
                if (!ReferenceEquals(_placeProfileFields, value))
                {
                    var previousValue = _placeProfileFields as FixupCollection<PlaceProfileFieldPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPlaceProfileFields;
                    }
                    _placeProfileFields = value;
                    var newValue = value as FixupCollection<PlaceProfileFieldPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPlaceProfileFields;
                    }
                }
            }
        }
        private ICollection<PlaceProfileFieldPoco> _placeProfileFields;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupCompany(CompanyPoco previousValue)
        {
            if (previousValue != null && previousValue.Places.Contains(this))
            {
                previousValue.Places.Remove(this);
            }
    
            if (Company != null)
            {
                if (!Company.Places.Contains(this))
                {
                    Company.Places.Add(this);
                }
                if (CompanyOwner != Company.ID)
                {
                    CompanyOwner = Company.ID;
                }
            }
            else if (!_settingFK)
            {
                CompanyOwner = null;
            }
        }
    
        private void FixupPlaceCategory(PlaceCategoryPoco previousValue)
        {
            if (previousValue != null && previousValue.Places.Contains(this))
            {
                previousValue.Places.Remove(this);
            }
    
            if (PlaceCategory != null)
            {
                if (!PlaceCategory.Places.Contains(this))
                {
                    PlaceCategory.Places.Add(this);
                }
                if (CategoryId != PlaceCategory.Id)
                {
                    CategoryId = PlaceCategory.Id;
                }
            }
        }
    
        private void FixupParent(PlacePoco previousValue)
        {
            if (previousValue != null && previousValue.Children.Contains(this))
            {
                previousValue.Children.Remove(this);
            }
    
            if (Parent != null)
            {
                if (!Parent.Children.Contains(this))
                {
                    Parent.Children.Add(this);
                }
                if (ParentId != Parent.Id)
                {
                    ParentId = Parent.Id;
                }
            }
            else if (!_settingFK)
            {
                ParentId = null;
            }
        }
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.Places.Contains(this))
            {
                previousValue.Places.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.Places.Contains(this))
                {
                    Network.Places.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupPlaceHistories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PlaceHistoryPoco item in e.NewItems)
                {
                    item.Place = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PlaceHistoryPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Place, this))
                    {
                        item.Place = null;
                    }
                }
            }
        }
    
        private void FixupEvents(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EventPoco item in e.NewItems)
                {
                    item.Place = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (EventPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Place, this))
                    {
                        item.Place = null;
                    }
                }
            }
        }
    
        private void FixupChildren(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PlacePoco item in e.NewItems)
                {
                    item.Parent = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PlacePoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Parent, this))
                    {
                        item.Parent = null;
                    }
                }
            }
        }
    
        private void FixupCompanyPlaces(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CompanyPlacePoco item in e.NewItems)
                {
                    item.Place = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CompanyPlacePoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Place, this))
                    {
                        item.Place = null;
                    }
                }
            }
        }
    
        private void FixupPlaceProfileFields(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PlaceProfileFieldPoco item in e.NewItems)
                {
                    item.Place = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PlaceProfileFieldPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Place, this))
                    {
                        item.Place = null;
                    }
                }
            }
        }

        #endregion

    }
}
