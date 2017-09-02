
namespace Sparkle.Services.Networks.Texts
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class EditTextRequest : BaseRequest
    {
        public int TargetId { get; set; }

        public string TargetName { get; set; }

        public int? TextId { get; set; }

        public string CultureName { get; set; }

        [Required]
        [Display(Name = "Title", ResourceType = typeof(NetworksLabels))]
        [StringLength(140, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Title { get; set; }

        [Required]
        [Display(Name = "EmailContent", ResourceType = typeof(NetworksLabels))]
        public string Value { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public int UpdatedByUserId { get; set; }

        public int UserId { get; set; }

        public void UpdateFrom(TextValue item)
        {
            this.TextId = item.TextId;
            this.CultureName = item.CultureName;
            this.Title = item.Title;
            this.Value = item.Value;
            this.DateUpdatedUtc = item.DateUpdatedUtc;
            this.UpdatedByUserId = item.UpdatedByUserId;
        }

        public string ReturnUrl { get; set; }

        public IDictionary<string, string> Rules { get; set; }
    }

    public class EditTextResult : BaseResult<EditTextRequest, EditTextError>
    {
        public EditTextResult(EditTextRequest request)
            : base(request)
        {
        }

        public int InsertedTextId { get; set; }
    }

    public enum EditTextError
    {
        NoSuchSubscriptionTemplate,
        SetTextValueFailed
    }
}
