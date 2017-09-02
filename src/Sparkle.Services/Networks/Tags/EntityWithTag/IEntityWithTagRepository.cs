
namespace Sparkle.Services.Networks.Tags.EntityWithTag
{
    using Sparkle.Entities.Networks.Neutral;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IEntityWithTagRepository
    {
        ////void Insert(IServiceFactory services, IEntityWithTag item);
        ////void Update(IServiceFactory services, IEntityWithTag item);
        void Add(IServiceFactory services, IEntityWithTag item);
        void Remove(IServiceFactory services, IEntityWithTag item);
        IList<Tag2Model> GetAllTagsForCategory(IServiceFactory services, int categoryId, int entityId);
        IList<Tag2Model> GetEntityTagsForCategory(IServiceFactory services, int categoryId, int entityId);
        IList<Tag2Model> GetUsedEntityTags(IServiceFactory services);
        IDictionary<int, int[]> GetEntitiesIdsByUsedTagsIds(IServiceFactory services, int[] tagIds);
    }

    public interface IEntityWithTag
    {
        int? TagId { get; set; }
        string TagName { get; set; }
        int TagCategoryId { get; set; }
        int NetworkId { get; set; }
        int? ActingUserId { get; set; }
        int EntityId { get; set; }
    }

    public enum EntityWithTagRepositoryType
    {
        Sql,
        ApplyRequest
    }
}
