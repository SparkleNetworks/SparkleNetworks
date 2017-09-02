
namespace Sparkle.Services.Networks.Tags
{
    using Sparkle.Services.Networks.Tags.EntityWithTag;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AddOrRemoveTagRequest : BaseRequest
    {
        public bool AddTag { get; set; }

        public string TagId { get; set; }

        public string CategoryAlias { get; set; }

        public string EntityIdentifier { get; set; }

        public string EntityTypeName { get; set; }

        public int? ActingUserId { get; set; }
    }

    public class AddOrRemoveTagResult : BaseResult<AddOrRemoveTagRequest, AddOrRemoveTagError>
    {
        public AddOrRemoveTagResult(AddOrRemoveTagRequest request)
            : base(request)
        {
        }

        public IEntityWithTag Entity { get; set; }

        public IList<Tag2Model> TagsList { get; set; }
    }

    public enum AddOrRemoveTagError
    {
        NoSuchCompany,
        NoSuchTagCategory,
        MustSpecifyTagId,
        NotAuthorized,
        NoSuchTag,
        DoesNotApplyToCompanies,
        CannotAddTagForCategory,
        MaxNumberOfTagForCategory,
        NoSuchApplyRequest,
        CategoryNotAvailableForEntity,
        NoSuchUser,
        NoSuchGroup,
        TagAlreadyExists,
        NoSuchAd,
    }

    public delegate bool ValidateEntityWithTag(IServiceFactory services, string entityIdentifier, string tagId, int? actingUserId, TagCategoryModel tagCategory, AddOrRemoveTagResult result);

    public delegate bool ValidateEntity(IServiceFactory services, string entityIdentifier, AddOrRemoveTagResult result, out int entityId);
}
