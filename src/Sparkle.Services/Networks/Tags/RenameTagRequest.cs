
namespace Sparkle.Services.Networks.Tags
{
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class RenameTagRequest : BaseRequest
    {
        public TagModelType Type { get; set; }

        public int Id { get; set; }

        [Display(ResourceType = typeof(NetworksLabels), Name = "NewName")]
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        public string NewName { get; set; }

        [Display(ResourceType = typeof(NetworksLabels), Name = "ApplyToAllNetworks")]
        public bool AllNetworks { get; set; }

        public bool MayApplyToAllNetworks { get; set; }
    }

    public class RenameTagResult : BaseResult<RenameTagRequest, RenameTagError>
    {
        public RenameTagResult(RenameTagRequest request)
            : base(request)
        {
        }

        public TagModel UpdatedTag { get; set; }
    }

    public enum RenameTagError
    {
        NoSuchTag,
        SameTag
    }
}
