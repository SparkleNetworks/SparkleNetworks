using System;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IPicturesService
    {
        void Delete(Picture item);
        int Insert(Picture item);
        System.Collections.Generic.IList<Picture> SelectFromAlbum(int albumId);
        Picture Update(Picture item);
    }
}
