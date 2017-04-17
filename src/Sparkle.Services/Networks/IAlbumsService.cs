using System;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IAlbumsService
    {
        void Delete(Album item);
        int NewAlbum(Album item);
        Album SelectById(int id);
        System.Collections.Generic.IList<Album> SelectMyAlbums(int userId);
        Album Update(Album item);
    }
}
