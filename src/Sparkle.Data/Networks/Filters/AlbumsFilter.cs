
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class AlbumsFilter
    {
        public static IQueryable<Album> ById(this IQueryable<Album> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<Album> UserId(this IQueryable<Album> qry, int userId)
        {
            return qry.Where(o => o.UserId == userId);
        }
    }

    public static class PicturesFilter
    {
        public static IQueryable<Picture> ByAlbumId(this IQueryable<Picture> qry, int albumId)
        {
            return qry.Where(o => o.AlbumId == albumId);
        }
    }
}
