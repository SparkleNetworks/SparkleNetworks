
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public interface IPlacesCategoriesService
    {
        IList<PlaceCategory> SelectAll();
        IList<PlaceCategory> SelectParents();
        PlaceCategory SelectPlaceCategoryById(int id);

        PlaceCategory Insert(PlaceCategory item);

        IList<PlaceCategoryModel> GetAll();

        IDictionary<int, PlaceCategoryModel> GetAllAsDictionary();

        void Initialize();
        string GetSymbolFromFoursquareUrlPrefix(string prefix);
    }
}
