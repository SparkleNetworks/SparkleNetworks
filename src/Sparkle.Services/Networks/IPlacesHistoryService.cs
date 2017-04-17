using System;
using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IPlacesHistoryService
    {
        void Delete(PlaceHistory item);
        long Insert(PlaceHistory item);
        IList<PlaceHistory> SelectAll();
        IList<PlaceHistory> SelectForLunch();
        IList<PlaceHistory> SelectHistory(int userId);
        IList<PlaceHistory> SelectPlaceById(int id);
        PlaceHistory SelectPlaceHistoryByPlaceIdAndUserId(int placeId, int userId);
        PlaceHistory SelectPlaceHistoryForLunchToday(int userId);
        PlaceHistory SelectPlaceHistoryForLunchTodayWithPlaceId(int placeId, int userId);
        PlaceHistory SelectPlaceHistoryForLunchTomorrow(int userId);
        PlaceHistory SelectPlaceHistoryForLunchTomorrowWithPlaceId(int placeId, int userId);
        IList<PlaceHistory> SelectTodayPlaceHistoryById(int id);
        IList<PlaceHistory> SelectTodayPlaceHistoryByPlaceId(int placeId);
        IList<PlaceHistory> SelectTomorrowPlaceHistoryByPlaceId(int placeId);
        long Update(PlaceHistory item);
    }
}
