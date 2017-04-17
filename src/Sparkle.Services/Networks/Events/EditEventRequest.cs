
namespace Sparkle.Services.Networks.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Resources;
    using SrkToolkit.DataAnnotations;
    using SrkToolkit.Domain;
    using Sparkle.Services.Networks.Lang;

    public class EditEventRequest : BaseRequest
    {
        private string timezone = "Romance Standard Time";

        [Required]
        [Display(Name = "Name", ResourceType = typeof(NetworksLabels))]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Display(Name = "Description", ResourceType = typeof(NetworksLabels))]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Description { get; set; }

        [Display(Name = "Timezone", ResourceType = typeof(NetworksLabels))]
        [Timezone]
        ////public string Timezone { get; set; }
        public string Timezone
        {
            get { return this.timezone; }
            set { this.timezone = value; }
        }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "DateBegin", ResourceType = typeof(NetworksLabels))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy hh:mm}")]
        public DateTime DateEvent { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "DateEnd", ResourceType = typeof(NetworksLabels))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm}")]
        public DateTime DateEndEvent { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "AnswerBefore", ResourceType = typeof(NetworksLabels))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime? NeedAnswerBefore { get; set; }

        [Display(Name = "Category", ResourceType = typeof(NetworksLabels))]
        public int Category { get; set; }

        public IList<EventCategoryModel> AvailableCategories { get; set; }

        [Display(Name = "For", ResourceType = typeof(NetworksLabels))]
        public EventVisibility Visibility { get; set; }

        public IList<EnumValueText<EventVisibility>> AvailableVisibilities { get; set; }

        public string[] NewGuestsSelected { get; set; }

        [Display(Name = "Website", ResourceType = typeof(NetworksLabels))]
        [Sparkle.Common.DataAnnotations.Url]
        public string Website { get; set; }

        [Display(Name = "TicketsWebsite", ResourceType = typeof(NetworksLabels))]
        [Sparkle.Common.DataAnnotations.Url]
        public string TicketsWebsite { get; set; }



        public List<PlaceModel> AvailablePlaces { get; set; }

        [Display(Name = "Place", ResourceType = typeof(NetworksLabels))]
        public int? PlaceId { get; set; }

        ////public Recurrence Recurrence { get; set; }

        public int? GroupId { get; set; }

        public IList<GroupModel> AvailableGroups { get; set; }

        public int UserId { get; set; }

        public Group Group { get; set; }

        protected override void ValidateFields()
        {
            base.ValidateFields();

            var minDate = new DateTime(1990, 1, 1, 0, 0, 0);

            if (this.DateEvent < minDate)
                this.AddValidationError("DateEvent", "On ne peut créer des événements que dans le futur.");

            if (this.DateEndEvent < minDate)
                this.AddValidationError("DateEndEvent", "On ne peut créer des événements que dans le futur.");

            if (this.DateEndEvent < this.DateEvent)
                this.AddValidationError("DateEndEvent", "La date de fin doit être suéprieure à la date de début.");

        }

        public string Title { get; set; }

        public int Id { get; set; }
    }
}
