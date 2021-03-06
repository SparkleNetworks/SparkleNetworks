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
    public partial class TagCategoryPoco
    {
        #region Primitive Properties
    
        public int Id
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
    
        public bool CanUsersCreate
        {
            get;
            set;
        }
    
        public Nullable<int> NetworkId
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
        private Nullable<int> _networkId;
    
        public string Rules
        {
            get;
            set;
        }
    
        public int SortOrder
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        // TagDefinition
        public ICollection<TagDefinitionPoco> TagDefinitions
        {
            get
            {
                if (_tagDefinitions == null)
                {
                    var newCollection = new FixupCollection<TagDefinitionPoco>();
                    newCollection.CollectionChanged += FixupTagDefinitions;
                    _tagDefinitions = newCollection;
                }
                return _tagDefinitions;
            }
            set
            {
                if (!ReferenceEquals(_tagDefinitions, value))
                {
                    var previousValue = _tagDefinitions as FixupCollection<TagDefinitionPoco>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupTagDefinitions;
                    }
                    _tagDefinitions = value;
                    var newValue = value as FixupCollection<TagDefinitionPoco>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupTagDefinitions;
                    }
                }
            }
        }
        private ICollection<TagDefinitionPoco> _tagDefinitions;
    
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
    
        private bool _settingFK = false;
    
        private void FixupNetwork(NetworkPoco previousValue)
        {
            if (previousValue != null && previousValue.TagCategories.Contains(this))
            {
                previousValue.TagCategories.Remove(this);
            }
    
            if (Network != null)
            {
                if (!Network.TagCategories.Contains(this))
                {
                    Network.TagCategories.Add(this);
                }
                if (NetworkId != Network.Id)
                {
                    NetworkId = Network.Id;
                }
            }
            else if (!_settingFK)
            {
                NetworkId = null;
            }
        }
    
        private void FixupTagDefinitions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TagDefinitionPoco item in e.NewItems)
                {
                    item.Category = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (TagDefinitionPoco item in e.OldItems)
                {
                    if (ReferenceEquals(item.Category, this))
                    {
                        item.Category = null;
                    }
                }
            }
        }

        #endregion

    }
}
