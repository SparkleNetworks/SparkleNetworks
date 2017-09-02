
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using Newtonsoft.Json;
    using Sparkle.Services.Networks.Places;

    public class CacheService : ServiceBase, ICacheService
    {
        private IServiceCache cache;
        private TimeSpan defaultCacheDuration = TimeSpan.FromMinutes(5D);
        private TimeSpan longCacheDuration = TimeSpan.FromHours(5D);
        private static readonly BinaryFormatter serializer = new BinaryFormatter();

        // READ THIS

        //        .==.        .==.          
        //       //`^\\      //^`\\         
        //      // ^ ^\(\__/)/^ ^^\\        
        //     //^ ^^ ^/6  6\ ^^ ^ \\       
        //    //^ ^^ ^/( .. )\^ ^ ^ \\      
        //   // ^^ ^/\| v""v |/\^ ^ ^\\     
        //  // ^^/\/ /  `~~`  \ \/\^ ^\\    
        //  -----------------------------
        // HERE BE DRAGONS
        // 
        // Don't play with this code too fast.
        // Always use the SQL profiler to verify the cache still works.
        // DO     store active elements.
        // DO     store inactive/deleted/hidden elements.
        // DO NOT store elements from other networks.
        // ALWAYS copy the elements from the cache before giving them away.
        // DO NOT give away cache objects without copying them.
        // DO     search for missing elements from other networks.
        // 
        // Need to change this code? Ask SandRock.
        // Need to call this code?   Then do.
        // You break this code, I break you.
        // 

        // 
        // TODO LIST
        // 
        // [x] enhance methods to copy only the required elements
        // [ ] provide invalidate methods
        // 

        [DebuggerStepThrough]
        internal CacheService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory, IServiceCache cache)
            : base(repositoryFactory, serviceFactory)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            this.cache = cache;
        }

        public IDictionary<int, UserModel> AllUsers
        {
            get
            {
                var obj = this.GetUsersCache();
                var copy = new Dictionary<int, UserModel>(obj.Count);
                foreach (var item in obj)
                {
                    copy.Add(item.Key, Clone(item.Value));
                }

                return copy;
            }
        }

        public IDictionary<int, PlaceModel> AllPlaces
        {
            get
            {
                var obj = GetPlacesCache();

                var copy = new Dictionary<int, PlaceModel>(obj.Count);
                foreach (var item in obj)
                {
                    copy.Add(item.Key, Clone(item.Value));
                }

                return copy;
            }
        }

        public IDictionary<int, EventCategoryModel> AllEventCategories
        {
            get
            {
                var obj = GetEventCategoriesCache();
                var copy = new Dictionary<int, EventCategoryModel>(obj.Count);
                foreach (var item in obj)
                {
                    copy.Add(item.Key, Clone(item.Value));
                }

                return copy;
            }
        }

        public IDictionary<int, ProfileFieldModel> AllProfileFields
        {
            get
            {
                var obj = this.GetProfileFieldsCache();

                var copy = new Dictionary<int, ProfileFieldModel>(obj.Count);
                foreach (var item in obj)
                {
                    copy.Add(item.Key, Clone(item.Value));
                }

                return copy;
            }
        }

        public IDictionary<int, HintModel> AllHints
        {
            get
            {
                var obj = this.GetHintsCache();
                var copy = new Dictionary<int, HintModel>(obj.Count);
                foreach (var item in obj)
                {
                    copy.Add(item.Key, Clone(item.Value));
                }

                return copy;
            }
        }

        public IDictionary<int, UserModel> GetUsers(int[] ids)
        {
            var source = this.GetUsersCache();
            var items = new Dictionary<int, UserModel>(ids.Length);
            var missing = new List<int>();

            foreach (var id in ids)
            {
                if (source.ContainsKey(id))
                {
                    if (!items.ContainsKey(id))
                    {
                        items.Add(id, Clone(source[id]));
                    }
                }
                else
                {
                    if (!items.ContainsKey(id))
                    {
                        items.Add(id, null);
                    }

                    if (!missing.Contains(id))
                    {
                        missing.Add(id);
                    }
                }
            }

            source = this.Services.People.GetForCache(missing.ToArray());
            foreach (var id in missing)
            {
                items[id] = source[id];
            }

            return items;
        }

        public UserModel GetUser(int userId)
        {
            return this.GetUsers(new int[] { userId, }).Values.Single();
        }

        public IDictionary<int, UserModel> FindUsers(Func<UserModel, bool> predicate)
        {
            var obj = this.GetUsersCache();

            var copy = new Dictionary<int, UserModel>(obj.Count);
            foreach (var item in obj.Values.Where(u => predicate(u)))
            {
                copy.Add(item.Id, Clone(item));
            }

            return copy;
        }

        public IDictionary<int, PlaceModel> GetPlaces(int[] ids)
        {
            var source = this.GetPlacesCache();
            var items = new Dictionary<int, PlaceModel>(ids.Length);
            var missing = new List<int>();

            foreach (var id in ids)
            {
                if (source.ContainsKey(id))
                {
                    if (!items.ContainsKey(id))
                    {
                        items.Add(id, Clone(source[id]));
                    }
                }
                else
                {
                    if (!items.ContainsKey(id))
                    {
                        items.Add(id, null);
                    }

                    if (!missing.Contains(id))
                    {
                        missing.Add(id);
                    }
                }
            }

            source = this.Services.Places.GetForCache(missing.ToArray());
            foreach (var id in missing)
            {
                if (source.ContainsKey(id))
                {
                    items[id] = source[id];
                }
            }

            return items;
        }

        public void InvalidatePlaces()
        {
            Debug.WriteLine("CacheService.InvalidatePlaces");
            var key = this.GetKey("AllPlaces");
            this.cache.Clear(key);
        }

        public IDictionary<int, EventCategoryModel> GetEventCategories(int[] ids)
        {
            var source = this.GetEventCategoriesCache();
            var items = new Dictionary<int, EventCategoryModel>(ids.Length);
            var missing = new List<int>();

            foreach (var id in ids)
            {
                if (source.ContainsKey(id))
                {
                    if (!items.ContainsKey(id))
                    {
                        items.Add(id, Clone(source[id]));
                    }
                }
                else
                {
                    if (!items.ContainsKey(id))
                    {
                        items.Add(id, null);
                    }

                    if (!missing.Contains(id))
                    {
                        missing.Add(id);
                    }
                }
            }

            source = this.Services.EventsCategories.GetForCache(missing.ToArray());
            foreach (var id in missing)
            {
                items[id] = source[id];
            }

            return items;
        }

        public FreeGeoIpModel GetLocationViaFreegeoip(string ip)
        {
            var watch = Stopwatch.StartNew();
            var keyBase = "GetLocationViaFreegeoip(" + ip + ")";
            var key = this.GetKey(keyBase);
            var obj = this.cache.GetObject<FreeGeoIpModel>(key);
            if (obj == null)
            {
                obj = this.Services.Places.GetLocationViaFreegeoip(ip);
                Debug.WriteLine("CacheService." + keyBase + ": POPULATED");
                this.cache.Add(key, obj, this.longCacheDuration);
            }

            var copy = Clone(obj);

            watch.Stop();
            Debug.WriteLine("CacheService." + keyBase + ": copy from cache took " + watch.Elapsed);

            return copy;
        }

        public HintModel GetHintByAlias(string alias)
        {
            var source = this.GetHintsCache();
            HintModel item;
            item = source.Values.FirstOrDefault(x => x.Alias.Equals(alias));
            if (item == null)
            {
                var entity = this.Services.Repositories.Hints.GetByAlias(alias);
                if (entity != null)
                {
                    item = new HintModel(entity);
                    source.Add(item.Id, Clone(item));
                }
            }

            if (item != null)
            {
                return Clone(item);
            }

            return null;
        }

        public void InvalidateProfileFields()
        {
            Debug.WriteLine("CacheService.InvalidateProfileFields");
            var key = this.GetKey("AllProfileFields");
            this.cache.Clear(key);
        }

        public int CountProfileFields()
        {
            var cache = this.GetProfileFieldsCache();
            return cache.Count;
        }

        public IList<ProfileFieldModel> FindProfileFields(Func<ProfileFieldModel, bool> predicate)
        {
            var obj = this.GetProfileFieldsCache();

            var copy = new List<ProfileFieldModel>();
            foreach (var item in obj.Values.Where(u => predicate(u)))
            {
                copy.Add(Clone(item));
            }

            return copy;
        }

        private static T Clone<T>(T item)
            where T : class
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(item));
        }

        private string GetKey(string key)
        {
            return "SparkleNetworkCache;N=" + this.Services.NetworkId + ";" + key;
        }

        private IDictionary<int, ProfileFieldModel> GetProfileFieldsCache()
        {
            IDictionary<int, ProfileFieldModel> obj;
            var key = this.GetKey("AllProfileFields");
            obj = this.cache.GetObject<Dictionary<int, ProfileFieldModel>>(key);
            if (obj == null)
            {
                obj = this.Services.ProfileFields.GetAllForCache();
                Debug.WriteLine("CacheService.AllProfileFields: POPULATED");
                this.cache.Add(key, obj, this.longCacheDuration);
            }

            return obj;
        }

        private IDictionary<int, UserModel> GetUsersCache()
        {
            var watch = Stopwatch.StartNew();
            IDictionary<int, UserModel> obj;
            var key = this.GetKey("AllUsers");
            obj = this.cache.GetObject<Dictionary<int, UserModel>>(key);
            if (obj == null)
            {
                obj = this.Services.People.GetAllForCache();
                Debug.WriteLine("CacheService.AllUsers: POPULATED");
                this.cache.Add(key, obj, this.defaultCacheDuration);
            }

            watch.Stop();
            if (watch.ElapsedMilliseconds > 100)
            {
                Debug.WriteLine("CacheService.AllUsers: copy from cache took " + watch.Elapsed);
            }

            return obj;
        }

        private IDictionary<int, PlaceModel> GetPlacesCache()
        {
            var watch = Stopwatch.StartNew();
            IDictionary<int, PlaceModel> obj;
            var key = this.GetKey("AllPlaces");
            obj = this.cache.GetObject<IDictionary<int, PlaceModel>>(key);
            if (obj == null)
            {
                obj = this.Services.Places.GetAllForCache();
                Debug.WriteLine("CacheService.AllPlaces: POPULATED");
                this.cache.Add(key, obj, this.defaultCacheDuration);
            }

            watch.Stop();
            if (watch.ElapsedMilliseconds > 100)
            {
                Debug.WriteLine("CacheService.AllPlaces: copy from cache took " + watch.Elapsed);
            }

            return obj;
        }

        private IDictionary<int, EventCategoryModel> GetEventCategoriesCache()
        {
            var watch = Stopwatch.StartNew();
            IDictionary<int, EventCategoryModel> obj;
            var key = this.GetKey("AllEventCategories");
            obj = this.cache.GetObject<Dictionary<int, EventCategoryModel>>(key);
            if (obj == null)
            {
                obj = this.Services.EventsCategories.GetAllForCache();
                Debug.WriteLine("CacheService.AllEventCategories: POPULATED");
                this.cache.Add(key, obj, this.defaultCacheDuration);
            }

            watch.Stop();
            if (watch.ElapsedMilliseconds > 100)
            {
                Debug.WriteLine("CacheService.AllEventCategories: copy from cache took " + watch.Elapsed);
            }

            return obj;
        }

        private IDictionary<int, HintModel> GetHintsCache()
        {
            var watch = Stopwatch.StartNew();
            IDictionary<int, HintModel> obj;
            var key = this.GetKey("AllHints");
            obj = this.cache.GetObject<Dictionary<int, HintModel>>(key);
            if (obj == null)
            {
                obj = this.Services.Hints.GetAllForCache();
                Debug.WriteLine("CacheService.AllHints: POPULATED");
                this.cache.Add(key, obj, this.longCacheDuration);
            }

            watch.Stop();
            if (watch.ElapsedMilliseconds > 100)
            {
                Debug.WriteLine("CacheService.AllHints: copy from cache took " + watch.Elapsed);
            }

            return obj;
        }
    }
}
