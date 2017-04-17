
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Data;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Entity.Networks.Sql;
    using System.Linq;
    using Sparkle.Entities.Networks;
    using Sparkle.Data;
    using System.Data.Objects;
    using System.Collections.Generic;
    using System.Data.EntityClient;
    using System.Globalization;
    using System.Data.Spatial;
    using System.Data.Common;

    public class PlacesRepository : BaseNetworkRepositoryInt<Place>, IPlacesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PlacesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Places)
        {
        }

        public int Count()
        {
            return this.Set.Count();
        }

        public Place GetById(int placeId, PlaceOptions options)
        {
            return this.CreateQuery(options)
                .SingleOrDefault(p => p.Id == placeId);
        }

        public Place GetById(int placeId, int networkId, PlaceOptions options)
        {
            return this.CreateQuery(options)
                .SingleOrDefault(p => p.Id == placeId && p.NetworkId == networkId);
        }

        public IQueryable<Place> CreateQuery(PlaceOptions options)
        {
            ObjectQuery<Place> query = this.Set;

            ////if ((options & PlaceOptions.CreatedBy) == PlaceOptions.CreatedBy)
            ////    query = query.Include("CreatedBy");
            if ((options & PlaceOptions.Category) == PlaceOptions.Category)
                query = query.Include("PlaceCategory");
            
            return query;
        }

        public IDictionary<int, Place> GetById(int[] placeIds, PlaceOptions options)
        {
            return this.CreateQuery(options)
                .Where(p => placeIds.Contains(p.Id))
                .ToDictionary(p => p.Id, p => p);
        }

        public IList<Place> GetAll(int networkId)
        {
            var commandText = @"SELECT p.*, p.[Geography].STAsText() as GeographyText 
FROM [dbo].[Places] p 
WHERE p.NetworkId = @NetworkId ";

            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText(commandText)
                .AddParameter("@NetworkId", networkId);

            var values = new List<Place>();
            this.ExecuteReader(cmd, values);

            return values;
        }

        public IList<Place> GetAllByEventPopularity(int networkId, PlaceOptions options)
        {
            return this.CreateQuery(options)
                .Where(o => o.NetworkId == networkId)
                .OrderByDescending(o => o.Events.Count())
                .ThenBy(o => o.Name)
                .ToList();
        }

        public IList<Place> GetPlacesByRadius(int networkId, string[] geocodes, int radiusKm, bool includeItemsWithoutLocation)
        {
            var numberStyleFloat = NumberStyles.Float;
            var invariantCulture = CultureInfo.InvariantCulture;
            var numberFormat = invariantCulture.NumberFormat;

            var geoQuery = new List<string>();
            foreach (var item in geocodes)
            {
                double lat;
                double lon;
                var geoSplit = item.Split(new char[] { ' ', });
                if (geoSplit.Length == 2
                    && double.TryParse(geoSplit[0], numberStyleFloat, invariantCulture, out lat)
                    && double.TryParse(geoSplit[1], numberStyleFloat, invariantCulture, out lon))
                {
                    geoQuery.Add("(p.[Geography].STDistance(" + SqlGeometryPoint(lat, lon) + ") / 1000) < @Radius");
                }
            }

            var commandText = "SELECT p.*, p.[Geography].STAsText() as GeographyText FROM [dbo].[Places] p WHERE p.NetworkId = @NetworkId";
            if (geoQuery.Count > 0)
                commandText += " AND ( (" + string.Join(" OR ", geoQuery) + ") " + (includeItemsWithoutLocation ? "OR p.[Geography] IS NULL" : "") + ")";

            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText(commandText)
                .AddParameter("@Radius", radiusKm)
                .AddParameter("@NetworkId", networkId);

            var values = new List<Place>();
            this.ExecuteReader(cmd, values);

            return values;
        }

        public IList<Place> GetPlacesByNameAndRadius(int networkId, string[] words, string[] geocodes, int radiusKm, bool includeItemsWithoutLocation)
        {
            var numberStyleFloat = NumberStyles.Float;
            var invariantCulture = CultureInfo.InvariantCulture;
            var numberFormat = invariantCulture.NumberFormat;

            var geoQuery = new List<string>();
            foreach (var item in geocodes)
            {
                double lat;
                double lon;
                var geoSplit = item.Split(new char[] { ' ', });
                if (geoSplit.Length == 2
                    && double.TryParse(geoSplit[0], numberStyleFloat, invariantCulture, out lat)
                    && double.TryParse(geoSplit[1], numberStyleFloat, invariantCulture, out lon))
                {
                    geoQuery.Add("(p.[Geography].STDistance(" + SqlGeometryPoint(lat, lon) + ") / 1000) < @Radius");
                }
            }

            int i = 0;
            var wordsQuery = words.Select(o => "p.Name LIKE @Name" + i++).ToArray();

            var commandText = "SELECT p.*, p.[Geography].STAsText() as GeographyText FROM [dbo].[Places] p WHERE p.NetworkId = @NetworkId";
            if (geoQuery.Count > 0)
                commandText += " AND ( (" + string.Join(" OR ", geoQuery) + ") " + (includeItemsWithoutLocation ? " OR p.[Geography] IS NULL" : "") + ")";
            if (wordsQuery.Length > 0)
                commandText += " AND (" + string.Join(" AND ", wordsQuery) + ")";
            
            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText(commandText)
                .AddParameter("@Radius", radiusKm)
                .AddParameter("@NetworkId", networkId);

            if (wordsQuery.Length > 0)
            {
                for (i = 0; i < words.Length; i++)
                {
                    cmd = cmd.AddParameter("@Name" + i, '%' + words[i] + '%');
                }
            }

            var values = new List<Place>();
            this.ExecuteReader(cmd, values);

            return values;
        }

        public Place GetByAlias(int networkId, string alias)
        {
            var commandText = @"SELECT p.*, p.[Geography].STAsText() as GeographyText 
FROM [dbo].[Places] p 
WHERE p.NetworkId = @NetworkId AND p.Alias = @Alias";

            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText(commandText)
                .AddParameter("@NetworkId", networkId)
                .AddParameter("@Alias", alias, DbType.String);

            var values = new List<Place>();
            this.ExecuteReader(cmd, values);

            return values.SingleOrDefault();
        }

        public IList<Place> GetByParentId(int id, PlaceOptions options)
        {
            var commandText = @"SELECT p.*, p.[Geography].STAsText() as GeographyText 
FROM [dbo].[Places] p 
WHERE p.ParentId = @id";

            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText(commandText)
                .AddParameter("@id", id);

            var values = new List<Place>();
            this.ExecuteReader(cmd, values);

            return values;
        }

        public void SetGeography(int placeId, DbGeography geography)
        {
            var sql = "UPDATE dbo.Places SET [Geography] = " + SqlGeometryPoint(geography) + " WHERE Id = @Id";
            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText(sql)
                .AddParameter("@Id", placeId, DbType.Int32);
            var rows = cmd.ExecuteNonQuery();
            if (rows != 1)
                throw new InvalidOperationException("SetGeography command altered " + rows + " rows");
        }

        private void ExecuteReader(IDbCommand cmd, List<Place> values)
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    values.Add(new Place
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Alias = reader.GetString(reader.GetOrdinal("Alias")),
                        Description = !reader.IsDBNull(reader.GetOrdinal("Description")) ? reader.GetString(reader.GetOrdinal("Description")) : null,
                        Address = !reader.IsDBNull(reader.GetOrdinal("Address")) ? reader.GetString(reader.GetOrdinal("Address")) : null,
                        ZipCode = !reader.IsDBNull(reader.GetOrdinal("ZipCode")) ? reader.GetString(reader.GetOrdinal("ZipCode")) : null,
                        City = !reader.IsDBNull(reader.GetOrdinal("City")) ? reader.GetString(reader.GetOrdinal("City")) : null,
                        Country = !reader.IsDBNull(reader.GetOrdinal("Country")) ? reader.GetString(reader.GetOrdinal("Country")) : null,
                        Phone = !reader.IsDBNull(reader.GetOrdinal("Phone")) ? reader.GetString(reader.GetOrdinal("Phone")) : null,
                        CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                        FoursquareId = !reader.IsDBNull(reader.GetOrdinal("FoursquareId")) ? reader.GetString(reader.GetOrdinal("FoursquareId")) : null,
                        NetworkId = reader.GetInt32(reader.GetOrdinal("NetworkId")),
                        Main = reader.GetBoolean(reader.GetOrdinal("Main")),
                        Geography = !reader.IsDBNull(reader.GetOrdinal("Geography"))
                            ? DbGeography.FromText(reader.GetString(reader.GetOrdinal("GeographyText")))
                            : null,
                        ParentId = !reader.IsDBNull(reader.GetOrdinal("ParentId"))? reader.GetInt32(reader.GetOrdinal("ParentId")): default(int?),
                        CompanyOwner = !reader.IsDBNull(reader.GetOrdinal("CompanyOwner")) ? reader.GetInt32(reader.GetOrdinal("CompanyOwner")) : default(int?),
                        lat = !reader.IsDBNull(reader.GetOrdinal("lat")) ? reader.GetDouble(reader.GetOrdinal("lat")) : default(double?),
                        lon = !reader.IsDBNull(reader.GetOrdinal("lon")) ? reader.GetDouble(reader.GetOrdinal("lon")) : default(double?),
                    });
                }
            }
        }

        private string SqlGeometryPoint(double latitude, double longitude)
        {
            var numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            var value = "geography::STGeomFromText('POINT(" + longitude.ToString(numberFormat) + " " + latitude.ToString(numberFormat) + ")', 4326)";
            return value;
        }

        private string SqlGeometryPoint(DbGeography geography)
        {
            var value = geography.AsText();
            value = value.Replace("'", "''");
            value = "geography::STGeomFromText('" + value + "', 4326)";
            return value;
        }

        public IList<Place> GetDefaultPlaces(int networkId)
        {
            var items = this.Set
                .Where(o => o.NetworkId == networkId && o.Main)
                .ToList();

            if (items.Count > 0)
                return items;
            else
                return this.Set
                    .Take(1)
                    .ToList();
        }
    }

    public class PlacesCategoriesRepository : BaseNetworkRepositoryInt<PlaceCategory>, IPlacesCategoriesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PlacesCategoriesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.PlaceCategories)
        {
        }

        public IDictionary<int, PlaceCategory> GetAll(bool sorted = true)
        {
            var query = (IQueryable<PlaceCategory>)this.Set;
            if (sorted)
                query = query.OrderBy(o => o.Name);

            return query.ToDictionary(c => c.Id, c => c);
        }

        public IDictionary<int, PlaceCategory> GetById(int[] categoryIds)
        {
            return this.Set
                .Where(c => categoryIds.Contains(c.Id))
                .OrderBy(c => c.Name)
                .ToDictionary(c => c.Id, c => c);
        }

        public PlaceCategory GetByFoursquareId(string foursquareId)
        {
            return this.Set
                .Where(o => o.FoursquareId == foursquareId)
                .SingleOrDefault();
        }

        public PlaceCategory GetDefaultPlaceCategory()
        {
            // default PlaceCategory is 'Office'
            var foursquareId = "4bf58dd8d48988d124941735";
            var item = this.GetByFoursquareId(foursquareId);
            if (item != null)
                return item;
            else
                return this.Set
                    .Take(1)
                    .SingleOrDefault();
        }
    }

    public class PlacesHistoryRepository : BaseNetworkRepositoryInt<PlaceHistory>, IPlacesHistoryRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PlacesHistoryRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.PlaceHistories)
        {
        }
    }
}
