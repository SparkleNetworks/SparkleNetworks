
namespace Sparkle.Services.Main.Networks
{
    using FourSquare.SharpSquare.Core;
    using FourSquare.SharpSquare.Entities;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Places;
    using System;
    using System.Collections.Generic;
    using System.Linq;
using System.Text.RegularExpressions;

    public class PlacesCategoriesService : ServiceBase, IPlacesCategoriesService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacesCategoriesService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal PlacesCategoriesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Gets the places categories repository.
        /// </summary>
        protected IPlacesCategoriesRepository PlacesCategoriesRepository
        {
            get { return this.Repo.PlacesCategories; }
        }

        public void Initialize()
        {
            var logPath = "PlaceCategoriesService.Initialize";

            #region PlaceCategories dico
            var dico = @"
PlaceOldName                | FoursquareId              | FoursquareParent
---------------------------------------------------------------------------------
Professionnel				| 4d4b7105d754a06375d81259  | NULL
Culture et loisirs			| 4d4b7104d754a06370d81259  | NULL
Etablissement universitaire | 4d4b7105d754a06372d81259  | NULL
Alimentation				| 4d4b7105d754a06374d81259  | NULL
Grands Espaces				| 4d4b7105d754a06377d81259  | NULL
Résidentiel					| 4e67e38e036454776db1fb3a  | NULL
Magasins et services		| 4d4b7105d754a06378d81259  | NULL
Voyage						| 4d4b7105d754a06379d81259  | NULL

Galerie d'Art				| 4bf58dd8d48988d1e2931735  | 4d4b7104d754a06370d81259
Bowling						| 4bf58dd8d48988d1e4931735  | 4d4b7104d754a06370d81259
Casino						| 4bf58dd8d48988d17c941735  | 4d4b7104d754a06370d81259
Site historique				| 4deefb944765f83613cdba6e  | 4d4b7104d754a06370d81259
Cinéma						| 4bf58dd8d48988d17f941735  | 4d4b7104d754a06370d81259
Musé						| 4bf58dd8d48988d181941735  | 4d4b7104d754a06370d81259
Stade						| 4bf58dd8d48988d184941735  | 4d4b7104d754a06370d81259
Parc d'attraction			| 4bf58dd8d48988d182941735  | 4d4b7104d754a06370d81259
Zoo et aquarium				| 4bf58dd8d48988d17b941735  | 4d4b7104d754a06370d81259

Performing Arts Venue       | 4bf58dd8d48988d1f2931735  | 4d4b7104d754a06370d81259
Théatre						| 4bf58dd8d48988d137941735  | 4bf58dd8d48988d1f2931735

Collège						| 4bf58dd8d48988d198941735  | 4d4b7105d754a06372d81259
Université					| 4bf58dd8d48988d1ae941735  | 4d4b7105d754a06372d81259

Restaurant					| 4bf58dd8d48988d1c4941735  | 4d4b7105d754a06374d81259
Café						| 4bf58dd8d48988d16d941735  | 4d4b7105d754a06374d81259
Pizzeria					| 4bf58dd8d48988d1ca941735  | 4d4b7105d754a06374d81259
Boulangerie					| 4bf58dd8d48988d16a941735  | 4d4b7105d754a06374d81259
Friterie					| 4edd64a0c7ddd24ca188df1a  | 4d4b7105d754a06374d81259
Sandwitcherie				| 4bf58dd8d48988d1c5941735  | 4d4b7105d754a06374d81259
Fast-Food					| 4bf58dd8d48988d16e941735  | 4d4b7105d754a06374d81259
Charcuterie					| 4bf58dd8d48988d146941735  | 4d4b7105d754a06374d81259

Parc						| 4bf58dd8d48988d163941735  | 4d4b7105d754a06377d81259
Jardin						| 4bf58dd8d48988d15a941735  | 4d4b7105d754a06377d81259
Plage						| 4bf58dd8d48988d1e2941735  | 4d4b7105d754a06377d81259
Lac							| 4bf58dd8d48988d161941735  | 4d4b7105d754a06377d81259
Canal						| 4eb1d4dd4b900d56c88a45fd  | 4d4b7105d754a06377d81259
Place						| 4bf58dd8d48988d164941735  | 4d4b7105d754a06377d81259

Athletics & Sports          | 4f4528bc4b90abdf24c9de85  | 4d4b7105d754a06377d81259
Terrain de Foot				| 4cce455aebf7b749d5e191f5  | 4f4528bc4b90abdf24c9de85
Terrain de Basket			| 4bf58dd8d48988d1e1941735  | 4f4528bc4b90abdf24c9de85
Terrain de Baseball			| 4bf58dd8d48988d1e8941735  | 4f4528bc4b90abdf24c9de85
Terrain de Golf				| 4bf58dd8d48988d1e6941735  | 4f4528bc4b90abdf24c9de85
Terrain de jeu				| 52e81612bcbc57f1066b7a2e  | 4f4528bc4b90abdf24c9de85

Nightlife Spot              | 4d4b7105d754a06376d81259  | NULL
Bar							| 4bf58dd8d48988d116941735  | 4d4b7105d754a06376d81259
Bar à Cocktail				| 4bf58dd8d48988d11e941735  | 4d4b7105d754a06376d81259
Bar sportif					| 4bf58dd8d48988d11d941735  | 4d4b7105d754a06376d81259
Bar à Vin					| 4bf58dd8d48988d123941735  | 4d4b7105d754a06376d81259
Pub							| 4bf58dd8d48988d11b941735  | 4d4b7105d754a06376d81259
Brasserie					| 50327c8591d4c4b30a586d5d  | 4d4b7105d754a06376d81259
Salon						| 4bf58dd8d48988d121941735  | 4d4b7105d754a06376d81259

Bureau						| 4bf58dd8d48988d124941735  | 4d4b7105d754a06375d81259
Salle de conférence			| 4bf58dd8d48988d127941735  | 4bf58dd8d48988d124941735

Convention Center           | 4bf58dd8d48988d1ff931735  | 4d4b7105d754a06375d81259
Salle de réunion			| 4bf58dd8d48988d100941735  | 4bf58dd8d48988d1ff931735

Maison						| 4bf58dd8d48988d103941735  | 4e67e38e036454776db1fb3a
Appartement					| 4d954b06a243a5684965b473  | 4e67e38e036454776db1fb3a

Banque						| 4bf58dd8d48988d10a951735  | 4d4b7105d754a06378d81259
Magasin de vêtement			| 4bf58dd8d48988d103951735  | 4d4b7105d754a06378d81259
Magasin de moto				| 4bf58dd8d48988d115951735  | 4d4b7105d754a06378d81259
Centre commercial			| 4bf58dd8d48988d1fd941735  | 4d4b7105d754a06378d81259

Gym / Fitness Center        | 4bf58dd8d48988d175941735  | 4d4b7105d754a06378d81259
Salle de sports				| 4bf58dd8d48988d176941735  | 4bf58dd8d48988d175941735

Food & Drink Shop           | 4bf58dd8d48988d1f9941735  | 4d4b7105d754a06378d81259
Supermarché					| 52f2ab2ebcbc57f1066b8b46  | 4bf58dd8d48988d1f9941735
Boucherie					| 4bf58dd8d48988d11d951735  | 4bf58dd8d48988d1f9941735
";
            #endregion

            var lines = dico
                .Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(2)
                .Select(o => o.Split(new string[] { "|" }, StringSplitOptions.None).Select(s => s.Trim()).ToArray())
                .ToArray();

            var all = this.Repo.PlacesCategories.GetAll(false).Select(o => o.Value).ToList();

            // old datas need to be updated
            bool missingFoursquareId = all.All(o => string.IsNullOrEmpty(o.FoursquareId));
            // foursquare places need an update
            bool updateNeeded = all.Any(o => (!o.LastUpdateDateUtc.HasValue || o.LastUpdateDateUtc.Value.AddMonths(1) <= DateTime.UtcNow) && !string.IsNullOrEmpty(o.FoursquareId));

            if (!missingFoursquareId && !updateNeeded)
            {
                this.Services.Logger.Verbose(logPath, ErrorLevel.Success, "All place categories OK.");
                return;
            }

            var transaction = this.Services.Repositories.NewTransaction();
            using (transaction.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            {
                all = transaction.Repositories.PlacesCategories.GetAll(false).Select(o => o.Value).ToList();
                missingFoursquareId = all.All(o => string.IsNullOrEmpty(o.FoursquareId));

                if (missingFoursquareId)
                {
                    foreach (var item in lines)
                    {
                        var name = item[0];
                        var foursquareId = item[1];
                        var foursquareParentId = item[2];
                        var parentId = foursquareParentId != "NULL" ? all.SingleOrDefault(o => o.FoursquareId == foursquareParentId).Id : 0;

                        PlaceCategory entity = all.SingleOrDefault(o => o.Name == name);
                        if (entity == null)
                        {
                            entity = new PlaceCategory
                            {
                                ParentId = parentId,
                                Name = name,
                                FoursquareId = foursquareId,
                            };

                            entity = transaction.Repositories.PlacesCategories.Insert(entity);
                            all.Add(entity);
                            this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created place category " + name);
                        }
                        else if (string.IsNullOrEmpty(entity.FoursquareId))
                        {
                            entity.FoursquareId = foursquareId;
                            entity.ParentId = parentId;

                            transaction.Repositories.PlacesCategories.Update(entity);
                            this.Services.Logger.Info(logPath, ErrorLevel.Success, "Updated place category " + name);
                        }
                    }
                }

                if (updateNeeded)
                {
                    if (string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Externals.Foursquare.ApiClientId)
                        || string.IsNullOrEmpty(this.Services.AppConfiguration.Tree.Externals.Foursquare.ApiClientSecret))
                    {
                        this.Services.Logger.Info(logPath, ErrorLevel.Internal, "Foursquare ApiClientId & ApiClientSecret need to be set.");
                    }
                    else
                    {
                        IList<Category> categories = null;
                        try
                        {
                            var api = new SharpSquare(
                                this.Services.AppConfiguration.Tree.Externals.Foursquare.ApiClientId,
                                this.Services.AppConfiguration.Tree.Externals.Foursquare.ApiClientSecret);
                            categories = api.GetVenueCategories();
                        }
                        catch (Exception ex)
                        {
                            this.Services.Logger.Error(
                                logPath,
                                ErrorLevel.ThirdParty,
                                ex);
                        }

                        if (categories != null)
                        {
                            this.DoUpdateCategoriesFromFoursquare(transaction.Repositories, all, categories, 0);
                        }
                    }
                }

                transaction.CompleteTransaction();
            }
        }

        private void DoUpdateCategoriesFromFoursquare(IRepositoryFactory repositories, IList<PlaceCategory> placeCategories, IList<Category> items, int parentId)
        {
            var logPath = "PlaceCategoriesService.DoUpdateCategories";
            var now = DateTime.UtcNow;

            foreach (var item in items)
            {
                var placeCategory = placeCategories.SingleOrDefault(o => o.FoursquareId == item.id);
                string symbol = null;
                if (item.icon != null)
                    symbol = this.GetSymbolFromFoursquareUrlPrefix(item.icon.prefix);

                if (placeCategory == null)
                {
                    placeCategory = new PlaceCategory
                    {
                        Name = item.name,
                        FoursquareId = item.id,
                        ParentId = parentId,
                        Symbol = symbol,
                        LastUpdateDateUtc = now,
                    };

                    placeCategory = repositories.PlacesCategories.Insert(placeCategory);
                    placeCategories.Add(placeCategory);
                    this.Services.Logger.Info(logPath, ErrorLevel.Success, "Created place category " + item.name);
                }
                else
                {
                    placeCategory.Name = item.name;
                    placeCategory.ParentId = parentId;
                    placeCategory.Symbol = symbol;
                    placeCategory.LastUpdateDateUtc = now;

                    repositories.PlacesCategories.Update(placeCategory);
                    this.Services.Logger.Info(logPath, ErrorLevel.Success, "Updated place category " + item.name);
                }

                if (item.categories != null && item.categories.Count > 0)
                    this.DoUpdateCategoriesFromFoursquare(repositories, placeCategories, item.categories, placeCategory.Id);
            }
        }

        public string GetSymbolFromFoursquareUrlPrefix(string prefix)
        {
            var start = prefix.IndexOf("/categories_v2/");
            var end = prefix.LastIndexOf("_");
            if (start == -1 || end == -1)
                return null;

            start += ("/categories_v2/").Length;
            return prefix.Substring(start, end - start).Replace('/', '_');
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public IList<PlaceCategory> SelectAll()
        {
            return PlacesCategoriesRepository
                .Select()
                .ToList();
        }

        /// <summary>
        /// Selects the parents.
        /// </summary>
        /// <returns></returns>
        public IList<PlaceCategory> SelectParents()
        {
            return PlacesCategoriesRepository
                .Select()
                .Parents()
                .OrderBy(o => o.Name)
                .ToList();
        }

        /// <summary>
        /// Selects the place category by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public PlaceCategory SelectPlaceCategoryById(int id)
        {
            return PlacesCategoriesRepository
                .Select()
                .WithId(id)
                .FirstOrDefault();
        }

        public PlaceCategory Insert(PlaceCategory item)
        {
            return this.PlacesCategoriesRepository.Insert(item);
        }

        public IList<PlaceCategoryModel> GetAll()
        {
            return this.Repo.PlacesCategories.GetAll().Values
                .Select(c => new PlaceCategoryModel(c))
                .ToList();
        }

        public IDictionary<int, PlaceCategoryModel> GetAllAsDictionary()
        {
            return this.Repo.PlacesCategories.GetAll()
                .ToDictionary(c => c.Key, c => new PlaceCategoryModel(c.Value));
        }
    }
}
