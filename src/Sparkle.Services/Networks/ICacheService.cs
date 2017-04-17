
namespace Sparkle.Services.Networks
{
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ICacheService
    {
        IDictionary<int, UserModel> AllUsers { get; }
        IDictionary<int, PlaceModel> AllPlaces { get; }
        IDictionary<int, EventCategoryModel> AllEventCategories { get; }
        IDictionary<int, ProfileFieldModel> AllProfileFields { get; }

        IDictionary<int, UserModel> GetUsers(int[] ids);
        IDictionary<int, UserModel> FindUsers(Func<UserModel, bool> predicate);
        UserModel GetUser(int userId);

        IDictionary<int, PlaceModel> GetPlaces(int[] ids);
        void InvalidatePlaces();

        IDictionary<int, EventCategoryModel> GetEventCategories(int[] ids);

        Places.FreeGeoIpModel GetLocationViaFreegeoip(string ip);

        HintModel GetHintByAlias(string alias);

        void InvalidateProfileFields();

        int CountProfileFields();

        IList<ProfileFieldModel> FindProfileFields(Func<ProfileFieldModel, bool> predicate);
    }
}
