using Sparkle.Data.Networks;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks;

namespace Sparkle.Services.Main.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;

    public class PlacesHistoryService : ServiceBase, IPlacesHistoryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacesHistoryService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal PlacesHistoryService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Gets the places history repository.
        /// </summary>
        protected IPlacesHistoryRepository PlacesHistoryRepository
        {
            get { return this.Repo.PlacesHistory; }
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public IList<PlaceHistory> SelectAll()
        {
            return PlacesHistoryRepository
                .Select()
                .ToList();
        }

        /// <summary>
        /// Selects the place history by place id and user id.
        /// </summary>
        /// <param name="placeId">The place id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public PlaceHistory SelectPlaceHistoryByPlaceIdAndUserId(int placeId, int userId)
        {
            return PlacesHistoryRepository
                .Select()
                .PlaceId(placeId)
                .UserId(userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the place history for lunch today.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public PlaceHistory SelectPlaceHistoryForLunchToday(int userId)
        {
            return PlacesHistoryRepository
                .Select()
                .Today()
                .UserId(userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the place history for lunch tomorrow.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public PlaceHistory SelectPlaceHistoryForLunchTomorrow(int userId)
        {
            return PlacesHistoryRepository
                .Select()
                .Tomorrow()
                .UserId(userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the place history for lunch today with place id.
        /// </summary>
        /// <param name="placeId">The place id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public PlaceHistory SelectPlaceHistoryForLunchTodayWithPlaceId(int placeId, int userId)
        {
            return PlacesHistoryRepository
                .Select()
                .Today()
                .PlaceId(placeId)
                .UserId(userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the place history for lunch tomorrow with place id.
        /// </summary>
        /// <param name="placeId">The place id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public PlaceHistory SelectPlaceHistoryForLunchTomorrowWithPlaceId(int placeId, int userId)
        {
            return PlacesHistoryRepository
                .Select()
                .Tomorrow()
                .PlaceId(placeId)
                .UserId(userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects for lunch.
        /// </summary>
        /// <returns></returns>
        public IList<PlaceHistory> SelectForLunch()
        {
            return PlacesHistoryRepository
                .Select()
                .ForLunch()
                .ToList();
        }

        /// <summary>
        /// Selects the place by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public IList<PlaceHistory> SelectPlaceById(int id)
        {
            return PlacesHistoryRepository
                .Select()
                .WithId(id)
                .ToList();
        }

        /// <summary>
        /// Selects the today place history by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public IList<PlaceHistory> SelectTodayPlaceHistoryById(int id)
        {
            return PlacesHistoryRepository
                .Select()
                .WithId(id)
                .ToList();
        }

        /// <summary>
        /// Selects the today place history by place id.
        /// </summary>
        /// <param name="placeId">The place id.</param>
        /// <returns></returns>
        public IList<PlaceHistory> SelectTodayPlaceHistoryByPlaceId(int placeId)
        {
            return PlacesHistoryRepository
                .Select()
                .Today()
                .PlaceId(placeId)
                .ToList();
        }

        /// <summary>
        /// Selects the tomorrow place history by place id.
        /// </summary>
        /// <param name="placeId">The place id.</param>
        /// <returns></returns>
        public IList<PlaceHistory> SelectTomorrowPlaceHistoryByPlaceId(int placeId)
        {
            return PlacesHistoryRepository
                .Select()
                .Tomorrow()
                .PlaceId(placeId)
                .ToList();
        }

        /// <summary>
        /// Selects the history.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<PlaceHistory> SelectHistory(int userId)
        {
            return PlacesHistoryRepository
                .Select()
                .UserId(userId)
                .ToList();
        }

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public long Insert(PlaceHistory item)
        {
            this.PlacesHistoryRepository.Insert(item);
            return 1;
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public long Update(PlaceHistory item)
        {
            this.PlacesHistoryRepository.Update(item);
            return 1;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(PlaceHistory item)
        {
            this.PlacesHistoryRepository.Delete(item);
        }

    }
}
