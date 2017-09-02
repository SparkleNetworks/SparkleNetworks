
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Resources;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class EditInformationNoteRequest : LocalBaseRequest
    {
        [DataMember]
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NetworksLabels), Name = "Title")]
        [StringLength(200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        [DataMember]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NetworksLabels), Name = "Description")]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        [DataMember]
        public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NetworksLabels), Name = "InformationNote_StartDate")]
        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime UserStartDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(NetworksLabels), Name = "InformationNote_EndDate")]
        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime UserEndDate { get; set; }

        public void RefreshFrom(Entities.Networks.InformationNote item, TimeZoneInfo tz)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Description = item.Description;

            if (tz.Equals(TimeZoneInfo.Utc))
            {
                // this block is because of a bug in SrkToolkit that is not resolved yet
                // fix is in https://github.com/sandrock/SrkToolkit/commit/5d3e82fb9326b0588d95e877efc1a950a3fc729c
                // nuget is not up-to-date yet
                this.UserStartDate = tz.ConvertFromUtc(item.StartDateUtc).AsUtc();
                this.UserEndDate = tz.ConvertFromUtc(item.EndDateUtc).AsUtc();
            }
            else
            {
                this.UserStartDate = tz.ConvertFromUtc(item.StartDateUtc);
                this.UserEndDate = tz.ConvertFromUtc(item.EndDateUtc);
            }
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();

            if (this.UserEndDate < this.UserStartDate)
            {
                this.AddValidationError("UserEndDate", ValidationMessages.DateStartEnd);
            }

            if (this.UserStartDate < DateTime.UtcNow.ToPrecision(DateTimePrecision.Year))
                this.AddValidationError("UserStartDate", ValidationMessages.MinimumDateThisYear);

            if (this.UserEndDate < DateTime.UtcNow.ToPrecision(DateTimePrecision.Year))
                this.AddValidationError("UserEndDate", ValidationMessages.MinimumDateThisYear);
        }
    }

    [DataContract(Namespace = Names.PublicNamespace)]
    public class EditInformationNoteResult : BaseResult<EditInformationNoteRequest, EditInformationNoteError>
    {
        public EditInformationNoteResult()
            : base(null)
        {
        }

        public EditInformationNoteResult(EditInformationNoteRequest request)
            : base(request)
        {
        }

        [DataMember]
        public InformationNoteModel Item { get; set; }
    }

    public enum EditInformationNoteError
    {
        InvalidRequest,
        CannotEdit,
        NoSuchItem,
        NoSuchActingUser,
        NotAuthorized,
    }
}
