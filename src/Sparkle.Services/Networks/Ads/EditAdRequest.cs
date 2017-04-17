
namespace Sparkle.Services.Networks.Ads
{
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditAdRequest : BaseRequest
    {
        public EditAdRequest()
        {
        }

        public int Id { get; set; }

        public int ActingUserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Content { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        public int? TypeId { get; set; }

        public IList<AdCategoryModel> AvailableCategories { get; set; }

        public IList<Tags.Tag2Model> AvailableTypes { get; set; }
    }

    public class EditAdResult : BaseResult<EditAdRequest, EditAdError>
    {
        public EditAdResult(EditAdRequest request)
            : base(request)
        {
        }

        public AdModel Item { get; set; }

        public bool IsPendingValidation { get; set; }

        public bool IsPendingEdit { get; set; }
    }

    public enum EditAdError
    {
        NoSuchActingUser,
        NotAuthorized,
        NoSuchItem,
        Invalid,
        NoSuchCategory,
        NoSuchType,
        CannotEditWhenClosed,
    }
}
