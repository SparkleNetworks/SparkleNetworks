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
    public partial class TouchCommunicationItemPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int ParentId
        {
            get { return _parentId; }
            set
            {
                if (this._parentId != value)
                {
                    if (this.TouchCommunication != null && this.TouchCommunication.Id != value)
                    {
                        this.TouchCommunication = null;
                    }
                    this._parentId = value;
                }
            }
        }
        private int _parentId;
    
        public string Title
        {
            get;
            set;
        }
    
        public string Description
        {
            get;
            set;
        }
    
        public System.DateTime Start
        {
            get;
            set;
        }
    
        public System.DateTime End
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual TouchCommunicationPoco TouchCommunication
        {
            get { return _touchCommunication; }
            set
            {
                if (!ReferenceEquals(_touchCommunication, value))
                {
                    var previousValue = _touchCommunication;
                    _touchCommunication = value;
                    FixupTouchCommunication(previousValue);
                }
            }
        }
        private TouchCommunicationPoco _touchCommunication;

        #endregion

        #region Association Fixup
    
        private void FixupTouchCommunication(TouchCommunicationPoco previousValue)
        {
            if (previousValue != null && previousValue.Items.Contains(this))
            {
                previousValue.Items.Remove(this);
            }
    
            if (TouchCommunication != null)
            {
                if (!TouchCommunication.Items.Contains(this))
                {
                    TouchCommunication.Items.Add(this);
                }
                if (ParentId != TouchCommunication.Id)
                {
                    ParentId = TouchCommunication.Id;
                }
            }
        }

        #endregion

    }
}
