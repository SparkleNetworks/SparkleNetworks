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
    public partial class CompanyCategoryPoco
    {
        #region Primitive Properties
    
        public short Id
        {
            get;
            set;
        }
    
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
    
        public byte KnownCategory
        {
            get;
            set;
        }
    
        public bool IsDefault
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        // CompanyRequest
        public ICollection<CompanyRequestPoco> CompanyRequests
        {
            get
            {
                if (_companyRequests == null)
                {
                    var newCollection = new FixupCollection<CompanyRequestPoco>();
                    newCollection.CollectionChanged += FixupCompanyRequests;
                    _companyRequests = newCollection;
                }
                return _companyRequests;
            }
            set
            {
                if (!ReferenceEquals(_companyRequests, value))
                {
                    var previousValue = _companyRequests as FixupCollection<CompanyRequestPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCompanyRequests;
                    }
                    _companyRequests = value;
                    var newValue = value as FixupCollection<CompanyRequestPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCompanyRequests;
                    }
                }
            }
        }
        private ICollection<CompanyRequestPoco> _companyRequests;
    
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
    
        // Company
        public ICollection<CompanyPoco> Companies
        {
            get
            {
                if (_companies == null)
                {
                    var newCollection = new FixupCollection<CompanyPoco>();
                    newCollection.CollectionChanged += FixupCompanies;
                    _companies = newCollection;
                }
                return _companies;
            }
            set
            {
                if (!ReferenceEquals(_companies, value))
                {
                    var previousValue = _companies as FixupCollection<CompanyPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCompanies;
                    }
                    _companies = value;
                    var newValue = value as FixupCollection<CompanyPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCompanies;
                    }
                }
            }
        }
        private ICollection<CompanyPoco> _companies;

        #endregion

        #region Association Fixup
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.CompanyCategories.Contains(this))
            {
                previousValue.CompanyCategories.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.CompanyCategories.Contains(this))
                {
                    Network.CompanyCategories.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupCompanyRequests(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CompanyRequestPoco item in e.NewItems)
                {
                    item.CompanyCategory = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CompanyRequestPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.CompanyCategory, this))
                    {
                        item.CompanyCategory = null;
                    }
                }
            }
        }
    
        private void FixupCompanies(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CompanyPoco item in e.NewItems)
                {
                    item.CompanyCategory = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CompanyPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.CompanyCategory, this))
                    {
                        item.CompanyCategory = null;
                    }
                }
            }
        }

        #endregion

    }
}
