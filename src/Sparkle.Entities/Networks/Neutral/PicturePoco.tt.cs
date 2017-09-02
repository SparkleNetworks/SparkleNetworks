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
    public partial class PicturePoco
    {
        #region Primitive Properties
    
        public int Id
        {
            get;
            set;
        }
    
        public int AlbumId
        {
            get { return _albumId; }
            set
            {
                if (this._albumId != value)
                {
                    if (this.Album != null && this.Album.Id != value)
                    {
                        this.Album = null;
                    }
                    this._albumId = value;
                }
            }
        }
        private int _albumId;
    
        public System.DateTime Date
        {
            get;
            set;
        }
    
        public string Comment
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
    
        public virtual AlbumPoco Album
        {
            get { return _album; }
            set
            {
                if (!ReferenceEquals(_album, value))
                {
                    var previousValue = _album;
                    _album = value;
                    FixupAlbum(previousValue);
                }
            }
        }
        private AlbumPoco _album;

        #endregion

        #region Association Fixup
    
        private void FixupAlbum(AlbumPoco previousValue)
        {
            if (previousValue != null && previousValue.Pictures.Contains(this))
            {
                previousValue.Pictures.Remove(this);
            }
    
            if (Album != null)
            {
                if (!Album.Pictures.Contains(this))
                {
                    Album.Pictures.Add(this);
                }
                if (AlbumId != Album.Id)
                {
                    AlbumId = Album.Id;
                }
            }
        }

        #endregion

    }
}
