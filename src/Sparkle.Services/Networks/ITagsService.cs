
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Tags;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ITagsService
    {
        bool DisableV1 { get; }

        IList<TagCategoryModel> GetCategories();
        IList<Tag2Model> GetAll();
        IList<Tag2Model> GetTagsByType(TagType type);
        IList<Tag2Model> SearchForCities(string search);

        IList<Tag2Model> GetTagsFromPartnerResourceId(int partnerId);

        string GetSOTagFromTags(IList<Tag2Model> list);
        IList<Tag2Model> GetTagsFromSOTagString(string sotag);

        IList<Tag2Model> SearchFor(string search, params TagType[] type);

        Tag2Model GetByAlias(string alias);
        IList<CityTagModel> GetCityTags();

        void Initialize();

        TagCategoryModel GetCategoryByAlias(string alias);

        IList<Tag2Model> GetTagsByCategoryId(int categoryId);

        IList<Tag2Model> GetTagsByCompanyIdAndCategoryId(int companyId, int categoryId);

        IList<TagCategoryModel> GetCategoriesApplyingToCompanies();
        IList<TagCategoryModel> GetCategoriesApplyingToUsers();
        IList<TagCategoryModel> GetCategoriesApplyingToGroups();

        IDictionary<TagCategoryModel, IList<Tag2Model>> GetTagsByCompanyIdAndCategories(int companyId, IList<TagCategoryModel> categories);
        IDictionary<TagCategoryModel, IList<Tag2Model>> GetTagsByUserIdAndCategories(int userId, IList<TagCategoryModel> categories);
        IDictionary<TagCategoryModel, IList<Tag2Model>> GetTagsByGroupIdAndCategories(int groupId, IList<TagCategoryModel> categories);

        IList<Tag2Model> GetCompanyTagsInApplyRequest(Guid applyKey);
        IList<Tag2Model> GetCompanyTagsInApplyRequest(Guid applyKey, int categoryId);

        AddOrRemoveTagResult Create(AddOrRemoveTagRequest request);
        AddOrRemoveTagResult AddOrRemoveTag(AddOrRemoveTagRequest request);
        AddOrRemoveTagResult GetTagsListAccordingToEntityType(AddOrRemoveTagRequest request);
        AddOrRemoveTagResult GetEntityTagsAccordingToEntityType(AddOrRemoveTagRequest request);

        Tag2Model GetTagsById(int tagId);

        TagCategoryModel GetCategoryById(int categoryId);

        string MakeAlias(string name);

        IDictionary<TagCategoryModel, IList<Tag2Model>> GetUsedEntityTags(string entityTypeName);

        IList<Tag2Model> GetTagsById(int[] ids);

        int[] GetEntitiesIdsWhoUsesTagsIds(string entityTypeName, int[] tagIds, bool accrued);

        int CountByCategory(int categoryId);
        
        BasicResult OneShotPopulateIndustries(int categoryId, int userId);

        IList<Tag2Model> GetByNameAndCategory(int categoryId, string tagName);
        IList<Tag2Model> GetByNameAndCategory(int[] categoryId, string tagName);

        PagedListModel<Tag2Model> GetTagsByCategoryId(int categoryId, int offset, int count);
    }
}
