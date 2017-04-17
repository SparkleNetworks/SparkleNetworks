
namespace Sparkle.Services.Networks
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public interface IEventsCategoriesService
    {
        void DeleteEventCategory(EventCategory item);
        int InsertEventCategory(EventCategory item);
        EventCategory SelectEventCategoryById(int Id);
        EventCategory UpdateEventCategory(EventCategory item);

        IList<EventCategory> SelectAll();

        int Count();

        IList<EventCategoryModel> GetAll();

        IList<EventCategoryModel> GetAllWithStats();

        IDictionary<int, EventCategoryModel> GetAllForCache();
        IDictionary<int, EventCategoryModel> GetForCache(int[] ids);

        void Initialize();
    }
}
