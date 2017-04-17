
namespace Sparkle.Services.Networks.Tags.EntityWithTag
{
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EntityWithTagApplyRequestRepository : IEntityWithTagRepository
    {
        private const string logPath = "EntityWithTagApplyRequestRepository.";
        private static readonly char[] forbiddenChars = { '"', '\'', '\\', '`', };
        private readonly string subEntity;

        private void CheckIdentifier(string identifier)
        {
            foreach (var ch in identifier)
            {
                if (forbiddenChars.Contains(ch))
                    throw new InvalidOperationException(logPath + "CheckIdentifier: Found forbidden char in identifier '" + identifier + "'");
            }
        }

        public EntityWithTagApplyRequestRepository(string subEntity)
        {
            this.CheckIdentifier(subEntity);

            this.subEntity = subEntity;
        }

        private IList<Tag2Model> RetrieveCorrespondingTags(ApplyRequestModel model)
        {
            switch (this.subEntity)
            {
                case "Company":
                    return model.CompanyDataModel.CompanyTags;
                default:
                    throw new InvalidOperationException(logPath + ".RetrieveCorrespondingTags: Cannot retrieve tags for sub-entity '" + this.subEntity + "'");
            }
        }

        private void UpdateCorrespondingTags(ApplyRequestModel model, IList<Tag2Model> items)
        {
            switch (this.subEntity)
            {
                case "Company":
                    model.CompanyDataModel.CompanyTags = items;
                    break;
                default:
                    throw new InvalidOperationException(logPath + ".UpdateCorrespondingTags: Cannot retrieve tags for sub-entity '" + this.subEntity + "'");
            }
        }

        public void Add(IServiceFactory services, IEntityWithTag item)
        {
            var model = services.People.GetApplyRequest(item.EntityId);
            var tags = this.RetrieveCorrespondingTags(model);
            var wanted = tags.Where(o => o.Name == item.TagName && o.CategoryId == item.TagCategoryId).SingleOrDefault();
            if (wanted == null)
            {
                wanted = new Tag2Model
                {
                    CategoryId = item.TagCategoryId,
                    Name = item.TagName,
                };
                if (item.TagId.HasValue)
                    wanted.Id = item.TagId.Value;

                tags.Add(wanted);
                this.UpdateCorrespondingTags(model, tags);
                services.People.UpdateApplyRequestData(model);
            }
        }

        public void Remove(IServiceFactory services, IEntityWithTag item)
        {
            var model = services.People.GetApplyRequest(item.EntityId);
            var tags = this.RetrieveCorrespondingTags(model);
            var wanted = tags.Where(o => o.Name == item.TagName && o.CategoryId == item.TagCategoryId).SingleOrDefault();
            if (wanted != null)
            {
                tags.Remove(wanted);
                this.UpdateCorrespondingTags(model, tags);
                services.People.UpdateApplyRequestData(model);
            }
        }

        public IList<Tag2Model> GetAllTagsForCategory(IServiceFactory services, int categoryId, int entityId)
        {
            var tags = services.Tags.GetTagsByCategoryId(categoryId);
            var entityTags = this.GetEntityTagsForCategory(services, categoryId, entityId);
            tags.AddRange(entityTags);

            return tags.GroupBy(o => o.Name).Select(o => o.First()).ToList();
        }

        public IList<Tag2Model> GetEntityTagsForCategory(IServiceFactory services, int categoryId, int entityId)
        {
            var model = services.People.GetApplyRequest(entityId);
            return this.RetrieveCorrespondingTags(model).Where(o => o.CategoryId == categoryId).ToList();
        }

        public IList<Tag2Model> GetUsedEntityTags(IServiceFactory services)
        {
            throw new NotImplementedException();
        }

        public IDictionary<int, int[]> GetEntitiesIdsByUsedTagsIds(IServiceFactory services, int[] tagIds)
        {
            throw new NotImplementedException();
        }
    }
}
