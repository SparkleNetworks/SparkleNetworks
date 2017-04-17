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
    public partial class CompanyRelationshipTypePoco
    {
        #region Primitive Properties
    
        public int Id
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
    
        public string Verb
        {
            get;
            set;
        }
    
        public byte KnownType
        {
            get;
            set;
        }
    
        public string Rules
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        // CompanyRelationship
        public ICollection<CompanyRelationshipPoco> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    var newCollection = new FixupCollection<CompanyRelationshipPoco>();
                    newCollection.CollectionChanged += FixupRelationships;
                    _relationships = newCollection;
                }
                return _relationships;
            }
            set
            {
                if (!ReferenceEquals(_relationships, value))
                {
                    var previousValue = _relationships as FixupCollection<CompanyRelationshipPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupRelationships;
                    }
                    _relationships = value;
                    var newValue = value as FixupCollection<CompanyRelationshipPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupRelationships;
                    }
                }
            }
        }
        private ICollection<CompanyRelationshipPoco> _relationships;
    
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

        #endregion

        #region Association Fixup
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.CompanyRelationshipTypes.Contains(this))
            {
                previousValue.CompanyRelationshipTypes.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.CompanyRelationshipTypes.Contains(this))
                {
                    Network.CompanyRelationshipTypes.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
        }
    
        private void FixupRelationships(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CompanyRelationshipPoco item in e.NewItems)
                {
                    item.RelationshipType = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CompanyRelationshipPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.RelationshipType, this))
                    {
                        item.RelationshipType = null;
                    }
                }
            }
        }

        #endregion

    }
}