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
    public partial class ListItemPoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int ListId
        {
            get { return _listId; }
            set
            {
                if (this._listId != value)
                {
                    if (this.List != null && this.List.Id != value)
                    {
                        this.List = null;
                    }
                    this._listId = value;
                }
            }
        }
        private int _listId;
    
        public string Text
        {
            get;
            set;
        }
    
        public int CreatedByUserId
        {
            get { return _createdByUserId; }
            set
            {
                if (this._createdByUserId != value)
                {
                    if (this.User != null && this.User.Id != value)
                    {
                        this.User = null;
                    }
                    this._createdByUserId = value;
                }
            }
        }
        private int _createdByUserId;

        #endregion

        #region Navigation Properties
    
        public virtual ListPoco List
        {
            get { return _list; }
            set
            {
                if (!ReferenceEquals(_list, value))
                {
                    var previousValue = _list;
                    _list = value;
                    FixupList(previousValue);
                }
            }
        }
        private ListPoco _list;
    
        public virtual UserPoco User
        {
            get { return _user; }
            set
            {
                if (!ReferenceEquals(_user, value))
                {
                    var previousValue = _user;
                    _user = value;
                    FixupUser(previousValue);
                }
            }
        }
        private UserPoco _user;

        #endregion

        #region Association Fixup
    
        private void FixupList(ListPoco previousValue)
        {
            if (previousValue != null && previousValue.ListItems.Contains(this))
            {
                previousValue.ListItems.Remove(this);
            }
    
            if (List != null)
            {
                if (!List.ListItems.Contains(this))
                {
                    List.ListItems.Add(this);
                }
                if (ListId != List.Id)
                {
                    ListId = List.Id;
                }
            }
        }
    
        private void FixupUser(UserPoco previousValue)
        {
            if (previousValue != null && previousValue.ListItems.Contains(this))
            {
                previousValue.ListItems.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.ListItems.Contains(this))
                {
                    User.ListItems.Add(this);
                }
                if (CreatedByUserId != User.Id)
                {
                    CreatedByUserId = User.Id;
                }
            }
        }

        #endregion

    }
}
